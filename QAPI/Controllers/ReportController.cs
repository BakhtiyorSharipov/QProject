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
    public async Task<ActionResult<CompanyReportItemResponseModel>> GetCompanyReportAsync([FromQuery] CompanyReportRequest request)
    {
        _logger.LogInformation("Received request to get company report with Id: {compnayId}", request.CompanyId);
        var companyReport=await _reportService.GetCompanyReportAsync(request);
        _logger.LogInformation("Successfully returned company report with Id: {companyId}", request.CompanyId);
        return Ok(companyReport);
    }
    
    [HttpGet("employee/report ")]
    public async Task<ActionResult< EmployeeReportResponseModel>> GetEmployeeReportAsync([FromQuery] EmployeeReportRequest request)
    {
        _logger.LogInformation("Received request to get employee report with Id: {employeeId}", request.EmployeeId);
        var employeeReport=await _reportService.GetEmployeeReportAsync(request);
        _logger.LogInformation("Successfully returned employee report with Id: {employeeId}", request.EmployeeId);
        return Ok(employeeReport);
    }
    
    
    [HttpGet("queue/report")]
    public async Task<ActionResult<QueueReportResponseModel>> GetQueueReportAsync([FromQuery]QueueReportRequest request)
    {
        _logger.LogInformation("Received request to get queue report.");
        var queueReport=await _reportService.GetQueueReportAsync(request);
        _logger.LogInformation("Successfully returned queue report.");
        return Ok(queueReport);
        
    }

    [HttpGet("complaint/report")]
    public async Task<ActionResult< ComplaintReportResponseModel>> GetComplaintReportAsync([FromQuery]ComplaintReportRequest request)
    {
        _logger.LogInformation("Received request to get complaint report.");
        var complaintReport=await _reportService.GetComplaintReportAsync(request);
        _logger.LogInformation("Successfully returned complaint report.");
        return Ok(complaintReport);
    }

    [HttpGet("review/report")]
    public async Task<ActionResult< ReviewReportResponseModel>> GetReviewReportAsync([FromQuery] ReviewReportRequest request)
    {
        _logger.LogInformation("Received request to get review report.");
        var reviewReport=await _reportService.GetReviewReportAsync(request);
        _logger.LogInformation("Successfully returned review report.");
        return Ok(reviewReport);
    }

    [HttpGet("service/report")]
    public async Task<ActionResult< ServiceReportResponseModel>> GetServiceReportAsync([FromQuery] ServiceReportRequest request)
    {
        _logger.LogInformation("Received request to get service report.");
        var serviceReport=await _reportService.GetServiceReportAsync(request);
        _logger.LogInformation("Successfully returned service report.");
        return Ok(serviceReport);
    }
}