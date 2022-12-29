using Ecommerce.Core.Entities;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.EmailSender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Data;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Infrastructure.EmailSender.Models;
using Ecommerce.Infrastructure.EmailSender.Common;
using Ecommerce.Infrastructure.Payment;
using Ecommerce.Infrastructure.Payment.Models;

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

    [HttpPost("pay")]
    public async Task<IActionResult> CreateOrder(PayRequest card)
    {
        #region GET USER BASKET
        ApplicationUser user = await GetUser(HttpContext);
        
        List<Basket> userBasket = await _db.Baskets.Include(b => b.Product).Where(b => b.ApplicationUserId == user.Id).ToListAsync();

        if (userBasket is null || userBasket.Count < 1) return BadRequest("The user not have items in basket");
        #endregion

        #region MAKE CHARGE
        decimal totalToPay = Convert.ToDecimal(userBasket.Select(sb => sb.Total).Aggregate((acc, next) => acc + next));
        
        Stripe.Token validatedCardToken = _stripeService.CreateCardToken(card);
        
        if (validatedCardToken.Id is null || validatedCardToken.Id.Length <= 0) return BadRequest("Invalid card");
        
        Stripe.Card validCard = await _stripeService.CreateCard(validatedCardToken.Id, user.CustomerId!);
        
        Stripe.Charge chargeToken = _stripeService.CreateChargeToken(cardToken: validCard.Id, totalToPay, user: user);
        
        if (chargeToken.Status.ToLower() != "succeeded") return BadRequest("Payment could not be made");
        #endregion

        #region CREATE ORDER & ORDER DETAIL
        Order order = new(applicationUserId: user.Id, paymentTransactionId: chargeToken.BalanceTransactionId, totalToPay);
        
        await _db.Orders.AddAsync(order);
        
        await _db.SaveChangesAsync();
        
        IEnumerable<OrderDetail> orderDetail = CreateOrderDetail(userBaskets: userBasket, userId: user.Id , orderId: order.Id);
        
        await _db.OrderDetails.AddRangeAsync(orderDetail);

        await _db.SaveChangesAsync();
        #endregion

        #region REMOVE BASKET
        _db.Baskets.RemoveRange(userBasket);

        await _db.SaveChangesAsync();
        #endregion

        #region SEND MAIL
        List<OrderDetail> OrderDetailWithProduct = await _db.OrderDetails.Include(od => od.Product).Where(od => od.OrderId == order.Id).ToListAsync();

        var mailRequest = await CreateMailRequest(user, OrderDetailWithProduct);
        
        await _emailService.SendAsync(mailRequest);
        #endregion

        return Ok(new { Order = order, Charge = chargeToken.Id });
    }
    
    [HttpPost("refund")]
    public async Task<IActionResult> RefundCharge(string chargeToken)
    {
        Stripe.Refund refundToken = await _stripeService.CreateRefundToken(chargeToken);
            
        if (refundToken.Status != "succeeded") return BadRequest("Could not refund");

        return Ok(new {
            RefundStatus = refundToken.Status,
            RefundId = refundToken.Id,
            RefundAt = refundToken.StripeResponse.Date
        });
    }

    private async Task<MailRequest> CreateMailRequest(ApplicationUser user, IEnumerable<OrderDetail> orderDetail)
    {
        PurchaseDetailsMailModel purchaseDetailsMailModel = new(User: user, OrderDetails: orderDetail);

        string template = await _emailService.GetTemplate(((int)MailTemplates.PurchaseDetails));

        string templateCompiled = await _emailService.GetCompiledTemplateAsync(template, purchaseDetailsMailModel);

        return new MailRequest(Body: templateCompiled, Subject: "Purchased products", Email: user.Email);
    }

    private async Task<ApplicationUser> GetUser(HttpContext ctx)
    {
        string userId = _currentUserService.UserId!;

        return await _userManager.FindByIdAsync(userId);
    }

    private IEnumerable<OrderDetail> CreateOrderDetail(IEnumerable<Basket> userBaskets, string userId, int orderId)
    {
        foreach (Basket basket in userBaskets)
        {
            yield return new OrderDetail(orderId: orderId, applicationUserId: userId, productId: basket.ProductId, quantity: basket.Quantity, unitPrice: basket.Product.Price);
        }
    }
}