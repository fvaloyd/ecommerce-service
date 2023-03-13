using Ecommerce.Contracts.Endpoints;

using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route(ApiEndpoints.Root)]
public class ApiControllerBase : ControllerBase {}