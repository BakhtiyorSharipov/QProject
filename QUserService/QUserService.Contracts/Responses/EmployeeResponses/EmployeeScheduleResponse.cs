using MessagePack;

namespace QUserService.Contracts.Responses.EmployeeResponses;

[MessagePackObject]
public class EmployeeScheduleResponse
{
    [Key(0)]
    public int EmployeeId { get; set; }
    
    [Key(1)]
    public DateTimeOffset Date { get; set; }
    
    [Key(2)]
    public List<TimeSlot> WorkingHours { get; set; } = new();
    
    [Key(3)]
    public List<TimeSlot> BookedSlots { get; set; } = new();
}