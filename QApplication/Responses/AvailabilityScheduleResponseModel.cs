using QDomain.Models;

namespace QApplication.Responses;

public class AvailabilityScheduleResponseModel: BaseResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    
    public string? Description { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    
    public List<Interval<DateTimeOffset>> AvailableSlots { get; set; } = [];
}