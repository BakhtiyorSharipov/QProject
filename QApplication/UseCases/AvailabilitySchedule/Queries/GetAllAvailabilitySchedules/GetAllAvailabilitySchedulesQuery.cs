using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.AvailabilitySchedule.Queries.GetAllAvailabilitySchedules;

public record GetAllAvailabilitySchedulesQuery(int PageNumber, int PageSize):IRequest<PagedResponse<AvailabilityScheduleResponseModel>>;