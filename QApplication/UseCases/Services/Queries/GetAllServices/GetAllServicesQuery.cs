using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Services.Queries.GetAllServices;

public record GetAllServicesQuery(int PageNumber, int PageSize): IRequest<PagedResponse<ServiceResponseModel>>;