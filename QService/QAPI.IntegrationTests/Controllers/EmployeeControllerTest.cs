using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MagicOnion;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using QAPI.IntegrationTests.Models;
using QApplication.Exceptions;
using QApplication.Responses;
using QApplication.UseCases.Employees.Commands.CreateEmployee;
using QBranchService.Contracts.Interfaces;
using QBranchService.Contracts.Requests;
using QBranchService.Contracts.Responses;
using QDomain.Enums;
using QInfrastructure.Persistence.DataBase;
using Shouldly;

namespace QAPI.IntegrationTests.Controllers;

public class EmployeeControllerTest : IntegrationTestBase
{
    private readonly IBranchService _branchServiceMock;

    public EmployeeControllerTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _branchServiceMock = factory.BranchServiceMock;
    }

    [Fact]
    public async Task CreateEmployee_ShouldReturnCreatedEmployee_WithValidRequest()
    {
        var token = await GetJwtToken(nameof(UserRoles.CompanyAdmin));
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
        var testData = await QueueTestData.SeedDatabaseAsync(db, token);
        var createEmployeeCommand = new CreateEmployeeCommand(
            ServiceId: testData.ServiceId,
            Firstname: "Test Customer",
            Lastname: "Test Customer",
            PhoneNumber: "+992987654321",
            Position: "Developer");

        _branchServiceMock.CheckCompanyServiceId(Arg.Any<CompanyServiceRequest>())
            .Returns(UnaryResult.FromResult(new CompanyServiceResponse { IsValid = true }));

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await Client.PostAsJsonAsync("/api/Employee", createEmployeeCommand);


        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<EmployeeResponseModel>();
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.ServiceId.ShouldBe(testData.ServiceId);
        
    }

    [Fact]
    public async Task GetEmployee_ExistingEmployee_ReturnsSuccess()
    {
        var token = await GetJwtToken(nameof(UserRoles.CompanyAdmin));
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
        var testData = await QueueTestData.SeedDatabaseAsync(db, token);
        
        var createEmployeeCommand = new CreateEmployeeCommand(
            ServiceId: testData.ServiceId,
            Firstname: "Test Customer",
            Lastname: "Test Customer",
            PhoneNumber: "+992987654321",
            Position: "Developer");

        _branchServiceMock.CheckCompanyServiceId(Arg.Any<CompanyServiceRequest>())
            .Returns(UnaryResult.FromResult(new CompanyServiceResponse { IsValid = true }));

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var createdEmployeeResponse = await Client.PostAsJsonAsync("/api/Employee", createEmployeeCommand);


        createdEmployeeResponse.EnsureSuccessStatusCode();
        var createdEmployeeResult = await createdEmployeeResponse.Content.ReadFromJsonAsync<EmployeeResponseModel>();
        createdEmployeeResult.ShouldNotBeNull();
        createdEmployeeResult.Id.ShouldNotBe(0);
        var employeeId = createdEmployeeResult.Id;
        var response = await Client.GetAsync($"api/Employee/{employeeId}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<EmployeeResponseModel>();
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.ServiceId.ShouldBe(testData.ServiceId);
        
    }

    [Fact]
    public async Task GetEmployee_NonExistentEmployee_ReturnsNotFound()
    {
        var token = await GetJwtToken(nameof(UserRoles.CompanyAdmin));
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
        var nonExistentEmployeeId = 999;
        var url = $"/api/Employee/{nonExistentEmployeeId}";

       
        var exception = await Assert.ThrowsAsync<HttpStatusCodeException>(() => Client.GetAsync(url));
        exception.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}