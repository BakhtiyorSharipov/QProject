using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.AvailabilitySchedule.Queries.GetAvailabilityScheduleById;

public record GetAvailabilityScheduleByIdQuery(int Id): IRequest<AvailabilityScheduleResponseModel>;