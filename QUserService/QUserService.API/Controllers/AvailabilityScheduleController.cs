using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUserService.Application.Requests.AvailabilityScheduleRequest;
using QUserService.Application.Responses;
using QUserService.Application.UseCases.AvailabilitySchedule.Commands.CreateAvailabilitySchedule;
using QUserService.Application.UseCases.AvailabilitySchedule.Commands.DeleteAvailabilitySchedule;
using QUserService.Application.UseCases.AvailabilitySchedule.Commands.UpdateAvailabilitySchedule;
using QUserService.Application.UseCases.AvailabilitySchedule.Queries.GetAllAvailabilitySchedules;
using QUserService.Application.UseCases.AvailabilitySchedule.Queries.GetAvailabilityScheduleById;
using QUserService.Domain.Enums;

namespace QUserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRoles.Employee))]
public class AvailabilityScheduleController : ControllerBase
{
    private readonly ILogger<AvailabilityScheduleController> _logger;
    private readonly IMediator _mediator;

    public AvailabilityScheduleController(ILogger<AvailabilityScheduleController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AvailabilityScheduleResponseModel>>> GetAllAsync([FromQuery]int pageNumber=1)
    {
        _logger.LogInformation("Received request to get all schedules. PageNumber: {PageNumber}, PageSize: 15",
            pageNumber);
        var query = new GetAllAvailabilitySchedulesQuery(pageNumber);
        var schedules = await _mediator.Send(query);
        return Ok(schedules);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AvailabilityScheduleResponseModel>> GetByIdAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get schedule by Id: {scheduleId}", id);
        var query = new GetAvailabilityScheduleByIdQuery(id);
        var schedule = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned schedules with Id: {scheduleId}", id);
        return Ok(schedule);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] CreateAvailabilityScheduleCommand request)
    {
        _logger.LogInformation("Received request to create new schedule.");
        var schedule = await _mediator.Send(request);
        _logger.LogInformation("Successfully created schedule with Id: {scheduleId}", schedule.Select(s => s.Id));
        return Ok(schedule);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id,
        [FromBody] UpdateAvailabilityScheduleRequest request, [FromQuery] bool updateAllSlots)
    {
        _logger.LogInformation("Received request to update schedule with Id: {scheduleId}", id);
        var command = new UpdateAvailabilityScheduleCommand(id, request.Description, request.RepeatSlot,
            request.RepeatDuration, request.AvailableSlots, updateAllSlots);
        var update = await _mediator.Send(command);
        _logger.LogInformation("Successfully updated schedule with Id: {scheduleId}", id);
        return Ok(update);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id, [FromQuery] bool deleteAllSlots)
    {
        _logger.LogInformation("Received request to delete company with Id: {scheduleId}", id);
        var command = new DeleteAvailabilityScheduleCommand(id, deleteAllSlots);
        await _mediator.Send(command);
        _logger.LogInformation("Successfully deleted company with Id: {scheduleId}", id);
        return NoContent();
    }
}