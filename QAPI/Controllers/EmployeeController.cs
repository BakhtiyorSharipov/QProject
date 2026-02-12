using System.Runtime.InteropServices.ComTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.EmployeeRequest;
using QApplication.Responses;
using QApplication.UseCases.Employees.Commands.CreateEmployee;
using QApplication.UseCases.Employees.Commands.DeleteEmployee;
using QApplication.UseCases.Employees.Commands.UpdateEmployee;
using QApplication.UseCases.Employees.Queries.GetAllEmployees;
using QApplication.UseCases.Employees.Queries.GetEmployeeById;
using QDomain.Enums;
using QDomain.Models;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly IMediator _mediator;

    public EmployeeController(ILogger<EmployeeController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponseModel>>> GetAllAsync([FromQuery] int pageNumber=1)
    {
        _logger.LogInformation("Received request to get all employees. PageNumber: {PageNUmber}, PageSize: 15",
            pageNumber);
        var query = new GetAllEmployeesQuery(pageNumber);
        var employees = await _mediator.Send(query);

        return Ok(employees);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeResponseModel>> GetByIdAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get employee with Id: {employeeId}", id);
        var query = new GetEmployeeByIdQuery(id);
        var employee = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned employee with Id: {employeeId}", id);
        return Ok(employee);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateEmployeeCommand request)
    {
        _logger.LogInformation("Received request to create employee with EmployeeName: {name}", request.Firstname);
        var employee = await _mediator.Send(request);
        _logger.LogInformation("Successfully created employee with Id: {employeeId}", employee.Id);
        return Ok(employee);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] UpdateEmployeeRequest request)
    {
        _logger.LogInformation("Received request to update employee with Id: {employeeId}", id);
        var command = new UpdateEmployeeCommand(id, request.ServiceId, request.FirstName, request.LastName,
            request.Position, request.PhoneNumber);
        var update = await _mediator.Send(command);
        _logger.LogInformation("Successfully updated employee with Id: {employeeId}", id);
        return Ok(update);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete employee with Id: {employeeId}", id);
        var command = new DeleteEmployeeCommand(id);
        await _mediator.Send(command);
        _logger.LogInformation("Successfully deleted employee with Id: {employeeId}", id);
        return NoContent();
    }
}