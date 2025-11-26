using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.EmployeeRequest;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController: ControllerBase
{
    private readonly IEmployeeService _service;
    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(IEmployeeService service, ILogger<EmployeeController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponseModel>>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all employees. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var employees=await _service.GetAllAsync(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {employeeCount} employees.", employees.Count());
        return Ok(employees);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeResponseModel>> GetByIdAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get employee with Id: {employeeId}", id);
        var employee=await _service.GetByIdAsync(id);
        _logger.LogInformation("Successfully returned employee with Id: {employeeId}", id);
        return Ok(employee);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateEmployeeRequest request)
    {
        _logger.LogInformation("Received request to create employee with EmployeeName: {name}", request.FirstName );
        var employee =await _service.AddAsync(request);
        _logger.LogInformation("Successfully created employee with Id: {employeeId}", employee.Id);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = employee.Id }, employee);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] UpdateEmployeeRequest request)
    {
        _logger.LogInformation("Received request to update employee with Id: {employeeId}", id);
        var update=await _service.UpdateAsync(id, request);
        _logger.LogInformation("Successfully updated employee with Id: {employeeId}", id);
        return Ok(update);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete employee with Id: {employeeId}", id);
       var delete= await _service.DeleteAsync(id);
       _logger.LogInformation("Successfully deleted employee with Id: {employeeId}", id);
       return NoContent();
    }
}