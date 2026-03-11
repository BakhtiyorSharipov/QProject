using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Extensions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Reviews.Queries.GetReviewById;

public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, ReviewResponseModel>
{
    private readonly ILogger<GetReviewByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetReviewByIdQueryHandler(ILogger<GetReviewByIdQueryHandler> logger, IQueueApplicationDbContext dbContext,
        IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<ReviewResponseModel> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting review by Id {id}", request.Id);

        var isEmployee = await _contextAccessor.IsEmployee(_dbContext, cancellationToken);
        int employeeId = 0;
        int customerId = 0;
        if (isEmployee)
        {
            var currentEmployee = await _contextAccessor.CurrentEmployee(_dbContext, cancellationToken);
            employeeId = currentEmployee.Id;
        }
        else
        {
            var currentCustomer = await _contextAccessor.CurrentCustomer(_dbContext, cancellationToken);
            customerId = currentCustomer.Id;
        }

        var dbReview = await _dbContext.Reviews
            .Include(s=>s.Queue.Employee)
            .Where(s => isEmployee
                ? s.Queue.EmployeeId == employeeId
                : s.CustomerId == customerId)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
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
            EmployeeId = dbReview.Queue.EmployeeId,
            Grade = dbReview.Grade,
            ReviewText = dbReview.ReviewText
        };

        _logger.LogInformation("Review with Id {id} fetched successfully.", request.Id);
        return response;
    }
}