using MediatR;
using QApplication.Responses.ReportResponse;

namespace QApplication.UseCases.Customers.Commands.UpdateCustomerProfile;

public record UpdateCustomerProfileCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    DateTime? DateOfBirth,
    string? Gender,
    string? Country,
    string? City,
    string? Address,
    string? PostalCode) : IRequest<CustomerProfileResponse>;