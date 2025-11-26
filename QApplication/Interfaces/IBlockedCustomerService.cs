using QApplication.Requests.BlockedCustomerRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IBlockedCustomerService
{
    IEnumerable<BlockedCustomerResponseModel> GetAll(int pageList, int pageNumber);
    IEnumerable<BlockedCustomerResponseModel> GetAllBlockedCustomersByCompany(int companyId);
    BlockedCustomerResponseModel GetById(int id);
    BlockedCustomerResponseModel Block(BlockedCustomerRequestModel request);
    bool Unblock(int id);
}