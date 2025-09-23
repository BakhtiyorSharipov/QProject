namespace QApplication.Requests.QueueRequest;

public class QueueRequestModel: BaseRequest
{
    public int EmployeeId { get; set; }
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public DateTime StartTime { get; set; }
}