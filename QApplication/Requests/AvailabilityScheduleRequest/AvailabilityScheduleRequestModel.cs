using QDomain.Models;

namespace QApplication.Requests.AvailabilityScheduleRequest;

public class AvailabilityScheduleRequestModel : BaseRequest
{
    public int EmployeeId { get; set; }
    public string? Description { get; set; }
    public DayOfWeek DayOfWeek { get; set; }

    public List<Interval<DateTimeOffset>> AvailableSlots { get; set; } = [];
}