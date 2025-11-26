namespace QApplication.Requests.ComplaintRequest;

public class ComplaintRequestModel: BaseRequest
{
    public int QueueId { get; set; }
    public int CustomerId { get; set; }
    public string ComplaintText { get; set; }
    
}