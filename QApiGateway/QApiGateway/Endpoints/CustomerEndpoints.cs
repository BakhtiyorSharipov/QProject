using System.Text;
using System.Text.Json;
using QApiGateway.DTO.CustomerDTO;

namespace QApiGateway.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app)
    {
        var customerGroup = app.MapGroup("api/Customer").WithTags("Customer");


        customerGroup.MapGet("/get-customer-profile", async (HttpClient client, HttpRequest httpRequest) =>
        {
            var token = httpRequest.Headers["Authorization"].ToString();
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"http://localhost:5003/api/Customer/get-customer-profile");
            request.Headers.Add("Authorization", token);

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
        }).RequireAuthorization();

        customerGroup.MapPut("/customer-profile-update",
            async (HttpClient client, HttpRequest httpRequest, UpdateProfileRequest updateProfileRequest) =>
            {
                var token = httpRequest.Headers["Authorization"].ToString();
                var request = new HttpRequestMessage(
                    HttpMethod.Put, $"http://localhost:5003/api/Customer/customer-profile-update");
                request.Headers.Add("Authorization", token);
                request.Content = new StringContent(JsonSerializer.Serialize(updateProfileRequest), Encoding.UTF8,
                    "application/json");

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
            }).RequireAuthorization();
    }
}