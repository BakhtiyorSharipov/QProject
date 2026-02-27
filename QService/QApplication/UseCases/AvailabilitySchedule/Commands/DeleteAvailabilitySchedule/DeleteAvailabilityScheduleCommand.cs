using MediatR;

namespace QApplication.UseCases.AvailabilitySchedule.Commands.DeleteAvailabilitySchedule;

public record DeleteAvailabilityScheduleCommand(int Id, bool DeleteAllSlots): IRequest<bool>;