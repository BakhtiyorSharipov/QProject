using MagicOnion;
using MagicOnion.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QUserService.Application.Interfaces;
using QUserService.Contracts.Interfaces;
using QUserService.Contracts.Requests.BlockedCustomersRequests;
using QUserService.Contracts.Requests.CustomerRequests;
using QUserService.Contracts.Requests.EmployeeRequests;
using QUserService.Contracts.Requests.UserRequests;
using QUserService.Contracts.Responses.BlockedCustomersResponses;
using QUserService.Contracts.Responses.CustomerResponses;
using QUserService.Contracts.Responses.EmployeeResponses;
using QUserService.Contracts.Responses.UserResponses;
using QUserService.Domain.Models;

namespace QUserService.Application.Services;

public class UserService : ServiceBase<IUserService>, IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;

    public UserService(ILogger<UserService> logger, IUserServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }


    public async UnaryResult<List<EmployeeInfo>> GetAllCompanyEmployees(int companyId)
    {
        var employees = await _dbContext.Employees
            .Where(s => s.CompanyId == companyId)
            .ToListAsync();
        if (!employees.Any())
        {
            _logger.LogWarning("Not found any employee");
            return [];
        }

        var response = employees.Select(employee => new EmployeeInfo
        {
            CompanyId = employee.CompanyId,
            BranchId = employee.BranchId,
            CompanyServiceId = employee.ServiceId,
            EmployeeId = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            PhoneNumber = employee.PhoneNumber,
            CreatedAt = employee.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<List<BlockedCustomerInfo>> GetAllCompanyBlockedCustomers(int companyId)
    {
        var blockedCustomers = await _dbContext.BlockedCustomers
            .Where(s => s.CompanyId == companyId)
            .ToListAsync();
        if (!blockedCustomers.Any())
        {
            _logger.LogWarning("Not found any blocked customer");
            return [];
        }

        var response = blockedCustomers.Select(blockedCustomer => new BlockedCustomerInfo
        {
            BlockedId = blockedCustomer.Id,
            CustomerId = blockedCustomer.CustomerId,
            CompanyId = blockedCustomer.CompanyId,
            CustomerName = blockedCustomer.Customer != null
                ? $"{blockedCustomer.Customer.FirstName} {blockedCustomer.Customer.LastName}"
                : "Unknown",
            Reason = blockedCustomer.Reason,
            BannedUntil = blockedCustomer.BannedUntil,
            DoesBanForever = blockedCustomer.DoesBanForever,
            CreatedAt = blockedCustomer.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<List<EmployeeInfo>> GetAllEmployees()
    {
        var employees = await _dbContext.Employees.ToListAsync();
        if (!employees.Any())
        {
            _logger.LogWarning("Not found any employees");
            return [];
        }

        var response = employees.Select(employee => new EmployeeInfo
        {
            CompanyId = employee.CompanyId,
            BranchId = employee.BranchId,
            CompanyServiceId = employee.ServiceId,
            EmployeeId = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            PhoneNumber = employee.PhoneNumber,
            CreatedAt = employee.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<List<CustomerInfo>> GetAllCustomers()
    {
        var customers = await _dbContext.Customer.ToListAsync();
        if (!customers.Any())
        {
            _logger.LogWarning("Not found any customer");
            return [];
        }

        var response = customers.Select(customer => new CustomerInfo()
        {
            CustomerId = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            CreatedAt = customer.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<CustomerResponse> GetCustomerById(CustomerByIdRequest request)
    {
        var customer = await _dbContext.Customer
            .FirstOrDefaultAsync(s => s.Id == request.CustomerId);

        if (customer == null)
        {
            return new CustomerResponse
            {
                Id = request.CustomerId,
                FirstName = "Unknown",
                LastName = "Unknown",
                PhoneNumber = "Unknown",
                DateOfBirth = null,
                Gender = null,
                Country = null,
                City = null,
                Address = null,
                PostalCode = null,
                IsValid = false,
                ErrorMessage = "Customer not found",
                CreatedAt = DateTime.MinValue
            };
        }

        return new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            DateOfBirth = customer.DateOfBirth,
            Gender = customer.Gender,
            Country = customer.Country,
            City = customer.City,
            Address = customer.Address,
            PostalCode = customer.PostalCode,
            IsValid = true,
            ErrorMessage = null,
            CreatedAt = customer.CreatedAt
        };
    }

    public async UnaryResult<CustomerResponse> GetCustomerByUserId(GetCustomerByUserIdRequest request)
    {
        var user = await _dbContext.Users
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user == null)
        {
            return new CustomerResponse
            {
                Id = 0,
                FirstName = "Unknown",
                LastName = "Unknown",
                PhoneNumber = "Unknown",
                IsValid = false,
                ErrorMessage = "User not found",
                CreatedAt = DateTime.MinValue
            };
        }

        if (user.CustomerId == null || user.Customer == null)
        {
            return new CustomerResponse
            {
                Id = 0,
                FirstName = "Unknown",
                LastName = "Unknown",
                PhoneNumber = "Unknown",
                IsValid = false,
                ErrorMessage = "User is not a customer",
                CreatedAt = DateTime.MinValue
            };
        }

        var customer = user.Customer;

        return new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            DateOfBirth = customer.DateOfBirth,
            Gender = customer.Gender,
            Country = customer.Country,
            City = customer.City,
            Address = customer.Address,
            PostalCode = customer.PostalCode,
            IsValid = true,
            ErrorMessage = null,
            CreatedAt = customer.CreatedAt
        };
    }

    public async UnaryResult<CustomerValidationResponse> ValidateCustomerNotBlocked(
        CustomerBlockValidationRequest request)
    {
        var blockedCustomer = await _dbContext.BlockedCustomers
            .FirstOrDefaultAsync(b => b.CustomerId == request.CustomerId && b.CompanyId == request.CompanyId);

        if (blockedCustomer == null)
        {
            return new CustomerValidationResponse
            {
                IsValid = true,
                IsBlocked = false
            };
        }

        if (!blockedCustomer.DoesBanForever && blockedCustomer.BannedUntil < DateTime.UtcNow)
        {
            return new CustomerValidationResponse
            {
                IsValid = true,
                IsBlocked = false
            };
        }

        return new CustomerValidationResponse
        {
            IsValid = false,
            IsBlocked = true,
            BlockReason = blockedCustomer.Reason,
            BannedUntil = blockedCustomer.BannedUntil
        };
    }
    

    public async UnaryResult<EmployeeResponse> GetEmployeeById(EmployeeByIdRequest request)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId);

        if (employee == null)
        {
            return new EmployeeResponse
            {
                Id = request.EmployeeId,
                CompanyId = 0,
                FirstName = "Unknown",
                LastName = "Unknown",
                Position = "Unknown",
                PhoneNumber = "Unknown",
                IsValid = false,
                ErrorMessage = "Employee not found",
                CreatedAt = DateTime.MinValue
            };
        }

        return new EmployeeResponse
        {
            Id = employee.Id,
            CompanyId = employee.CompanyId,
            BranchId = employee.BranchId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            PhoneNumber = employee.PhoneNumber,
            ServiceId = employee.ServiceId,
            IsValid = true,
            ErrorMessage = null,
            CreatedAt = employee.CreatedAt
        };
    }

    public async UnaryResult<EmployeeResponse> GetEmployeeByUserId(GetEmployeeByUserIdRequest request)
    {
        var user = await _dbContext.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user == null)
        {
            return new EmployeeResponse
            {
                Id = 0,
                CompanyId = 0,
                FirstName = "Unknown",
                LastName = "Unknown",
                Position = "Unknown",
                PhoneNumber = "Unknown",
                IsValid = false,
                ErrorMessage = "User not found",
                CreatedAt = DateTime.MinValue
            };
        }

        if (user.EmployeeId == null || user.Employee == null)
        {
            return new EmployeeResponse
            {
                Id = 0,
                CompanyId = 0,
                FirstName = "Unknown",
                LastName = "Unknown",
                Position = "Unknown",
                PhoneNumber = "Unknown",
                IsValid = false,
                ErrorMessage = "User is not an employee",
                CreatedAt = DateTime.MinValue
            };
        }

        var employee = user.Employee;

        return new EmployeeResponse
        {
            Id = employee.Id,
            CompanyId = employee.CompanyId,
            BranchId = employee.BranchId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            PhoneNumber = employee.PhoneNumber,
            ServiceId = employee.ServiceId,
            IsValid = true,
            ErrorMessage = null,
            CreatedAt = employee.CreatedAt
        };
    }

   

    public async UnaryResult<EmployeeAvailabilityResponse> CheckEmployeeAvailability(
        EmployeeAvailabilityRequest request)
    {
        DateTimeOffset endTime;

        if (request.EndTime.HasValue)
        {
            endTime = request.EndTime.Value;
        }
        else if (request.DurationMinutes.HasValue)
        {
            endTime = request.StartTime.AddMinutes(request.DurationMinutes.Value);
        }
        else
        {
            return new EmployeeAvailabilityResponse
            {
                IsAvailable = false,
                ErrorMessage = "Either EndTime or DurationMinutes must be provided"
            };
        }

        var schedules = await _dbContext.AvailabilitySchedules
            .Where(s => s.EmployeeId == request.EmployeeId)
            .ToListAsync();

        if (!schedules.Any())
        {
            return new EmployeeAvailabilityResponse
            {
                IsAvailable = false,
                AvailableSlots = new List<TimeSlot>(),
                ErrorMessage = "No availability schedule found for this employee"
            };
        }

        var isWithinWorkingHours = schedules.Any(s => s.AvailableSlots.Any(slot =>
            request.StartTime >= slot.From && endTime <= slot.To));

        if (!isWithinWorkingHours)
        {
            return new EmployeeAvailabilityResponse
            {
                IsAvailable = false,
                AvailableSlots = schedules
                    .SelectMany(s => s.AvailableSlots)
                    .Select(slot => new TimeSlot { From = slot.From, To = slot.To })
                    .ToList(),
                ErrorMessage =
                    $"The selected time slot from {request.StartTime:HH:mm} to {endTime:HH:mm} is outside working hours"
            };
        }


        return new EmployeeAvailabilityResponse
        {
            IsAvailable = true,
            AvailableSlots = schedules
                .SelectMany(s => s.AvailableSlots)
                .Select(slot => new TimeSlot { From = slot.From, To = slot.To })
                .ToList(),
            ErrorMessage = null
        };
    }

    public async UnaryResult<EmployeeScheduleResponse> GetEmployeeSchedule(EmployeeScheduleRequest request)
    {
        var schedules = await _dbContext.AvailabilitySchedules
            .Where(s => s.EmployeeId == request.EmployeeId)
            .ToListAsync();

        var workingHours = schedules
            .SelectMany(s => s.AvailableSlots)
            .Where(slot => slot.From.Date == request.Date.Date || slot.To.Date == request.Date.Date)
            .Select(slot => new TimeSlot { From = slot.From, To = slot.To })
            .ToList();

        return new EmployeeScheduleResponse
        {
            EmployeeId = request.EmployeeId,
            Date = request.Date,
            WorkingHours = workingHours,
            BookedSlots = new List<TimeSlot>()
        };
    }

    public async UnaryResult<UserResponse> GetUserById(UserByIdRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user == null)
        {
            return new UserResponse
            {
                Id = request.UserId,
                EmailAddress = "Unknown",
                Roles = "Unknown",
                IsValid = false,
                ErrorMessage = "User not found"
            };
        }

        return new UserResponse
        {
            Id = user.Id,
            EmailAddress = user.EmailAddress,
            Roles = user.Roles.ToString(),
            EmployeeId = user.EmployeeId,
            CustomerId = user.CustomerId,
            IsValid = true,
            ErrorMessage = null
        };
    }

    public async UnaryResult<UserResponse> GetUserByCustomerId(GetUserByCustomerIdRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.CustomerId == request.CustomerId);

        if (user == null)
        {
            return new UserResponse
            {
                Id = 0,
                EmailAddress = "Unknown",
                Roles = "Unknown",
                IsValid = false,
                ErrorMessage = "User not found for this customer"
            };
        }

        return new UserResponse
        {
            Id = user.Id,
            EmailAddress = user.EmailAddress,
            Roles = user.Roles.ToString(),
            EmployeeId = user.EmployeeId,
            CustomerId = user.CustomerId,
            IsValid = true,
            ErrorMessage = null
        };
    }

    public async UnaryResult<UserResponse> GetUserByEmployeeId(GetUserByEmployeeIdRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.EmployeeId == request.EmployeeId);

        if (user == null)
        {
            return new UserResponse
            {
                Id = 0,
                EmailAddress = "Unknown",
                Roles = "Unknown",
                IsValid = false,
                ErrorMessage = "User not found for this employee"
            };
        }

        return new UserResponse
        {
            Id = user.Id,
            EmailAddress = user.EmailAddress,
            Roles = user.Roles.ToString(),
            EmployeeId = user.EmployeeId,
            CustomerId = user.CustomerId,
            IsValid = true,
            ErrorMessage = null
        };
    }

    public async UnaryResult<UserEmailResponse> GetUserEmailByCustomerId(GetUserEmailByCustomerIdRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.CustomerId == request.CustomerId);

        if (user == null)
        {
            return new UserEmailResponse
            {
                UserId = 0,
                EmailAddress = "Unknown",
                IsValid = false,
                ErrorMessage = "User not found for this customer"
            };
        }

        return new UserEmailResponse
        {
            UserId = user.Id,
            EmailAddress = user.EmailAddress,
            CustomerId = user.CustomerId,
            EmployeeId = user.EmployeeId,
            IsValid = true,
            ErrorMessage = null
        };
    }

    public async UnaryResult<BlockedCustomerResponse> GetBlockedCustomerById(BlockedCustomerByIdRequest request)
    {
        var blockedCustomer = await _dbContext.BlockedCustomers
            .FirstOrDefaultAsync(b => b.Id == request.BlockedCustomerId);

        if (blockedCustomer == null)
        {
            return new BlockedCustomerResponse
            {
                Id = request.BlockedCustomerId,
                IsValid = false,
                ErrorMessage = "Blocked customer not found"
            };
        }

        return new BlockedCustomerResponse
        {
            Id = blockedCustomer.Id,
            CustomerId = blockedCustomer.CustomerId,
            CompanyId = blockedCustomer.CompanyId,
            Reason = blockedCustomer.Reason,
            BannedUntil = blockedCustomer.BannedUntil,
            DoesBanForever = blockedCustomer.DoesBanForever,
            CreatedAt = blockedCustomer.CreatedAt,
            IsValid = true,
            ErrorMessage = null
        };
    }

  
    public async UnaryResult<BlockedCustomerValidationResponse> IsCustomerBlockedForCompany(
        IsCustomerBlockedRequest request)
    {
        var blockedCustomer = await _dbContext.BlockedCustomers
            .FirstOrDefaultAsync(b => b.CustomerId == request.CustomerId && b.CompanyId == request.CompanyId);

        if (blockedCustomer == null)
        {
            return new BlockedCustomerValidationResponse
            {
                IsBlocked = false
            };
        }

        if (!blockedCustomer.DoesBanForever && blockedCustomer.BannedUntil < DateTime.UtcNow)
        {
            return new BlockedCustomerValidationResponse
            {
                IsBlocked = false
            };
        }

        return new BlockedCustomerValidationResponse
        {
            IsBlocked = true,
            IsBlockedForever = blockedCustomer.DoesBanForever,
            BannedUntil = blockedCustomer.BannedUntil,
            BlockReason = blockedCustomer.Reason
        };
    }

    public async UnaryResult<BlockCustomerResponse> BlockCustomer(BlockCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var existingBlock = await _dbContext.BlockedCustomers
            .FirstOrDefaultAsync(b => b.CustomerId == request.CustomerId && b.CompanyId == request.CompanyId);

        if (existingBlock != null)
        {
            return new BlockCustomerResponse
            {
                Success = false,
                ErrorMessage = "Customer is already blocked for this company"
            };
        }

        var blockedCustomer = new BlockedCustomerEntity
        {
            CustomerId = request.CustomerId,
            CompanyId = request.CompanyId,
            Reason = request.Reason,
            BannedUntil = request.BannedUntil,
            DoesBanForever = request.DoesBanForever,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.BlockedCustomers.AddAsync(blockedCustomer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new BlockCustomerResponse
        {
            Success = true,
            BlockedCustomerId = blockedCustomer.Id,
            ErrorMessage = null
        };
    }

    public async UnaryResult<UnblockCustomerResponse> UnblockCustomer(UnblockCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var blockedCustomer = await _dbContext.BlockedCustomers
            .FirstOrDefaultAsync(b => b.Id == request.BlockedCustomerId);

        if (blockedCustomer == null)
        {
            return new UnblockCustomerResponse
            {
                Success = false,
                ErrorMessage = "Blocked customer not found"
            };
        }

        _dbContext.BlockedCustomers.Remove(blockedCustomer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UnblockCustomerResponse
        {
            Success = true,
            ErrorMessage = null
        };
    }

    public async UnaryResult<CurrentUserResponse> GetCurrentUserInfo(CurrentUserRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user == null)
        {
            return new CurrentUserResponse
            {
                UserId = request.UserId,
                EmailAddress = "Unknown",
                Role = "Unknown",
                IsValid = false,
                ErrorMessage = "User not found"
            };
        }

        return new CurrentUserResponse
        {
            UserId = user.Id,
            EmailAddress = user.EmailAddress,
            Role = user.Roles.ToString(),
            EmployeeId = user.EmployeeId,
            CustomerId = user.CustomerId,
            IsValid = true,
            ErrorMessage = null
        };
    }

    public async UnaryResult<CurrentEmployeeResponse> GetCurrentEmployee(CurrentUserRequest request)
    {
        var user = await _dbContext.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user == null)
        {
            return new CurrentEmployeeResponse
            {
                EmployeeId = 0,
                IsValid = false,
                ErrorMessage = "User not found"
            };
        }

        if (user.EmployeeId == null || user.Employee == null)
        {
            return new CurrentEmployeeResponse
            {
                EmployeeId = 0,
                IsValid = false,
                ErrorMessage = "User is not an employee"
            };
        }

        var employee = user.Employee;

        return new CurrentEmployeeResponse
        {
            EmployeeId = employee.Id,
            CompanyId = employee.CompanyId,
            BranchId = employee.BranchId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            PhoneNumber = employee.PhoneNumber,
            IsValid = true,
            ErrorMessage = null
        };
    }

    public async UnaryResult<CurrentCustomerResponse> GetCurrentCustomer(CurrentUserRequest request)
    {
        var user = await _dbContext.Users
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user == null)
        {
            return new CurrentCustomerResponse
            {
                CustomerId = 0,
                IsValid = false,
                ErrorMessage = "User not found"
            };
        }

        if (user.CustomerId == null || user.Customer == null)
        {
            return new CurrentCustomerResponse
            {
                CustomerId = 0,
                IsValid = false,
                ErrorMessage = "User is not a customer"
            };
        }

        var customer = user.Customer;

        return new CurrentCustomerResponse
        {
            CustomerId = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            IsValid = true,
            ErrorMessage = null
        };
    }

    public async UnaryResult<IsEmployeeResponse> IsCurrentUserEmployee(CurrentUserRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user == null)
        {
            return new IsEmployeeResponse
            {
                IsEmployee = false
            };
        }

        return new IsEmployeeResponse
        {
            IsEmployee = user.EmployeeId.HasValue
        };
    }
}