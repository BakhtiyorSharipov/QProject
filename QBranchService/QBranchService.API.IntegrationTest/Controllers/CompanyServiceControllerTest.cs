using System.Net.Http.Json;
using QBranchService.Application.Response;
using QBranchService.Application.UseCases.Companies.Commands.CreateCompany;
using QBranchService.Application.UseCases.CompanyServices.Commands.CreateService;
using Shouldly;

namespace QBranchService.API.IntegrationTest.Controllers;

public class CompanyServiceControllerTest : IClassFixture<QBranchServiceWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly QBranchServiceWebApplicationFactory _factory;

    public CompanyServiceControllerTest(QBranchServiceWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCompanyService_ShouldReturnCreatedCompanyService_WithValidRequest()
    {
        var createCompanyCommand = new CreateCompanyCommand(
            CompanyName: "TestCompany",
            Address: "TestAddress",
            EmailAddress: "test@gmail.com",
            PhoneNumber: "+992921111112");

        var companyResponse = await _client.PostAsJsonAsync("/api/Company", createCompanyCommand);

        companyResponse.EnsureSuccessStatusCode();
        var companyResult = await companyResponse.Content.ReadFromJsonAsync<CompanyResponseModel>();
        companyResult.ShouldNotBeNull();
        companyResult.Id.ShouldNotBe(0);
        
        var createCompanyService = new CreateServiceCommand(
            CompanyId: companyResult.Id,
            ServiceName: "TestServiceName",
            ServiceDescription: "TestServiceDescription");

        var response = await _client.PostAsJsonAsync("/api/CompanyService", createCompanyService);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CompanyServiceResponseModel>();
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
    }
}