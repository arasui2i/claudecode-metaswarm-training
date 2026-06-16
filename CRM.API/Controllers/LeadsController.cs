using CRM.Application.Features.Leads.Create;
using CRM.Application.Features.Leads.Delete;
using CRM.Application.Features.Leads.GetAll;
using CRM.Application.Features.Leads.GetById;
using CRM.Application.Features.Leads.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeadsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "customers.view")]
    public async Task<IActionResult> GetAll([FromQuery] GetLeadsQuery query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "customers.view")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLeadByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "customers.edit")]
    public async Task<IActionResult> Create([FromBody] CreateLeadCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "customers.edit")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLeadCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest("Route id does not match body id.");
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "customers.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteLeadCommand(id), ct);
        return NoContent();
    }
}
