using QApplication.Requests.BlockedCustomerRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IBlockedCustomerService: IBaseService<BlockedCustomerEntity, BlockedCustomerResponseModel, BlockedCustomerRequestModel>
{
    
}