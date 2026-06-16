namespace QAuthService.Contracts.Events.Enums;

public enum UpdatedQueueStatus
{
    Confirmed,
    CanceledByCustomer,
    CanceledByEmployee,
    CanceledByAdmin,
    Completed
}