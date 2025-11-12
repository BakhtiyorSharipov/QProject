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
    private readonly ILogger<ReviewController> _logger;

    public ReviewController(IReviewService service, ILogger<ReviewController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult< IEnumerable<ReviewResponseModel>> GetAll(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all reviews. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var reviews= _service.GetAll(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {reviewCount} reviews.", reviews.Count());

        return Ok(reviews);

    }

    [HttpGet("{id}")]
    public ActionResult< ReviewResponseModel> GetById([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get review with Id: {reviewId}", id);
        var review= _service.GetById(id);
        _logger.LogInformation("Successfully returned review with Id: {reviewId}", id);
        return Ok(review);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateReviewRequest request)
    {
        _logger.LogInformation("Received request to create review to queue with Id: {queueId}", request.QueueId);
        var review = _service.Add(request);
        _logger.LogInformation("Successfully created review with Id: {reviewId}", review.Id);
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
    }

}