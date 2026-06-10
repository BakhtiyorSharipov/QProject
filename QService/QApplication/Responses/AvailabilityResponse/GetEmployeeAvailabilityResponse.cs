using QDomain.Enums;

namespace QApplication.Responses.AvailabilityResponse;

public class GetEmployeeAvailabilityResponse
{
    public List<AvailabilityDayResponse> Days { get; set; } = [];
}