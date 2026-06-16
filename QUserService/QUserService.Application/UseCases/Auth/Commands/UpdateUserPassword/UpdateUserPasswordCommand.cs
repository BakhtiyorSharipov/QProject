using MediatR;

namespace QUserService.Application.UseCases.Auth.Commands.UpdateUserPassword;

public record UpdateUserPasswordCommand( string OldPassword, string NewPassword): IRequest<bool>;