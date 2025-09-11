using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.QueueRequest;
using QApplication.Responses;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueueController: ControllerBase
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

    [HttpPost]
    public IActionResult Post([FromBody] CreateQueueRequest request)
    {
        var queue = _service.Add(request);

        return CreatedAtAction(nameof(GetById), new { id = queue.Id }, queue);
    }

    [HttpPut("{id}")]
    public IActionResult Put([FromRoute] int id, [FromBody] UpdateQueueRequest request)
    {
       var update= _service.Update(id, request);
       return Ok(update);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var delete=_service.Delete(id);
        return NoContent();
    }
}