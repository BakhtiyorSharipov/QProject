using System.Net;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.CompanyRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public class CompanyService: BaseService<CompanyEntity, CompanyResponseModel, CompanyRequestModel>, ICompanyService
{
    private readonly ICompanyRepository _repository;
    
    public CompanyService(ICompanyRepository repository) : base(repository)
    {
        _repository = repository;
    }

    public IEnumerable<CompanyResponseModel> GetAll(int pageList, int pageNumber)
    {
        var dbCompany = _repository.GetAll(pageList, pageNumber);

        var response = dbCompany.Select(company => new CompanyResponseModel()
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            Address = company.Address,
            EmailAddress = company.EmailAddress,
            PhoneNumber = company.PhoneNumber
        }).ToList();

        return response;
    }

    public CompanyResponseModel GetById(int id)
    {
        var dbCompany = _repository.FindById(id);
        if (dbCompany==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }

        var response = new CompanyResponseModel()
        {
            Id = dbCompany.Id,
            CompanyName = dbCompany.CompanyName,
            Address = dbCompany.Address,
            EmailAddress = dbCompany.EmailAddress,
            PhoneNumber = dbCompany.PhoneNumber
        };

        return response;
    }

    public CompanyResponseModel Add(CompanyRequestModel request)
    {
        var parsedToCreate = request as CreateCompanyRequest;
        if (parsedToCreate==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(CompanyEntity));
        }

        var company = new CompanyEntity()
        {
            CompanyName = parsedToCreate.CompanyName,
            Address = parsedToCreate.Address,
            EmailAddress = parsedToCreate.EmailAddress,
            PhoneNumber = parsedToCreate.PhoneNumber
        };
        
        _repository.Add(company);
        _repository.SaveChanges();

        var response = new CompanyResponseModel()
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            Address = company.Address,
            EmailAddress = company.EmailAddress,
            PhoneNumber = company.PhoneNumber
        };

        return response;
    }

    public CompanyResponseModel Update(int id, CompanyRequestModel request)
    {
        var dbCompany = _repository.FindById(id);
        if (dbCompany==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }

        var requestToUpdate = request as UpdateCompanyRequest;
        if (requestToUpdate==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(CompanyEntity));
        }


        dbCompany.CompanyName = requestToUpdate.CompanyName;
        dbCompany.Address = requestToUpdate.Address;
        dbCompany.EmailAddress = requestToUpdate.EmailAddress;
        dbCompany.PhoneNumber = requestToUpdate.PhoneNumber;
        
        
        _repository.Update(dbCompany);
        _repository.SaveChanges();

        var response = new CompanyResponseModel()
        {
            Id = dbCompany.Id,
            CompanyName = dbCompany.CompanyName,
            Address = dbCompany.Address,
            EmailAddress = dbCompany.EmailAddress,
            PhoneNumber = dbCompany.PhoneNumber
        };

        return response;

    }


    public bool Delete(int id)
    {
        var dbCompany = _repository.FindById(id);
        if (dbCompany==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }
        
        _repository.Delete(dbCompany);
        _repository.SaveChanges();

        return true;
    }
}