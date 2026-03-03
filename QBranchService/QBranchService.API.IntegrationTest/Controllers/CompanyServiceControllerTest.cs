using System.Net;
using System.Net.Http.Json;
using QBranchService.Application.Exceptions;
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
    
    [Fact]
    public async Task GetCompanyService_ExistingCompanyService_ReturnsSuccess()
    {
        var createCompanyCommand = new CreateCompanyCommand(
            CompanyName: "TestCompany",
            Address: "TestAddress",
            EmailAddress: "test@gmail.com",
            PhoneNumber: "+992921111112");

        var companyCreatedResponse = await _client.PostAsJsonAsync("/api/Company", createCompanyCommand);

        companyCreatedResponse.EnsureSuccessStatusCode();
        var companyCreatedResult = await companyCreatedResponse.Content.ReadFromJsonAsync<CompanyResponseModel>();
        companyCreatedResult.ShouldNotBeNull();
        companyCreatedResult.Id.ShouldNotBe(0);
        
        var createCompanyService = new CreateServiceCommand(
            CompanyId: companyCreatedResult.Id,
            ServiceName: "TestServiceName",
            ServiceDescription: "TestServiceDescription");

        var companyServiceCreatedResponse = await _client.PostAsJsonAsync("/api/CompanyService", createCompanyService);
        companyServiceCreatedResponse.EnsureSuccessStatusCode();
        var companyServiceCreatedResult = await companyServiceCreatedResponse.Content.ReadFromJsonAsync<CompanyServiceResponseModel>();
        companyServiceCreatedResult.ShouldNotBeNull();
        companyServiceCreatedResult.Id.ShouldNotBe(0);
      
      
        var companyServiceId = companyServiceCreatedResult.Id;
        var response = await _client.GetAsync($"api/CompanyService/{companyServiceId}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CompanyServiceResponseModel>();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(companyServiceId);
    }

    [Fact]
    public async Task GetCompanyService_NonExistentCompanyService_ReturnsNotFound()
    {
        var nonExistentCompanyServiceId = 999;
        var url = $"/api/Branch/{nonExistentCompanyServiceId}";
       
        var exception = await Assert.ThrowsAsync<HttpStatusCodeException>(() => _client.GetAsync(url));
        exception.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}