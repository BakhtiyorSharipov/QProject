using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.ReportRequest;
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

    [HttpGet("company/report")]
    public CompanyReportItemResponseModel GetCompanyReport([FromQuery] CompanyReportRequest request)
    {
        return _reportService.GetCompanyReport(request);
    }
    
    [HttpGet("employee/report ")]
    public EmployeeReportResponseModel GetEmployeeReport([FromQuery] EmployeeReportRequest request)
    {
        return _reportService.GetEmployeeReport(request);
    }
    
    
    [HttpGet("queue/report")]
    public QueueReportResponseModel GetQueueReport([FromQuery]QueueReportRequest request)
    {
        return _reportService.GetQueueReport(request);
    }

    [HttpGet("complaint/report")]
    public ComplaintReportResponseModel GetComplaintReport([FromQuery]ComplaintReportRequest request)
    {
        return _reportService.GetComplaintReport(request);
    }

    [HttpGet("review/report")]
    public ReviewReportResponseModel GetReviewReport([FromQuery] ReviewReportRequest request)
    {
        return _reportService.GetReviewReport(request);
    }

    [HttpGet("service/report")]
    public ServiceReportResponseModel GetServiceReport([FromQuery] ServiceReportRequest request)
    {
        return _reportService.GetServiceReport(request);
    }
}