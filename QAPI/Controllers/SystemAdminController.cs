using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests;
using QApplication.Services;
using QDomain.Enums;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemAdminController: ControllerBase
{
    private readonly IAuthService _authService;

    public SystemAdminController(IAuthService authService)
    {
        _authService = authService;
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin))]
    [HttpPost("create-company-admin")]
    public async Task<IActionResult> CreateCompanyAdminAsync([FromBody] CreateCompanyAdminRequest request)
    {
        var creatorId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        var user = await _authService.CreateCompanyAdminAsync(request, creatorId);
        return CreatedAtAction(null, new { id = user.Id },
            new { user.Id, user.EmailAddress, Role = user.Roles.ToString() });
    }
    
    [Authorize(Roles = nameof(UserRoles.CompanyAdmin) + "," + nameof(UserRoles.SystemAdmin))]
    [HttpPost("create-employee")]
    public async Task<IActionResult> CreateEmployeeAsync([FromBody] CreateEmployeeRoleRequest roleRequest)
    {
        var creatorId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        var user =await _authService.CreateEmployeeAsync(roleRequest, creatorId);
        return CreatedAtAction(null, new { id = user.Id }, new { user.Id, user.EmailAddress, Role = user.Roles.ToString() });
    }
}