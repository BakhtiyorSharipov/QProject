using QApplication.Requests.AvailabilityScheduleRequest;
using QApplication.Responses;

namespace QApplication.Interfaces;

public interface IAvailabilityScheduleService
{
    Task<IEnumerable<AvailabilityScheduleResponseModel>> GetAllAsync(int pageList, int pageNumber);
    Task<AvailabilityScheduleResponseModel> GetByIdAsync(int id);
    Task<IEnumerable<AvailabilityScheduleResponseModel>> AddAsync(AvailabilityScheduleRequestModel request);
    Task<AvailabilityScheduleResponseModel> UpdateAsync(int id, UpdateAvailabilityScheduleRequest request, bool updateAllSchedules);
    Task<bool> DeleteAsync(int id, bool deleteAllSlots);
}