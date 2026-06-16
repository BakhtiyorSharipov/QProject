namespace QApplication.Caching;

public static class CacheKeys
{
    
    public static string EmployeeAvailabilityBase(int employeeId, DateTime date)
        => $"employee:{employeeId}:availability:{date:yyyy-MM-dd}:base";

    public static string EmployeeAvailabilityQueues(int employeeId, DateTime date)
        => $"employee:{employeeId}:availability:{date:yyyy-MM-dd}:queues";
    
}