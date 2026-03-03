using System.Net.Http.Json;
using QBranchService.Application.Response;
using QBranchService.Application.UseCases.Companies.Commands.CreateCompany;
using Shouldly;

namespace QBranchService.API.IntegrationTest.Controllers;

public class CompanyControllerTest:IClassFixture<QBranchServiceWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly QBranchServiceWebApplicationFactory _factory;

    public CompanyControllerTest( QBranchServiceWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCompany_ShouldReturnCreatedCompany_WithValidRequest()
    {
        var createCompanyCommand = new CreateCompanyCommand(
            CompanyName: "TestCompany",
            Address: "TestAddress",
            EmailAddress: "test@gmail.com",
            PhoneNumber: "+992921111112");
        
        var response = await _client.PostAsJsonAsync("/api/Company", createCompanyCommand);
        
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CompanyResponseModel>();
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
    }
}