using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUserService.Application.Requests;
using QUserService.Application.UseCases.Auth.Commands.DeleteCustomerAccount;
using QUserService.Application.UseCases.Auth.Commands.ForgotPassword;
using QUserService.Application.UseCases.Auth.Commands.Logout;
using QUserService.Application.UseCases.Auth.Commands.RegisterCustomer;
using QUserService.Application.UseCases.Auth.Commands.ResendCode;
using QUserService.Application.UseCases.Auth.Commands.ResetPassword;
using QUserService.Application.UseCases.Auth.Commands.UpdateUserPassword;
using QUserService.Application.UseCases.Auth.Commands.VerifyAccount;
using QUserService.Application.UseCases.Auth.Queries.Login;
using QUserService.Domain.Enums;

namespace QUserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController( IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutCommand request)
    {
        await _mediator.Send(request);
        return NoContent();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCustomerCommand request)
    {
        var user = await _mediator.Send(request);
        return CreatedAtAction(null, new { id = user.Id }, new { user.Id, user.EmailAddress, Role = user.Roles.ToString() });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        await _mediator.Send(new VerifyEmailCommand(request.EmailAddress, request.Code));
        return Ok("Email verified successfully");
    }
    
    [HttpPost("resend-code")]
    public async Task<IActionResult> ResendCode([FromBody] ResendCodeRequest request)
    {
        await _mediator.Send(new ResendVerificationCodeCommand(request.EmailAddress));
        return Ok("Code resent");
    }
    
    [Authorize(Roles = nameof(UserRoles.Customer))]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        _logger.LogInformation("Received request to change customer password" );
        var command = new UpdateUserPasswordCommand(request.OldPassword, request.NewPassword);
        await _mediator.Send(command);
       
        _logger.LogInformation("Successfully updated customer password");
        
        return Ok("Password updated successfully");
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _mediator.Send(new ForgotPasswordCommand(request.EmailAddress));
        return Ok("If email exists, code sent");
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok("Password updated");
    }

    [Authorize(Roles = nameof(UserRoles.Customer))]
    [HttpDelete("delete-customer-account")]
    public async Task<IActionResult> DeleteCustomerAccount()
    {
        _logger.LogInformation("Received request to delete customer account" );
        var command = new DeleteCustomerAccountCommand();
        await _mediator.Send(command);
        _logger.LogInformation("Successfully deleted customer account");
        return NoContent();
    }
    
}