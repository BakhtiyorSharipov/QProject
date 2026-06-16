using System.Text;
using QApiGateway.DTO;

namespace QApiGateway.Endpoints;

public static class EmployeeInfoEndpoints
{
    public static void MapEmployeeInfoEndpoints(this WebApplication app)
    {
        var groupEmployee = app.MapGroup("api/Employee").WithTags("Employee");

        groupEmployee.MapGet("/get-branch-employees",
            async (HttpClient client, HttpRequest httpRequest, int companyId, int branchId, int pageNumber = 1) =>
            {
                var token = httpRequest.Headers["Authorization"].ToString();
                var query = $"?companyId={companyId}" +
                            $"&branchId={branchId}" +
                            $"&pageNumber={pageNumber}";

                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"http://localhost:5008/api/Employee/get-branch-employees{query}");

                request.Headers.Add("Authorization", token);

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
            }).RequireAuthorization();

        groupEmployee.MapGet("/get-service-employees",
            async (HttpClient client, HttpRequest httpRequest, int companyId, int serviceId, int pageNumber = 1) =>
            {
                var token = httpRequest.Headers["Authorization"].ToString();
                var query = $"?companyId={companyId}" +
                            $"&serviceId={serviceId}" +
                            $"&pageNumber={pageNumber}";

                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"http://localhost:5008/api/Employee/get-service-employees{query}");

                request.Headers.Add("Authorization", token);

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(content, "application/json", Encoding.UTF8, (int)response.StatusCode);
            }).RequireAuthorization();
    }
}