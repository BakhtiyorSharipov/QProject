using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.CustomerRequest;
using QApplication.Responses;

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

    [HttpGet]
    public ActionResult<IEnumerable<CustomerResponseModel>> GetAll(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all customers. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var customers= _service.GetAll(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {customerCount} customers.", customers.Count());
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public ActionResult< CustomerResponseModel> GetById([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get customer with Id: {customerId}", id);
        var customer= _service.GetById(id);
        _logger.LogInformation("Successfully returned customer with Id: {customerId}", id);
        return Ok(customer);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateCustomerRequest request)
    {
        _logger.LogInformation("Received request to create customer with CustomerName: {name}", request.FirstName );
        var customer = _service.Add(request);
        _logger.LogInformation("Successfully created customer with Id: {customerId}", customer.Id);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut("{id}")]
    public IActionResult Put([FromRoute] int id, [FromBody] UpdateCustomerRequest request)
    {
        _logger.LogInformation("Received request to update customer with Id: {customerId}", id);
       var update= _service.Update(id,request);
       _logger.LogInformation("Successfully updated customer with Id: {customerId}", id);
       return Ok(update);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete with Id: {customerId}", id);
       var delete= _service.Delete(id);
       _logger.LogInformation("Successfully deleted customer with Id: {customerId}", id);
       return NoContent();
    }

}