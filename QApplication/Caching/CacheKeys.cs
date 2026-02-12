namespace QApplication.Caching;

public static class CacheKeys
{
    public static string CompanyById(int id)
        => $"company:{id}";

    public static string AllCompaniesKey = "companies:pages";
    public static string AllCompaniesFiled(int pageNumber)
        => $"{pageNumber}";
    

    public static string CustomerById(int id)
        => $"queue:{id}";

    public static string QueueId(int id) 
        => $"queue:{id}";
    
    public static string EmployeeId(int id)
        => $"queue_Employee:{id}";
    
    public static string AllQueuesHashKey => "queues:pages";
    public static string AllQueuesField(int pageNumber )
        => $"{pageNumber}";
    
    public static string CustomerQueuesHashKey(int customerId)
        => $"customer:{customerId}:queues";
    
    public static string CustomerQueuesField(int pageNumber)
        => $"{pageNumber}";
    
}