using QApplication.Requests.QueueRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IQueueService: IBaseService<QueueEntity, QueueResponseModel, QueueRequestModel>
{
    
}