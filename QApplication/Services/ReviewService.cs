using System.Net;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.ReviewRequest;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.Services;

public class ReviewService:  IReviewService
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

    public IEnumerable<ReviewResponseModel> GetAll(int pageList, int pageNumber)
    {
        _logger.LogInformation("Getting all reviews. PageNumber: {pageNumber}, PageList: {pageList}", pageNumber, pageList);
        var dbReview = _repository.GetAll(pageList, pageNumber);
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

    public IEnumerable<ReviewResponseModel> GetAllReviewsByQueue(int queueId)
    {
        _logger.LogInformation("Getting reviews by queue Id {queueId}", queueId);
        var dbReview = _repository.GetAllReviewsByQueue(queueId);
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

    public IEnumerable<ReviewResponseModel> GetAllReviewsByCompany(int companyId)
    {
        _logger.LogInformation("Getting reviews by company Id {companyId}", companyId);
        var dbReview = _repository.GetAllReviewsByCompany(companyId);
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

        _logger.LogInformation("Fetched {response.Count} reviews with this company Id {companyId}.", response.Count, companyId);
        return response;
    }

    public ReviewResponseModel GetById(int id)
    {
        _logger.LogInformation("Getting review by Id {id}", id);
        var dbReview = _repository.FindById(id);
        if (dbReview==null)
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

    public ReviewResponseModel Add(ReviewRequestModel request)
    {
        _logger.LogInformation("Adding new review to this queue Id {request.QueueId}" ,request.QueueId);
        var requestToCreate = request as CreateReviewRequest;
        
        var queue = _queueRepository.FindById(requestToCreate.QueueId);
        if (queue==null)
        {
            _logger.LogWarning("Queue with Id {request.QueueId} not found for adding new review.", request.QueueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var customer = _customerRepository.FindById(request.CustomerId);
        if (customer==null)
        {
            _logger.LogWarning("Customer with Id {request.CustomerId} not found for adding new review.", request.CustomerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }
        
        if (requestToCreate==null)
        {
            _logger.LogError($"Invalid request model while adding new review");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(ReviewEntity));
        }
        

        if (queue.Status != QueueStatus.Completed)
        {
            _logger.LogError("Invalid queue status while adding new review for this queue Id {queueId}", requestToCreate.QueueId);
            throw new Exception("You can leave review only if status is completed");
        }

        var reviews = GetAllReviewsByQueue(queue.Id);
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

        if (review.Grade <1 || review.Grade>5)
        {
            _logger.LogError("Invalid grade for review.");
            throw new Exception("Grade should be between 1 and 5!");
        }
        
        _repository.Add(review);
        _repository.SaveChanges();

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