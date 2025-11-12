using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.QueueRequest;
using QApplication.Responses;
using QDomain.Enums;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueueController : ControllerBase
{
    private readonly IQueueService _service;
    private readonly ILogger<QueueController> _logger;

    public QueueController(IQueueService service, ILogger<QueueController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult< IEnumerable<QueueResponseModel>> GetAll(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all queues. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var queues= _service.GetAll(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {queueCount} queues.", queues.Count());
        return Ok(queues);
    }

    [HttpGet("{id}")]
    public ActionResult< QueueResponseModel> GetById([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get queue by Id: {queueId}", id);
        var queue= _service.GetById(id);
        _logger.LogInformation("Successfully returned queue with Id: {queueId}", id);
        return Ok(queue);
    }

    [HttpPost("book")]
    public IActionResult Post([FromBody] CreateQueueRequest request)
    {
        _logger.LogInformation("Received request to create queue.");
        var queue = _service.Add(request);
        _logger.LogInformation("Successfully created queue with Id: {queueId}", queue.Id);
        return Created(nameof(GetById), queue);
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete queue with Id: {queueId}", id);
        var delete = _service.Delete(id);
        _logger.LogInformation("Successfully deleted queue with Id: {queueId}", id);
        return NoContent();
    }

    [HttpPut("cancel/customer")]
    public IActionResult CancelQueueByCustomer([FromBody] QueueCancelRequest request)
    {
        _logger.LogInformation("Received request to cancel queue with Id {queueId} by customer.", request.QueueId );
        var cancel = _service.CancelQueueByCustomer(request);
        _logger.LogInformation("Successfully canceled queue with Id: {queueId} by customer.");
        return Ok(cancel);
    }

    [HttpPut("cancel/employee")]
    public IActionResult CancelQueueByEmployee([FromBody] QueueCancelRequest request)
    {
        _logger.LogInformation("Received request to cancel queue with Id {queueId} by employee.", request.QueueId);
        var cancel = _service.CancelQueueByEmployee(request);
        _logger.LogInformation("Successfully canceled queue with Id {queueId} by employee.", request.QueueId);
        return Ok(cancel);
    }

    [HttpPut("status/update")]
    public ActionResult<QueueResponseModel> UpdateStatus([FromBody] UpdateQueueRequest request)
    {
        _logger.LogInformation("Received request to update queue status {newStatus} with Id: {queueId}",request.newStatus, request.QueueId);
        var result = _service.UpdateQueueStatus(request);
        _logger.LogInformation("Successfully updated queue status with Id: {queueId}", request.QueueId);
        return Ok(result);
    }

    [HttpGet("history/customer/{customerId}")]
    public IEnumerable<QueueResponseModel> GetQueuesByCustomer([FromRoute] int customerId)
    {
        _logger.LogInformation("Received request to get customer queue history with Id: {customerId}", customerId);
        var queue = _service.GetQueuesByCustomer(customerId);
        _logger.LogInformation("Successfully returned {queueCount} queues.");
        return queue;
    }

    [HttpGet("history/employee/{employeeId}")]
    public IEnumerable<QueueResponseModel> GetQueuesByEmployee([FromRoute] int employeeId)
    {
        _logger.LogInformation("Received request to get employee queue history with Id: {employeeId}", employeeId);
        var queue = _service.GetQueuesByEmployee(employeeId);
        _logger.LogInformation("Successfully returned {queueCount} queues.", employeeId );
        return queue;
    }
}