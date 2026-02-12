using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Requests.CompanyRequest;
using QApplication.Responses;
using QApplication.UseCases.Companies.Commands.CreateCompany;
using QApplication.UseCases.Companies.Commands.DeleteCompany;
using QApplication.UseCases.Companies.Commands.UpdateCompany;
using QApplication.UseCases.Companies.Queries.GetAllCompanies;
using QApplication.UseCases.Companies.Queries.GetCompanyById;
using QDomain.Enums;


namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ILogger<CompanyController> _logger;
    private readonly IMediator _mediator;

    public CompanyController( ILogger<CompanyController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    [Authorize(Roles = nameof(UserRoles.SystemAdmin))]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyResponseModel>>> GetAllAsync([FromQuery]int pageNumber=1)
    {
        _logger.LogInformation("Received request to get all companies. PageNumber: {PageNumber}, PageSize: 15",
            pageNumber);

        var query = new GetAllCompaniesQuery(pageNumber);
        var companies = await _mediator.Send(query);
        return Ok(companies);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin))]
    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyResponseModel>> GetByIdAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get company by Id: {companyId}", id);
        var query = new GetCompanyByIdQuery(id);
        var company = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned company with Id: {companyId}", id);

        return Ok(company);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin))]
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateCompanyCommand request)
    {
        _logger.LogInformation("Received request to create new company. CompanyName: {companyName}",
            request.CompanyName);
        var createCompany = await _mediator.Send(request);
        _logger.LogInformation("Successfully created company with Id: {companyId}", createCompany.Id);
        return Ok(createCompany);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin))]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] UpdateCompanyRequest request)
    {
        _logger.LogInformation("Received request to update company with Id: {companyId}", id);

        var command = new UpdateCompanyCommand(id, request.CompanyName, request.Address, request.EmailAddress,
            request.PhoneNumber);
        
        var update = await _mediator.Send(command);
        _logger.LogInformation("Successfully updated company with Id: {companyId}", id);
        return Ok(update);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete company with Id: {companyId}", id);
        var command = new DeleteCompanyCommand(id);
        await _mediator.Send(command);
        _logger.LogInformation("Successfully deleted company with Id: {companyId}", id);
        return NoContent();
    }
}