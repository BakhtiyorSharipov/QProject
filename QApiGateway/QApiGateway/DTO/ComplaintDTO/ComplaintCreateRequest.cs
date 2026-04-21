namespace QApiGateway.DTO.ComplaintDTO;

public class ComplaintCreateRequest
{
    public int QueueId { get; set; }
    public string ComplaintText { get; set; }
}