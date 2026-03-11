namespace QBranchService.Application.Requests;

public class UpdateBranchConfigurationRequest
{
    public int MaxTicketsPerDay { get; set; }
    public TimeOnly OpenTime { get; set; } 
    public TimeOnly CloseTime { get; set; }
    public TimeOnly? BreakStartTime { get; set; }
    public TimeOnly? BreakEndTime { get; set; }
    
}