using QDomain.Enums;
using DayOfWeek = System.DayOfWeek;

namespace QDomain.Models;

public class AvailabilityScheduleEntity : BaseEntity
{
    public int? GroupId { get; set; }
    public string? Description { get; set; }
    public RepeatSlot RepeatSlot { get; set; } = RepeatSlot.None;
    public int? RepeatDuration { get; set; }
    public List<Interval<DateTimeOffset>> AvailableSlots { get; set; } = [];
    
    public int EmployeeId { get; set; }
    public EmployeeEntity Employee { get; set; }
}

public class Interval<T> where T : notnull
{
    public Interval(T from, T to)
    {
        From = from;
        To = to;
    }

    public T From { get; set; }
    public T To { get; set; }  
}