namespace QApiGateway.DTO.QueueDTO;

public class QueueCancelRequest
{
    public int QueueId { get; set; }
    public string? CancelReason { get; set; }
}