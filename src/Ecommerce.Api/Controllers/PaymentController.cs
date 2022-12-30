using Ecommerce.Core.Entities;
using Ecommerce.Application.Data;
using Ecommerce.Infrastructure.Payment;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.EmailSender;
using Ecommerce.Infrastructure.Payment.Models;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Infrastructure.EmailSender.Models;
using Ecommerce.Infrastructure.EmailSender.Common;

using Stripe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Api.Controllers;

[Authorize]
public class PaymentController : ApiControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStripeService _stripeService;
    private readonly IEmailSender _emailService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEcommerceDbContext _db;

    public PaymentController(
        UserManager<ApplicationUser> userManager,
        IStripeService stripeService,
        IEmailSender emailService,
        ICurrentUserService currentUserService,
        IEcommerceDbContext db)
    {
        _userManager = userManager;
        _stripeService = stripeService;
        _emailService = emailService;
        _currentUserService = currentUserService;
        _db = db;
    }

    [HttpPost("refund")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefundCharge(string chargeToken)
    {
        Stripe.Refund refundToken = await _stripeService.CreateRefundToken(chargeToken);

        if (refundToken.Status != "succeeded") return BadRequest("Could not refund");

        return Ok(new
        {
            RefundStatus = refundToken.Status,
            RefundId = refundToken.Id,
            RefundAt = refundToken.StripeResponse.Date
        });
    }

    [HttpPost("pay")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrder(PayRequest card)
    {
        ApplicationUser? user = await GetUser();

        if (user is null) return BadRequest("Something wrong happened");
        
        List<Basket> userBasket = await _db.Baskets.Include(b => b.Product).Where(b => b.ApplicationUserId == user.Id).ToListAsync();

        if (userBasket is null || userBasket.Count < 1) return BadRequest("The user not have items in basket");

        decimal totalToPay = GetTotalToPay(userBasket);

        Stripe.Card validCard = await ValidateCard(card, user);

        if (validCard is null) return BadRequest("Invalid Card");

        Stripe.Charge chargeToken = MakeCharge(validCard, user, totalToPay);

        if (chargeToken is null) return BadRequest("Could not made the charge");
        
        Order order = await CreateOrder(user, chargeToken, totalToPay, userBasket);

        await RemoveBasket(userBasket);

        await SendEmailWrapper(order, user);

        return Ok(new { Order = order, Charge = chargeToken.Id });
    }

    private Stripe.Charge MakeCharge(Stripe.Card validCard, ApplicationUser user, decimal totalToPay)
    {
        Stripe.Charge chargeToken = _stripeService.CreateChargeToken(cardToken: validCard.Id, totalToPay, user: user);

        if (chargeToken.Status.ToLower() != "succeeded") return null!;

        return chargeToken;
    }

    private async Task<Stripe.Card> ValidateCard(PayRequest card, ApplicationUser user)
    {
        Stripe.Token validatedCardToken = _stripeService.CreateCardToken(card);

        if (validatedCardToken.Id is null || validatedCardToken.Id.Length <= 0) return null!;

        Stripe.Card validCard = await _stripeService.CreateCard(validatedCardToken.Id, user.CustomerId!);

        return validCard;
    }

    private async Task<Order> CreateOrder(ApplicationUser user, Charge charge, decimal totalToPay, IEnumerable<Basket> basket)
    {
        Order order = new(applicationUserId: user.Id, paymentTransactionId: charge.BalanceTransactionId, totalToPay);

        await _db.Orders.AddAsync(order);

        await _db.SaveChangesAsync();

        IEnumerable<OrderDetail> orderDetail = CreateOrderDetail(userBaskets: basket, userId: user.Id, orderId: order.Id);

        await _db.OrderDetails.AddRangeAsync(orderDetail);

        await _db.SaveChangesAsync();

        return order;
    }

    private async Task RemoveBasket(IEnumerable<Basket> basket)
    {
        _db.Baskets.RemoveRange(basket);

        await _db.SaveChangesAsync();
    }

    private async Task SendEmailWrapper(Order order, ApplicationUser user)
    {
        List<OrderDetail> OrderDetailWithProduct = await _db.OrderDetails.Include(od => od.Product).Where(od => od.OrderId == order.Id).ToListAsync();

        var mailRequest = await CreateMailRequest(user, OrderDetailWithProduct);

        await _emailService.SendAsync(mailRequest);
    }

    private async Task<MailRequest> CreateMailRequest(ApplicationUser user, IEnumerable<OrderDetail> orderDetail)
    {
        PurchaseDetailsMailModel purchaseDetailsMailModel = new(User: user, OrderDetails: orderDetail);

        string template = await _emailService.GetTemplate(((int)MailTemplates.PurchaseDetails));

        string templateCompiled = await _emailService.GetCompiledTemplateAsync(template, purchaseDetailsMailModel);

        return new MailRequest(Body: templateCompiled, Subject: "Purchased products", Email: user.Email!);
    }

    private async Task<ApplicationUser?> GetUser()
    {
        string userId = _currentUserService.UserId!;

        return await _userManager.FindByIdAsync(userId)!;
    }

    private IEnumerable<OrderDetail> CreateOrderDetail(IEnumerable<Basket> userBaskets, string userId, int orderId)
    {
        foreach (Basket basket in userBaskets)
        {
            yield return new OrderDetail(orderId: orderId, applicationUserId: userId, productId: basket.ProductId, quantity: basket.Quantity, unitPrice: basket.Product.Price);
        }
    }

    private decimal GetTotalToPay(IEnumerable<Basket> basket)
    {
        return Convert.ToDecimal(basket.Select(sb => sb.Total).Aggregate((acc, next) => acc + next));
    }
}