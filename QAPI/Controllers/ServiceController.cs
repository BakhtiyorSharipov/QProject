using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.ServiceRequest;
using QApplication.Responses;
using QDomain.Enums;

namespace QAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ServiceController: ControllerBase
{
    private readonly IServiceService _service;
    private readonly ILogger<ServiceController> _logger;
    public ServiceController(IServiceService service, ILogger<ServiceController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpGet]
    public async Task<ActionResult< IEnumerable<ServiceResponseModel>>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all services. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var services=await _service.GetAllAsync(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {serviceCount} services.", services.Count());
        return Ok(services);

    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponseModel>> GetByIdAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get service with Id: {serviceId}", id);
        var service=await _service.GetByIdAsync(id);
        _logger.LogInformation("Successfully returned service with Id: {serviceId}", id);
        return Ok(service);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateServiceRequest request)
    {
        _logger.LogInformation("Received request to create service. ServiceName: {serviceName}", request.ServiceName);
        var service =await _service.AddAsync(request);
        _logger.LogInformation("Successfully created service with Id: {serviceId}", service.Id);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = service.Id }, service);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put([FromRoute] int id, [FromBody] UpdateServiceRequest request)
    {
        _logger.LogInformation("Received request to update service with Id: {serviceId}", id);
       var update=await _service.UpdateAsync(id, request);
       _logger.LogInformation("Successfully updated service with Id: {serviceId}", id);
       return Ok(update);
    }

    [Authorize(Roles = nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.CompanyAdmin))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete service with Id: {serviceId}", id);
       var delete=await _service.DeleteAsync(id);
       _logger.LogInformation("Successfully deleted service with Id: {serviceId}", id);
       return NoContent();
    }
}