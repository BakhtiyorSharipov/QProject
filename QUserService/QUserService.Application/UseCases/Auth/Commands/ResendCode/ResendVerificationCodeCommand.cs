using MediatR;

namespace QUserService.Application.UseCases.Auth.Commands.ResendCode;

public record ResendVerificationCodeCommand(string EmailAddress): IRequest<bool>;