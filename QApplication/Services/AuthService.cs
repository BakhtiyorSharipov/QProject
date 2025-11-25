using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.Services;

public class AuthService: IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, ITokenService tokenService, IPasswordHasher<User> passwordHasher, IConfiguration config)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _config = config;
    }


    public AuthResponse Login(LoginRequest request)
    {
        var user = _userRepository.FindByEmail(request.Email);
        if (user == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "Invalid credentials");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "Invalid credentials");
        }

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
        
        _refreshTokenRepository.Add(refreshEntity);
        _refreshTokenRepository.SaveChanges();

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

    public AuthResponse Refresh(RefreshTokenRequest request)
    {
        var stored = _refreshTokenRepository.FindByToken(request.RefreshToken);
        if (stored == null) throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "Invalid refresh token");
        if (stored.RevokedAt.HasValue) throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "Refresh token revoked");
        if (stored.ExpiresAt < DateTime.UtcNow) throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "Refresh token expired");

        var user = _userRepository.FindById(stored.UserId) ?? throw new HttpStatusCodeException(HttpStatusCode.NotFound, "User not found");
        
        stored.RevokedAt = DateTime.UtcNow;
        _refreshTokenRepository.Update(stored);
        
        var accessExpire = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "60"));
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.EmailAddress, user.Roles.ToString(), accessExpire);

        var (refreshToken, refreshExpires) = _tokenService.GenerateRefreshToken();
        var refreshEntity = new RefreshTokenEntity
        {
            Token = refreshToken,
            ExpiresAt = refreshExpires,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        _refreshTokenRepository.Add(refreshEntity);
        _refreshTokenRepository.SaveChanges();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessExpire,
            RefreshTokenExpiresAt = refreshExpires,
            UserId = user.Id,
            EmailAddress = user.EmailAddress,
            Role = user.Roles.ToString()
        };
    }

    public void Logout(string refreshToken)
    {
        var stored = _refreshTokenRepository.FindByToken(refreshToken);
        if (stored == null)
        {
            return;
        }
        
        stored.RevokedAt = DateTime.UtcNow;
        _refreshTokenRepository.Update(stored);
        _refreshTokenRepository.SaveChanges();
    }

    public User RegisterCustomer(RegisterCustomerRequestModel request)
    {
        if (_userRepository.FindByEmail(request.EmailAddress) != null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Username already exists");
        }

        var user = new User
        {
            EmailAddress= request.EmailAddress,
            Roles = UserRoles.Customer,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _userRepository.Add(user);
        _userRepository.SaveChanges();
        return user;
    }

    public User CreateEmployee(CreateEmployeeRequest request, int createdByUserId)
    {
        var creator = _userRepository.FindById(createdByUserId);
        if (creator==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Creator not found");
        }

        if (creator.Roles != UserRoles.CompanyAdmin && creator.Roles != UserRoles.SystemAdmin)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Not allowed to create employee");
        }

        if (_userRepository.FindByEmail(request.EmailAddress) != null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Username already exists");
        }

        var user = new User
        {
            EmailAddress = request.EmailAddress,
            Roles = UserRoles.Employee
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _userRepository.Add(user);
        _userRepository.SaveChanges();
        return user;
    }

    public User CreateCompanyAdmin(CreateCompanyAdminRequest request, int createdByUserId)
    {
        var creator = _userRepository.FindById(createdByUserId);
        if (creator==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Creator not found");

        }

        if (creator.Roles != UserRoles.SystemAdmin)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Not allowed to create barbershop admin");
        }

        if (_userRepository.FindByEmail(request.EmailAddress) != null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Username already exists");
        }

        var user = new User
        {
            EmailAddress = request.EmailAddress,
            Roles = UserRoles.CompanyAdmin
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _userRepository.Add(user);
        _userRepository.SaveChanges();
        return user;
    }
}