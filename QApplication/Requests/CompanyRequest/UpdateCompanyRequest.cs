namespace QApplication.Requests.CompanyRequest;

public class UpdateCompanyRequest
{
    public string CompanyName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
}