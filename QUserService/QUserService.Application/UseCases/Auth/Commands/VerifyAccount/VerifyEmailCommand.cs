using MediatR;

namespace QUserService.Application.UseCases.Auth.Commands.VerifyAccount;

public record VerifyEmailCommand(string EmailAddress, string Code): IRequest<bool>;