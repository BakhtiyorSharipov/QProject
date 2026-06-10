using System.Text;
using System.Text.Json;
using QApiGateway.DTO.ReportDTO;

namespace QApiGateway.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this WebApplication app)
    {
        var reportGroup = app.MapGroup("/api/Report").WithTags("Report");

        reportGroup.MapGet("/customer-report",
            async (HttpClient client, HttpRequest httpRequest, [AsParameters]CustomerReportRequest reportRequest) =>
            {

                var token = httpRequest.Headers["Authorization"].ToString();

                var query = $"?CustomerId={reportRequest.CustomerId}" +
                            $"&FromDate={reportRequest.FromDate}" +
                            $"&ToDate={reportRequest.ToDate}";
                
                var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5005/api/Report/customer-report{query}");
                request.Headers.Add("Authorization", token);
                
                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);

            }).RequireAuthorization();

    }
}