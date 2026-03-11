using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.Companies.Commands.CreateCompany;

public record CreateCompanyCommand(string CompanyName, string Address, string EmailAddress, string PhoneNumber)
    : IRequest<CompanyResponseModel>;