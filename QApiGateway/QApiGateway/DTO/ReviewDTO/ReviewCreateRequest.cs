namespace QApiGateway.DTO.ReviewDTO;

public class ReviewCreateRequest
{
    public int QueueId { get; set; }
    public int Grade { get; set; }
    public string? ReviewText { get; set; }
}