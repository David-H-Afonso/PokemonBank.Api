using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace PokemonBank.Api.Endpoints
{
    public static class HealthEndpoints
    {
        public static IEndpointRouteBuilder MapHealthChecks(this IEndpointRouteBuilder app, string pattern = "/health")
        {
            app.MapGet(pattern, () => Results.Ok(new { status = "ok" }));
            return app;
        }
    }
}
