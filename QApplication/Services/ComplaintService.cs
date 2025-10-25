using System.Net;
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

    public ComplaintService(IComplaintRepository complaintRepository, IQueueRepository queueRepository, ICustomerRepository customerRepository)
    {
        _complaintRepository = complaintRepository;
        _queueRepository = queueRepository;
        _customerRepository = customerRepository;
    }
    
    public IEnumerable<ComplaintResponseModel> GetAllComplaints(int pageList, int pageNumber)
    {
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

        return response;
    }

    public IEnumerable<ComplaintResponseModel> GetAllComplaintsByQueue(int id)
    {
        var dbComplaints = _complaintRepository.GetAllComplaintsByQueue(id);
        if (!dbComplaints.Any())
        {
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

        return response;
    }

    public IEnumerable<ComplaintResponseModel> GetAllComplaintsByCompany(int companyId)
    {
        var dbComplaints = _complaintRepository.GetAllComplaintsByCompany(companyId);
        if (!dbComplaints.Any())
        {
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

        return response;
    }

    public ComplaintResponseModel GetComplaintById(int id)
    {
        var dbComplaint = _complaintRepository.FindComplaintById(id);
        if (dbComplaint==null)
        {
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


        return response;
    }

    public ComplaintResponseModel AddComplaint(ComplaintRequestModel request)
    {
        var requestToCreate = request as CreateComplaintRequest;
        if (requestToCreate==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(ComplaintEntity));
        }

        var customerId = _customerRepository.FindById(requestToCreate.CustomerId);
        if (customerId==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var queueId = _queueRepository.FindById(requestToCreate.QueueId);
        if (queueId==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }


        if (queueId.Status != QueueStatus.Completed && queueId.Status != QueueStatus.CanceledByAdmin && queueId.Status != QueueStatus.CancelledByEmployee)
        {
            throw new Exception("You can leave complaint when status is Completed or CanceledByAdmin/ByEmployee");
        }

        var complaints = GetAllComplaintsByQueue(queueId.Id);
        var isDouble = complaints.Any(s => s.CustomerId == customerId.Id);
        if (isDouble)
        {
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
        var dbComplaint = _complaintRepository.FindComplaintById(id);
        if (dbComplaint==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ComplaintEntity));
        }

        switch (request.ComplaintStatus)
        {
            case ComplaintStatus.Pending:
                if (dbComplaint.ComplaintStatus != ComplaintStatus.Resolved)
                {
                    throw new Exception("Pending status can only be Reviewed");
                }
                if (dbComplaint.ComplaintStatus == ComplaintStatus.Resolved)
                {
                    throw new Exception("This complaint is already finalized and cannot be updated!");
                }
                break;
            case ComplaintStatus.Reviewed:
                if (dbComplaint.ComplaintStatus !=ComplaintStatus.Pending)
                {
                    throw new Exception("Reviewed status can only be Resolved");
                }
                
                break;
            case ComplaintStatus.Resolved:
                if (dbComplaint.ComplaintStatus!= ComplaintStatus.Reviewed)
                {
                    throw new Exception("Resolved can be when status is Reviewed");
                }
                break;
            default:
                throw new Exception("There is not this kind of status");
        }

        dbComplaint.ComplaintStatus = request.ComplaintStatus;
        dbComplaint.ResponseText = request.ResponseText;
        
        _complaintRepository.UpdateComplaintStatus(dbComplaint);
        _complaintRepository.SaveChanges();

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