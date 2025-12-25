using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.BlockedCustomers.Commands.CreateBlockedCustomer;

public record CreateBlockedCustomerCommand(
    int CompanyId,
    int CustomerId,
    string? Reason,
    DateTime BannedUntil,
    bool DoesBanForever) : IRequest<BlockedCustomerResponseModel>;