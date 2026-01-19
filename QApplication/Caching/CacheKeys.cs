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
}