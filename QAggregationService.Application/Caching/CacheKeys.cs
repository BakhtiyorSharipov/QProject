namespace QAggregationService.Application.Caching;

public static class CacheKeys
{
    public static string EmployeeId(int id)
        => $"Employee:{id}";
    public static string CompanyEmployees(int companyId)
        => $"Company:{companyId}:Employees";

    public static string AllEmployees()
        => "AllEmployees:";

    public static string CustomerId(int id)
        => $"Customer:{id}";

    public static string CompanyCustomers(int companyId)
        => $"Company:{companyId}:Customers";
    
    public static string AllCustomers()
    =>"AllCustomers:";
    
    public static string BlockedCustomerId(int id)
        => $"BlockedCustomer:{id}";

    public static string CompanyBlockedCustomers(int companyId)
        => $"Company:{companyId}:BlockedCustomers";

    public static string BranchId(int id)
        => $"Branch:{id}";

    public static string CompanyBranches(int companyId)
        => $"Company:{companyId}:Branches";

    public static string CompanyServiceId(int id)
        => $"CompanyServiceId:{id}";

    public static string CompanyServices(int companyId)
        => $"Company:{companyId}:Services";
    


}