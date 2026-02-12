using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Requests.ServiceRequest;
using QApplication.Responses;
using QApplication.UseCases.Services.Commands.CreateService;
using QApplication.UseCases.Services.Commands.DeleteService;
using QApplication.UseCases.Services.Commands.UpdateService;
using QApplication.UseCases.Services.Queries.GetAllServices;
using QApplication.UseCases.Services.Queries.GetServiceById;
using QDomain.Enums;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceController : ControllerBase
{
    private readonly ILogger<ServiceController> _logger;
    private readonly IMediator _mediator;

    public ServiceController(ILogger<ServiceController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceResponseModel>>> GetAllAsync([FromQuery]int pageNumber=1)
    {
        _logger.LogInformation("Received request to get all services. PageNumber: {PageNumber}, PageSize: 15",
            pageNumber);
        var query = new GetAllServicesQuery(pageNumber);
        var services = await _mediator.Send(query);
        return Ok(services);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponseModel>> GetByIdAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get service with Id: {serviceId}", id);
        var query = new GetServiceByIdQuery(id);
        var service = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned service with Id: {serviceId}", id);
        return Ok(service);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateServiceCommand request)
    {
        _logger.LogInformation("Received request to create service. ServiceName: {serviceName}", request.ServiceName);
        var service = await _mediator.Send(request);
        _logger.LogInformation("Successfully created service with Id: {serviceId}", service.Id);
        return Ok(service);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put([FromRoute] int id, [FromBody] UpdateServiceRequest request)
    {
        _logger.LogInformation("Received request to update service with Id: {serviceId}", id);
        var command = new UpdateServiceCommand(id, request.CompanyId, request.ServiceName, request.ServiceDescription);
        var update = await _mediator.Send(command);
        _logger.LogInformation("Successfully updated service with Id: {serviceId}", id);
        return Ok(update);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin) + "," + nameof(UserRoles.CompanyAdmin))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete service with Id: {serviceId}", id);
        var command = new DeleteServiceCommand(id);
        await _mediator.Send(command);
        _logger.LogInformation("Successfully deleted service with Id: {serviceId}", id);
        return NoContent();
    }
}