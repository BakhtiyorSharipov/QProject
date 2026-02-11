using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Companies.Commands.CreateCompany;

public record CreateCompanyCommand(string CompanyName, string Address, string EmailAddress, string PhoneNumber)
    : IRequest<CompanyResponseModel>;