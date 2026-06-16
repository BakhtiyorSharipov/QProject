using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUserService.Application.Requests.CustomerRequest;
using QUserService.Application.Responses;
using QUserService.Application.UseCases.Customers.Commands.CreateCustomer;
using QUserService.Application.UseCases.Customers.Commands.DeleteCustomer;
using QUserService.Application.UseCases.Customers.Commands.UpdateCustomer;
using QUserService.Application.UseCases.Customers.Commands.UpdateCustomerProfile;
using QUserService.Application.UseCases.Customers.Queries.GetAllCustomers;
using QUserService.Application.UseCases.Customers.Queries.GetCustomerById;
using QUserService.Application.UseCases.Customers.Queries.GetCustomerProfile;
using QUserService.Domain.Enums;

namespace QUserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ILogger<CustomerController> _logger;
    private readonly IMediator _mediator;

    public CustomerController(ILogger<CustomerController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerResponseModel>>> GetAllAsync([FromQuery] int pageNumber = 1)
    {
        _logger.LogInformation("Received request to get all customers. PageNumber: {PageNumber}, PageSize: 15",
            pageNumber);

        var query = new GetAllCustomersQuery(pageNumber);
        var customers = await _mediator.Send(query);

        return Ok(customers);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponseModel>> GetByIdAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get customer with Id: {customerId}", id);
        var query = new GetCustomerByIdQuery(id);
        var customer = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned customer with Id: {customerId}", id);
        return Ok(customer);
    }

    [Authorize(Roles = nameof(UserRoles.Customer))]
    [HttpGet("get-customer-profile")]
    public async Task<ActionResult<CustomerProfileResponse>> GetCustomerProfile()
    {
        _logger.LogInformation("Received request to get customer profile");
        var query = new GetCustomerProfileQuery();
        var customer = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned customer profile");
        return Ok(customer);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateCustomerCommand request)
    {
        _logger.LogInformation("Received request to create customer with CustomerName: {name}", request.Firstname);
        var customer = await _mediator.Send(request);
        _logger.LogInformation("Successfully created customer with Id: {customerId}", customer.Id);
        return Ok(customer);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin) + "," +
                       nameof(UserRoles.Customer) + "," + nameof(UserRoles.Employee))]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] UpdateCustomerRequest request)
    {
        _logger.LogInformation("Received request to update customer with Id: {customerId}", id);

        var command = new UpdateCustomerCommand(id, request.FirstName, request.LastName, request.PhoneNumber);
        var update = await _mediator.Send(command);
        _logger.LogInformation("Successfully updated customer with Id: {customerId}", id);
        return Ok(update);
    }

    [Authorize(Roles = nameof(UserRoles.Customer))]
    [HttpPut("customer-profile-update")]
    public async Task<IActionResult> ProfileUpdate([FromBody] UpdateCustomerProfileCommand request)
    {
        _logger.LogInformation("Received request to update customer profile");
        
        var update = await _mediator.Send(request);
        _logger.LogInformation("Successfully updated customer profile");
        return Ok(update);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete with Id: {customerId}", id);
        var command = new DeleteCustomerCommand(id);
        await _mediator.Send(command);
        _logger.LogInformation("Successfully deleted customer with Id: {customerId}", id);
        return NoContent();
    }
}