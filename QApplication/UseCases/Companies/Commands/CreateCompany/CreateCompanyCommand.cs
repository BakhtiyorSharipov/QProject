using MediatR;
using QApplication.Responses;

namespace QApplication.UseCase.Companies.Commands;

public record CreateCompanyCommand(string CompanyName, string Address, string EmailAddress, string PhoneNumber)
    : IRequest<CompanyResponseModel>;