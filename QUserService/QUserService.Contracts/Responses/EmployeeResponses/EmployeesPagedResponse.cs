using MessagePack;

namespace QUserService.Contracts.Responses.EmployeeResponses;

[MessagePackObject]
public class EmployeesPagedResponse
{
    [Key(0)]
    public List<EmployeeInfo> Items { get; set; } = new();
    
    [Key(1)]
    public int PageNumber { get; set; }
    
    [Key(2)]
    public int PageSize { get; set; }
    
    [Key(3)]
    public int TotalCount { get; set; }
    
    [Key(4)]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}