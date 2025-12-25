using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Customers.Queries.GetAllCustomers;

public record GetAllCustomersQuery(int pageNumber, int pageSize) : IRequest<PagedResponse<CustomerResponseModel>>;
