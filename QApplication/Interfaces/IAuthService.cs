using Microsoft.AspNetCore.Identity.Data;
using QApplication.Requests;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IAuthService
{
    AuthResponse Login(LoginRequest request);
    AuthResponse Refresh(RefreshTokenRequest request);
    void Logout(string refreshToken);
    User RegisterCustomer(RegisterCustomerRequestModel request);
    User CreateEmployee(CreateEmployeeRequest request, int createdByUserId);
    User CreateCompanyAdmin(CreateCompanyAdminRequest request, int createdByUserId);
}