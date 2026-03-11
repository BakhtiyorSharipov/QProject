using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.BlockedCustomers.Commands.CreateBlockedCustomer;

public record CreateBlockedCustomerCommand(
    int CustomerId,
    string? Reason,
    DateTime BannedUntil,
    bool DoesBanForever) : IRequest<BlockedCustomerResponseModel>;