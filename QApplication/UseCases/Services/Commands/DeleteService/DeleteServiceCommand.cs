using MediatR;

namespace QApplication.UseCases.Services.Commands.DeleteService;

public record DeleteServiceCommand(int Id): IRequest<bool>;