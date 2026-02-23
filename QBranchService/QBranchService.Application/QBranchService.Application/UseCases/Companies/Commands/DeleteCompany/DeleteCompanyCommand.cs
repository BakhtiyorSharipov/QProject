using MediatR;

namespace QBranchService.Application.UseCases.Companies.Commands.DeleteCompany;

public record DeleteCompanyCommand(int Id): IRequest<bool>;