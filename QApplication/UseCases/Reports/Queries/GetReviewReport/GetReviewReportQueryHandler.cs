using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses.ReportResponse;
using QApplication.UseCases.Reports.ReportQueryExtensions;
using QDomain.Models;

namespace QApplication.UseCases.Reports.Queries.GetReviewReport;

public class GetReviewReportQueryHandler: IRequestHandler<GetReviewReportQuery, ReviewReportResponseModel>
{
    private readonly ILogger<GetReviewReportQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetReviewReportQueryHandler(ILogger<GetReviewReportQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ReviewReportResponseModel> Handle(GetReviewReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting review report with filters - CompanyId: {CompanyId}, EmployeeId: {EmployeeId}, ServiceId: {ServiceId}",
            request.CompanyId, request.EmployeeId, request.ServiceId);

        var reviews = await _dbContext.GetAllReviewsAsync(cancellationToken);
        _logger.LogDebug("{reviewCount} reviews from repository", reviews.Count());

        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value > request.To.Value)
            {
                _logger.LogError("Invalid date range for review report. From: {From}, To: {To}", request.From,
                    request.To);
                throw new Exception("'From' must be less than 'To'");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.CompanyId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError(
                    "Invalid parameter combination for review report. EmployeeId cannot be used with CompanyId or ServiceId");
                throw new Exception("EmployeeId cannot be used together with CompanyId or ServiceId.");
            }
        }

        if (request.CompanyId.HasValue)
        {
            if (request.EmployeeId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError(
                    "Invalid parameter combination for review report. CompanyId cannot be used with EmployeeId or ServiceId");
                throw new Exception("CompanyId cannot be used together with EmployeeId or ServiceId.");
            }
        }


        if (request.EmployeeId.HasValue)
        {
            var found = await _dbContext.Employees.FirstOrDefaultAsync(s => s.Id == request.EmployeeId.Value,
                cancellationToken);

            if (found == null)
            {
                _logger.LogWarning("Employee with Id {id} not found for getting review report.",
                    request.EmployeeId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
            }
        }

        if (request.CompanyId.HasValue)
        {
            var found = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.CompanyId.Value,
                cancellationToken);
            if (found == null)
            {
                _logger.LogWarning("Company with Id {id} not found for getting review report.",
                    request.CompanyId.Value);

                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == request.ServiceId,
                cancellationToken);
            if (found == null)
            {
                _logger.LogWarning("Service with Id {id} not found for getting review report.",
                    request.ServiceId.Value);

                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }


        _logger.LogDebug("Applying filter: {reviewCount} reviews", reviews.Count());
        if (request.From.HasValue)
        {
            reviews = reviews.Where(s => s.Queue.StartTime >= request.From.Value.ToUniversalTime()).ToList();
            _logger.LogDebug("After From filter: {reviewCount} reviews", reviews.Count());
        }

        if (request.To.HasValue)
        {
            reviews = reviews.Where(s =>
                (s.Queue.EndTime.HasValue ? s.Queue.EndTime.Value : s.Queue.StartTime.AddMinutes(30)) <=
                request.To.Value.ToUniversalTime()).ToList();
            _logger.LogDebug("After To filter: {reviewCount} reviews", reviews.Count());
        }

        if (request.EmployeeId.HasValue)
        {
            reviews = reviews.Where(s => s.Queue.EmployeeId == request.EmployeeId.Value).ToList();
            _logger.LogDebug("After EmployeeId {id} filter: {reviewCount} reviews", request.EmployeeId.Value,
                reviews.Count());
        }

        if (request.CompanyId.HasValue)
        {
            reviews = reviews.Where(s => s.Queue.Service.CompanyId == request.CompanyId.Value).ToList();
            _logger.LogDebug("After CompanyId {id} filter: {reviewCount} reviews", request.CompanyId.Value,
                reviews.Count());
        }

        if (request.ServiceId.HasValue)
        {
            reviews = reviews.Where(s => s.Queue.Service != null && s.Queue.ServiceId == request.ServiceId.Value).ToList();
            _logger.LogDebug("After ServiceId {id} filter: {reviewCount} reviews", request.ServiceId.Value,
                reviews.Count());
        }
        
        _logger.LogDebug("Final review count: {reviewCount}", reviews.Count());

        var reviewItem = reviews.Select(review => new ReviewReportItemResponseModel
        {
            Id = review.Id,
            QueueId = review.QueueId,
            EmployeeName = review.Queue.Employee.FirstName,
            CompanyName = review.Queue.Service.Company.CompanyName,
            CustomerName = review.Customer.FirstName,
            ServiceName = review.Queue.Service.ServiceName,
            Grade = review.Grade,
            ReviewText = review.ReviewText
        }).ToList();

        int totalReviews = 0, rating1 = 0, rating2 = 0, rating3 = 0, rating4 = 0, rating5 = 0;
        double average = 0;

        if (request.IncludeStatistics)
        {
            rating1 = reviews.Count(r => r.Grade == 1);
            rating2 = reviews.Count(r => r.Grade == 2);
            rating3 = reviews.Count(r => r.Grade == 3);
            rating4 = reviews.Count(r => r.Grade == 4);
            rating5 = reviews.Count(r => r.Grade == 5);

            totalReviews = rating1 + rating2 + rating3 + rating4 + rating5;
            average = (rating1 + (rating2 * 2) + (rating3 * 3) + (rating4 * 4) + (rating5 * 5)) / totalReviews;

            _logger.LogDebug(
                "Review statistics - Total: {total}, Average: {average}, Ratings: 1={r1}, 2={r2}, 3={r3}, 4={r4}, 5={r5}",
                totalReviews, average, rating1, rating2, rating3, rating4, rating5);
        }


        var response = new ReviewReportResponseModel
        {
            Reviews = reviewItem,
            Rating1 = rating1,
            Rating2 = rating2,
            Rating3 = rating3,
            Rating4 = rating4,
            Rating5 = rating5,
            TotalReviews = totalReviews,
            AverageRating = average
        };

        _logger.LogInformation("Review report completed.");
        return response;
    }
}