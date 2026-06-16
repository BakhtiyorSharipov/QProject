using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.AvailabilitySchedule.Queries.GetAllAvailabilitySchedules;

public record GetAllAvailabilitySchedulesQuery(int PageNumber):IRequest<PagedResponse<AvailabilityScheduleResponseModel>>;