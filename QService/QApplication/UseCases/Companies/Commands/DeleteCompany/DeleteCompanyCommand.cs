using MediatR;

namespace QApplication.UseCases.Companies.Commands.DeleteCompany;

public record DeleteCompanyCommand(int Id) : IRequest<bool>;
