using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Modular.Ecommerce.Core.Logging.Enrichment;

internal sealed class UserIdEnricher : ILogEventEnricher
{
    private const string UserIdPropertyName = "UserId";
    private readonly IHttpContextAccessor _contextAccessor;

    public UserIdEnricher() : this(new HttpContextAccessor())
    {
    }

    internal UserIdEnricher(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_contextAccessor.HttpContext == null)
            return;

        if (!IsAuthenticatedRequest(_contextAccessor.HttpContext.User.Identity)) return;

        var userId = GetUserId(_contextAccessor.HttpContext.User);

        if (userId.HasValue)
        {
            var userIdProperty = new LogEventProperty(UserIdPropertyName, new ScalarValue(userId.Value));
            logEvent.AddPropertyIfAbsent(userIdProperty);
        }
    }

    private static bool IsAuthenticatedRequest(IIdentity? identity)
    {
        return identity?.IsAuthenticated ?? false;
    }

    private static int? GetUserId(ClaimsPrincipal user)
    {
        return int.TryParse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out var userId) ? userId : null;
    }
}