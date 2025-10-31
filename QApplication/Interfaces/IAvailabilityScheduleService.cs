using QApplication.Requests.AvailabilityScheduleRequest;
using QApplication.Responses;

namespace QApplication.Interfaces;

public interface IAvailabilityScheduleService
{
    IEnumerable<AvailabilityScheduleResponseModel> GetAll(int pageList, int pageNumber);
    AvailabilityScheduleResponseModel GetById(int id);
    IEnumerable<AvailabilityScheduleResponseModel> Add(AvailabilityScheduleRequestModel request);
    AvailabilityScheduleResponseModel Update(int id, UpdateAvailabilityScheduleRequest request);
    bool Delete(int id, bool deleteAllSlots);
}