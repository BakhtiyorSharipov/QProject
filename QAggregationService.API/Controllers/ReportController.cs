using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QAggregationService.Contracts.Interfaces;
using QAggregationService.Contracts.Requests;
using QAggregationService.Contracts.Responses;

namespace QAggregationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IAggregationService _service;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IAggregationService service, ILogger<ReportController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [Authorize(Roles = "CompanyAdmin")]
    [HttpGet("company-report")] 
    public async Task<ActionResult<CompanyReportResponse>> GetCompanyReport([FromQuery] ReportRequest request)
    {
        _logger.LogInformation("Getting company report - CompanyId: {CompanyId}, Page: {PageNumber}, Size: {PageSize}",
            request.CompanyId, request.PageNumber, request.PageSize);
    
        var result = await _service.GetReportAsync(request);
    
        return Ok(result);
    }

    [Authorize(Roles = "Employee")]
    [HttpGet("employee-report")]
    public async Task<ActionResult<EmployeeReportResponse>> GetEmployeeReport([FromQuery] EmployeeReportRequest request)
    {
        _logger.LogInformation("Getting employee report - EmployeeId: {EmployeeId}", request.EmployeeId);
    
        var result = await _service.GetEmployeeReport(request);
        
        return Ok(result);
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("customer-report")]
    public async Task<ActionResult<CustomerReportResponse>> GetCustomerReport([FromQuery] CustomerReportRequest request)
    {
        _logger.LogInformation("Getting employee report - CustomerId: {CustomerId}", request.CustomerId);
    
        var result = await _service.GetCustomerReport(request);
        
        return Ok(result);
    }



}