using System.Net;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Models;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.EmailSender;
using Ecommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[Authorize]
public class PaymentController : ApiControllerBase
{
    private readonly IEfRepository<Basket> _basketRepo;
    private readonly IEfRepository<Order> _orderRepo;
    private readonly IEfRepository<OrderDetail> _orderDetailRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStripeService _stripeService;
    private readonly IEmailSender _emailService;
    private readonly ICurrentUserService _currentUserService;

    public PaymentController(
        IEfRepository<Basket> basketRepo,
        UserManager<ApplicationUser> userManager,
        IStripeService stripeService,
        IEfRepository<Order> orderRepo,
        IEfRepository<OrderDetail> orderDetailRepo,
        IEmailSender emailService,
        ICurrentUserService currentUserService)
    {
        _basketRepo = basketRepo;
        _userManager = userManager;
        _stripeService = stripeService;
        _orderRepo = orderRepo;
        _orderDetailRepo = orderDetailRepo;
        _emailService = emailService;
        _currentUserService = currentUserService;
    }

    [HttpPost("pay")]
    public async Task<IActionResult> CreateOrder(CardOptions card)
    {
        try
        {
            #region GET USER BASKET
            ApplicationUser user = await GetUser(HttpContext);

            IEnumerable<Basket> userBasket = _basketRepo.GetAll(b => b.ApplicationUserId == user.Id, IncludeProperty: "Product");

            if (userBasket is null || userBasket.Count() == 0) return BadRequest("The user not have items in basket");
            #endregion

            #region MAKE CHARGE
            decimal totalToPay = Convert.ToDecimal(userBasket.Select(sb => sb.Total).Aggregate((acc, next) => acc + next));

            Stripe.Token validatedCardToken = _stripeService.CreateCardToken(card);

            if (validatedCardToken.Id is null || validatedCardToken.Id.Count() <= 0) return BadRequest("Invalid card");

            Stripe.Card validCard = await _stripeService.CreateCard(validatedCardToken.Id, user.CustomerId!);

            Stripe.Charge chargeToken = _stripeService.CreateChargeToken(cardToken: validCard.Id, totalToPay, user: user);

            if (chargeToken.Status.ToLower() != "succeeded") return BadRequest("Payment could not be made");
            #endregion

            #region CREATE ORDER & ORDER DETAIL
            Order order = new(applicationUserId: user.Id, paymentTransactionId: chargeToken.BalanceTransactionId, totalToPay);

            Order orderCreated = await _orderRepo.AddAsync(order);

            if (orderCreated is null) return BadRequest("Could not create the order");

            IEnumerable<OrderDetail> orderDetail = CreateOrderDetail(userBaskets: userBasket, userId: user.Id , orderId: orderCreated.Id);

            await _orderDetailRepo.AddRangeAsync(orderDetail);
            #endregion

            #region REMOVE BASKET
            _basketRepo.RemoveRange(userBasket);

            await _basketRepo.SaveChangeAsync();
            #endregion

            #region SEND MAIL
            IEnumerable<OrderDetail> OrderDetailWithProduct = _orderDetailRepo.GetAll(od => od.OrderId == orderCreated.Id, IncludeProperty: "Product");

            var mailRequest = await CreateMailRequest(user, OrderDetailWithProduct);

            await _emailService.SendAsync(mailRequest);
            #endregion

            return Ok(new { Order = orderCreated, Charge = chargeToken.Id });
        }
        catch (Stripe.StripeException ex)
        {
            return BadRequest(new Response(Status: HttpStatusCode.BadRequest, Message: ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(Status: HttpStatusCode.BadRequest, Message: ex.Message));
        }
    }

    [HttpPost("refund")]
    public async Task<IActionResult> RefundCharge(string chargeToken)
    {
        try
        {
            Stripe.Refund refundToken = await _stripeService.CreateRefundToken(chargeToken);
            
            if (refundToken.Status != "succeeded") return BadRequest("Could not refund");

            return Ok(new {
                RefundStatus = refundToken.Status,
                RefundId = refundToken.Id,
                RefundAt = refundToken.StripeResponse.Date
            });
        }
        catch (Stripe.StripeException ex)
        {
            return BadRequest(ex.Message);
        }
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