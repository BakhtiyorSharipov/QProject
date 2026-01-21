using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Services.Queries.GetServiceById;

public record GetServiceByIdQuery(int Id): IRequest<ServiceResponseModel>;