using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.AvailabilityScheduleRequest;
using QApplication.Responses;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailabilityScheduleController: ControllerBase
{
    private readonly IAvailabilityScheduleService _service;

    public AvailabilityScheduleController(IAvailabilityScheduleService service)
    {
        _service = service;
    }

    [HttpGet]
    public IEnumerable<AvailabilityScheduleResponseModel> GetAll(int pageList, int pageNumber)
    {
        return _service.GetAll(pageList, pageNumber);
    }

    [HttpGet("{id}")]
    public AvailabilityScheduleResponseModel GetById([FromRoute]int id)
    {
        return _service.GetById(id);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateAvailabilityScheduleRequest request)
    {
        var schedule = _service.Add(request);
        return Created(nameof(GetById), schedule);
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateAvailabilityScheduleRequest request)
    {
        var update = _service.Update(id, request);
        return Ok(update);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var delete = _service.Delete(id);
        return NoContent();
    }
}