using System.Net.Http.Json;
using QBranchService.Application.Response;
using QBranchService.Application.UseCases.Branches.Commands.CreateBranch;
using QBranchService.Application.UseCases.Companies.Commands.CreateCompany;
using Shouldly;

namespace QBranchService.API.IntegrationTest.Controllers;

public class BranchControllerTest: IClassFixture<QBranchServiceWebApplicationFactory>
{
   private readonly HttpClient _client;
   private readonly QBranchServiceWebApplicationFactory _factory;

   public BranchControllerTest(QBranchServiceWebApplicationFactory factory)
   {
      _factory = factory;
      _client = factory.CreateClient();
   }

   [Fact]
   public async Task CreateBranch_ShouldReturnBranchCreatedBranch_WithValidRequest()
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
   }
}