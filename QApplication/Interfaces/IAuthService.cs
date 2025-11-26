using Microsoft.AspNetCore.Identity.Data;
using QApplication.Requests;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequestModel request);
    Task LogoutAsync(string refreshToken);
    Task<User> RegisterCustomerAsync(RegisterCustomerRequestModel request);
    Task<User> CreateEmployeeAsync(CreateEmployeeRoleRequest roleRequest, int createdByUserId);
    Task<User> CreateCompanyAdminAsync(CreateCompanyAdminRequest request, int createdByUserId);
}