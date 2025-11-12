using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.CompanyRequest;
using QApplication.Responses;


namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController: ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompanyController> _logger;

    public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CompanyResponseModel>> GetAll(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all companies. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var companies= _companyService.GetAll(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {compnayCount} companies.", companies.Count());

        return Ok(companies);
    }

    [HttpGet("{id}")]
    public ActionResult<CompanyResponseModel> GetById([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get company by Id: {companyId}", id);
        var company=_companyService.GetById(id);
        _logger.LogInformation("Successfully returned company with Id: {companyId}", id);

        return Ok(company);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateCompanyRequest request)
    {
        _logger.LogInformation("Received request to create new company. CompanyName: {companyName}", request.CompanyName);
        var createCompany = _companyService.Add(request);
        _logger.LogInformation("Successfully created company with Id: {companyId}", createCompany.Id);
        return CreatedAtAction(nameof(GetById), new { id = createCompany.Id }, createCompany);
    }
    
    [HttpPut("{id}")]
    public IActionResult Put([FromRoute] int id, [FromBody] UpdateCompanyRequest request)
    {
        _logger.LogInformation("Received request to update company with Id: {companyId}", id);
       var update= _companyService.Update(id, request);
       _logger.LogInformation("Successfully updated company with Id: {companyId}",id);
       return Ok(update);

    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete company with Id: {companyId}", id);
       var delete= _companyService.Delete(id);
       _logger.LogInformation("Successfully deleted company with Id: {companyId}", id);
       return NoContent();
    }
}