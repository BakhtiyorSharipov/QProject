using QApplication.Requests.QueueRequest;
using QApplication.Responses;


namespace QApplication.Interfaces;

public interface IQueueService
{
    IEnumerable<QueueResponseModel> GetAll(int pageList, int pageNumber);

    QueueResponseModel GetById(int id);//
    AddQueueResponseModel Add(QueueRequestModel request);
    bool Delete(int id);//
    QueueResponseModel CancelQueueByCustomer(QueueCancelRequest request);

    QueueResponseModel CancelQueueByEmployee(QueueCancelRequest request);
    
    UpdateQueueStatusResponseModel UpdateQueueStatus(UpdateQueueRequest request);

    IEnumerable<QueueResponseModel> GetQueuesByCustomer(int customerId);
    IEnumerable<QueueResponseModel> GetQueuesByEmployee(int employeeId);

    IEnumerable<QueueResponseModel> GetQueuesByService(int serviceId);
    IEnumerable<QueueResponseModel> GetQueuesByCompany(int companyId);

}