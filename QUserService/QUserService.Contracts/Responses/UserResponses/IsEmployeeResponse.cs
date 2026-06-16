using MessagePack;

namespace QUserService.Contracts.Responses.UserResponses;

[MessagePackObject]
public class IsEmployeeResponse
{
    [Key(0)]
    public bool IsEmployee { get; set; }
}