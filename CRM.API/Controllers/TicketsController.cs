using CRM.Application.Features.Tickets.CreateTicket;
using CRM.Application.Features.Tickets.DeleteTicket;
using CRM.Application.Features.Tickets.GetTicketById;
using CRM.Application.Features.Tickets.GetTickets;
using CRM.Application.Features.Tickets.UpdateTicket;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "tickets.view")]
    public async Task<IActionResult> GetAll([FromQuery] GetTicketsQuery query, CancellationToken ct) =>
        Ok(await mediator.Send(query, ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "tickets.view")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetTicketByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "tickets.create")]
    public async Task<IActionResult> Create(CreateTicketCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "tickets.edit")]
    public async Task<IActionResult> Update(Guid id, UpdateTicketCommand cmd, CancellationToken ct)
    {
        if (id != cmd.Id) return BadRequest();
        await mediator.Send(cmd, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "tickets.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteTicketCommand(id), ct);
        return NoContent();
    }
}
