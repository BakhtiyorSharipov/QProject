using System.Net;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.ComplaintRequest;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.Services;

public class ComplaintService: IComplaintService
{
    private readonly IComplaintRepository _complaintRepository;
    private readonly IQueueRepository _queueRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<ComplaintService> _logger;

    public ComplaintService(IComplaintRepository complaintRepository, IQueueRepository queueRepository,
        ICustomerRepository customerRepository, ILogger<ComplaintService> logger)
    {
        _complaintRepository = complaintRepository;
        _queueRepository = queueRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }
    
    public IEnumerable<ComplaintResponseModel> GetAllComplaints(int pageList, int pageNumber)
    {
        _logger.LogInformation("Getting all complaints. PageNumber: {pageNumber}, PageList {pageList}", pageNumber, pageList);
        var dbComplaint = _complaintRepository.GetAllComplaints(pageList, pageNumber);
        
        var response = dbComplaint.Select(complaint => new ComplaintResponseModel
        {
            Id = complaint.Id,
            CustomerId = complaint.CustomerId,
            QueueId = complaint.QueueId,
            ComplaintText = complaint.ComplaintText,
            ResponseText = complaint.ResponseText,
            ComplaintStatus = complaint.ComplaintStatus
        }).ToList();

        _logger.LogInformation("Fetched {cmoplaintCount} complaints.", response.Count);
        return response;
    }

    public IEnumerable<ComplaintResponseModel> GetAllComplaintsByQueue(int id)
    {
        _logger.LogInformation("Getting complaints by queue Id {id}", id);
        var dbComplaints = _complaintRepository.GetAllComplaintsByQueue(id);
        if (!dbComplaints.Any())
        {
            _logger.LogWarning("Complaint by queue Id {id} not found", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ComplaintEntity));
        }

        var response = dbComplaints.Select(complaint => new ComplaintResponseModel
        {
            Id = complaint.Id,
            CustomerId = complaint.CustomerId,
            QueueId = complaint.QueueId,
            ComplaintText = complaint.ComplaintText,
            ResponseText = complaint.ResponseText,
            ComplaintStatus = complaint.ComplaintStatus
        }).ToList();

