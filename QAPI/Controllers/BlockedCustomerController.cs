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

    [HttpPost("block")]
    public IActionResult Block([FromBody] CreateBlockedCustomerRequest request)
    {
        var blocked = _service.Block(request);

        return CreatedAtAction(nameof(GetById), new { id = blocked.Id }, blocked);
    }
    

    [HttpDelete("{id}/unblock")]
    public IActionResult Unblock([FromRoute] int id)
    {
        var delete = _service.Unblock(id);
        return Ok(delete);
    }
}