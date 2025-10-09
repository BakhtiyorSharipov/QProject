namespace QDomain.Models;

public class AvailabilityScheduleEntity : BaseEntity
{
    
    public string? Description { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
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