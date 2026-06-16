// using System.Net.Http.Headers;
// using System.Net.Http.Json;
// using MagicOnion;
// using Microsoft.Extensions.DependencyInjection;
// using NSubstitute;
// using QAPI.IntegrationTests.Models;
// using QApplication.Responses;
// using QApplication.UseCases.Queues.Commands.CreateQueue;
// using QBranchService.Contracts.Interfaces;
// using QBranchService.Contracts.Requests;
// using QBranchService.Contracts.Responses;
// using QDomain.Enums;
// using QInfrastructure.Persistence.DataBase;
// using Shouldly;
//
// namespace QAPI.IntegrationTests.Controllers;
//
// public class QueueControllerTest : IntegrationTestBase
// {
//     private readonly IBranchService _branchServiceMock;
//
//     public QueueControllerTest(CustomWebApplicationFactory factory) : base(factory)
//     {
//         _branchServiceMock = factory.BranchServiceMock;
//     }
//     
//
//     [Fact]
//     public async Task BookQueue_WithValidRequest_ShouldReturnSuccess()
//     {
//         var token = await GetJwtToken(nameof(UserRoles.Customer));
//         Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
//
//         using var scope = Factory.Services.CreateScope();
//         var db = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
//         var testData = await QueueTestData.SeedDatabaseAsync(db, token);
//
//         var command = new CreateQueueCommand(
//             CompanyId: testData.CompanyId,
//             BranchId: testData.BranchId,
//             ServiceId: testData.ServiceId,
//             EmployeeId: testData.EmployeeId,
//             StartTime: DateTime.UtcNow.Date.AddHours(10)
//         );
//
//         _branchServiceMock.ValidateQueueCreationAsync(Arg.Any<QueueCreationValidationRequest>())
//             .Returns(UnaryResult.FromResult(new QueueCreationValidationResponse
//             {
//                 IsValid = true,
//                 MaxTicketsPerDay = 100
//             }));
//
//         _branchServiceMock.CheckCompanyId(Arg.Any<CompanyRequest>())
//             .Returns(UnaryResult.FromResult(new CompanyResponse { IsValid = true }));
//
//         _branchServiceMock.CheckBranchId(Arg.Any<BranchRequest>())
//             .Returns(UnaryResult.FromResult(new BranchResponse { IsValid = true }));
//
//         _branchServiceMock.CheckCompanyServiceId(Arg.Any<CompanyServiceRequest>())
//             .Returns(UnaryResult.FromResult(new CompanyServiceResponse { IsValid = true }));
//
//         var response = await Client.PostAsJsonAsync("/api/Queue/book", command);
//
//         response.EnsureSuccessStatusCode();
//         var result = await response.Content.ReadFromJsonAsync<AddQueueResponseModel>();
//         
//         result.ShouldNotBeNull();
//         result.Id.ShouldBeGreaterThan(0);
//         result.Status.ShouldBe(QueueStatus.Pending);
//     }
//     
//     
// }