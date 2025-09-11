using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.BlockedCustomerRequest;
using QApplication.Responses;

namespace QAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class BlockedCustomerController: ControllerBase
{
    private readonly IBlockedCustomerService _service;

    public BlockedCustomerController(IBlockedCustomerService service)
    {
        _service = service;
    }

    [HttpGet]
    public IEnumerable<BlockedCustomerResponseModel> GetAll(int pageList, int pageNumber)
    {
        return _service.GetAll(pageList, pageNumber);
    }

    [HttpGet("{id}")]
    public BlockedCustomerResponseModel GetById([FromRoute] int id)
    {
        return _service.GetById(id);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateBlockedCustomerRequest request)
    {
        var blocked = _service.Add(request);

        return CreatedAtAction(nameof(GetById), new { id = blocked.Id }, blocked);
    }

    [HttpPut("{id}")]
    public IActionResult Put([FromRoute] int id, [FromBody] UpdateBlockedCustomerRequest request)
    {
      var update=  _service.Update(id, request);
      return Ok(update);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
       var delete= _service.Delete(id);
       return NoContent();
    }
}