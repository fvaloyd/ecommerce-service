using Ecommerce.Application.Data;
using Ecommerce.Core.Entities;
using Ecommerce.Infrastructure.EmailSender;
using Ecommerce.Infrastructure.EmailSender.Common;
using Ecommerce.Infrastructure.EmailSender.Models;
using Ecommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.BackgroundJobs;

public class SendEmailWithOrderDetails
{
    private readonly IEcommerceDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly UserManager<ApplicationUser> _userManager;

    public SendEmailWithOrderDetails(IEcommerceDbContext context, IEmailSender emailSender, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _emailSender = emailSender;
        _userManager = userManager;
    }
    public async Task Handle(int orderId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        List<OrderDetail> OrderDetailWithProduct = await _context.OrderDetails.Include(od => od.Product).Where(od => od.OrderId == orderId).ToListAsync();

        var mailRequest = await CreateMailRequest(user!, OrderDetailWithProduct);

        await _emailSender.SendAsync(mailRequest);
    }

    private async Task<MailRequest> CreateMailRequest(ApplicationUser user, IEnumerable<OrderDetail> orderDetail)
    {
        PurchaseDetailsMailModel purchaseDetailsMailModel = new(User: user, OrderDetails: orderDetail);

        string template = await _emailSender.GetTemplate(((int)MailTemplates.PurchaseDetails));

        string templateCompiled = await _emailSender.GetCompiledTemplateAsync(template, purchaseDetailsMailModel);

        return new MailRequest(Body: templateCompiled, Subject: "Purchased products", Email: user.Email!);
    }
}
