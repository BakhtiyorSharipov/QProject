using MediatR;
using QApplication.Responses.AvailabilityResponse;
using QDomain.Enums;

namespace QApplication.UseCases.Employees.Queries.GetEmployeeSchedule;

public record GetEmployeeScheduleQuery(int EmployeeId, DateTimeOffset Date): IRequest<GetEmployeeAvailabilityResponse>;