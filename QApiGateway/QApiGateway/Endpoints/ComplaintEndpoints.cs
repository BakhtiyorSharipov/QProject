using System.Text;
using System.Text.Json;
using QApiGateway.DTO.ComplaintDTO;

namespace QApiGateway.Endpoints;

public static class ComplaintEndpoints
{
    public static void MapComplaintEndpoints(this WebApplication app)
    {
        var complaintGroup = app.MapGroup("/api/Complaint").WithTags("Complaint");

        complaintGroup.MapPost("create-complaint",
            async (HttpClient client, HttpRequest httpRequest, ComplaintCreateRequest createRequest) =>
            {
                var token = httpRequest.Headers["Authorization"].ToString();
                var request =
                    new HttpRequestMessage(HttpMethod.Post, "http://localhost:5003/api/Complaint/create-complaint");
                request.Headers.Add("Authorization", token);
                request.Content = new StringContent(JsonSerializer.Serialize(createRequest), Encoding.UTF8,
                    "application/json");

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
            }).RequireAuthorization();


        complaintGroup.MapGet("complaint-history/customer",
            async (HttpClient client, HttpRequest httpRequest, int pageNumber = 1) =>
            {
                var token = httpRequest.Headers["Authorization"].ToString();
                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"http://localhost:5003/api/Complaint/complaint-history/customer?pageNumber={pageNumber}");
                request.Headers.Add("Authorization", token);

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
            }).RequireAuthorization();
    }
}