using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QUserService.Application.Exceptions;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;
using QUserService.Domain.Enums;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Auth.Queries.Login;

public class LoginQueryHandler: IRequestHandler<LoginQuery, AuthResponse>
{
    private readonly ILogger<LoginQueryHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;


    public LoginQueryHandler(ILogger<LoginQueryHandler> logger, IUserServiceApplicationDbContext dbContext, IPasswordHasher<UserEntity> passwordHasher, IConfiguration config, ITokenService tokenService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _config = config;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting login with {email} email address", request.EmailAddress);
        
        var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.EmailAddress == request.EmailAddress,
            cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("UserEntity with {email} email address not found", request.EmailAddress);
            throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "UserEntity not found");
        }
        
        

        _logger.LogDebug("Verifying password");
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Password failed while verifying");
            throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "Invalid password");
        }

        
        if (!user.IsEmailVerified && user.Roles != UserRoles.SystemAdmin)
        {
            throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "Email not verified");
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

        await _dbContext.RefreshTokens.AddAsync(refreshEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
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
}