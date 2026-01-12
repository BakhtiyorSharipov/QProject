using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Responses.ReportResponse;
using QApplication.UseCases.Reports.Queries;
using QApplication.UseCases.Reports.Queries.GetEmployeeReport;
using QApplication.UseCases.Reports.Queries.GetQueueReport;
using QApplication.UseCases.Reports.Queries.GetReviewReport;
using QApplication.UseCases.Reports.Queries.GetServiceReport;
using QDomain.Enums;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly ILogger<ReportController> _logger;
    private readonly IMediator _mediator;

    public ReportController(ILogger<ReportController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Authorize(Roles = nameof(UserRoles.CompanyAdmin) + "," + nameof(UserRoles.SystemAdmin))]
    [HttpGet("company/report")]
    public async Task<ActionResult<CompanyReportItemResponseModel>> GetCompanyReportAsync(
        [FromQuery] GetCompanyReportQuery query)
    {
        _logger.LogInformation("Received request to get company report with Id: {compnayId}", query.CompanyId);
        var companyReport = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned company report with Id: {companyId}", query.CompanyId);
        return Ok(companyReport);
    }

    [Authorize(Roles =
        nameof(UserRoles.CompanyAdmin) + "," + nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.Employee))]
    [HttpGet("employee/report ")]
    public async Task<ActionResult<EmployeeReportResponseModel>> GetEmployeeReportAsync(
        [FromQuery] GetEmployeeReportQuery query)
    {
        _logger.LogInformation("Received request to get employee report with Id: {employeeId}", query.EmployeeId);
        var employeeReport = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned employee report with Id: {employeeId}", query.EmployeeId);
        return Ok(employeeReport);
    }

    [Authorize(Roles = nameof(UserRoles.CompanyAdmin) + "," + nameof(UserRoles.SystemAdmin))]
    [HttpGet("queue/report")]
    public async Task<ActionResult<QueueReportResponseModel>> GetQueueReportAsync([FromQuery] GetQueueReportQuery query)
    {
        _logger.LogInformation("Received request to get queue report.");
        var queueReport = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned queue report.");
        return Ok(queueReport);
    }

    [Authorize(Roles = nameof(UserRoles.CompanyAdmin) + "," + nameof(UserRoles.SystemAdmin))]
    [HttpGet("complaint/report")]
    public async Task<ActionResult<ComplaintReportResponseModel>> GetComplaintReportAsync(
        [FromQuery] GetCompanyReportQuery query)
    {
        _logger.LogInformation("Received request to get complaint report.");
        var complaintReport = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned complaint report.");
        return Ok(complaintReport);
    }

    [Authorize(Roles = nameof(UserRoles.CompanyAdmin) + "," + nameof(UserRoles.SystemAdmin))]
    [HttpGet("review/report")]
    public async Task<ActionResult<ReviewReportResponseModel>> GetReviewReportAsync(
        [FromQuery] GetReviewReportQuery query)
    {
        _logger.LogInformation("Received request to get review report.");
        var reviewReport = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned review report.");
        return Ok(reviewReport);
    }

    [Authorize(Roles = nameof(UserRoles.CompanyAdmin) + "," + nameof(UserRoles.SystemAdmin))]
    [HttpGet("service/report")]
    public async Task<ActionResult<ServiceReportResponseModel>> GetServiceReportAsync(
        [FromQuery] GetServiceReportQuery query)
    {
        _logger.LogInformation("Received request to get service report.");
        var serviceReport = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned service report.");
        return Ok(serviceReport);
    }
}