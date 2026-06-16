using MediatR;

namespace QUserService.Application.UseCases.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken): IRequest;