using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Data;
using QDomain.Models;

namespace QApplication.UseCases.Reports.ReportQueryExtensions;

public static class ReportQueryExtensions
{
    public static async Task<List<QueueEntity>> GetQueuesByCompanyIdAsync(this IQueueApplicationDbContext dbContext,
        int companyId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Queues
            .Where(s => s.Service.CompanyId == companyId)
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<EmployeeEntity>> GetEmployeesByCompanyIdAsync(
        this IQueueApplicationDbContext dbContext,
        int companyId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Employees
            .Where(s => s.Service.CompanyId == companyId)
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<ServiceEntity>> GetServicesByCompanyIdAsync(this IQueueApplicationDbContext dbContext,
        int companyId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Services
            .Where(s => s.CompanyId == companyId)
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<CustomerEntity>> GetCustomersByCompanyIdAsync(
        this IQueueApplicationDbContext dbContext,
        int companyId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers
            .Where(s => s.Queues
                .Any(s => s.Service.CompanyId == companyId))
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<ReviewEntity>> GetReviewsByCompanyIdAsync(this IQueueApplicationDbContext dbContext,
        int companyId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .Where(s => s.Queue.Service.CompanyId == companyId)
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<ComplaintEntity>> GetComplaintsByCompanyIdAsync(
        this IQueueApplicationDbContext dbContext,
        int companyId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Complaints
            .Where(s => s.Queue.Service.CompanyId == companyId)
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<BlockedCustomerEntity>> GetBlockedCustomersByCompanyIdASync(
        this IQueueApplicationDbContext dbContext,
        int companyId, CancellationToken cancellationToken = default)
    {
        return await dbContext.BlockedCustomers
            .Where(s => s.CompanyId == companyId)
            .ToListAsync(cancellationToken);
    }

    
    public static async Task<List<CompanyEntity>> GetAllCompaniesAsync(this IQueueApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Companies
            .ToListAsync(cancellationToken);
    }
    
    public static async Task<List<EmployeeEntity>> GetAllEmployeesAsync(this IQueueApplicationDbContext dbContext,
        CancellationToken cancellationToken= default)
    {
        return await dbContext.Employees
            .Include(s => s.Service)
                .ThenInclude(s => s.Company)
            .Include(s=>s.Queues)
            .Include(s=>s.AvailabilitySchedules)
            .Include(s=>s.Service.Employees)
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<CustomerEntity>> GetAllCustomersAsync(this IQueueApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<BlockedCustomerEntity>> GetAllBlockedCustomersAsync(
        this IQueueApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        return await dbContext.BlockedCustomers
            .Include(s => s.Company)
            .Include(s => s.Customer)
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<ServiceEntity>> GetAllServicesAsync(this IQueueApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Services
            .Include(s => s.Company)
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<QueueEntity>> GetAllQueuesAsync(this IQueueApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Queues
            .Include(s => s.Service)
            .Include(s => s.Customer)
            .Include(s => s.Employee)
            .Include(s=>s.Service.Company)
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<ComplaintEntity>> GetAllComplaintsAsync(this IQueueApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Complaints
            .Include(s => s.Customer)
            .Include(s => s.Queue)
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<ReviewEntity>> GetAllReviewsAsync(this IQueueApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Queue)
                .Include(r=>r.Queue.Service.Company)
                .Include(s=>s.Queue.Employee)
                .ThenInclude(r => r.Service)
                .ToListAsync(cancellationToken);
    }
}