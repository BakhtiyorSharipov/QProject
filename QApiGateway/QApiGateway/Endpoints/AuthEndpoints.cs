using System.Text;
using System.Text.Json;
using QApiGateway.DTO.AuthDTO;

namespace QApiGateway.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("/api/Auth").WithTags("Auth");

        authGroup.MapPost("/login", async (HttpClient client, LoginRequest request) =>
        {
            var response = await client.PostAsJsonAsync(
                "http://localhost:5003/api/Auth/login",
                request
            );
            var content = await response.Content.ReadAsStringAsync();
            return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
        });

        authGroup.MapPost("/register", async (HttpClient client, RegisterRequest request) =>
        {
            var response = await client.PostAsJsonAsync(
                "http://localhost:5003/api/Auth/register", request);

            var content = await response.Content.ReadAsStringAsync();
            return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
        });
        
        authGroup.MapPost("/logout",  async (HttpClient client, HttpRequest httpRequest, string refreshToken) =>
        {
            var token = httpRequest.Headers["Authorization"].ToString();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5003/api/Auth/logout");
            request.Headers.Add("Authorization", token);
            request.Content = new StringContent(JsonSerializer.Serialize(refreshToken), Encoding.UTF8,
                "application/json");
            
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
        }).RequireAuthorization();

        authGroup.MapPut("/change-password",
            async (HttpClient client, HttpRequest httpRequest, UpdatePasswordRequest updatePasswordRequest) =>
            {
                var token = httpRequest.Headers["Authorization"].ToString();
                var request = new HttpRequestMessage(HttpMethod.Put,
                    "http://localhost:5003/api/Auth/change-password");
                request.Headers.Add("Authorization", token);
                request.Content = new StringContent(JsonSerializer.Serialize(updatePasswordRequest), Encoding.UTF8,
                    "application/json");

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
            });

        authGroup.MapDelete("/delete-customer-account", async (HttpClient client, HttpRequest httpRequest) =>
        {
            var token = httpRequest.Headers["Authorization"].ToString();
            var request = new HttpRequestMessage(HttpMethod.Delete,
                "http://localhost:5003/api/Auth/delete-customer-account");

            request.Headers.Add("Authorization", token);
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
        }).RequireAuthorization();
    }
}