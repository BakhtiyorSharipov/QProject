using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.ComplaintRequest;
using QApplication.Responses;

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

    [HttpGet]
    public ActionResult<IEnumerable<ComplaintResponseModel>> GetAllComplaints(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all complaints. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var complaints= _complaintService.GetAllComplaints(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {complaintsCount} complaints.", complaints.Count());
        return Ok(complaints);
    }

    [HttpGet("{id}")]
    public ActionResult<ComplaintResponseModel> GetComplaintById([FromRoute]int id)
    {
        _logger.LogInformation("Received request to get complaint with Id: {complaintId}", id);
        var complaint= _complaintService.GetComplaintById(id);
        _logger.LogInformation("Successfully returned complaint with Id: {complaintId}", id);
        return Ok(complaint);
    }

    [HttpPost]
    public IActionResult AddComplaint([FromBody]CreateComplaintRequest request)
    {
        _logger.LogInformation("Received request to create complaint to queueId: {id}", request.QueueId);
        var complaint=  _complaintService.AddComplaint(request);
        _logger.LogInformation("Successfully created complaint with Id: {complaintId}", complaint.Id);
        return Created(nameof(GetComplaintById), complaint);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateComplaintStatus([FromRoute] int id, [FromBody] UpdateComplaintStatusRequest request)
    {
        _logger.LogInformation("Received complaint to update with Id: {complaintId}", id);
        var complaint = _complaintService.UpdateComplaintStatus(id, request);
        _logger.LogInformation("Successfully updated complaint with Id: {complaintId}", id);
        return Ok(complaint);
    }
}