using System.Text;

namespace QApiGateway.Endpoints;

public static class CompanyEndpoints
{
    public static void MapCompanyEndpoints(this WebApplication app)
    {
        var companyGrouped = app.MapGroup("api/Company").WithTags("Company");
        companyGrouped.MapGet("/get-all-companies",
            async (HttpClient client, HttpRequest httpRequest, int pageNUmber = 1) =>
            {
                var token = httpRequest.Headers["Authorization"].ToString();
                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"http://localhost:5006/api/Company/get-all-companies?pageNumber={pageNUmber}");
                request.Headers.Add("Authorization", token);

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
            }).RequireAuthorization();

        companyGrouped.MapGet("/company-info-by-id/{id}", async (HttpClient client, HttpRequest httpRequest, int id) =>
        {
            var token = httpRequest.Headers["Authorization"].ToString();
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"http://localhost:5006/api/Company/company-info-by-id/{id}");
            request.Headers.Add("Authorization", token);

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
        }).RequireAuthorization();
    }
}