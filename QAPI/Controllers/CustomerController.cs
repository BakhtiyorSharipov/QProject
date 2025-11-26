using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.CustomerRequest;
using QApplication.Responses;
using QDomain.Enums;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController: ControllerBase
{
    private readonly ICustomerService _service;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ICustomerService service, ILogger<CustomerController> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerResponseModel>>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all customers. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var customers=await _service.GetAllAsync(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {customerCount} customers.", customers.Count());
        return Ok(customers);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpGet("{id}")]
    public async Task<ActionResult< CustomerResponseModel>> GetByIdAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get customer with Id: {customerId}", id);
        var customer=await _service.GetByIdAsync(id);
        _logger.LogInformation("Successfully returned customer with Id: {customerId}", id);
        return Ok(customer);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateCustomerRequest request)
    {
        _logger.LogInformation("Received request to create customer with CustomerName: {name}", request.FirstName );
        var customer = await _service.AddAsync(request);
        _logger.LogInformation("Successfully created customer with Id: {customerId}", customer.Id);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = customer.Id }, customer);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] UpdateCustomerRequest request)
    {
        _logger.LogInformation("Received request to update customer with Id: {customerId}", id);
       var update= await _service.UpdateAsync(id,request);
       _logger.LogInformation("Successfully updated customer with Id: {customerId}", id);
       return Ok(update);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete with Id: {customerId}", id);
       var delete= await _service.DeleteAsync(id);
       _logger.LogInformation("Successfully deleted customer with Id: {customerId}", id);
       return NoContent();
    }

}