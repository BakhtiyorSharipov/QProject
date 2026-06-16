using QUserService.Domain.Enums;
using QUserService.Domain.Models;

namespace QUserService.Application.Requests.AvailabilityScheduleRequest;

public class UpdateAvailabilityScheduleRequest
{
    public string? Description { get; set; }
    public RepeatSlot RepeatSlot { get; set; } = RepeatSlot.None;
    public int? RepeatDuration { get; set; }
    public List<Interval<DateTimeOffset>> AvailableSlots { get; set; } = [];
}