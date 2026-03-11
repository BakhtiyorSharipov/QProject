using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Extensions;
using QApplication.Interfaces.Data;
using QApplication.Responses;

namespace QApplication.UseCases.Reviews.Queries.GetAllReviews;

public class GetAllReviewsQueryHandler: IRequestHandler<GetAllReviewsQuery, PagedResponse<ReviewResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllReviewsQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetAllReviewsQueryHandler(ILogger<GetAllReviewsQueryHandler> logger, IQueueApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<PagedResponse<ReviewResponseModel>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all reviews. PageNumber: {pageNumber}, PageSize: {pageSize}", request.PageNumber,
            PageSize);

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

        var totalCount = await _dbContext.Reviews
            .Where(s=>isEmployee
            ? s.Queue.EmployeeId== employeeId
            : s.CustomerId== customerId)
            .CountAsync(cancellationToken);

        var dbReviews =await  _dbContext.Reviews
            .Include(s=>s.Queue.Employee)
            .Where(s=>isEmployee
            ? s.Queue.EmployeeId== employeeId
            : s.CustomerId== customerId)
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        
        var response = dbReviews.Select(reviews => new ReviewResponseModel()
        {
            Id = reviews.Id,
            CustomerId = reviews.CustomerId,
            QueueId = reviews.QueueId,
            EmployeeId = reviews.Queue.EmployeeId,
            Grade = reviews.Grade,
            ReviewText = reviews.ReviewText
        }).ToList();
        
        _logger.LogInformation("Fetched {reviewCount} reviews.", response.Count);

        return new PagedResponse<ReviewResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }
}