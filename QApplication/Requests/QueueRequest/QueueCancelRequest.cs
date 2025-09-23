namespace QApplication.Requests.QueueRequest;

public class QueueCancelRequest
{
    public int QueueId { get; set; }
    public string? CancelReason { get; set; }
}