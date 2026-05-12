using MediatR;

namespace QApplication.UseCases.Auth.Commands.VerifyAccount;

public record VerifyEmailCommand(string EmailAddress, string Code): IRequest<bool>;