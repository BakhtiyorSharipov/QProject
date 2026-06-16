using MediatR;

namespace QUserService.Application.UseCases.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(
    string EmailAddress,
    string Code,
    string NewPassword,
    string ConfirmPassword): IRequest<bool>;