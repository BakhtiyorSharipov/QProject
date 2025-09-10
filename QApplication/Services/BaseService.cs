using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public abstract class BaseService<TEntity, TResponseModel, TRequestModel>: IBaseService<TEntity, TResponseModel, TRequestModel>
where TEntity: BaseEntity
where TResponseModel: BaseResponse
where TRequestModel: BaseRequest
{
    private readonly IRepository<TEntity> _repository;

    public BaseService(IRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public IEnumerable<TResponseModel> GetAll(int pageList, int pageNumber)
    {
        throw new NotImplementedException();
    }

    public TResponseModel GetById(int id)
    {
        throw new NotImplementedException();
    }

    public TResponseModel Add(TRequestModel request)
    {
        throw new NotImplementedException();
    }

    public TResponseModel Update(int id, TRequestModel request)
    {
        throw new NotImplementedException();
    }

    public bool Delete(int id)
    {
        throw new NotImplementedException();
    }
}