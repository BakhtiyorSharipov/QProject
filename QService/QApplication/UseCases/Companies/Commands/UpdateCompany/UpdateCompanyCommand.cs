using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Companies.Commands.UpdateCompany;

public record UpdateCompanyCommand(int Id,string CompanyName, string Address, string EmailAddress, string PhoneNumber)
    : IRequest<CompanyResponseModel>;