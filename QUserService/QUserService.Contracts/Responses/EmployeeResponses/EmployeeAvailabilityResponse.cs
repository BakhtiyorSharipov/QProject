using MessagePack;

namespace QUserService.Contracts.Responses.EmployeeResponses;

[MessagePackObject]
public class EmployeeAvailabilityResponse
{
    [Key(0)]
    public bool IsAvailable { get; set; }
    
    [Key(1)]
    public List<TimeSlot> AvailableSlots { get; set; } = new();
    
    [Key(2)]
    public string? ErrorMessage { get; set; }
}