using System.Net;
using System.Net.Http.Json;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Response;
using QBranchService.Application.UseCases.BranchConfigurations.Commands.CreateBranchConfiguration;
using QBranchService.Application.UseCases.Branches.Commands.CreateBranch;
using QBranchService.Application.UseCases.Companies.Commands.CreateCompany;
using Shouldly;

namespace QBranchService.API.IntegrationTest.Controllers;

public class BranchConfigurationControllerTests : IClassFixture<QBranchServiceWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly QBranchServiceWebApplicationFactory _factory;

    public BranchConfigurationControllerTests(QBranchServiceWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }


    [Fact]
    public async Task CreateBranchConfiguration_ShouldReturnCreatedBranchConfiguration_WithValidRequest()
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

        var createBranchCommand = new CreateBranchCommand(
            CompanyId: companyResult.Id,
            BranchName: "TestBranchName",
            Address: "TestAddress",
            City: "TestCity",
            EmailAddress: "test@gmail.com",
            PhoneNumber: "+992981111112");

        var branchResponse = await _client.PostAsJsonAsync("/api/Branch", createBranchCommand);
        branchResponse.EnsureSuccessStatusCode();
        var branchResult = await branchResponse.Content.ReadFromJsonAsync<BranchResponseModel>();
        branchResult.ShouldNotBeNull();
        branchResult.Id.ShouldNotBe(0);

        var createBranchConfigurationCommand = new CreateBranchConfigurationCommand(
            BranchId: branchResult.Id,
            MaxTicketsPerDay: 50,
            OpenTime: new TimeOnly(8, 0),
            CloseTime: new TimeOnly(17, 0),
            BreakStartTime: new TimeOnly(12, 0),
            BreakEndTime: new TimeOnly(13, 0));

        var branchConfigurationResponse =
            await _client.PostAsJsonAsync("/api/BranchConfiguration", createBranchConfigurationCommand);
        branchConfigurationResponse.EnsureSuccessStatusCode();
        var branchConfigurationResult =
            await branchConfigurationResponse.Content.ReadFromJsonAsync<BranchConfigurationResponseModel>();
        branchConfigurationResult.ShouldNotBeNull();
        branchConfigurationResult.Id.ShouldNotBe(0);
    }
    
    [Fact]
    public async Task GetBranchConfiguration_ExistingBranchConfiguration_ReturnsSuccess()
    {
        var createCompanyCommand = new CreateCompanyCommand(
            CompanyName: "TestCompany",
            Address: "TestAddress",
            EmailAddress: "test@gmail.com",
            PhoneNumber: "+992921111112");

        var companyCreateResponse = await _client.PostAsJsonAsync("/api/Company", createCompanyCommand);

        companyCreateResponse.EnsureSuccessStatusCode();
        var companyCreatedResult = await companyCreateResponse.Content.ReadFromJsonAsync<CompanyResponseModel>();
        companyCreatedResult.ShouldNotBeNull();
        companyCreatedResult.Id.ShouldNotBe(0);

        var createBranchCommand = new CreateBranchCommand(
            CompanyId: companyCreatedResult.Id,
            BranchName: "TestBranchName",
            Address: "TestAddress",
            City: "TestCity",
            EmailAddress: "test@gmail.com",
            PhoneNumber: "+992981111112");

        var branchCreateResponse = await _client.PostAsJsonAsync("/api/Branch", createBranchCommand);
        branchCreateResponse.EnsureSuccessStatusCode();
        var branchCreatedResult = await branchCreateResponse.Content.ReadFromJsonAsync<BranchResponseModel>();
        branchCreatedResult.ShouldNotBeNull();
        branchCreatedResult.Id.ShouldNotBe(0);

        var createBranchConfigurationCommand = new CreateBranchConfigurationCommand(
            BranchId: branchCreatedResult.Id,
            MaxTicketsPerDay: 50,
            OpenTime: new TimeOnly(8, 0),
            CloseTime: new TimeOnly(17, 0),
            BreakStartTime: new TimeOnly(12, 0),
            BreakEndTime: new TimeOnly(13, 0));

        var createBranchConfigurationResponse =
            await _client.PostAsJsonAsync("/api/BranchConfiguration", createBranchConfigurationCommand);
        createBranchConfigurationResponse.EnsureSuccessStatusCode();
        var createdBranchConfigurationResult =
            await createBranchConfigurationResponse.Content.ReadFromJsonAsync<BranchConfigurationResponseModel>();
        createdBranchConfigurationResult.ShouldNotBeNull();
        createdBranchConfigurationResult.Id.ShouldNotBe(0);
      
      
        var branchConfigurationId = createdBranchConfigurationResult.Id;
        var response = await _client.GetAsync($"api/BranchConfiguration/{branchConfigurationId}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CompanyServiceResponseModel>();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(branchConfigurationId);
    }

    [Fact]
    public async Task GetBranchConfiguration_NonExistentBranchConfiguration_ReturnsNotFound()
    {
        var nonExistentBranchConfigurationId = 999;
        var url = $"/api/BranchConfiguration/{nonExistentBranchConfigurationId}";
       
        var exception = await Assert.ThrowsAsync<HttpStatusCodeException>(() => _client.GetAsync(url));
        exception.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}