using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.AvailabilitySchedule.Commands.DeleteAvailabilitySchedule;

public class DeleteAvailabilityScheduleCommandHandler : IRequestHandler<DeleteAvailabilityScheduleCommand, bool>
{
    private readonly ILogger<DeleteAvailabilityScheduleCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public DeleteAvailabilityScheduleCommandHandler(ILogger<DeleteAvailabilityScheduleCommandHandler> logger,
        IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeleteAvailabilityScheduleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting schedule with Id: {id}, DeleteAllSlots: {delete}", request.Id,
            request.DeleteAllSlots);

        var dbAvailabilitySchedule =
            await _dbContext.AvailabilitySchedules.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (dbAvailabilitySchedule == null)
        {
            _logger.LogWarning("Schedule with Id {id} not found for deleting", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }


        var allSchedules = _dbContext.AvailabilitySchedules;
        if (request.DeleteAllSlots)
        {
            if (dbAvailabilitySchedule.GroupId == null)
            {
                _logger.LogDebug("GroupId == null");
                throw new Exception("This schedule is not part of a group, nothing to delete in group.");
            }

            var scheduleToDelete = await allSchedules
                .Where(s => s.GroupId == dbAvailabilitySchedule.GroupId).ToListAsync(cancellationToken);

            if (!scheduleToDelete.Any())
            {
                _logger.LogWarning("No schedules found for GroupId {id}", dbAvailabilitySchedule.GroupId);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "No schedule found for the given GroupId");
            }

            foreach (var schedule in scheduleToDelete)
            {
                _dbContext.AvailabilitySchedules.Remove(schedule);
                _logger.LogDebug("Deleted schedule Id {id} from group Id {groupId}", schedule.Id, schedule.GroupId);
            }
        }
        else
        {
            _dbContext.AvailabilitySchedules.Remove(dbAvailabilitySchedule);
            _logger.LogDebug("Deleted single schedule with Id: {scheduleId}", request.Id);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Deletion successfully completed for schedule Id {id}", request.Id);

        return true;
    }
}