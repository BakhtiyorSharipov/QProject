using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.BlockedCustomers.Commands.CreateBlockedCustomer;

public record CreateBlockedCustomerCommand(
    int CustomerId,
    string? Reason,
    DateTime BannedUntil,
    bool DoesBanForever) : IRequest<BlockedCustomerResponseModel>;