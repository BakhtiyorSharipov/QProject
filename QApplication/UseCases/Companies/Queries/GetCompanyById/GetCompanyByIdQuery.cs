using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Companies.Queries.GetCompanyById;

public record GetCompanyByIdQuery(int Id): IRequest<CompanyResponseModel>;