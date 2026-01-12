using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Complaints.Queries.GetComplaintById;

public class GetComplaintByIdQueryHandler: IRequestHandler<GetComplaintByIdQuery, ComplaintResponseModel>
{
    private readonly ILogger<GetComplaintByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetComplaintByIdQueryHandler(ILogger<GetComplaintByIdQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ComplaintResponseModel> Handle(GetComplaintByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting complaint by Id {id}", request.Id);
        var dbComplaint = await _dbContext.Complaints.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbComplaint == null)
        {
            _logger.LogWarning("Complaint with Id {id} not found.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ComplaintEntity));
        }

        var response = new ComplaintResponseModel
        {
            Id = dbComplaint.Id,
            CustomerId = dbComplaint.CustomerId,
            QueueId = dbComplaint.QueueId,
            ComplaintText = dbComplaint.ComplaintText,
            ResponseText = dbComplaint.ResponseText,
            ComplaintStatus = dbComplaint.ComplaintStatus
        };

        _logger.LogInformation("Complaint by Id {id} fetched successfully.", request.Id);
        return response;
    }
}