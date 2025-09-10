namespace QApplication.Responses;

public class CompanyResponseModel: BaseResponse
{
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    
}