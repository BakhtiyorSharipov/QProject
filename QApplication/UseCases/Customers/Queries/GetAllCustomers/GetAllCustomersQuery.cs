using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Customers.Queries.GetAllCustomers;

public record GetAllCustomersQuery(int PageNumber) : IRequest<PagedResponse<CustomerResponseModel>>;
