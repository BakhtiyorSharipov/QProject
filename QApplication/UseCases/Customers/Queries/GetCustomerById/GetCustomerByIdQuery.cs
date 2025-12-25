using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(int Id): IRequest<CustomerResponseModel>;