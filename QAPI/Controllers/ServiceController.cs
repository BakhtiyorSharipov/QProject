using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.ServiceRequest;
using QApplication.Responses;

namespace QAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ServiceController: ControllerBase
{
    private readonly IServiceService _service;

    public ServiceController(IServiceService service)
    {
        _service = service;
    }

    [HttpGet]
    public IEnumerable<ServiceResponseModel> GetAll(int pageList, int pageNumber)
    {
        return _service.GetAll(pageList, pageNumber);
    }

    [HttpGet("{id}")]
    public ServiceResponseModel GetById([FromRoute] int id)
    {
        return _service.GetById(id);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateServiceRequest request)
    {
        var service = _service.Add(request);

        return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
    }

    [HttpPut("{id}")]
    public IActionResult Put([FromRoute] int id, [FromBody] UpdateServiceRequest request)
    {
       var update= _service.Update(id, request);
       return Ok(update);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
       var delete= _service.Delete(id);
       return NoContent();
    }
}