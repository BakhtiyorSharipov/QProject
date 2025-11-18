using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.BlockedCustomerRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public class BlockedCustomerService : IBlockedCustomerService
{
    private readonly IBlockedCustomerRepository _repository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<BlockedCustomerService> _logger;

    public BlockedCustomerService(IBlockedCustomerRepository repository, ICustomerRepository customerRepository,
        ICompanyRepository companyRepository, ILogger<BlockedCustomerService> logger)
    {
        _repository = repository;
        _customerRepository = customerRepository;
        _companyRepository = companyRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<BlockedCustomerResponseModel>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Getting blocked customers. PageNumber {pageNumber}, PageList {pageList}", pageNumber,
            pageList);
        var dbBlockedCustomer = await _repository.GetAll(pageList, pageNumber).ToListAsync();
        var response = dbBlockedCustomer.Select(blocked => new BlockedCustomerResponseModel()
        {
            Id = blocked.Id,
            CompanyId = blocked.CompanyId,
            CustomerId = blocked.CustomerId,
            BannedUntil = blocked.BannedUntil,
            DoesBanForever = blocked.DoesBanForever,
            Reason = blocked.Reason
        }).ToList();

        _logger.LogInformation("Fetched {response.Count} blocked customers.", response.Count);
        return response;
    }

    public async Task<IEnumerable<BlockedCustomerResponseModel>> GetAllBlockedCustomersByCompanyAsync(int companyId)
    {
        _logger.LogInformation("Getting all blocked customers by company Id {companyId}", companyId);
        var dbBlockedCustomer = await _repository.GetAllBlockedCustomersByCompany(companyId).ToListAsync();
        if (!dbBlockedCustomer.Any())
        {
            _logger.LogWarning("No blocked customer found for this company Id {companyId}.", companyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BlockedCustomerEntity));
        }

        var response = dbBlockedCustomer.Select(blocked => new BlockedCustomerResponseModel()
        {
            Id = blocked.Id,
            CompanyId = blocked.CompanyId,
            CustomerId = blocked.CustomerId,
            BannedUntil = blocked.BannedUntil,
            DoesBanForever = blocked.DoesBanForever,
            Reason = blocked.Reason
        }).ToList();

        _logger.LogInformation("Fetched {response.Count} blocked customers for this company Id {companyId}.",
            response.Count, companyId);
        return response;
    }


    public async Task<BlockedCustomerResponseModel> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting blocked customer by Id {id}", id);
        var dbBlockedCustomer = await _repository.FindByIdAsync(id);
        if (dbBlockedCustomer == null)
        {
            _logger.LogInformation("Blocked customer with Id {id} not found.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BlockedCustomerEntity));
        }

        var response = new BlockedCustomerResponseModel()
        {
            Id = dbBlockedCustomer.Id,
            CompanyId = dbBlockedCustomer.CompanyId,
            CustomerId = dbBlockedCustomer.CustomerId,
            BannedUntil = dbBlockedCustomer.BannedUntil,
            DoesBanForever = dbBlockedCustomer.DoesBanForever,
            Reason = dbBlockedCustomer.Reason
        };

        _logger.LogInformation("Blocked customer with Id {id} fetched successfully.", id);
        return response;
    }

    public async Task<BlockedCustomerResponseModel> BlockAsync(BlockedCustomerRequestModel request)
    {
        _logger.LogInformation("Blocking customer with Id {request.CustomerId}.", request.CustomerId);

        var customer = await _customerRepository.FindByIdAsync(request.CustomerId);
        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {request.CustomerId} not found.", request.CustomerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var company = await _companyRepository.FindByIdAsync(request.CompanyId);
        if (company == null)
        {
            _logger.LogWarning("Company with Id {request.CompanyId} not found.", request.CompanyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }

        var requestToCreate = request as CreateBlockedCustomerRequest;
        if (requestToCreate == null)
        {
            _logger.LogError("Invalid request model while blocking customer. ");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(BlockedCustomerEntity));
        }

        var blockedCustomer = new BlockedCustomerEntity()
        {
            CompanyId = requestToCreate.CompanyId,
            CustomerId = requestToCreate.CustomerId,
            BannedUntil = requestToCreate.BannedUntil,
            DoesBanForever = requestToCreate.DoesBanForever,
            Reason = requestToCreate.Reason
        };


        await _repository.AddAsync(blockedCustomer);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Customer with Id {request.CustomerId} blocked successfully.", request.CustomerId);

        var response = new BlockedCustomerResponseModel()
        {
            Id = blockedCustomer.Id,
            CompanyId = blockedCustomer.CompanyId,
            CustomerId = blockedCustomer.CustomerId,
            BannedUntil = blockedCustomer.BannedUntil,
            DoesBanForever = blockedCustomer.DoesBanForever,
            Reason = blockedCustomer.Reason
        };

        return response;
    }


    public async Task<bool> UnblockAsync(int id)
    {
        _logger.LogInformation("Unblocking customer with Id {id}.", id);
        var dbBlockedCustomer = await _repository.FindByIdAsync(id);
        if (dbBlockedCustomer == null)
        {
            _logger.LogWarning("Blocked customer with Id {id} not found.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BlockedCustomerEntity));
        }

        _repository.Delete(dbBlockedCustomer);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Blocked customer with Id {id} unblocked successfully.", id);

        return true;
    }
}