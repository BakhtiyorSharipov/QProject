using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUserService.Application.Requests;
using QUserService.Application.UseCases.Auth.Commands.CreateEmployee;
using QUserService.Domain.Enums;

namespace QUserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRoles.CompanyAdmin))]
public class CompanyAdminController: ControllerBase
{
    private readonly IMediator _mediator;

    public CompanyAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    
    [HttpPost("create-employee")]
    public async Task<IActionResult> CreateEmployeeAsync([FromBody] CreateEmployeeRoleRequest request)
    {
        var creatorId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        var command = new CreateEmployeeRoleCommand(request.BranchId, request.ServiceId, request.EmailAddress, request.Password,
            request.FirstName, request.LastName, request.Position, request.PhoneNumber, creatorId);
        var user = await _mediator.Send(command);
        return CreatedAtAction(null, new { id = user.Id },
            new { user.Id, user.EmailAddress, Role = user.Roles.ToString() });
    }
}