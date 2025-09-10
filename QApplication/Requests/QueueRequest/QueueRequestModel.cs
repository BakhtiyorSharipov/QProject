namespace QApplication.Requests.QueueRequest;

public class QueueRequestModel: BaseRequest
{
    public int EmployeeId { get; set; }
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string DayOfWeek { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? CancelReason { get; set; }
}