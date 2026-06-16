using CRM.Application.Features.Activities.Create;
using CRM.Application.Features.Activities.Delete;
using CRM.Application.Features.Activities.GetAll;
using CRM.Application.Features.Activities.GetById;
using CRM.Application.Features.Activities.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActivitiesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetActivitiesQuery query, CancellationToken ct) =>
        Ok(await mediator.Send(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetActivityByIdQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateActivityCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateActivityCommand cmd, CancellationToken ct)
    {
        if (id != cmd.Id) return BadRequest();
        await mediator.Send(cmd, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteActivityCommand(id), ct);
        return NoContent();
    }
}
