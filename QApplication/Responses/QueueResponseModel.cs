namespace QApplication.Responses;

public class QueueResponseModel: BaseResponse
{
    public int EmployeeId { get; set; }
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    
    public string DayOfWeek { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? CancelReason { get; set; }
}