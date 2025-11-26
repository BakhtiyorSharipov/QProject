using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;
using QApplication.Requests.ReviewRequest;
using QApplication.Responses;
using QDomain.Enums;

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

    [Authorize(Roles = nameof(UserRoles.CompanyAdmin)+","+ nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.Employee))]
    [HttpGet]
    public async Task<ActionResult< IEnumerable<ReviewResponseModel>>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Received request to get all reviews. PageList: {PageList}, PageNumber: {PageNumber}", pageList, pageNumber);
        var reviews=await _service.GetAllAsync(pageList, pageNumber);
        _logger.LogInformation("Successfully returned {reviewCount} reviews.", reviews.Count());

        return Ok(reviews);

    }
    
    [Authorize(Roles = nameof(UserRoles.CompanyAdmin)+","+ nameof(UserRoles.SystemAdmin)+","+ nameof(UserRoles.Employee))]
    [HttpGet("{id}")]
    public async Task<ActionResult< ReviewResponseModel>> GetByIdAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get review with Id: {reviewId}", id);
        var review=await _service.GetByIdAsync(id);
        _logger.LogInformation("Successfully returned review with Id: {reviewId}", id);
        return Ok(review);
    }

    [Authorize(Roles = nameof(UserRoles.Customer))]
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateReviewRequest request)
    {
        _logger.LogInformation("Received request to create review to queue with Id: {queueId}", request.QueueId);
        var review =await _service.AddAsync(request);
        _logger.LogInformation("Successfully created review with Id: {reviewId}", review.Id);
        return Created(nameof(GetByIdAsync), review);
    }

}