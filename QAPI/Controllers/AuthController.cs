using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests;

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
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestModel request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] RefreshTokenRequest request)
    {
       await _authService.LogoutAsync(request.RefreshToken);
        return NoContent();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCustomerRequestModel request)
    {
        var user =await _authService.RegisterCustomerAsync(request);
        return CreatedAtAction(null, new { id = user.Id }, new { user.Id, user.EmailAddress, Role = user.Roles.ToString() });
    }
    
}