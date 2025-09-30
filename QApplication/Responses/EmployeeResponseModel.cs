namespace QApplication.Responses;

public class EmployeeResponseModel: BaseResponse
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Position { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}