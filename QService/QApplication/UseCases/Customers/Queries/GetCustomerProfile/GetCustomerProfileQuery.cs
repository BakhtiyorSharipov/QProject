using MediatR;
using QApplication.Responses.ReportResponse;

namespace QApplication.UseCases.Customers.Queries.GetCustomerProfile;

public record GetCustomerProfileQuery: IRequest<CustomerProfileResponse>;