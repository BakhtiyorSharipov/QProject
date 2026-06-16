using QUserService.Domain.Enums;
using QUserService.Domain.Models;
using DayOfWeek = System.DayOfWeek;

namespace QUserService.Application.Responses;

public class AvailabilityScheduleResponseModel
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int? GroupId { get; set; }
    
    public string? Description { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public RepeatSlot RepeatSlot { get; set; }
    public int? RepeatDuration { get; set; }
    
    public List<Interval<DateTimeOffset>> AvailableSlots { get; set; } = [];
}