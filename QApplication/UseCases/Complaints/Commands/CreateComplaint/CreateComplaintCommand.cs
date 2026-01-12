using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Complaints.Commands.CreateComplaint;

public record CreateComplaintCommand(int QueueId, int CustomerId, string ComplaintText): IRequest<ComplaintResponseModel>;