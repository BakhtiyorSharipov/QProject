namespace QApplication.Caching;

public static class CacheKeys
{
    public static string CompanyById(int id)
        => $"company:{id}";

    public static string AllCompanies(int pageNumber, int pageSize)
        => $"companies:page:{pageNumber}:size:{pageSize}";

    public static string CompanyServices(int companyId)
        => $"company:{companyId}:services";

    public static string EmployeeById(int id)
        => $"employee:{id}";

    public static string CustomerById(int id)
        => $"customer:{id}";

    public static string QueueId(int id)
        => $"queue:{id}";

    public static string CustomerQueues(int customerId, int pageNumber, int pageSize)
        => $"customer:{customerId}:page:{pageNumber}:size:{pageSize}";
    
    public static string AllQueues(int pageNumber, int pageSize)
        => $"queues:page:{pageNumber}:size:{pageSize}";
}