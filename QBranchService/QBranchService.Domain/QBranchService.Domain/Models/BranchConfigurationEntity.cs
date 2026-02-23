namespace QBranchService.Domain.Models;

public class BranchConfigurationEntity
{
    public int Id { get; set; }
    public int MaxTickets { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public int BranchId { get; set; }
    public required BranchEntity Branch { get; set; } 
}