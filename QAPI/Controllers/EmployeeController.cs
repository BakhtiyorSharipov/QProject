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

    public EmployeeController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    public IEnumerable<EmployeeResponseModel> GetAll(int pageList, int pageNumber)
    {
        return _service.GetAll(pageList, pageNumber);
    }

    [HttpGet("{id}")]
    public EmployeeResponseModel GetById([FromRoute] int id)
    {
        return _service.GetById(id);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateEmployeeRequest request)
    {
        var employee = _service.Add(request);

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [HttpPut("{id}")]
    public IActionResult Put([FromRoute] int id, [FromBody] UpdateEmployeeRequest request)
    {
        var update=_service.Update(id, request);
        return Ok(update);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
       var delete= _service.Delete(id);
       return NoContent();
    }
}