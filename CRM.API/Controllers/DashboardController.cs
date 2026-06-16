using CRM.Application.Features.Dashboard.GetDashboardSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("summary")]
    [Authorize(Policy = "dashboard.view")]
    public async Task<IActionResult> GetSummary(CancellationToken ct) =>
        Ok(await mediator.Send(new GetDashboardSummaryQuery(), ct));
}
