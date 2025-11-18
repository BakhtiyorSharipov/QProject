using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.CompanyRequest;
using QApplication.Responses;


namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompanyController> _logger;

    public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyResponseModel>>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all companies. PageList: {PageList}, PageNumber: {PageNumber}",
            pageList, pageNumber);
        var companies = await _companyService.GetAllAsync(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {compnayCount} companies.", companies.Count());
        return Ok(companies);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyResponseModel>> GetByIdAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get company by Id: {companyId}", id);
        var company = await _companyService.GetByIdAsync(id);
        _logger.LogInformation("Successfully returned company with Id: {companyId}", id);

        return Ok(company);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateCompanyRequest request)
    {
        _logger.LogInformation("Received request to create new company. CompanyName: {companyName}",
            request.CompanyName);
        var createCompany = await _companyService.AddAsync(request);
        _logger.LogInformation("Successfully created company with Id: {companyId}", createCompany.Id);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createCompany.Id }, createCompany);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] UpdateCompanyRequest request)
    {
        _logger.LogInformation("Received request to update company with Id: {companyId}", id);
        var update = await _companyService.UpdateAsync(id, request);
        _logger.LogInformation("Successfully updated company with Id: {companyId}", id);
        return Ok(update);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete company with Id: {companyId}", id);
        var delete = await _companyService.DeleteAsync(id);
        _logger.LogInformation("Successfully deleted company with Id: {companyId}", id);
        return NoContent();
    }
}