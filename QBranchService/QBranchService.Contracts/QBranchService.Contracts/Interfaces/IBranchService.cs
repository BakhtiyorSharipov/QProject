using MagicOnion;
using QBranchService.Contracts.Requests;
using QBranchService.Contracts.Responses;

namespace QBranchService.Contracts.Interfaces;

public interface IBranchService: IService<IBranchService>
{
    UnaryResult<BranchResponse> CheckBranchId(BranchRequest request);
    UnaryResult<CompanyResponse> CheckCompanyId(CompanyRequest request);
    UnaryResult<CompanyServiceResponse> CheckCompanyServiceId(CompanyServiceRequest request);
    UnaryResult<QueueCreationValidationResponse> ValidateQueueCreationAsync(QueueCreationValidationRequest request);

}