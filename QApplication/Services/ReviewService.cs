using System.Net;
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
    public ReviewService(IReviewRepository repository, IQueueRepository queueRepository)
    {
        _repository = repository;
        _queueRepository = queueRepository;
    }

    public IEnumerable<ReviewResponseModel> GetAll(int pageList, int pageNumber)
    {
        var dbReview = _repository.GetAll(pageList, pageNumber);
        if (dbReview==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ReviewEntity));
        }

        var response = dbReview.Select(review => new ReviewResponseModel()
        {
            Id = review.Id,
            CustomerId = review.CustomerId,
            QueueId = review.QueueId,
            Grade = review.Grade,
            ReviewText = review.ReviewText
        }).ToList();

        return response;
    }

    public IEnumerable<ReviewResponseModel> GetAllReviewsByQueue(int queueId)
    {
        var dbReview = _repository.GetAllReviewsByQueue(queueId);
        if (dbReview==null)
        {
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

        return response;
    }

    public ReviewResponseModel GetById(int id)
    {
        var dbReview = _repository.FindById(id);
        if (dbReview==null)
        {
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

        return response;
    }

    public ReviewResponseModel Add(ReviewRequestModel request)
    {
        var requestToCreate = request as CreateReviewRequest;
        if (requestToCreate==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(ReviewEntity));
        }

        var queue = _queueRepository.FindById(requestToCreate.QueueId);
        if (queue==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        if (queue.Status != QueueStatus.Completed)
        {
            throw new Exception("You can leave review only if status is completed");
        }

        var reviews = GetAllReviewsByQueue(queue.Id);
        var isDouble = reviews.Any(s => s.CustomerId == queue.CustomerId);
        if (isDouble)
        {
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
            throw new Exception("Grade should be between 1 and 5!");
        }
        
        _repository.Add(review);
        _repository.SaveChanges();

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