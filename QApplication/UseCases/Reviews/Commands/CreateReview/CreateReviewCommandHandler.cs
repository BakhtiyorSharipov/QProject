using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler: IRequestHandler<CreateReviewCommand, ReviewResponseModel>
{
    private readonly ILogger<CreateReviewCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public CreateReviewCommandHandler(ILogger<CreateReviewCommandHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ReviewResponseModel> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding new review to this queue Id {request.QueueId}", request.QueueId);


        var queue = await _dbContext.Queues.FirstOrDefaultAsync(s => s.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            _logger.LogWarning("Queue with Id {request.QueueId} not found for adding new review.", request.QueueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var customer =
            await _dbContext.Customers.FirstOrDefaultAsync(s => s.Id == request.CustomerId, cancellationToken);
        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {request.CustomerId} not found for adding new review.",
                request.CustomerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        if (queue.Status != QueueStatus.Completed)
        {
            _logger.LogError("Invalid queue status while adding new review for this queue Id {queueId}",
                request.QueueId);
            throw new Exception("You can leave review only if status is completed");
        }

        var reviews = await _dbContext.Reviews.Where(s => s.QueueId == request.QueueId).ToListAsync(cancellationToken);
        var isDouble = reviews.Any(s => s.CustomerId == queue.CustomerId);
        if (isDouble)
        {
            _logger.LogError("Overlapping review for this queue Id {queueId}.", request.QueueId);
            throw new Exception("You have already left a review for this queue!");
        }

        var review = new ReviewEntity()
        {
            CustomerId = request.CustomerId,
            QueueId = request.QueueId,
            Grade = request.Grade,
            ReviewText = request.ReviewText,
            CreatedAt = DateTime.UtcNow
        };

        if (review.Grade < 1 || review.Grade > 5)
        {
            _logger.LogError("Invalid grade for review.");
            throw new Exception("Grade should be between 1 and 5!");
        }

        await _dbContext.Reviews.AddAsync(review, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

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