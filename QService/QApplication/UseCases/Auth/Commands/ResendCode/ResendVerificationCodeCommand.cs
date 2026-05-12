using MediatR;

namespace QApplication.UseCases.Auth.Commands.ResendCode;

public record ResendVerificationCodeCommand(string EmailAddress): IRequest<bool>;