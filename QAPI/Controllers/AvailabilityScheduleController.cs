using System.Collections.Immutable;
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
    private readonly ILogger<AvailabilityScheduleController> _logger;

    public AvailabilityScheduleController(IAvailabilityScheduleService service, ILogger<AvailabilityScheduleController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<AvailabilityScheduleResponseModel>> GetAll(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all schedules. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var schedules= _service.GetAll(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {schedulesCount} schedules.", schedules.Count());
        return Ok(schedules);
    }

    [HttpGet("{id}")]
    public ActionResult< AvailabilityScheduleResponseModel> GetById([FromRoute]int id)
    {
        _logger.LogInformation("Received request to get schedule by Id: {scheduleId}", id);
        var schedule= _service.GetById(id);
        _logger.LogInformation("Successfully returned schedules with Id: {scheduleId}", id);
        return Ok(schedule);
    }

    [HttpPost]
    public ActionResult Post([FromBody] CreateAvailabilityScheduleRequest request)
    {
        _logger.LogInformation("Received request to create new schedule.");
        var schedule = _service.Add(request);
        _logger.LogInformation("Successfully created schedule with Id: {scheduleId}", schedule.Select(s=>s.Id));
        return Created(nameof(GetById), schedule);
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateAvailabilityScheduleRequest request,[FromQuery] bool updateAllSlots)
    {
        _logger.LogInformation("Received request to update schedule with Id: {scheduleId}", id);
        var update = _service.Update(id, request, updateAllSlots);
        _logger.LogInformation("Successfully updated schedule with Id: {scheduleId}", id);
        return Ok(update);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id, [FromQuery] bool deleteAllSlots)
    {
        _logger.LogInformation("Received request to delete company with Id: {scheduleId}", id);
        var delete = _service.Delete(id, deleteAllSlots);
        _logger.LogInformation("Successfully deleted company with Id: {scheduleId}", id);
        return NoContent();
    }
}