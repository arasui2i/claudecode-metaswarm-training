using CRM.Application.Features.Contacts.Create;
using CRM.Application.Features.Contacts.Delete;
using CRM.Application.Features.Contacts.GetAll;
using CRM.Application.Features.Contacts.GetById;
using CRM.Application.Features.Contacts.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContactsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "customers.view")]
    public async Task<IActionResult> GetAll([FromQuery] GetContactsQuery query, CancellationToken ct)
        => Ok(await mediator.Send(query, ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "customers.view")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetContactByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "customers.edit")]
    public async Task<IActionResult> Create([FromBody] CreateContactCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "customers.edit")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route id does not match body id.");
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "customers.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteContactCommand(id), ct);
        return NoContent();
    }
}
