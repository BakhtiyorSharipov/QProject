using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.EmployeeRequest;
using QApplication.Responses;
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

    [HttpGet]
    public ActionResult<IEnumerable<EmployeeResponseModel>> GetAll(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all employees. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var employees= _service.GetAll(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {employeeCount} employees.", employees.Count());
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public ActionResult<EmployeeResponseModel> GetById([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get employee with Id: {employeeId}", id);
        var employee= _service.GetById(id);
        _logger.LogInformation("Successfully returned employee with Id: {employeeId}", id);
        return Ok(employee);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateEmployeeRequest request)
    {
        _logger.LogInformation("Received request to create employee with EmployeeName: {name}", request.FirstName );
        var employee = _service.Add(request);
        _logger.LogInformation("Successfully created employee with Id: {employeeId}", employee.Id);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [HttpPut("{id}")]
    public IActionResult Put([FromRoute] int id, [FromBody] UpdateEmployeeRequest request)
    {
        _logger.LogInformation("Received request to update employee with Id: {employeeId}", id);
        var update=_service.Update(id, request);
        _logger.LogInformation("Successfully updated employee with Id: {employeeId}", id);
        return Ok(update);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete employee with Id: {employeeId}", id);
       var delete= _service.Delete(id);
       _logger.LogInformation("Successfully deleted employee with Id: {employeeId}", id);
       return NoContent();
    }
}