using QApplication.Requests;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IBaseService<TEntity, TResponseModel, TRequestModel>
    where TEntity : BaseEntity
    where TResponseModel : BaseResponse
    where TRequestModel : BaseRequest
{
    IEnumerable<TResponseModel> GetAll(int pageList, int pageNumber);
    TResponseModel GetById(int id);
    TResponseModel Add(TRequestModel request);
    TResponseModel Update(int id, TRequestModel request);
    bool Delete(int id);
}