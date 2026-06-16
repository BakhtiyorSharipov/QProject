using MediatR;

namespace QUserService.Application.UseCases.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string EmailAddress) : IRequest<bool>;
