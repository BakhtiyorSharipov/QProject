using MediatR;

namespace QUserService.Application.UseCases.AvailabilitySchedule.Commands.DeleteAvailabilitySchedule;

public record DeleteAvailabilityScheduleCommand(int Id, bool DeleteAllSlots): IRequest<bool>;