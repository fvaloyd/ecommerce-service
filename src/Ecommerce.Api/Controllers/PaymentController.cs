using System.Net;
using System.Security.Claims;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Models;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.MailTemplates;
using Ecommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Ecommerce.Api.Controllers;

[Authorize]
public class PaymentController : ApiControllerBase
{
    private readonly IEfRepository<Basket> _basketRepo;
    private readonly IEfRepository<Order> _orderRepo;
    private readonly IEfRepository<OrderDetail> _orderDetailRepo;
    private readonly IBasketService _basketService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStripeService _stripeService;
    private readonly IEmailService _emailService;

    public PaymentController(
        IBasketService basketService,
        IEfRepository<Basket> basketRepo,
        UserManager<ApplicationUser> userManager,
        IStripeService stripeService,
        IEfRepository<Order> orderRepo,
        IEfRepository<OrderDetail> orderDetailRepo,
        IEmailService emailService)
    {
        _basketService = basketService;
        _basketRepo = basketRepo;
        _userManager = userManager;
        _stripeService = stripeService;
        _orderRepo = orderRepo;
        _orderDetailRepo = orderDetailRepo;
        _emailService = emailService;
    }

    [HttpPost("pay")]
    public async Task<IActionResult> CreateOrder(CardOptions card)
    {
        try
        {
            #region GET USER BASKET
            ApplicationUser user = await GetUserFromHttpContext(HttpContext);

            if (user is null) return NotFound("Could not found the user");

            IEnumerable<Basket> userBasket = await _basketRepo.GetAllAsync(b => b.ApplicationUserId == user.Id, IncludeProperty: "Product");

            if (userBasket is null) return BadRequest("The user not have items in basket");
            #endregion

            #region MAKE CHARGE
            decimal totalToPay = GetTotalFromUserBasket(userBasket);

            Stripe.Token validatedCardToken = _stripeService.CreateCardToken(card);

            if (validatedCardToken.Id is null || validatedCardToken.Id.Count() <= 0) return BadRequest("Invalid card");

            Stripe.Card validCard = await _stripeService.CreateCard(validatedCardToken.Id, user.CustomerId!);

            Stripe.Charge chargeToken = _stripeService.CreateChargeToken(cardToken: validCard.Id, totalToPay, user: user);

            if (chargeToken.Status.ToLower() != "succeeded") return BadRequest("Payment could not be made");
            #endregion

            #region CREATE ORDER & ORDER DETAIL
            Order order = CreateOrder(userId: user.Id, transactionId: chargeToken.BalanceTransactionId, totalToPay: totalToPay);

            Order orderCreated = await _orderRepo.AddAsync(order);

            if (orderCreated is null) return BadRequest("Could not create the order");

            IEnumerable<OrderDetail> orderDetail = CreateOrderDetail(userBaskets: userBasket, user: user, orderId: orderCreated.Id);

            await _orderDetailRepo.AddRangeAsync(orderDetail);
            #endregion

            #region REMOVE BASKET
            _basketRepo.RemoveRange(userBasket);

            int removeItemsFromBasketResult = await _basketRepo.SaveChangeAsync();

            if (removeItemsFromBasketResult < 1) return BadRequest(new Response(Status: HttpStatusCode.BadRequest, Message: "Could not remove the items from the basket"));
            #endregion

            #region SEND MAIL
            IEnumerable<OrderDetail> OrderDetailWithProduct = await _orderDetailRepo.GetAllAsync(od => od.OrderId == orderCreated.Id, IncludeProperty: "Product");
            var mailRequest = CreateMailRequest(user, OrderDetailWithProduct);
            await _emailService.SendEmailAsync(mailRequest);
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

    [HttpPost("refound")]
    public async Task<IActionResult> RefoundCharge(string chargeToken)
    {
        try
        {
            Stripe.Refund refundToken = await _stripeService.CreateRefundToken(chargeToken);
            return Ok(refundToken);
        }
        catch (Stripe.StripeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private MailRequest CreateMailRequest(ApplicationUser user, IEnumerable<OrderDetail> orderDetail)
    {
        OrderDetailMailModel orderDetailMailModel = new(User: user, OrderDetails: orderDetail);

        return new MailRequest
        (
            Email: user.Email,
            Subject: "test email send",
            Body: _emailService.GetMailTemplate<OrderDetailMailModel>(mailTemplateName: MailTemplateNames.OrderDetail, mailTemplateModel: orderDetailMailModel)
        );
    }

    private async Task<ApplicationUser> GetUserFromHttpContext(HttpContext ctx)
    {
        ClaimsPrincipal currentUser = HttpContext.User;
        string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        return await _userManager.FindByIdAsync(userId);
    }

    private decimal GetTotalFromUserBasket(IEnumerable<Basket> userBasket)
    {
        return Convert.ToDecimal(userBasket.Select(sb => sb.Total).Aggregate((acc, next) => acc + next));
    }

    private IEnumerable<OrderDetail> CreateOrderDetail(IEnumerable<Basket> userBaskets, ApplicationUser user, int orderId)
    {
        foreach (Basket basket in userBaskets)
        {
            yield return new OrderDetail
            {
                OrderId = orderId,
                UserName = user.UserName,
                ProductId = basket.ProductId,
                Quantity = basket.Quantity,
                UnitPrice = basket.Product.Price
            };
        }
    }

    private Order CreateOrder(string userId, string transactionId, decimal totalToPay)
    {
        return new Order
        {
            OrderDate = DateTime.Now,
            ApplicationUserId = userId,
            PaymentTransactionId = transactionId,
            Total = totalToPay
        };
    }
}