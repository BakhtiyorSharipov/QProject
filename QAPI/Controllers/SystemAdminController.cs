using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Requests;
using QApplication.UseCases.Auth.Commands.CreateCompanyAdmin;
using QApplication.UseCases.Auth.Commands.CreateEmployee;
using QDomain.Enums;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemAdminController: ControllerBase
{
    private readonly IMediator _mediator;

    public SystemAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin))]
    [HttpPost("create-company-admin")]
    public async Task<IActionResult> CreateCompanyAdminAsync([FromBody] CreateCompanyAdminRequest request)
    {
        var creatorId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        var command = new CreateCompanyAdminCommand(request.ServiceId, request.EmailAddress, request.Password,
            request.FirstName, request.LastName, request.Position, request.PhoneNumber, creatorId);
        var user = await _mediator.Send(command);
        return CreatedAtAction(null, new { id = user.Id },
            new { user.Id, user.EmailAddress, Role = user.Roles.ToString() });
    }
    
    [Authorize(Roles = nameof(UserRoles.CompanyAdmin) + "," + nameof(UserRoles.SystemAdmin))]
    [HttpPost("create-employee")]
    public async Task<IActionResult> CreateEmployeeAsync([FromBody] CreateEmployeeRoleRequest request)
    {
        var creatorId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        var command = new CreateEmployeeRoleCommand(request.ServiceId, request.EmailAddress, request.Password,
            request.FirstName, request.LastName, request.Position, request.PhoneNumber, creatorId);
        var user = await _mediator.Send(command);
        return CreatedAtAction(null, new { id = user.Id },
            new { user.Id, user.EmailAddress, Role = user.Roles.ToString() });
    }
}