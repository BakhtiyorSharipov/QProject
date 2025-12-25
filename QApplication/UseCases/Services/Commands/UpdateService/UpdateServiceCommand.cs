using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Services.Commands.UpdateService;

public record UpdateServiceCommand(int Id,int CompanyId, string ServiceName, string ServiceDescription): IRequest<ServiceResponseModel>;