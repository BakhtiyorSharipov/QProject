using QDomain.Enums;
using QDomain.Models;
using DayOfWeek = System.DayOfWeek;

namespace QApplication.Requests.AvailabilityScheduleRequest;

public class AvailabilityScheduleRequestModel : BaseRequest
{
    public int EmployeeId { get; set; }
    public string? Description { get; set; }
    public RepeatSlot RepeatSlot { get; set; } = RepeatSlot.None;
    public int? RepeatDuration { get; set; }
    public List<Interval<DateTimeOffset>> AvailableSlots { get; set; } = [];
}