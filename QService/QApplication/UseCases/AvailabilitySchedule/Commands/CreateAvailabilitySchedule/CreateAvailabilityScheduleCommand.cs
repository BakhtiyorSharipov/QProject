using MediatR;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.AvailabilitySchedule.Commands.CreateAvailabilitySchedule;

public class CreateAvailabilityScheduleCommand: IRequest<List<AvailabilityScheduleResponseModel>>
{
    public int EmployeeId { get; set; }
    public string? Description { get; set; }
    public RepeatSlot RepeatSlot { get; set; } = RepeatSlot.None;
    public int? RepeatDuration { get; set; }
    public List<Interval<DateTimeOffset>> AvailableSlots { get; set; } = [];

    public CreateAvailabilityScheduleCommand(int employeeId, string? description, RepeatSlot repeatSlot, int? repeatDuration)
    {
        EmployeeId = employeeId;
        Description = description;
        RepeatSlot = repeatSlot;
        RepeatDuration = repeatDuration;
    }
}