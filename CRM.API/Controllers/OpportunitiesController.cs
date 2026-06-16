using CRM.Application.Features.Opportunities.Create;
using CRM.Application.Features.Opportunities.Delete;
using CRM.Application.Features.Opportunities.GetAll;
using CRM.Application.Features.Opportunities.GetById;
using CRM.Application.Features.Opportunities.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OpportunitiesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "customers.view")]
    public async Task<IActionResult> GetAll([FromQuery] GetOpportunitiesQuery query, CancellationToken ct)
        => Ok(await mediator.Send(query, ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "customers.view")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetOpportunityByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "customers.edit")]
    public async Task<IActionResult> Create([FromBody] CreateOpportunityCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "customers.edit")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOpportunityCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route id does not match body id.");
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "customers.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteOpportunityCommand(id), ct);
        return NoContent();
    }
}
