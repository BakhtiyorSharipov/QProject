using QApplication.Requests.QueueRequest;
using QApplication.Responses;


namespace QApplication.Interfaces;

public interface IQueueService
{
    Task<IEnumerable<QueueResponseModel>> GetAllAsync(int pageList, int pageNumber);
    Task<QueueResponseModel> GetByIdAsync(int id);
    Task<AddQueueResponseModel> AddAsync(QueueRequestModel request);
    Task<bool> DeleteAsync(int id);
    Task<QueueResponseModel> CancelQueueByCustomerAsync(QueueCancelRequest request);
    Task<QueueResponseModel> CancelQueueByEmployeeAsync(QueueCancelRequest request);

    Task<UpdateQueueStatusResponseModel> UpdateQueueStatusAsync(UpdateQueueRequest request);

    Task<IEnumerable<QueueResponseModel>> GetQueuesByCustomerAsync(int customerId);
    Task<IEnumerable<QueueResponseModel>> GetQueuesByEmployeeAsync(int employeeId);

    Task<IEnumerable<QueueResponseModel>> GetQueuesByServiceAsync(int serviceId);
    Task<IEnumerable<QueueResponseModel>> GetQueuesByCompanyAsync(int companyId);
}