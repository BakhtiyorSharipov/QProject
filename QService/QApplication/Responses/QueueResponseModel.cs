using QDomain.Enums;

namespace QApplication.Responses;

public class QueueResponseModel: BaseResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public QueueStatus Status { get; set; }
}