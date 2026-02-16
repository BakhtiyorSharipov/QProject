using MediatR;

namespace QApplication.UseCases.Auth.Commands.Logout;

public record LogoutCommand(string refreshToken): IRequest;