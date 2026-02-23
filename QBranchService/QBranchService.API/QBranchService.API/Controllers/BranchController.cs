using MediatR;
using Microsoft.AspNetCore.Mvc;
using QBranchService.Application.Requests;
using QBranchService.Application.Response;
using QBranchService.Application.UseCases.Branches.Commands.CreateBranch;
using QBranchService.Application.UseCases.Branches.Commands.DeleteBranch;
using QBranchService.Application.UseCases.Branches.Commands.UpdateBranch;
using QBranchService.Application.UseCases.Branches.Queries.GetAllBranches;
using QBranchService.Application.UseCases.Branches.Queries.GetBranchById;

namespace QBranchService.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class BranchController: ControllerBase
{
    private readonly ILogger<BranchController> _logger;
    private readonly IMediator _mediator;

    public BranchController(ILogger<BranchController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BranchResponseModel>>> GetAll([FromQuery] int pageNumber = 1)
    {
        _logger.LogInformation("Received request to get all branches. PageNumber: {PageNumber}, PageSize: 15",
            pageNumber);

        var query = new GetAllBranchesQuery(pageNumber);
        var branches = await _mediator.Send(query);
        return Ok(branches);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BranchResponseModel>> GetById([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get branch by Id: {branchId}", id);
        var query = new GetBranchByIdQuery(id);
        var branch = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned branch with Id: {branchId}", id);
        return Ok(branch);

    }
    
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateBranchCommand request)
    {
        _logger.LogInformation("Received request to create new branch. BranchName: {branchName}",
            request.BranchName);
        var createBranch = await _mediator.Send(request);
        _logger.LogInformation("Successfully created branch with Id: {branchId}", createBranch.Id);
        return Ok(createBranch);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] UpdateBranchRequest request)
    {
        _logger.LogInformation("Received request to update branch with Id: {branchId}", id);
        
        var command = new UpdateBranchCommand(id, request.BranchName, request.City, request.Address,
            request.PhoneNumber, request.EmailAddress);
        
        var update = await _mediator.Send(command);
        _logger.LogInformation("Successfully updated branch with Id: {branchId}", id);
        return Ok(update);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete branch with Id: {branchId}", id);
        var command = new DeleteBranchCommand(id);
        await _mediator.Send(command);
        _logger.LogInformation("Successfully deleted branch with Id: {branchId}", id);
        return NoContent();
    }
}