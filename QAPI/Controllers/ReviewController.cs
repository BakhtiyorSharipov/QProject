using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.ReviewRequest;
using QApplication.Responses;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController: ControllerBase
{
    private readonly IReviewService _service;

    public ReviewController(IReviewService service)
    {
        _service = service;
    }

    [HttpGet]
    public IEnumerable<ReviewResponseModel> GetAll(int pageList, int pageNumber)
    {
        return _service.GetAll(pageList, pageNumber);
    }

    [HttpGet("{id}")]
    public ReviewResponseModel GetById([FromRoute] int id)
    {
        return _service.GetById(id);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateReviewRequest request)
    {
        var review = _service.Add(request);

        return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
    }

    [HttpPut("{id}")]
    public IActionResult Put([FromRoute] int id, [FromBody] UpdateReviewRequest request)
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