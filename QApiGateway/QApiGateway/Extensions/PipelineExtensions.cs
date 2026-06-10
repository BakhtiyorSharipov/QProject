using QApiGateway.Middleware;

namespace QApiGateway.Extensions;

public static class PipelineExtensions
{
    public static IApplicationBuilder UseGatewayPipeline(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseMiddleware<CustomerAuthorizationMiddleware>();
        app.UseAuthorization();

        
        return app;
    }
}
