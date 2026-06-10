namespace QBranchService.Application.Response;

public class BranchConfigurationResponseModel
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public int MaxTicketsPerDay { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public TimeOnly? BreakStartTime { get; set; }
    public TimeOnly? BreakEndTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; } 
}