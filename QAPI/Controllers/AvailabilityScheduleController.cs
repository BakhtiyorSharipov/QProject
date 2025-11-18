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
    public async Task<ActionResult<IEnumerable<AvailabilityScheduleResponseModel>>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all schedules. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var schedules=await _service.GetAllAsync(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {schedulesCount} schedules.", schedules.Count());
        return Ok(schedules);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult< AvailabilityScheduleResponseModel>> GetByIdAsync([FromRoute]int id)
    {
        _logger.LogInformation("Received request to get schedule by Id: {scheduleId}", id);
        var schedule=await _service.GetByIdAsync(id);
        _logger.LogInformation("Successfully returned schedules with Id: {scheduleId}", id);
        return Ok(schedule);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] CreateAvailabilityScheduleRequest request)
    {
        _logger.LogInformation("Received request to create new schedule.");
        var schedule =await _service.AddAsync(request);
        _logger.LogInformation("Successfully created schedule with Id: {scheduleId}", schedule.Select(s=>s.Id));
        return Created(nameof(GetByIdAsync), schedule);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateAvailabilityScheduleRequest request,[FromQuery] bool updateAllSlots)
    {
        _logger.LogInformation("Received request to update schedule with Id: {scheduleId}", id);
        var update = await _service.UpdateAsync(id, request, updateAllSlots);
        _logger.LogInformation("Successfully updated schedule with Id: {scheduleId}", id);
        return Ok(update);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id, [FromQuery] bool deleteAllSlots)
    {
        _logger.LogInformation("Received request to delete company with Id: {scheduleId}", id);
        var delete =await _service.DeleteAsync(id, deleteAllSlots);
        _logger.LogInformation("Successfully deleted company with Id: {scheduleId}", id);
        return NoContent();
    }
}