using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace PokemonBank.Api.Endpoints
{
    public static class HealthEndpoints
    {
        public static IEndpointRouteBuilder MapHealthChecks(this IEndpointRouteBuilder app, string pattern = "/health")
        {
            app.MapGet(pattern, () => Results.Ok(new { status = "ok" }))
                .WithName("HealthCheck")
                .WithSummary("Check the API status")
                .WithDescription("Health check endpoint that returns the service status. Useful for monitoring and load balancers.")
                .WithTags("Health")
                .Produces<object>(200)
                .WithMetadata(new { example = new { status = "ok" } });
            return app;
        }
    }
}
