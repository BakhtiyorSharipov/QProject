using System.Text;
using System.Text.Json;
using QApiGateway.DTO.QueueDTO;

namespace QApiGateway.Endpoints;

public static class QueueEndpoints
{
    public static void MapQueueEndpoints(this WebApplication app)
    {
        var queueGroup = app.MapGroup("/api/Queue").WithTags("Queue");

        queueGroup.MapGet("/history/customer", async (HttpClient client, HttpRequest request, int pageNumber = 1) =>
        {
            var token = request.Headers["Authorization"].ToString();
            var req = new HttpRequestMessage(
                HttpMethod.Get,
                $"http://localhost:5003/api/Queue/history/customer?pageNumber={pageNumber}"
            );
            req.Headers.Add("Authorization", token);

            var response = await client.SendAsync(req);
            var content = await response.Content.ReadAsStringAsync();
            return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
        }).RequireAuthorization();

        queueGroup.MapGet("{id}", async (HttpClient client, HttpRequest httpRequest, int id) =>
        {
            var token = httpRequest.Headers["Authorization"].ToString();
            var req = new HttpRequestMessage(
                HttpMethod.Get,
                $"http://localhost:5003/api/Queue/{id}"
            );
            req.Headers.Add("Authorization", token);

            var response = await client.SendAsync(req);
            var content = await response.Content.ReadAsStringAsync();
            return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
        }).RequireAuthorization();
        
        queueGroup.MapPost("/book", async (HttpClient client, HttpRequest httpRequest, QueueCreateRequest request) =>
        {
            
            var token = httpRequest.Headers["Authorization"].ToString();

            
            var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5003/api/Queue/book");
            req.Headers.Add("Authorization", token);
            req.Content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            
            var response = await client.SendAsync(req);
            var content = await response.Content.ReadAsStringAsync();

            
            return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);

        }).RequireAuthorization();
        
        queueGroup.MapPut("/cancel/customer", async (HttpClient client, HttpRequest httpRequest, QueueCancelRequest request) =>
        {
            var token = httpRequest.Headers["Authorization"].ToString();

            var req = new HttpRequestMessage(HttpMethod.Put, "http://localhost:5003/api/Queue/cancel/customer");
            req.Headers.Add("Authorization", token);
            req.Content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.SendAsync(req);
            var content = await response.Content.ReadAsStringAsync();

            return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
        }).RequireAuthorization();
    }
}