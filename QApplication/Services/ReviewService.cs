using System.Net;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.ReviewRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public class ReviewService:  IReviewService
{
    private readonly IReviewRepository _repository;

    public ReviewService(IReviewRepository repository)
    {
        _repository = repository;
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
            EmployeeId = review.EmployeeId,
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
            EmployeeId = dbReview.EmployeeId,
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

        var review = new ReviewEntity()
        {
            CustomerId = requestToCreate.CustomerId,
            EmployeeId = requestToCreate.EmployeeId,
            QueueId = requestToCreate.QueueId,
            Grade = requestToCreate.Grade,
            ReviewText = requestToCreate.ReviewText
        };
        
        _repository.Add(review);
        _repository.SaveChanges();

        var response = new ReviewResponseModel()
        {
            Id = review.Id,
            CustomerId = review.CustomerId,
            EmployeeId = review.EmployeeId,
            QueueId = review.QueueId,
            Grade = review.Grade,
            ReviewText = review.ReviewText
        };

        return response;
    }

    public ReviewResponseModel Update(int id, ReviewRequestModel request)
    {
        var dbReview = _repository.FindById(id);
        if (dbReview==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ReviewEntity));
        }

        var requestToUpdate = request as UpdateReviewRequest;
        if (requestToUpdate== null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(ReviewEntity));
        }

        dbReview.CustomerId = requestToUpdate.CustomerId;
        dbReview.EmployeeId = requestToUpdate.EmployeeId;
        dbReview.QueueId = requestToUpdate.QueueId;
        dbReview.Grade = requestToUpdate.Grade;
        dbReview.ReviewText = requestToUpdate.ReviewText;
        
        _repository.Update(dbReview);
        _repository.SaveChanges();

        var response = new ReviewResponseModel()
        {
            Id = dbReview.Id,
            CustomerId = dbReview.CustomerId,
            EmployeeId = dbReview.EmployeeId,
            QueueId = dbReview.QueueId,
            Grade = dbReview.Grade,
            ReviewText = dbReview.ReviewText
        };

        return response;
    }


    public bool Delete(int id)
    {
        var dbReview = _repository.FindById(id);
        if (dbReview== null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ReviewEntity));
        }
        
        _repository.Delete(dbReview);
        _repository.SaveChanges();

        return true;
    }
}