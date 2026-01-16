using System.Net.Http.Headers;
using System.Net.Http.Json;
using QApplication.Responses;
using QApplication.UseCase.Companies.Commands;
using QDomain.Enums;
using Shouldly;

namespace QAPI.IntegrationTests.Controllers;

public class CompanyControllerTest:IntegrationTestBase
{
    public CompanyControllerTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateCompany_ShouldReturnCreatedCompany_WhenValidRequest()
    {

        var token = await GetJwtToken(nameof(UserRoles.SystemAdmin));
        var createCompanyCommand = new CreateCompanyCommand(
            CompanyName: "Test Company",
            Address: "Test Address",
            EmailAddress: "company@test.com",
            PhoneNumber: "+992987654321");

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await Client.PostAsJsonAsync("/api/Company", createCompanyCommand);
        
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CompanyResponseModel>();
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
    }


    [Fact]
    public async Task GetCompany_ExistingCompany_ReturnsSuccess()
    {
        var token = await GetJwtToken(nameof(UserRoles.SystemAdmin));
        var createCompanyCommand = new CreateCompanyCommand(
            CompanyName: "Test Company",
            Address: "Test Address",
            EmailAddress: "company@test.com",
            PhoneNumber: "+992987654321");

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var createResponse = await Client.PostAsJsonAsync("/api/Company", createCompanyCommand);
        createResponse.EnsureSuccessStatusCode();

        var createdCompany = await createResponse.Content.ReadFromJsonAsync<CompanyResponseModel>();
        var companyId = createdCompany?.Id;
        var response = await Client.GetAsync($"/api/Company/{companyId}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CompanyResponseModel>();

        Assert.NotNull(result);
        Assert.Equal(companyId, result.Id);
    }
}