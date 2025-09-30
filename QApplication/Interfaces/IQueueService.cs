using QApplication.Requests.QueueRequest;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IQueueService
{
    IEnumerable<QueueResponseModel> GetAll(int pageList, int pageNumber);

    QueueResponseModel GetById(int id);//
    QueueResponseModel Add(QueueRequestModel request);
    QueueResponseModel Update(int id, QueueRequestModel request);//
    bool Delete(int id);//
    QueueResponseModel CancelQueueByCustomer(QueueCancelRequest request);

    QueueResponseModel CancelQueueByEmployee(QueueCancelRequest request);
    
    QueueResponseModel UpdateQueueStatus(int id, QueueStatus status);

    IEnumerable<QueueResponseModel> GetQueuesByCustomer(int customerId);
    IEnumerable<QueueResponseModel> GetQueuesByEmployee(int employeeId);
}