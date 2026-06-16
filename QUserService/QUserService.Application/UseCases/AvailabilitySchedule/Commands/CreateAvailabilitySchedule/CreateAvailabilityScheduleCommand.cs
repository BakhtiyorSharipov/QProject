using MediatR;
using QUserService.Application.Responses;
using QUserService.Domain.Enums;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.AvailabilitySchedule.Commands.CreateAvailabilitySchedule;

public class CreateAvailabilityScheduleCommand: IRequest<List<AvailabilityScheduleResponseModel>>
{

    public string? Description { get; set; }
    public RepeatSlot RepeatSlot { get; set; } = RepeatSlot.None;
    public int? RepeatDuration { get; set; }
    public List<Interval<DateTimeOffset>> AvailableSlots { get; set; } = [];

    public CreateAvailabilityScheduleCommand( string? description, RepeatSlot repeatSlot, int? repeatDuration)
    {
        Description = description;
        RepeatSlot = repeatSlot;
        RepeatDuration = repeatDuration;
    }
}