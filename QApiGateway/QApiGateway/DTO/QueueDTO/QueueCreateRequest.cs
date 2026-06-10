namespace QApiGateway.DTO.QueueDTO;

public class QueueCreateRequest
{
    public int CompanyId { get; set; }
    public int BranchId { get; set; }
    public int EmployeeId { get; set; }
    public int ServiceId { get; set; }
    public DateTimeOffset StartTime { get; set; }
}