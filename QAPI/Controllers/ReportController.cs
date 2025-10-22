using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Responses.ReportResponse;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("employee/report")]
    public EmployeeReportResponseModel GetEmployeeReport([FromRoute] int employeeId)
    {
        return _reportService.GetEmployeeReport(employeeId);
    }

    [HttpGet("queue/report")]
    public QueueReportResponseModel GetQueueReport()
    {
        return _reportService.GetQueueReport();
    }

    [HttpGet("complaint/report")]
    public ComplaintReportResponseModel GetComplaintReport()
    {
        return _reportService.GetComplaintReport();
    }

    [HttpGet("review/report")]
    public ReviewReportResponseModel GetReviewReport()
    {
        return _reportService.GetReviewReport();
    }
}