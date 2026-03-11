namespace QBranchService.Application.Requests;

public class UpdateBranchConfigurationRequest
{
    public int MaxTickets { get; set; }
    public TimeOnly OpenTime { get; set; } 
    public TimeOnly CloseTime { get; set; }
}