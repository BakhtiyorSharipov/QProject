using MediatR;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.AvailabilitySchedule.Commands.UpdateAvailabilitySchedule;

public class UpdateAvailabilityScheduleCommand : IRequest<AvailabilityScheduleResponseModel>
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public RepeatSlot RepeatSlot { get; set; } = RepeatSlot.None;
    public int? RepeatDuration { get; set; }
    public List<Interval<DateTimeOffset>> AvailableSlots { get; set; } = [];
    public bool UpdateAllSlots { get; set; }

    public UpdateAvailabilityScheduleCommand(int id, string? description, RepeatSlot repeatSlot, int? repeatDuration, List<Interval<DateTimeOffset>> availableSlots, bool updateAllSlots)
    {
        Id = id;
        Description = description;
        RepeatSlot = repeatSlot;
        RepeatDuration = repeatDuration;
        AvailableSlots = availableSlots;
        UpdateAllSlots = updateAllSlots;
    }
}