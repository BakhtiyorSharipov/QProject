using MediatR;
using Microsoft.AspNetCore.Mvc;
using QBranchService.Application.Requests;
using QBranchService.Application.Response;
using QBranchService.Application.UseCases.BranchConfigurations.Commands.CreateBranchConfiguration;
using QBranchService.Application.UseCases.BranchConfigurations.Commands.DeleteBranchConfiguration;
using QBranchService.Application.UseCases.BranchConfigurations.Commands.UpdateBranchConfiguration;
using QBranchService.Application.UseCases.BranchConfigurations.Queries.GetAllBranchConfigurations;
using QBranchService.Application.UseCases.BranchConfigurations.Queries.GetBranchConfigurationById;


namespace QBranchService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchConfigurationController : ControllerBase
{
    private readonly ILogger<BranchConfigurationController> _logger;
    private readonly IMediator _mediator;

    public BranchConfigurationController(ILogger<BranchConfigurationController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BranchConfigurationResponseModel>>> GetAll(
        [FromQuery] int pageNumber = 1)
    {
        _logger.LogInformation(
            "Received request to get all branch configurations. PageNumber: {PageNumber}, PageSize: 15",
            pageNumber);

        var query = new GetAllBranchConfigurationsQuery(pageNumber);
        var branches = await _mediator.Send(query);
        return Ok(branches);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BranchResponseModel>> GetById([FromRoute] int id)
    {
        _logger.LogInformation("Received request to get branch configuration by Id: {configurationId}", id);
        var query = new GetBranchConfigurationByIdQuery(id);
        var branch = await _mediator.Send(query);
        _logger.LogInformation("Successfully returned branch configuration with Id: {branchId}", id);
        return Ok(branch);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateBranchConfigurationCommand request)
    {
        _logger.LogInformation("Received request to create new branch configuration. MaxTickets: {maxTickets}",
            request.MaxTickets);
        var createBranchConfiguration = await _mediator.Send(request);
        _logger.LogInformation("Successfully created branch configuration with Id: {branchId}",
            createBranchConfiguration.Id);
        return Ok(createBranchConfiguration);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] UpdateBranchConfigurationRequest request)
    {
        _logger.LogInformation("Received request to update branch configuration with Id: {branchId}", id);

        var command = new UpdateBranchConfigurationCommand(id, request.MaxTickets, request.OpenTime, request.CloseTime);

        var update = await _mediator.Send(command);
        _logger.LogInformation("Successfully updated branch configuration with Id: {branchId}", id);
        return Ok(update);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete branch configuration with Id: {branchId}", id);
        var command = new DeleteBranchConfigurationCommand(id);
        await _mediator.Send(command);
        _logger.LogInformation("Successfully deleted branch configuration with Id: {branchId}", id);
        return NoContent();
    }
}