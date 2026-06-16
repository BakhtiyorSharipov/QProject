using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.AvailabilitySchedule.Queries.GetAvailabilityScheduleById;

public record GetAvailabilityScheduleByIdQuery(int Id): IRequest<AvailabilityScheduleResponseModel>;