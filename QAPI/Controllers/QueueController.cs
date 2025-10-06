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
    private IQueueService _service;

    public QueueController(IQueueService service)
    {
        _service = service;
    }

    [HttpGet]
    public IEnumerable<QueueResponseModel> GetAll(int pageList, int pageNumber)
    {
        return _service.GetAll(pageList, pageNumber);
    }

    [HttpGet("{id}")]
    public QueueResponseModel GetById([FromRoute] int id)
    {
        return _service.GetById(id);
    }

    [HttpPost("book")]
    public IActionResult Post([FromBody] CreateQueueRequest request)
    {
        var queue = _service.Add(request);
        return Created(nameof(GetById), queue);
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var delete = _service.Delete(id);
        return NoContent();
    }

    [HttpPut("cancel/customer")]
    public IActionResult CancelQueueByCustomer([FromBody] QueueCancelRequest request)
    {
        var cancel = _service.CancelQueueByCustomer(request);
        return Ok(cancel);
    }

    [HttpPut("cancel/employee")]
    public IActionResult CancelQueueByEmployee([FromBody] QueueCancelRequest request)
    {
        var cancel = _service.CancelQueueByEmployee(request);
        return Ok(cancel);
    }

    [HttpPut("status/update")]
    public ActionResult<QueueResponseModel> UpdateStatus([FromQuery] int id, [FromQuery] QueueStatus status)
    {
        var result = _service.UpdateQueueStatus(id, status);
        return Ok(result);
    }

    [HttpGet("history/customer/{customerId}")]
    public IEnumerable<QueueResponseModel> GetQueuesByCustomer([FromRoute] int customerId)
    {
        var queue = _service.GetQueuesByCustomer(customerId);
        return queue;
    }

    [HttpGet("history/employee/{employeeId}")]
    public IEnumerable<QueueResponseModel> GetQueuesByEmployee([FromRoute] int employeeId)
    {
        var queue = _service.GetQueuesByEmployee(employeeId);
        return queue;
    }
}