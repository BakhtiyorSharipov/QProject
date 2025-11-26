using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles =nameof(UserRoles.CompanyAdmin)+","+nameof(UserRoles.SystemAdmin))]
    [HttpGet]
    public async Task<ActionResult< IEnumerable<QueueResponseModel>>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all queues. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var queues=await _service.GetAllAsync(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {queueCount} queues.", queues.Count());
        return Ok(queues);
    }

    [Authorize(Roles = nameof(UserRoles.Employee)+","+nameof(UserRoles.CompanyAdmin)+","+nameof(UserRoles.SystemAdmin))]
    [HttpGet("{id}")]
    public async Task<ActionResult< QueueResponseModel>> GetByIdAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get queue by Id: {queueId}", id);
        var queue=await _service.GetByIdAsync(id);
        _logger.LogInformation("Successfully returned queue with Id: {queueId}", id);
        return Ok(queue);
    }

    [Authorize(Roles = nameof(UserRoles.Customer))]
    [HttpPost("book")]
    public async Task<IActionResult> PostAsync([FromBody] CreateQueueRequest request)
    {
        _logger.LogInformation("Received request to create queue.");
        var queue =await _service.AddAsync(request);
        _logger.LogInformation("Successfully created queue with Id: {queueId}", queue.Id);
        return Created(nameof(GetByIdAsync), queue);
    }
    
    [Authorize(Roles = nameof(UserRoles.Customer))]
    [HttpPut("cancel/customer")]
    public async Task<IActionResult> CancelQueueByCustomerAsync([FromBody] QueueCancelRequest request)
    {
        _logger.LogInformation("Received request to cancel queue with Id {queueId} by customer.", request.QueueId );
        var cancel =await _service.CancelQueueByCustomerAsync(request);
        _logger.LogInformation("Successfully canceled queue with Id: {queueId} by customer.");
        return Ok(cancel);
    }

    [Authorize(Roles = nameof(UserRoles.CompanyAdmin)+","+ nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.Employee))]
    [HttpPut("cancel/employee")]
    public async Task<IActionResult> CancelQueueByEmployeeAsync([FromBody] QueueCancelRequest request)
    {
        _logger.LogInformation("Received request to cancel queue with Id {queueId} by employee.", request.QueueId);
        var cancel =await _service.CancelQueueByEmployeeAsync(request);
        _logger.LogInformation("Successfully canceled queue with Id {queueId} by employee.", request.QueueId);
        return Ok(cancel);
    }

    [Authorize(Roles = nameof(UserRoles.CompanyAdmin)+","+ nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.Employee))]
    [HttpPut("status/update")]
    public async Task<ActionResult<QueueResponseModel>> UpdateStatusAsync([FromBody] UpdateQueueRequest request)
    {
        _logger.LogInformation("Received request to update queue status {newStatus} with Id: {queueId}",request.newStatus, request.QueueId);
        var result =await _service.UpdateQueueStatusAsync(request);
        _logger.LogInformation("Successfully updated queue status with Id: {queueId}", request.QueueId);
        return Ok(result);
    }

    [Authorize(Roles = nameof(UserRoles.Customer))]
    [HttpGet("history/customer/{customerId}")]
    public async Task<IEnumerable<QueueResponseModel>> GetQueuesByCustomerAsync([FromRoute] int customerId)
    {
        _logger.LogInformation("Received request to get customer queue history with Id: {customerId}", customerId);
        var queue =await _service.GetQueuesByCustomerAsync(customerId);
        _logger.LogInformation("Successfully returned {queueCount} queues.");
        return queue;
    }

    [Authorize(Roles = nameof(UserRoles.CompanyAdmin)+","+ nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.Employee))]
    [HttpGet("history/employee/{employeeId}")]
    public async Task<IEnumerable<QueueResponseModel>> GetQueuesByEmployeeAsync([FromRoute] int employeeId)
    {
        _logger.LogInformation("Received request to get employee queue history with Id: {employeeId}", employeeId);
        var queue =await _service.GetQueuesByEmployeeAsync(employeeId);
        _logger.LogInformation("Successfully returned {queueCount} queues.", employeeId );
        return queue;
    }
}