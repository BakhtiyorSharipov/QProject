using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService, IPasswordHasher<User> passwordHasher, IConfiguration config,
        ILogger<AuthService> logger, ICustomerRepository customerRepository, IEmployeeRepository employeeRepository)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _config = config;
        _logger = logger;
        _customerRepository = customerRepository;
        _employeeRepository = employeeRepository;
    }


    public async Task<AuthResponse> LoginAsync(LoginRequestModel request)
    {
        _logger.LogInformation("Starting login with {email} email address", request.EmailAddress);
        var user = await _userRepository.FindByEmailAsync(request.EmailAddress);
        if (user == null)
        {
            _logger.LogWarning("User with {email} email address not found", request.EmailAddress);
            throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "User not found");
        }

        _logger.LogDebug("Verifying password");
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Password failed while verifying");
            throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "Invalid password");
        }

        _logger.LogDebug("Generating access and refresh tokens");
        var accessExpires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "60"));
        var accessToken =
            _tokenService.GenerateAccessToken(user.Id, user.EmailAddress, user.Roles.ToString(), accessExpires);
        var (refreshToken, refreshExpires) = _tokenService.GenerateRefreshToken();
        var refreshEntity = new RefreshTokenEntity
        {
            Token = refreshToken,
            ExpiresAt = refreshExpires,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.AddAsync(refreshEntity);
        await _refreshTokenRepository.SaveChangesAsync();
        _logger.LogInformation("Refresh token saved successfully");
        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessExpires,
            RefreshTokenExpiresAt = refreshExpires,
            UserId = user.Id,
            EmailAddress = user.EmailAddress,
            Role = user.Roles.ToString()
        };
    }


    public async Task LogoutAsync(string refreshToken)
    {
        _logger.LogInformation("Logout with {token} refresh token", refreshToken);
        var stored = await _refreshTokenRepository.FindByTokenAsync(refreshToken);
        if (stored == null)
        {
            _logger.LogWarning("Token was not found");
            return;
        }

        stored.RevokedAt = DateTime.UtcNow;
        _refreshTokenRepository.Update(stored);
        await _refreshTokenRepository.SaveChangesAsync();
        _logger.LogInformation("Token successfully updated");
    }

    public async Task<User> RegisterCustomerAsync(RegisterCustomerRequestModel request)
    {
        _logger.LogInformation("Registering customer with {email} email address", request.EmailAddress);
        if (await _userRepository.FindByEmailAsync(request.EmailAddress) != null)
        {
            _logger.LogWarning("Customer with {email} email address already exists", request.EmailAddress);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Email address already exists");
        }

        var customer = new CustomerEntity()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();
        
        
        var user = new User
        {
            CustomerId = customer.Id,
            EmailAddress = request.EmailAddress,
            Roles = UserRoles.Customer,
            CreatedAt = DateTime.UtcNow
        };
        
        _logger.LogDebug("Hashing password");
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        _logger.LogInformation("Customer registered successfully");
        return user;
    }

    public async Task<User> CreateEmployeeAsync(CreateEmployeeRoleRequest request, int createdByUserId) 
    {
        _logger.LogInformation("Registering employee with {email} email address", request.EmailAddress);
        _logger.LogDebug("Finding creator Id for registering employee");
        var creator = await _userRepository.FindByIdAsync(createdByUserId);
        if (creator == null)
        {
            _logger.LogWarning("Creator with Id {id} not found", createdByUserId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Creator not found");
        }

        _logger.LogDebug("Checking is creator role valid for creating employee");
        if (creator.Roles != UserRoles.CompanyAdmin && creator.Roles != UserRoles.SystemAdmin)
        {
            _logger.LogWarning("Not allowed to create. Creator's role: {role}", creator.Roles);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Not allowed to create employee");
        }

        _logger.LogDebug("Checking email for already exists emails");
        if (await _userRepository.FindByEmailAsync(request.EmailAddress) != null)
        {
            _logger.LogWarning("Email is already exists.");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Email already exists");
        }

        var employee = new EmployeeEntity
        {
            ServiceId = request.ServiceId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Position = request.Position,
            CreatedAt = DateTime.UtcNow
        };

        await _employeeRepository.AddAsync(employee);
        await _employeeRepository.SaveChangesAsync();
        
        var user = new User
        {
            EmployeeId = employee.Id,
            EmailAddress = request.EmailAddress,
            Roles = UserRoles.Employee
        };
        
        
        _logger.LogDebug("Hashing password");
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        _logger.LogInformation("Employee with {email} email address registered successfully");
        return user;
    }

    public async Task<User> CreateCompanyAdminAsync(CreateCompanyAdminRequest request, int createdByUserId)
    {
        _logger.LogInformation("Registering companyAdmin with {email} email address", request.EmailAddress);
        _logger.LogDebug("Finding creator Id for registering employee");
        var creator =await  _userRepository.FindByIdAsync(createdByUserId);
        if (creator == null)
        {
            _logger.LogWarning("Creator with Id {id} not found", createdByUserId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Creator not found");
        }

        _logger.LogDebug("Checking is creator role valid for creating employee");
        if (creator.Roles != UserRoles.SystemAdmin)
        {
            _logger.LogWarning("Not allowed to create. Creator's role: {role}", creator.Roles);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Not allowed to create barbershop admin");
        }

        _logger.LogDebug("Checking email for already exists emails");
        if (await _userRepository.FindByEmailAsync(request.EmailAddress) != null)
        {
            _logger.LogWarning("Email is already exists.");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Email already exists");
        }

        var employee = new EmployeeEntity
        {
            ServiceId = null,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            PhoneNumber = request.PhoneNumber,
            Position = request.Position
        };

        await _employeeRepository.AddAsync(employee);
        await _employeeRepository.SaveChangesAsync();
        
        var user = new User
        {
            EmployeeId = employee.Id,
            EmailAddress = request.EmailAddress,
            Roles = UserRoles.CompanyAdmin
        };

        _logger.LogDebug("Hashing password");
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        _logger.LogInformation("Employee with {email} email address registered successfully");
        return user;
    }
}