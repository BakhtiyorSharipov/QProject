using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests;
using QDomain.Models;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var result = _authService.Login(request);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = _authService.Refresh(request);
        return Ok(result);
    }

    [HttpPost("logout")]
    public IActionResult Logout([FromBody] RefreshTokenRequest request)
    {
        _authService.Logout(request.RefreshToken);
        return NoContent();
    }

    public IActionResult Register([FromBody] RegisterCustomerRequestModel request)
    {
        var user = _authService.RegisterCustomer(request);
        return CreatedAtAction(null, new { id = user.Id }, new { user.Id, user.EmailAddress, Role = user.Roles.ToString() });
    }
    
}