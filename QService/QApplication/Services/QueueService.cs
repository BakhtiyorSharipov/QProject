using System.Net;
using MagicOnion;
using MagicOnion.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QContracts.Enums;
using QContracts.Interfaces;
using QContracts.Responses;

namespace QApplication.Services;

public class QueueService : ServiceBase<IQueueService>, IQueueService
{
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ILogger<QueueService> _logger;

    public QueueService(IQueueApplicationDbContext dbContext, ILogger<QueueService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }


    public async UnaryResult<QueueInfo> GetQueueByIdAsync(int queueId)
    {
        _logger.LogInformation("Getting queue with Id :{queueId}", queueId);

        var queue = await _dbContext.Queues
            .AsNoTracking()
            .Include(s => s.Customer)
            .Include(s => s.Employee)
            .FirstOrDefaultAsync(s => s.Id == queueId);

        if (queue == null)
        {
            _logger.LogWarning("Queue with Id: {queueId} not found", queueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"Queue with Id {queueId} not found");
        }

        var response = new QueueInfo
        {
            Id = queue.Id,
            CompanyId = queue.CompanyId,
            BranchId = queue.BranchId,
            ServiceId = queue.ServiceId,
            CustomerId = queue.CustomerId,
            EmployeeName = queue.Employee.FirstName,
            CustomerName = queue.Customer.FirstName,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime,
            CurrentQueueStatus = (CurrentQueueStatus)queue.Status,
            CancelReason = queue.CancelReason,
            CreatedAt = queue.CreatedAt
        };

        return response;
    }

