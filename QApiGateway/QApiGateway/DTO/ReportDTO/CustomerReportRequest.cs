namespace QApiGateway.DTO.ReportDTO;

public class CustomerReportRequest
{
    public int CustomerId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}