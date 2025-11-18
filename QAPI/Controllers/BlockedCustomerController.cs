using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.BlockedCustomerRequest;
using QApplication.Responses;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlockedCustomerController : ControllerBase
{
    private readonly IBlockedCustomerService _service;
    private readonly ILogger<BlockedCustomerController> _logger;

    public BlockedCustomerController(IBlockedCustomerService service, ILogger<BlockedCustomerController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlockedCustomerResponseModel>>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all schedules. PageList: {PageList}, PageNumber: {PageNumber}",
            pageList, pageNumber);
        var blockedCustomers = await _service.GetAllAsync(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {blockedCustomers} blocked customers.", blockedCustomers.Count());
        return Ok(blockedCustomers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BlockedCustomerResponseModel>> GetById([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get blocked customer by Id: {blockedCustomer}", id);
        var blocked=await _service.GetByIdAsync(id);
        _logger.LogInformation("Successfully returned blocked customers with Id: {blockedCustomer}", id);
        return Ok(blocked);
    }

    [HttpPost("block")]
    public async Task<IActionResult> Block([FromBody] CreateBlockedCustomerRequest request)
    {
        _logger.LogInformation("Received request to block customer with Id: {customerId}", request.CustomerId);
        var blocked =await _service.BlockAsync(request);
        _logger.LogInformation("Successfully blocked customer with Id: {customerId}", request.CustomerId);
        return CreatedAtAction(nameof(GetById), new { id = blocked.Id }, blocked);
    }


    [HttpDelete("{id}/unblock")]
    public async Task<IActionResult> Unblock([FromRoute] int id)
    {
        _logger.LogInformation("Received request to unblock customer with Id: {customerId}", id);
        var delete =await _service.UnblockAsync(id);
        _logger.LogInformation("Successfully unblocked customer with Id: {customerId}", id);
        return Ok(delete);
    }
}