    public async UnaryResult<List<QueueInfo>> GetCustomerQueuesAsync(int customerId)
    {
        _logger.LogInformation("Getting queues with customer Id: {customerId}", customerId);

        var customer = await _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == customerId);

        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {customerId} not found", customerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"Customer with Id {customerId} not found");
        }


        var customerQueues = await _dbContext.Queues
            .AsNoTracking()
            .Include(s => s.Customer)
            .Include(s => s.Employee)
            .Where(s => s.CustomerId == customerId)
            .ToListAsync();

        if (!customerQueues.Any())
        {
            _logger.LogWarning("Not found queues for this customer");
            return [];
        }

        var response = customerQueues.Select(queue => new QueueInfo
        {
            Id = queue.Id,
            CompanyId = queue.CompanyId,
            BranchId = queue.BranchId,
            ServiceId = queue.ServiceId,
            CustomerId = queue.CustomerId,
            EmployeeName = queue.Employee.FirstName,
            CustomerName = queue.Customer.FirstName,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime,
            CurrentQueueStatus = (CurrentQueueStatus)queue.Status,
            CancelReason = queue.CancelReason,
            CreatedAt = queue.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<List<QueueInfo>> GetEmployeeQueuesAsync(int employeeId)
    {
        _logger.LogInformation("Getting queues with employee Id: {employeeId}", employeeId);

        var employee = await _dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == employeeId);

        if (employee == null)
        {
            _logger.LogWarning("Employee with Id {employeeId} not found", employeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"Employee with Id {employeeId} not found");
        }


        var employeeQueues = await _dbContext.Queues
            .AsNoTracking()
            .Include(s => s.Customer)
            .Include(s => s.Employee)
            .Where(s => s.EmployeeId == employeeId)
            .ToListAsync();

        if (!employeeQueues.Any())
        {
            _logger.LogWarning("Not found queues for this employee");
            return [];
        }
        

        var response = employeeQueues.Select(queue => new QueueInfo
        {
            Id = queue.Id,
            CompanyId = queue.CompanyId,
            BranchId = queue.BranchId,
            ServiceId = queue.ServiceId,
            CustomerId = queue.CustomerId,
            EmployeeName = queue.Employee.FirstName,
            CustomerName = queue.Customer.FirstName,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime,
            CurrentQueueStatus = (CurrentQueueStatus)queue.Status,
            CancelReason = queue.CancelReason,
            CreatedAt = queue.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<List<QueueInfo>> GetBranchQueuesAsync(int branchId)
    {
        _logger.LogInformation("Getting queues with branch Id: {branchId}", branchId);

        var branchQueues = await _dbContext.Queues
            .AsNoTracking()
            .Where(s => s.BranchId == branchId)
            .ToListAsync();


        if (!branchQueues.Any())
        {
            _logger.LogWarning("Not found queues for this branch");
            return [];
        }
        

        var response = branchQueues.Select(queue => new QueueInfo
        {
            Id = queue.Id,
            CompanyId = queue.CompanyId,
            BranchId = queue.BranchId,
            ServiceId = queue.ServiceId,
            CustomerId = queue.CustomerId,
            EmployeeName = queue.Employee.FirstName,
            CustomerName = queue.Customer.FirstName,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime,
            CurrentQueueStatus = (CurrentQueueStatus)queue.Status,
            CancelReason = queue.CancelReason,
            CreatedAt = queue.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<List<QueueInfo>> GetCompanyQueuesAsync(int companyId)
    {
        _logger.LogInformation("Getting queues with company Id: {companyId}", companyId);

        var companyQueues = await _dbContext.Queues
            .AsNoTracking()
            .Include(s=>s.Employee)
            .Include(s=>s.Customer)
            .Where(s => s.CompanyId == companyId)
            .ToListAsync();


        if (!companyQueues.Any())
        {
            _logger.LogWarning("Not found queues for this company");
            return [];
        }
        

        var response = companyQueues.Select(queue => new QueueInfo
        {
            Id = queue.Id,
            CompanyId = queue.CompanyId,
            BranchId = queue.BranchId,
            ServiceId = queue.ServiceId,
            CustomerId = queue.CustomerId,
            EmployeeName = queue.Employee.FirstName,
            CustomerName = queue.Customer.FirstName,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime,
            CurrentQueueStatus = (CurrentQueueStatus)queue.Status,
            CancelReason = queue.CancelReason,
            CreatedAt = queue.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<List<QueueInfo>> GetServiceQueuesAsync(int serviceId)
    {
        _logger.LogInformation("Getting queues with service Id: {serviceId}", serviceId);

        var serviceQueues = await _dbContext.Queues
            .AsNoTracking()
            .Where(s => s.ServiceId == serviceId)
            .ToListAsync();


        if (!serviceQueues.Any())
        {
            _logger.LogWarning("Not found queues for this service");
            return [];
        }
        

        var response = serviceQueues.Select(queue => new QueueInfo
        {
            Id = queue.Id,
            CompanyId = queue.CompanyId,
            BranchId = queue.BranchId,
            ServiceId = queue.ServiceId,
            CustomerId = queue.CustomerId,
            EmployeeName = queue.Employee.FirstName,
            CustomerName = queue.Customer.FirstName,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime,
            CurrentQueueStatus = (CurrentQueueStatus)queue.Status,
            CancelReason = queue.CancelReason,
            CreatedAt = queue.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<ReviewInfo> GetQueueReviewAsync(int queueId)
    {
        _logger.LogInformation("Getting review for queue Id {queueId}", queueId);

        var queue = await _dbContext.Queues
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == queueId);

        if (queue == null)
        {
            _logger.LogWarning("Queue with Id {queueId} not found", queueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"Queue with Id {queueId} not found");
        }

        var queueReview = await _dbContext.Reviews
            .FirstOrDefaultAsync(s => s.QueueId == queueId);

        if (queueReview == null)
        {
            _logger.LogInformation("Not found review for this queue");
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Not found review for this request");
        }

        var response = new ReviewInfo
        {
            Id = queueReview.Id,
            QueueId = queueReview.QueueId,
            CustomerId = queueReview.CustomerId,
            Grade = queueReview.Grade,
            ReviewText = queueReview.ReviewText,
            CreatedAt = queueReview.CreatedAt
        };

        return response;
    }

    public async UnaryResult<List<ReviewInfo>> GetCustomerReviewsAsync(int customerId)
    {
        _logger.LogInformation("Getting reviews for customer Id: {customerId}", customerId);

        var customer = await _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == customerId);

        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {customerId} not found", customerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"Customer with Id {customerId} not found");
        }

        var customerReviews = await _dbContext.Reviews
            .AsNoTracking()
            .Include(s => s.Queue)
            .Where(s => s.CustomerId == customerId)
            .ToListAsync();

        if (!customerReviews.Any())
        {
            _logger.LogWarning("Not found any review for this customer");
            return [];
        }

        var response = customerReviews.Select(review => new ReviewInfo()
        {
            Id = review.Id,
            QueueId = review.QueueId,
            CustomerId = review.CustomerId,
            Grade = review.Grade,
            ReviewText = review.ReviewText,
            CreatedAt = review.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<List<ReviewInfo>> GetCompanyReviewsAsync(int companyId)
    {
        _logger.LogInformation("Getting reviews for company Id: {companyId}", companyId);

        var companyReviews = await _dbContext.Reviews
            .AsNoTracking()
            .Include(s => s.Queue)
            .Where(s => s.Queue.CompanyId== companyId)
            .ToListAsync();

        if (!companyReviews.Any())
        {
            _logger.LogWarning("Not found any review for this company");
            return [];
        }

        var response = companyReviews.Select(review => new ReviewInfo()
        {
            Id = review.Id,
            QueueId = review.QueueId,
            CustomerId = review.CustomerId,
            Grade = review.Grade,
            ReviewText = review.ReviewText,
            CreatedAt = review.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<ComplaintInfo> GetQueueComplaintAsync(int queueId)
    {
        _logger.LogInformation("Getting complaint for queue Id {queueId}", queueId);

        var queue = await _dbContext.Queues
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == queueId);

        if (queue == null)
        {
            _logger.LogWarning("Queue with Id {queueId} not found", queueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"Queue with Id {queueId} not found");
        }

        var queueComplaint = await _dbContext.Complaints
            .FirstOrDefaultAsync(s => s.QueueId == queueId);

        if (queueComplaint == null)
        {
            _logger.LogInformation("Not found any complaint for this queue");
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Not found any complaint for this queue");
        }

        var response = new ComplaintInfo()
        {
            Id = queueComplaint.Id,
            QueueId = queueComplaint.QueueId,
            CustomerId = queueComplaint.CustomerId,
            ComplaintText = queueComplaint.ComplaintText,
            ResponseText = queueComplaint.ResponseText,
            Status = (CurrentComplaintStatus)queueComplaint.ComplaintStatus,
            CreatedAt = queueComplaint.CreatdAt
        };

        return response;
    }

    public async UnaryResult<List<ComplaintInfo>> GetCustomerComplaintsAsync(int customerId)
    {
        _logger.LogInformation("Getting complaints for customer Id: {customerId}", customerId);

        var customer = await _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == customerId);

        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {customerId} not found", customerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"Customer with Id {customerId} not found");
        }

        var customerComplaints = await _dbContext.Complaints
            .AsNoTracking()
            .Include(s => s.Queue)
            .Where(s => s.CustomerId == customerId)
            .ToListAsync();

        if (!customerComplaints.Any())
        {
            _logger.LogWarning("Not found any customer complaints");
            return [];
        }

        var response = customerComplaints.Select(complaint => new ComplaintInfo()
        {
            Id = complaint.Id,
            QueueId = complaint.QueueId,
            CustomerId = complaint.CustomerId,
            ComplaintText = complaint.ComplaintText,
            ResponseText = complaint.ResponseText,
            Status = (CurrentComplaintStatus)complaint.ComplaintStatus,
            CreatedAt = complaint.CreatdAt
        }).ToList();

        return response;
    }

    public async UnaryResult<List<ComplaintInfo>> GetCompanyComplaintsAsync(int companyId)
    {
        _logger.LogInformation("Getting complaints for company Id: {companyId}", companyId);

        var companyComplaints = await _dbContext.Complaints
            .AsNoTracking()
            .Include(s => s.Queue)
            .Where(s => s.Queue.CompanyId== companyId)
            .ToListAsync();

        if (!companyComplaints.Any())
        {
            _logger.LogWarning("Not found any company complaints");
            return [];
        }

        var response = companyComplaints.Select(complaint => new ComplaintInfo()
        {
            Id = complaint.Id,
            QueueId = complaint.QueueId,
            CustomerId = complaint.CustomerId,
            ComplaintText = complaint.ComplaintText,
            ResponseText = complaint.ResponseText,
            Status = (CurrentComplaintStatus)complaint.ComplaintStatus,
            CreatedAt = complaint.CreatdAt
        }).ToList();

        return response;
    }

    public async UnaryResult<List<CustomerInfo>> GetAllCompanyCustomers(int companyId)
    {
        var customers = await _dbContext.Queues
            .Include(s=>s.Customer)
            .Where(s=>s.CompanyId==companyId)
            .ToListAsync();
     
        if (!customers.Any())
        {
            _logger.LogWarning("Not found any customer");
            return [];
        }

        var response = customers.Select(customer => new CustomerInfo
        {
            CustomerId = customer.Id,
            FirstName = customer.Customer.FirstName,
            LastName = customer.Customer.LastName,
            CreatedAt = customer.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<List<EmployeeInfo>> GetAllCompanyEmployees(int companyId)
    {
        var employees = await _dbContext.Employees
            .Where(s=>s.CompanyId== companyId)
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
            CreatedAt = employee.CreatedAt
        }).ToList();

        return response;
    }

    public async UnaryResult<List<BlockedCustomerInfo>> GetAllCompanyBlockedCustomers(int companyId)
    {
        var blockedCustomers = await _dbContext.BlockedCustomers
            .Where(s=>s.CompanyId== companyId)
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
            Reason = blockedCustomer.Reason,
            BannedUntil = blockedCustomer.BannedUntil,
            DoesBanForever = blockedCustomer.DoesBanForever,
            CreatedAt = blockedCustomer.CreatedAt
        }).ToList();

        return response;
    }
}