using System.ComponentModel.DataAnnotations.Schema;
using QDomain.Enums;
using QDomain.Events;

namespace QDomain.Models;

public class QueueEntity: BaseEntity
{
    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }
    public string? CancelReason { get; set; }

    public QueueStatus Status { get; set; } = QueueStatus.Pending;
    public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
    
    public bool IsStartingSoonNotified { get; set; } = false;

    public int EmployeeId { get; set; }
    public EmployeeEntity Employee { get; set; }
    
    public int CustomerId { get; set; }
    public CustomerEntity Customer { get; set; }
    
    public int ServiceId { get; set; }
    public ServiceEntity Service { get; set; }
    
    private readonly List<BaseEvent> _domainEvents = new();
   [NotMapped] public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Book()
    {
        Status = QueueStatus.Pending;

        var bookedEvent = new QueueBookedEvent
        {
            QueueId = Id,
            CustomerId = CustomerId,
            EmployeeId = EmployeeId,
            StartTime = StartTime,
            OccuredAt = DateTime.UtcNow
        };
        
        _domainEvents.Add(bookedEvent);
    }

    public void Confirm()
    {
        Status = QueueStatus.Confirmed;

        var confirmedEvent = new QueueConfirmedEvent
        {
            QueueId = Id,
            CustomerId = CustomerId,
            EmployeeId = EmployeeId,
            StartTime = StartTime,
            OccuredAt = DateTime.UtcNow
        };
        
        _domainEvents.Add(confirmedEvent);
    }


    public void Complete()
    {
        Status = QueueStatus.Completed;
        
        var completedEvent = new QueueCompletedEvent
        {
            QueueId = Id,
            CustomerId = CustomerId,
            EmployeeId = EmployeeId,
            StartTime = StartTime,
            OccuredAt = DateTime.UtcNow
        };
        
        _domainEvents.Add(completedEvent);
    }

    public void CancelByCustomer()
    {
        Status = QueueStatus.CancelledByCustomer;

        var canceledByCustomerEvent = new QueueCanceledByCustomerEvent
        {
            QueueId = Id,
            CustomerId = CustomerId,
            EmployeeId = EmployeeId,
            Reason = CancelReason,
            OccuredAt = DateTime.UtcNow
        };
        _domainEvents.Add(canceledByCustomerEvent);
    }

    public void CancelByEmployee()
    {
        Status = QueueStatus.CancelledByEmployee;

        var canceledByEmployeeEvent = new QueueCanceledByEmployeeEvent
        {
            QueueId = Id,
            CustomerId = CustomerId,
            EmployeeId = EmployeeId,
            Reason = CancelReason,
            OccuredAt = DateTime.UtcNow
        };
        
        _domainEvents.Add(canceledByEmployeeEvent);
    }

    public void CancelByAdmin()
    {
        Status = QueueStatus.CanceledByAdmin;

        var canceledByAdmin = new QueueCanceledByAdminEvent
        {
            QueueId = Id,
            CustomerId = CustomerId,
            EmployeeId = EmployeeId,
            Reason = CancelReason,
            OccuredAt = DateTime.UtcNow
        };
        
        _domainEvents.Add(canceledByAdmin);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}