using System.Net;

namespace Ecommerce.Core.Models;
public record Response(HttpStatusCode Status, string Message);