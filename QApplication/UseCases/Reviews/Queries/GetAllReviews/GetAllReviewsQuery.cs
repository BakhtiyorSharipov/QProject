using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Reviews.Queries.GetAllReviews;

public record GetAllReviewsQuery(int PageNumber, int PageSize): IRequest<PagedResponse<ReviewResponseModel>>;