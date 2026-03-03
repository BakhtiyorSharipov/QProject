// QBranchService.Contracts.Responses/QueueCreationValidationResponse.cs
using MessagePack;

namespace QBranchService.Contracts.Responses;

[MessagePackObject]
public class QueueCreationValidationResponse
{
    [Key(0)]
    public Guid RequestId { get; set; }
    
    // Overall validation
    [Key(1)]
    public bool IsValid { get; set; }
    
    [Key(2)]
    public string? ErrorMessage { get; set; }
    
    // Working hours validation
    [Key(3)]
    public bool IsWithinWorkingHours { get; set; }
    
    [Key(4)]
    public string? WorkingHoursMessage { get; set; }
    
    // Break time validation
    [Key(5)]
    public bool IsWithinBreakTime { get; set; }
    
    [Key(6)]
    public string? BreakTimeMessage { get; set; }
    
    // Max tickets per day (so QService can check against its own count)
    [Key(7)]
    public int MaxTicketsPerDay { get; set; }
}