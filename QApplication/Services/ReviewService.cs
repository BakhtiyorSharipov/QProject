using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.ReviewRequest;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repository;
    private readonly IQueueRepository _queueRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(IReviewRepository repository, IQueueRepository queueRepository,
        ILogger<ReviewService> logger, ICustomerRepository customerRepository)
    {
        _repository = repository;
        _queueRepository = queueRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ReviewResponseModel>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Getting all reviews. PageNumber: {pageNumber}, PageList: {pageList}", pageNumber,
            pageList);
        var dbReview = await _repository.GetAll(pageList, pageNumber).ToListAsync();
        var response = dbReview.Select(review => new ReviewResponseModel()
        {
            Id = review.Id,
            CustomerId = review.CustomerId,
            QueueId = review.QueueId,
            Grade = review.Grade,
            ReviewText = review.ReviewText
        }).ToList();

        _logger.LogInformation("Fetched {response.Count} reviews.", response.Count);

        return response;
    }

    public async Task<IEnumerable<ReviewResponseModel>> GetAllReviewsByQueueAsync(int queueId)
    {
        _logger.LogInformation("Getting reviews by queue Id {queueId}", queueId);
        var dbReview = await _repository.GetAllReviewsByQueue(queueId).ToListAsync();
        if (!dbReview.Any())
        {
            _logger.LogWarning("No reviews found for this queue Id {queueId}", queueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var response = dbReview.Select(review => new ReviewResponseModel
        {
            Id = review.Id,
            CustomerId = review.CustomerId,
            QueueId = review.QueueId,
            Grade = review.Grade,
            ReviewText = review.ReviewText
        }).ToList();

        _logger.LogInformation("Fetched {response.Count} reviews with queue Id {queueId}.", response.Count, queueId);
        return response;
    }

    public async Task<IEnumerable<ReviewResponseModel>> GetAllReviewsByCompanyAsync(int companyId)
    {
        _logger.LogInformation("Getting reviews by company Id {companyId}", companyId);
        var dbReview = await _repository.GetAllReviewsByCompany(companyId).ToListAsync();
        if (!dbReview.Any())
        {
            _logger.LogWarning("No reviews found with this company ID {companyId}", companyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var response = dbReview.Select(review => new ReviewResponseModel
        {
            Id = review.Id,
            CustomerId = review.CustomerId,
            QueueId = review.QueueId,
            Grade = review.Grade,
            ReviewText = review.ReviewText
        }).ToList();

        _logger.LogInformation("Fetched {response.Count} reviews with this company Id {companyId}.", response.Count,
            companyId);
        return response;
    }

    public async Task<ReviewResponseModel> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting review by Id {id}", id);
        var dbReview = await _repository.FindByIdAsync(id);
        if (dbReview == null)
        {
            _logger.LogWarning("Review with Id {id} not found.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ReviewEntity));
        }

        var response = new ReviewResponseModel()
        {
            Id = dbReview.Id,
            CustomerId = dbReview.CustomerId,

            QueueId = dbReview.QueueId,
            Grade = dbReview.Grade,
            ReviewText = dbReview.ReviewText
        };

        _logger.LogInformation("Review with Id {id} fetched successfully.", id);
        return response;
    }

    public async Task<ReviewResponseModel> AddAsync(ReviewRequestModel request)
    {
        _logger.LogInformation("Adding new review to this queue Id {request.QueueId}", request.QueueId);
        var requestToCreate = request as CreateReviewRequest;
        if (requestToCreate == null)
        {
            _logger.LogError($"Invalid request model while adding new review");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(ReviewEntity));
        }

        var queue = await _queueRepository.FindByIdAsync(requestToCreate.QueueId);
        if (queue == null)
        {
            _logger.LogWarning("Queue with Id {request.QueueId} not found for adding new review.", request.QueueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var customer = await _customerRepository.FindByIdAsync(request.CustomerId);
        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {request.CustomerId} not found for adding new review.",
                request.CustomerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        if (queue.Status != QueueStatus.Completed)
        {
            _logger.LogError("Invalid queue status while adding new review for this queue Id {queueId}",
                requestToCreate.QueueId);
            throw new Exception("You can leave review only if status is completed");
        }

        var reviews = await _repository.GetAllReviewsByQueue(queue.Id).ToListAsync();
        var isDouble = reviews.Any(s => s.CustomerId == queue.CustomerId);
        if (isDouble)
        {
            _logger.LogError("Overlapping review for this queue Id {queueId}.", requestToCreate.QueueId);
            throw new Exception("You have already left a review for this queue!");
        }

        var review = new ReviewEntity()
        {
            CustomerId = requestToCreate.CustomerId,
            QueueId = requestToCreate.QueueId,
            Grade = requestToCreate.Grade,
            ReviewText = requestToCreate.ReviewText
        };

        if (review.Grade < 1 || review.Grade > 5)
        {
            _logger.LogError("Invalid grade for review.");
            throw new Exception("Grade should be between 1 and 5!");
        }

        await _repository.AddAsync(review);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Review added successfully with Id {review.Id}.", review.Id);
        var response = new ReviewResponseModel()
        {
            Id = review.Id,
            CustomerId = review.CustomerId,
            QueueId = review.QueueId,
            Grade = review.Grade,
            ReviewText = review.ReviewText
        };

        return response;
    }
}