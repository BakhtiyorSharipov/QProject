using MessagePack;

[MessagePackObject]
public class QueueReportItem
{
    [Key(0)] public int Id { get; set; }
    [Key(1)] public string CustomerName { get; set; }
    [Key(2)] public string? EmployeeName { get; set; }
    [Key(3)] public string Status { get; set; }
    [Key(4)] public DateTimeOffset StartTime { get; set; }
    [Key(5)] public DateTimeOffset? EndTime { get; set; }
}