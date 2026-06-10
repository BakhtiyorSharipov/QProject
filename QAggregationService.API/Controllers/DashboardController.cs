using Microsoft.AspNetCore.Mvc;
using QAggregationService.Contracts.Interfaces;
using QAggregationService.Contracts.Responses;

namespace QAggregationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController: ControllerBase
{
    private readonly IAggregationService _aggregationService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IAggregationService aggregationService, ILogger<DashboardController> logger)
    {
        _aggregationService = aggregationService;
        _logger = logger;
    }

    [HttpGet("{companyId}")]
    public async Task<ActionResult<DashboardResponse>> GetCompanyDashboard([FromRoute] int companyId)
    {
        _logger.LogInformation("Getting company dashboard - CompanyId: {CompanyId}", companyId);
    
        var result = await _aggregationService.GetCompanyDashboard(companyId);
        return Ok(result);
    }
}