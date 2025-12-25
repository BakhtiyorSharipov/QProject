using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Services.Commands.CreateService;

public record CreateServiceCommand(int CompanyId, string ServiceName, string ServiceDescription): IRequest<ServiceResponseModel>;