using MediatR;

namespace QApplication.UseCase.Companies.Commands.DeleteCompanyCommand;

public record DeleteCompanyCommand(int Id) : IRequest<bool>;
