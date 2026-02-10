namespace QApplication.Caching;

public static class CacheKeys
{
    public static string CompanyById(int id)
        => $"company:{id}";

    public static string AllCompanies(int pageNumber, int pageSize)
        => $"companies:page:{pageNumber}:size:{pageSize}";
    

    public static string CustomerById(int id)
        => $"customer:{id}";

    public static string QueueId(int id)
        => $"queue:{id}";
    
    public static string AllQueuesHashKey => "queues:pages";
    public static string AllQueuesField(int pageNumber, int pageSize)
        => $"{pageNumber}:{pageSize}";
    
    public static string CustomerQueuesHashKey(int customerId)
        => $"customer:{customerId}:queues";
    
    public static string CustomerQueuesField(int pageNumber, int pageSize)
        => $"{pageNumber}:{pageSize}";
    
}