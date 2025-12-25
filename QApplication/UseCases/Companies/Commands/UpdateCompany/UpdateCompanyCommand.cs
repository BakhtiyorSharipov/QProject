using MediatR;
using QApplication.Responses;

namespace QApplication.UseCase.Companies.Commands.UpdateCompanyCommand;

public record UpdateCompanyCommand(int Id,string CompanyName, string Address, string EmailAddress, string PhoneNumber)
    : IRequest<CompanyResponseModel>;