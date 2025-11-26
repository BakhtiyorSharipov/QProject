using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.ComplaintRequest;
using QApplication.Responses;
using QDomain.Enums;

namespace QAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ComplaintController: ControllerBase
{
    private readonly IComplaintService _complaintService;
    private readonly ILogger<ComplaintController> _logger;

    public ComplaintController(IComplaintService complaintService, ILogger<ComplaintController> logger)
    {
        _complaintService = complaintService;
        _logger = logger;
    }
    
    [Authorize(Roles = nameof(UserRoles.Employee)+","+nameof(UserRoles.CompanyAdmin)+","+nameof(UserRoles.SystemAdmin))]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComplaintResponseModel>>> GetAllComplaintsAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all complaints. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var complaints= await _complaintService.GetAllComplaintsAsync(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {complaintsCount} complaints.", complaints.Count());
        return Ok(complaints);
    }

    [Authorize(Roles = nameof(UserRoles.Employee)+","+nameof(UserRoles.CompanyAdmin)+","+nameof(UserRoles.SystemAdmin))]
    [HttpGet("{id}")]
    public async Task<ActionResult<ComplaintResponseModel>> GetComplaintByIdAsync([FromRoute]int id)
    {
        _logger.LogInformation("Received request to get complaint with Id: {complaintId}", id);
        var complaint=await _complaintService.GetComplaintByIdAsync(id);
        _logger.LogInformation("Successfully returned complaint with Id: {complaintId}", id);
        return Ok(complaint);
    }

    [Authorize(Roles = nameof(UserRoles.Customer))]
    [HttpPost]
    public async Task<IActionResult> AddComplaintAsync([FromBody]CreateComplaintRequest request)
    {
        _logger.LogInformation("Received request to create complaint to queueId: {id}", request.QueueId);
        var complaint= await _complaintService.AddComplaintAsync(request);
        _logger.LogInformation("Successfully created complaint with Id: {complaintId}", complaint.Id);
        return Created(nameof(GetComplaintByIdAsync), complaint);
    }

    [Authorize(Roles = nameof(UserRoles.Employee)+","+nameof(UserRoles.CompanyAdmin)+","+nameof(UserRoles.SystemAdmin))]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComplaintStatusAsync([FromRoute] int id, [FromBody] UpdateComplaintStatusRequest request)
    {
        _logger.LogInformation("Received complaint to update with Id: {complaintId}", id);
        var complaint = await _complaintService.UpdateComplaintStatusAsync(id, request);
        _logger.LogInformation("Successfully updated complaint with Id: {complaintId}", id);
        return Ok(complaint);
    }
}