using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Responses;
using QApplication.UseCases.BlockedCustomers.Commands.CreateBlockedCustomer;
using QApplication.UseCases.BlockedCustomers.Commands.DeleteBlockedCustomer;
using QApplication.UseCases.BlockedCustomers.Queries.GetAllBlockedCustomers;
using QApplication.UseCases.BlockedCustomers.Queries.GetBlockedCustomerById;
using QDomain.Enums;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlockedCustomerController : ControllerBase
{
    private readonly ILogger<BlockedCustomerController> _logger;
    private readonly IMediator _mediator;

    public BlockedCustomerController(ILogger<BlockedCustomerController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin) + ","+ nameof(UserRoles.Employee))]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlockedCustomerResponseModel>>> GetAllAsync([FromQuery] int pageNumber=1)
    {
        _logger.LogInformation("Received request to get all schedules. PageNumber: {PageNumber}, PageSize: 15",
            pageNumber);
        var query = new GetAllBlockedCustomersQuery(pageNumber);
        var blockedCustomers = await _mediator.Send(query);
        return Ok(blockedCustomers);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin) + ","+ nameof(UserRoles.Employee))]
    [HttpGet("{id}")]
    public async Task<ActionResult<BlockedCustomerResponseModel>> GetById([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get blocked customer by Id: {blockedCustomer}", id);
        var query = new GetBlockedCustomerByIdQuery(id);
        var blocked = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned blocked customers with Id: {blockedCustomer}", id);
        return Ok(blocked);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin) + ","+ nameof(UserRoles.Employee))]
    [HttpPost("block")]
    public async Task<IActionResult> Block([FromBody] CreateBlockedCustomerCommand request)
    {
        _logger.LogInformation("Received request to block customer with Id: {customerId}", request.CustomerId);
        var blocked = await _mediator.Send(request);
        _logger.LogInformation("Successfully blocked customer with Id: {customerId}", request.CustomerId);
        return CreatedAtAction(nameof(GetById), new { id = blocked.Id }, blocked);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin) + ","+ nameof(UserRoles.Employee))]
    [HttpDelete("{id}/unblock")]
    public async Task<IActionResult> Unblock([FromRoute] int id)
    {
        _logger.LogInformation("Received request to unblock customer with Id: {customerId}", id);
        var command = new DeleteBlockedCustomerCommand(id);
        await _mediator.Send(command);
        _logger.LogInformation("Successfully unblocked customer with Id: {customerId}", id);
        return NoContent();
    }
}