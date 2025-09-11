using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.CustomerRequest;
using QApplication.Responses;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController: ControllerBase
{
    private readonly ICustomerService _service;

    public CustomerController(ICustomerService service)
    {
        _service = service;
    }

    [HttpGet]
    public IEnumerable<CustomerResponseModel> GetAll(int pageList, int pageNumber)
    {
        return _service.GetAll(pageList, pageNumber);
    }

    [HttpGet("{id}")]
    public CustomerResponseModel GetById([FromRoute] int id)
    {
        return _service.GetById(id);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateCustomerRequest request)
    {
        var customer = _service.Add(request);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut("{id}")]
    public IActionResult Put([FromRoute] int id, [FromBody] UpdateCustomerRequest request)
    {
       var update= _service.Update(id,request);
       return Ok(update);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
       var delete= _service.Delete(id);
       return NoContent();
    }
    
    
    
    
    
}