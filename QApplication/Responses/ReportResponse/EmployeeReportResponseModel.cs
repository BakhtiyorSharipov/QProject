namespace QApplication.Responses.ReportResponse;

public class EmployeeReportResponseModel
{
    public List<EmployeeReportItemResponseModel> Employees { get; set; } = new();
    public int TotalEmployees { get; set; }
}