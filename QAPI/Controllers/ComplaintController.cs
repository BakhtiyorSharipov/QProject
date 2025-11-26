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

    public ComplaintController(IComplaintService complaintService)
    {
        _complaintService = complaintService;
    }

    [HttpGet]
    public IEnumerable<ComplaintResponseModel> GetAllComplaints(int pageList, int pageNumber)
    {
        return _complaintService.GetAllComplaints(pageList, pageNumber);
    }

    [HttpGet("{id}")]
    public ComplaintResponseModel GetComplaintById([FromRoute]int id)
    {
        return _complaintService.GetComplaintById(id);
    }

    [HttpPost]
    public IActionResult AddComplaint([FromBody]CreateComplaintRequest request)
    {
        var complaint=  _complaintService.AddComplaint(request);
        return Created(nameof(GetComplaintById), complaint);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateComplaintStatus([FromRoute] int id, [FromBody] UpdateComplaintStatusRequest request)
    {
        var complaint = _complaintService.UpdateComplaintStatus(id, request);
        return Ok(complaint);
    }
}