using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.Companies.Commands.UpdateCompany;

public record UpdateCompanyCommand(int Id,string CompanyName, string Address, string EmailAddress, string PhoneNumber) : IRequest<CompanyResponseModel>;
