using System.Text;
using System.Text.Json;
using QApiGateway.DTO.ReviewDTO;

namespace QApiGateway.Endpoints;

public static class ReviewEndpoints
{
    public static void MapReviewEndpoints(this WebApplication app)
    {
        var reviewGroup = app.MapGroup("/api/Review").WithTags("Review");

        reviewGroup.MapPost("create-review",
            async (HttpClient client, HttpRequest httpRequest, ReviewCreateRequest createRequest) =>
            {
                var token = httpRequest.Headers["Authorization"].ToString();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5003/api/Review/create-review");
                request.Headers.Add("Authorization", token);
                request.Content = new StringContent(JsonSerializer.Serialize(createRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
            }).RequireAuthorization();

        reviewGroup.MapGet("review-history/customer",
            async (HttpClient client, HttpRequest httpRequest, int pageNumber = 1) =>
            {
                var token = httpRequest.Headers["Authorization"].ToString();
                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"http://localhost:5003/api/Review/review-history/customer?pageNumber={pageNumber}");

                request.Headers.Add("Authorization", token);


                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
            }).RequireAuthorization();
    }
}