using QApiGateway.Endpoints;
using QApiGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGatewayServices(builder.Configuration);

builder.Services.AddHttpClient("QueueService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5003/");
});

builder.Services.AddHttpClient("AggregationService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5005/");
});

builder.Services.AddHttpClient("UserService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5008/");
});

var app = builder.Build();

app.UseGatewayPipeline();

app.MapAuthEndpoints();
app.MapQueueEndpoints();
app.MapReviewEndpoints();
app.MapComplaintEndpoints();
app.MapReportEndpoints();
app.MapCompanyEndpoints();
app.MapEmployeeInfoEndpoints();
app.MapCustomerEndpoints();
app.Run();