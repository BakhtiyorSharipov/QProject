using MediatR;

namespace QApplication.UseCases.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string EmailAddress) : IRequest<bool>;
