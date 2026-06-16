using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QUserService.Application.Exceptions;
using QUserService.Application.Interfaces;
using QUserService.Domain.Models;

namespace QUserService.Application.Extensions;

public static class CurrentUserExtension
{
    public static async Task<UserEntity> CurrentUser(this IHttpContextAccessor contextAccessor,
        IUserServiceApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var userClaim = contextAccessor.HttpContext!.User;
        var userIdClaim = userClaim.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("UserEntity not authenticated");
        }

        var currentUser = await dbContext.Users
            .Include(s => s.Employee)
            .FirstOrDefaultAsync(s => s.Id == userId, cancellationToken);

        if (currentUser == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "User Not Found");
        }

        return currentUser;
    }

    public static async Task<EmployeeEntity> CurrentEmployee(
        this IHttpContextAccessor contextAccessor,
        IUserServiceApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userClaim = contextAccessor.HttpContext!.User;

        var userIdClaim = userClaim.FindFirst("id");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("UserEntity not authenticated");
        }

        var currentUser = await dbContext.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(s => s.Id == userId, cancellationToken);

        if (currentUser?.EmployeeId == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Current user employee  not found");
        }

        var currentEmployee = await dbContext.Employees
            .FirstOrDefaultAsync(s => s.Id == currentUser.EmployeeId, cancellationToken);

        if (currentEmployee == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Employee  not found");
        }

        return currentEmployee;
    }

    public static async Task<CustomerEntity> CurrentCustomer(this IHttpContextAccessor contextAccessor,
        IUserServiceApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userClaim = contextAccessor.HttpContext!.User;
        var userIdClaim = userClaim.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("UserEntity not authenticated");
        }

        var currentUser = await dbContext.Users
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == userId, cancellationToken);

        if (currentUser?.CustomerId == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Current user customer  not found");
        }

        var currentCustomer = await dbContext.Customer
            .FirstOrDefaultAsync(s => s.Id == currentUser.CustomerId, cancellationToken);

        if (currentCustomer == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Customer not found");
        }

        return currentCustomer;
    }

    public static async Task<bool> IsEmployee(this IHttpContextAccessor contextAccessor,
        IUserServiceApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var userClaim = contextAccessor.HttpContext!.User;
        var userIdClaim = userClaim.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("UserEntity not authenticated");
        }

        var currentUser = await dbContext.Users
            .Include(s => s.Employee)
            .FirstOrDefaultAsync(s => s.Id == userId, cancellationToken);

        if (currentUser?.EmployeeId == null)
        {
            return false;
        }

        return true;
    }
}