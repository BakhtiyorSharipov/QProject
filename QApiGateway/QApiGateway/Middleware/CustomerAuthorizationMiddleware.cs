namespace QApiGateway.Middleware;

public class CustomerAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomerAuthorizationMiddleware> _logger;

    public CustomerAuthorizationMiddleware(RequestDelegate next, ILogger<CustomerAuthorizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";

        if (path.StartsWith("/api/Auth", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var user = context.User;

        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            _logger.LogWarning("Unauthorized request to {Path}", path);
            context.Response.StatusCode = 401;
            return;
        }

        if (!user.IsInRole("Customer"))
        {
            _logger.LogWarning("Forbidden request to {Path} by {User}", path, user.Identity.Name);
            context.Response.StatusCode = 403;
            return;
        }

        await _next(context);
    }
}