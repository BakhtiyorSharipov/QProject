using MessagePack;

namespace QUserService.Contracts.Responses.EmployeeResponses;

[MessagePackObject]
public class TimeSlot
{
    [Key(0)]
    public DateTimeOffset From { get; set; }
    
    [Key(1)]
    public DateTimeOffset To { get; set; }
}