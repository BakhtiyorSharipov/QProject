using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests;
using QApplication.UseCases.Auth.Commands.Logout;
using QApplication.UseCases.Auth.Commands.RegisterCustomer;
using QApplication.UseCases.Auth.Queries.Login;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController( IMediator mediator)
    {
        _mediator = mediator;
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
    
}