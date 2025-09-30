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

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    public IEnumerable<CompanyResponseModel> GetAll(int pageList, int pageNumber)
    {
        return _companyService.GetAll(pageList, pageNumber);
    }

    [HttpGet("{id}")]
    public CompanyResponseModel GetById([FromRoute] int id)
    {
        return _companyService.GetById(id);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateCompanyRequest request)
    {
        var createCompany = _companyService.Add(request);
        return CreatedAtAction(nameof(GetById), new { id = createCompany.Id }, createCompany);
    }
    
    [HttpPut("{id}")]
    public IActionResult Put([FromRoute] int id, [FromBody] UpdateCompanyRequest request)
    {
       var update= _companyService.Update(id, request);
       return Ok(update);

    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
       var delete= _companyService.Delete(id);
       return NoContent();
    }
}