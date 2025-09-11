using QApplication.Requests.QueueRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IQueueService
{
    IEnumerable<QueueResponseModel> GetAll(int pageList, int pageNumber);
    QueueResponseModel GetById(int id);
    QueueResponseModel Add(QueueRequestModel request);
    QueueResponseModel Update(int id, QueueRequestModel request);
    bool Delete(int id);
}