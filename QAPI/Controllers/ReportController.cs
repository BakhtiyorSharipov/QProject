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
    private readonly ILogger<ReportController> _logger;
    public ReportController(IReportService reportService, ILogger<ReportController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    [HttpGet("company/report")]
    public ActionResult<CompanyReportItemResponseModel> GetCompanyReport([FromQuery] CompanyReportRequest request)
    {
        _logger.LogInformation("Received request to get company report with Id: {compnayId}", request.CompanyId);
        var companyReport= _reportService.GetCompanyReport(request);
        _logger.LogInformation("Successfully returned company report with Id: {companyId}", request.CompanyId);
        return Ok(companyReport);
    }
    
    [HttpGet("employee/report ")]
    public ActionResult< EmployeeReportResponseModel> GetEmployeeReport([FromQuery] EmployeeReportRequest request)
    {
        _logger.LogInformation("Received request to get employee report with Id: {employeeId}", request.EmployeeId);
        var employeeReport= _reportService.GetEmployeeReport(request);
        _logger.LogInformation("Successfully returned employee report with Id: {employeeId}", request.EmployeeId);
        return Ok(employeeReport);
    }
    
    
    [HttpGet("queue/report")]
    public ActionResult<QueueReportResponseModel> GetQueueReport([FromQuery]QueueReportRequest request)
    {
        _logger.LogInformation("Received request to get queue report.");
        var queueReport= _reportService.GetQueueReport(request);
        _logger.LogInformation("Successfully returned queue report.");
        return Ok(queueReport);
        
    }

    [HttpGet("complaint/report")]
    public ActionResult< ComplaintReportResponseModel> GetComplaintReport([FromQuery]ComplaintReportRequest request)
    {
        _logger.LogInformation("Received request to get complaint report.");
        var complaintReport= _reportService.GetComplaintReport(request);
        _logger.LogInformation("Successfully returned complaint report.");
        return Ok(complaintReport);
    }

    [HttpGet("review/report")]
    public ActionResult< ReviewReportResponseModel> GetReviewReport([FromQuery] ReviewReportRequest request)
    {
        _logger.LogInformation("Received request to get review report.");
        var reviewReport= _reportService.GetReviewReport(request);
        _logger.LogInformation("Successfully returned review report.");
        return Ok(reviewReport);
    }

    [HttpGet("service/report")]
    public ActionResult< ServiceReportResponseModel> GetServiceReport([FromQuery] ServiceReportRequest request)
    {
        _logger.LogInformation("Received request to get service report.");
        var serviceReport= _reportService.GetServiceReport(request);
        _logger.LogInformation("Successfully returned service report.");
        return Ok(serviceReport);
    }
}