        _logger.LogInformation("Fetched {complaintCount} complaints with this queue Id {id}", response.Count, id);
        return response;
    }

    public IEnumerable<ComplaintResponseModel> GetAllComplaintsByCompany(int companyId)
    {
        _logger.LogInformation("Getting all complaints by company Id {id}", companyId);
        var dbComplaints = _complaintRepository.GetAllComplaintsByCompany(companyId);
        if (!dbComplaints.Any())
        {
            _logger.LogWarning("No complaints with company Id {id}", companyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ComplaintEntity));
        }

        var response = dbComplaints.Select(complaint => new ComplaintResponseModel
        {
            Id = complaint.Id,
            CustomerId = complaint.CustomerId,
            QueueId = complaint.QueueId,
            ComplaintText = complaint.ComplaintText,
            ResponseText = complaint.ResponseText,
            ComplaintStatus = complaint.ComplaintStatus
        }).ToList();

        _logger.LogInformation("Fetched {complaintCount} complaints with this company Id {id}", response.Count, companyId);
        return response;
    }

    public ComplaintResponseModel GetComplaintById(int id)
    {
        _logger.LogInformation("Getting complaint by Id {id}", id);
        var dbComplaint = _complaintRepository.FindComplaintById(id);
        if (dbComplaint==null)
        {
            _logger.LogWarning("Complaint with Id {id} not found.", id);
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

        _logger.LogInformation("Complaint by Id {id} fetched successfully.", id);
        return response;
    }

    public ComplaintResponseModel AddComplaint(ComplaintRequestModel request)
    {
        _logger.LogInformation("Adding new complaint to this queue Id {queueId}", request.QueueId);
        var requestToCreate = request as CreateComplaintRequest;
        if (requestToCreate==null)
        {
            _logger.LogError("Invalid request model while adding new complaint.");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(ComplaintEntity));
        }

        var customerId = _customerRepository.FindById(requestToCreate.CustomerId);
        if (customerId==null)
        {
            _logger.LogWarning("Customer with Id {customerId} not found for adding new complaint.", requestToCreate.CustomerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var queueId = _queueRepository.FindById(requestToCreate.QueueId);
        if (queueId==null)
        {
            _logger.LogWarning("Queue with Id {queueId} not found for adding new complaint,", requestToCreate.QueueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }


        if (queueId.Status != QueueStatus.Completed && queueId.Status != QueueStatus.CanceledByAdmin && queueId.Status != QueueStatus.CancelledByEmployee)
        {
            _logger.LogError("Invalid queue status while adding new complaint for this queue Id {queueId}", requestToCreate.QueueId);
            throw new Exception("You can leave complaint when status is Completed or CanceledByAdmin/ByEmployee");
        }

        var complaints = GetAllComplaintsByQueue(queueId.Id);
        var isDouble = complaints.Any(s => s.CustomerId == customerId.Id);
        if (isDouble)
        {
            _logger.LogError("Overlapping complaint for this queue Id {queueId}", requestToCreate.QueueId);
            throw new Exception("You have already left a complaint for this queue!");
        }
        
        var complaint = new ComplaintEntity
        {
            CustomerId = requestToCreate.CustomerId,
            QueueId = requestToCreate.QueueId,
            ComplaintText = requestToCreate.ComplaintText,
            ComplaintStatus = ComplaintStatus.Pending
        };
        
        _complaintRepository.AddComplaint(complaint);
        _complaintRepository.SaveChanges();

        _logger.LogInformation("Complaint added successfully with Id {id}.", complaint.Id);
        var response = new ComplaintResponseModel
        {
            Id = complaint.Id,
            CustomerId = complaint.CustomerId,
            QueueId = complaint.QueueId,
            ComplaintText = complaint.ComplaintText,
            ComplaintStatus = complaint.ComplaintStatus
        };

        return response;
    }

    public ComplaintResponseModel UpdateComplaintStatus(int id, UpdateComplaintStatusRequest request)
    {
        _logger.LogInformation("Updating complaint status with Id {id}", id);
        var dbComplaint = _complaintRepository.FindComplaintById(id);
        if (dbComplaint==null)
        {
            _logger.LogWarning("Complaint with Id {id} not found for updating status.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ComplaintEntity));
        }

        switch (request.ComplaintStatus)
        {
            case ComplaintStatus.Pending:
                if (dbComplaint.ComplaintStatus != ComplaintStatus.Resolved)
                {
                    _logger.LogError("Invalid Pending status updating for this complaint Id {id}", id);
                    throw new Exception("Pending status can only be Reviewed");
                }
                if (dbComplaint.ComplaintStatus == ComplaintStatus.Resolved)
                {
                    _logger.LogError("Trying to update already finalized status for this complaint Id {id}", id);
                    throw new Exception("This complaint is already finalized and cannot be updated!");
                }
                break;
            case ComplaintStatus.Reviewed:
                if (dbComplaint.ComplaintStatus !=ComplaintStatus.Pending)
                {
                    _logger.LogError("Invalid Reviewed status updating for this complaint Id {id}", id);
                    throw new Exception("Reviewed status can only be Resolved");
                }
                
                break;
            case ComplaintStatus.Resolved:
                if (dbComplaint.ComplaintStatus!= ComplaintStatus.Reviewed)
                {
                    _logger.LogError("Invalid Resolved status updating for this complaint Id {id}", id);
                    throw new Exception("Resolved can be when status is Reviewed");
                }
                break;
            default:
                _logger.LogError("Invalid status choice.");
                throw new Exception("There is not this kind of status");
        }

        dbComplaint.ComplaintStatus = request.ComplaintStatus;
        dbComplaint.ResponseText = request.ResponseText;
        
        _complaintRepository.UpdateComplaintStatus(dbComplaint);
        _complaintRepository.SaveChanges();
        _logger.LogInformation("Complaint status with Id {id} updated successfully.", id);
        var response = new ComplaintResponseModel
        {
            Id = dbComplaint.Id,
            CustomerId = dbComplaint.CustomerId,
            QueueId = dbComplaint.QueueId,
            ComplaintText = dbComplaint.ComplaintText,
            ResponseText = dbComplaint.ResponseText,
            ComplaintStatus = dbComplaint.ComplaintStatus
        };

        return response;

    }
}