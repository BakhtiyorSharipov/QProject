using QDomain.Enums;

namespace QApplication.Requests.QueueRequest;

public class UpdateQueueRequest
{
   public int QueueId { get; set; }
   public QueueStatus newStatus { get; set; }
}