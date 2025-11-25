using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Requests;
using QApplication.Services;
using QDomain.Enums;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemAdminController: ControllerBase
{
    private readonly AuthService _authService;

    public SystemAdminController(AuthService authService)
    {
        _authService = authService;
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin))]
    [HttpPost("create-company-admin")]
    public IActionResult CreateCompanyAdmin([FromBody] CreateCompanyAdminRequest request)
    {
        var creatorId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        var user = _authService.CreateCompanyAdmin(request, creatorId);
        return CreatedAtAction(null, new { id = user.Id },
            new { user.Id, user.EmailAddress, Role = user.Roles.ToString() });
    }
    
    [Authorize(Roles = nameof(UserRoles.CompanyAdmin) + "," + nameof(UserRoles.SystemAdmin))]
    [HttpPost("create-employee")]
    public IActionResult CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        var creatorId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        var user = _authService.CreateEmployee(request, creatorId);
        return CreatedAtAction(null, new { id = user.Id }, new { user.Id, user.EmailAddress, Role = user.Roles.ToString() });
    }
}