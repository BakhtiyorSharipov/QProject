namespace QBranchService.Contracts.Responses;

public class BranchConfigurationResponse
{
    public Guid RequestId { get; set; }
    public int BranchId { get; set; }
    public bool IsOpen { get; set; }
    public int MaxTickets { get; set; }
    public int CurrentTicket { get; set; }
    public bool CanAcceptTicket { get; set; }
    public string? ErrorMessage { get; set; }
}