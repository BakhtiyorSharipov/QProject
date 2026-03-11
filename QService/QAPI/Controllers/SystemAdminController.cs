using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Requests;
using QApplication.UseCases.Auth.Commands.CreateCompanyAdmin;
using QDomain.Enums;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRoles.SystemAdmin))]
public class SystemAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public SystemAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost("create-company-admin")]
    public async Task<IActionResult> CreateCompanyAdminAsync([FromBody] CreateCompanyAdminRequest request)
    {
        var creatorId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        var command = new CreateCompanyAdminCommand(request.CompanyId, request.EmailAddress, request.Password,
            request.FirstName, request.LastName, request.Position, request.PhoneNumber, creatorId);
        var user = await _mediator.Send(command);
        return CreatedAtAction(null, new { id = user.Id },
            new { user.Id, user.EmailAddress, Role = user.Roles.ToString() });
    }
}