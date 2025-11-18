using QApplication.Requests.BlockedCustomerRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IBlockedCustomerService
{
    Task<IEnumerable<BlockedCustomerResponseModel>> GetAllAsync(int pageList, int pageNumber);

    Task<IEnumerable<BlockedCustomerResponseModel>> GetAllBlockedCustomersByCompanyAsync(int companyId);
    Task<BlockedCustomerResponseModel> GetByIdAsync(int id);
    Task<BlockedCustomerResponseModel> BlockAsync(BlockedCustomerRequestModel request);
    Task<bool> UnblockAsync(int id);
}