using System.Net;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.BlockedCustomerRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public class BlockedCustomerService:  IBlockedCustomerService
{
    private readonly IBlockedCustomerRepository _repository;

    public BlockedCustomerService(IBlockedCustomerRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<BlockedCustomerResponseModel> GetAll(int pageList, int pageNumber)
    {
        var dbBlockedCustomer = _repository.GetAll(pageList, pageNumber);
        if (dbBlockedCustomer==null)
        {
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

        return response;
    }


    public BlockedCustomerResponseModel GetById(int id)
    {
        var dbBlockedCustomer = _repository.FindById(id);
        if (dbBlockedCustomer==null)
        {
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

        return response;
    }

    public BlockedCustomerResponseModel Add(BlockedCustomerRequestModel request)
    {
        var requestToCreate = request as CreateBlockedCustomerRequest;
        if (requestToCreate==null)
        {
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
        
    
        _repository.Add(blockedCustomer);
        _repository.SaveChanges();

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


    public BlockedCustomerResponseModel Update(int id, BlockedCustomerRequestModel request)
    {
        var dbBlockedCustomer = _repository.FindById(id);
        if (dbBlockedCustomer== null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BlockedCustomerEntity));
        }

        var requestToUpdate = request as UpdateBlockedCustomerRequest;
        if (requestToUpdate==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(BlockedCustomerEntity));
        }

        dbBlockedCustomer.CompanyId = requestToUpdate.CompanyId;
        dbBlockedCustomer.CustomerId = requestToUpdate.CustomerId;
        dbBlockedCustomer.BannedUntil = requestToUpdate.BannedUntil;
        dbBlockedCustomer.DoesBanForever = requestToUpdate.DoesBanForever;
        dbBlockedCustomer.Reason = requestToUpdate.Reason;
        
        _repository.Update(dbBlockedCustomer);
        _repository.SaveChanges();

        var response = new BlockedCustomerResponseModel()
        {
            Id=dbBlockedCustomer.Id,
            CompanyId = dbBlockedCustomer.CompanyId,
            CustomerId = dbBlockedCustomer.CustomerId,
            BannedUntil = dbBlockedCustomer.BannedUntil,
            DoesBanForever = dbBlockedCustomer.DoesBanForever,
            Reason = dbBlockedCustomer.Reason
        };

        return response;
    }

    public bool Delete(int id)
    {
        var dbBlockedCustomer = _repository.FindById(id);
        if (dbBlockedCustomer== null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BlockedCustomerEntity));
        }
        
        _repository.Delete(dbBlockedCustomer);
        _repository.SaveChanges();

        return true;
    }
}