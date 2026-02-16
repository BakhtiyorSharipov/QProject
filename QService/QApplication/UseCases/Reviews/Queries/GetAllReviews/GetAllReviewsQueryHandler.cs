using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QApplication.Responses;

namespace QApplication.UseCases.Reviews.Queries.GetAllReviews;

public class GetAllReviewsQueryHandler: IRequestHandler<GetAllReviewsQuery, PagedResponse<ReviewResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllReviewsQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetAllReviewsQueryHandler(ILogger<GetAllReviewsQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<ReviewResponseModel>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all reviews. PageNumber: {pageNumber}, PageSize: {pageSize}", request.PageNumber,
            PageSize);

        var totalCount = await _dbContext.Reviews.CountAsync(cancellationToken);

        var dbReviews =await  _dbContext.Reviews
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        var response = dbReviews.Select(reviews => new ReviewResponseModel()
        {
            Id = reviews.Id,
            CustomerId = reviews.CustomerId,
            QueueId = reviews.QueueId,
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