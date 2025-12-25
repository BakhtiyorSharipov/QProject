using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Reviews.Queries.GetReviewById;

public class GetReviewByIdQueryHandler: IRequestHandler<GetReviewByIdQuery, ReviewResponseModel>
{
    private readonly ILogger<GetReviewByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetReviewByIdQueryHandler(ILogger<GetReviewByIdQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ReviewResponseModel> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting review by Id {id}", request.Id);
        var dbReview = await _dbContext.Reviews.FirstOrDefaultAsync(s=>s.Id== request.Id, cancellationToken);
        if (dbReview == null)
        {
            _logger.LogWarning("Review with Id {id} not found.", request.Id);
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

        _logger.LogInformation("Review with Id {id} fetched successfully.", request.Id);
        return response;
    }
}