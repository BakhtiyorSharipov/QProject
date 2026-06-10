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

namespace QApplication.UseCases.Reviews.Queries.GetAllCustomerReviews;

public class GetReviewsByCustomerQueryHandler: IRequestHandler<GetReviewsByCustomerQuery, PagedResponse<ReviewResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetReviewsByCustomerQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetReviewsByCustomerQueryHandler(ILogger<GetReviewsByCustomerQueryHandler> logger, IQueueApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<PagedResponse<ReviewResponseModel>> Handle(GetReviewsByCustomerQuery request, CancellationToken cancellationToken)
    {
        var currentCustomer = await _contextAccessor.CurrentCustomer(_dbContext, cancellationToken);
        var customerId = currentCustomer.Id;
        

        _logger.LogInformation("Getting all customer's review. PageNumber: {pageNumber}, PageSize: {pageSize}",
            request.PageNumber, PageSize); 
        
        var query = _dbContext.Reviews
            .Include(s=>s.Queue)
            .Where(s => s.CustomerId == customerId);
        
        if (!query.Any())
        {
            _logger.LogWarning("No reviews found for CustomerId: {customerId}", customerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ReviewEntity));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var reviews = await query
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Skip((request.PageNumber - 1) * 15)
            .Take(15).ToListAsync(cancellationToken);


        var response = reviews.Select(review => new ReviewResponseModel
        {
            Id = review.Id,
            CustomerId = review.CustomerId,
            QueueId = review.QueueId,
            EmployeeId = review.Queue.EmployeeId,
            Grade = review.Grade,
            ReviewText = review.ReviewText
        }).ToList();
        
                
        _logger.LogInformation("Successfully fetched {ReviewCount} reviews for CustomerId: {CustomerId}",
            response.Count,
            customerId);
        
        
        _logger.LogInformation("Fetched {companyCount} companies.", response.Count);
        var pagedResponse=new PagedResponse<ReviewResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };


        return pagedResponse;

    }
}