using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using QApplication.Exceptions;
using QApplication.Responses;
using QApplication.UseCases.Customers.Commands.CreateCustomer;
using QDomain.Enums;
using Shouldly;

namespace QAPI.IntegrationTests.Controllers;

public class CustomerControllerTest: IntegrationTestBase
{
    
    public CustomerControllerTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnCreatedCustomer_WithValidRequest()
    {
        var token = await GetJwtToken(nameof(UserRoles.SystemAdmin));
        var createCustomerCommand = new CreateCustomerCommand(
            Firstname: "Test Customer",
            Lastname: "Test Customer",
            PhoneNumber: "+992987654321");
        
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await Client.PostAsJsonAsync("/api/Customer", createCustomerCommand);
        
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CustomerResponseModel>();
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        
    }
    
    [Fact]
    public async Task GetCustomer_ExistingCustomer_ReturnsSuccess()
    {
        var token = await GetJwtToken(nameof(UserRoles.SystemAdmin));
        var createCustomerCommand = new CreateCustomerCommand(
            Firstname: "Test Customer",
            Lastname: "Test Customer",
            PhoneNumber: "+992987654321");

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var createResponse = await Client.PostAsJsonAsync("/api/Customer", createCustomerCommand);
        createResponse.EnsureSuccessStatusCode();

        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerResponseModel>();
        var customerId = createdCustomer?.Id;
        var response = await Client.GetAsync($"/api/Customer/{customerId}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CustomerResponseModel>();

        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
    }
    
    [Fact]
    public async Task GetCustomer_NonExistentCustomer_ReturnsNotFound()
    {
        
        var token = await GetJwtToken(nameof(UserRoles.SystemAdmin));
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
        var nonExistentCustomerId = 999;
        var url = $"/api/Customer/{nonExistentCustomerId}";

       
        var exception = await Assert.ThrowsAsync<HttpStatusCodeException>(() => Client.GetAsync(url));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }
}