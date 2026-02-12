using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.AvailabilitySchedule.Queries.GetAllAvailabilitySchedules;

public record GetAllAvailabilitySchedulesQuery(int PageNumber):IRequest<PagedResponse<AvailabilityScheduleResponseModel>>;