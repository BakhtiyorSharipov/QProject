using QApplication.Requests.AvailabilityScheduleRequest;
using QApplication.Responses;

namespace QApplication.Interfaces;

public interface IAvailabilityScheduleService
{
    IEnumerable<AvailabilityScheduleResponseModel> GetAll(int pageList, int pageNumber);
    AvailabilityScheduleResponseModel GetById(int id);
    AvailabilityScheduleResponseModel Add(AvailabilityScheduleRequestModel request);
    AvailabilityScheduleResponseModel Update(int id, AvailabilityScheduleRequestModel request);
    bool Delete(int id);
}