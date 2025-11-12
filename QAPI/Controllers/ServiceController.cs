using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.ServiceRequest;
using QApplication.Responses;

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

    [HttpGet]
    public ActionResult< IEnumerable<ServiceResponseModel>> GetAll(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all services. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var services= _service.GetAll(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {serviceCount} services.", services.Count());
        return Ok(services);

    }

    [HttpGet("{id}")]
    public ActionResult<ServiceResponseModel> GetById([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get service with Id: {serviceId}", id);
        var service= _service.GetById(id);
        _logger.LogInformation("Successfully returned service with Id: {serviceId}", id);
        return Ok(service);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateServiceRequest request)
    {
        _logger.LogInformation("Received request to create service. ServiceName: {serviceName}", request.ServiceName);
        var service = _service.Add(request);
        _logger.LogInformation("Successfully created service with Id: {serviceId}", service.Id);
        return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
    }

    [HttpPut("{id}")]
    public IActionResult Put([FromRoute] int id, [FromBody] UpdateServiceRequest request)
    {
        _logger.LogInformation("Received request to update service with Id: {serviceId}", id);
       var update= _service.Update(id, request);
       _logger.LogInformation("Successfully updated service with Id: {serviceId}", id);
       return Ok(update);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete service with Id: {serviceId}", id);
       var delete= _service.Delete(id);
       _logger.LogInformation("Successfully deleted service with Id: {serviceId}", id);
       return NoContent();
    }
}