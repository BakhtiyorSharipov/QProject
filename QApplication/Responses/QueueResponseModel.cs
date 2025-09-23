using QDomain.Enums;

namespace QApplication.Responses;

public class QueueResponseModel: BaseResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public QueueStatus Status { get; set; }
}