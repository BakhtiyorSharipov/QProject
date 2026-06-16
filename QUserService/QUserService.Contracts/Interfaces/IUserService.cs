using MagicOnion;
using QUserService.Contracts.Requests.BlockedCustomersRequests;
using QUserService.Contracts.Requests.CustomerRequests;
using QUserService.Contracts.Requests.EmployeeRequests;
using QUserService.Contracts.Requests.UserRequests;
using QUserService.Contracts.Responses.BlockedCustomersResponses;
using QUserService.Contracts.Responses.CustomerResponses;
using QUserService.Contracts.Responses.EmployeeResponses;
using QUserService.Contracts.Responses.UserResponses;

namespace QUserService.Contracts.Interfaces;

public interface IUserService: IService<IUserService>
{
    UnaryResult<List<EmployeeInfo>> GetAllCompanyEmployees(int companyId);
    UnaryResult<List<BlockedCustomerInfo>> GetAllCompanyBlockedCustomers(int companyId);
    UnaryResult<List<EmployeeInfo>> GetAllEmployees();
    UnaryResult<List<CustomerInfo>> GetAllCustomers();
    
    
    
    
    UnaryResult<CustomerResponse> GetCustomerById(CustomerByIdRequest request);
    UnaryResult<CustomerResponse> GetCustomerByUserId(GetCustomerByUserIdRequest request);
    UnaryResult<CustomerValidationResponse> ValidateCustomerNotBlocked(CustomerBlockValidationRequest request);
    
    UnaryResult<EmployeeResponse> GetEmployeeById(EmployeeByIdRequest request);
    UnaryResult<EmployeeResponse> GetEmployeeByUserId(GetEmployeeByUserIdRequest request);
    UnaryResult<EmployeeAvailabilityResponse> CheckEmployeeAvailability(EmployeeAvailabilityRequest request);
    UnaryResult<EmployeeScheduleResponse> GetEmployeeSchedule(EmployeeScheduleRequest request);
    
    UnaryResult<UserResponse> GetUserById(UserByIdRequest request);
    UnaryResult<UserResponse> GetUserByCustomerId(GetUserByCustomerIdRequest request);
    UnaryResult<UserResponse> GetUserByEmployeeId(GetUserByEmployeeIdRequest request);
    UnaryResult<UserEmailResponse> GetUserEmailByCustomerId(GetUserEmailByCustomerIdRequest request);
    
    UnaryResult<BlockedCustomerResponse> GetBlockedCustomerById(BlockedCustomerByIdRequest request);
    UnaryResult<BlockedCustomerValidationResponse> IsCustomerBlockedForCompany(IsCustomerBlockedRequest request);
    UnaryResult<BlockCustomerResponse> BlockCustomer(BlockCustomerRequest request, CancellationToken cancellationToken);
    UnaryResult<UnblockCustomerResponse> UnblockCustomer(UnblockCustomerRequest request, CancellationToken cancellationToken);
    
    UnaryResult<CurrentUserResponse> GetCurrentUserInfo(CurrentUserRequest request);
    UnaryResult<CurrentEmployeeResponse> GetCurrentEmployee(CurrentUserRequest request);
    UnaryResult<CurrentCustomerResponse> GetCurrentCustomer(CurrentUserRequest request);
    UnaryResult<IsEmployeeResponse> IsCurrentUserEmployee(CurrentUserRequest request);
}