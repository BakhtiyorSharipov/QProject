using QDomain.Enums;

namespace QApplication.Requests;

public class GetEmployeeAvailabilityRequest
{
    public int EmployeeId { get; set; }

    public DateTimeOffset Date { get; set; }
    
}