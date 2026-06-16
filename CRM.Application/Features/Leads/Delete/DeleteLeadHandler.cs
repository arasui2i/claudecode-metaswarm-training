using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Leads.Delete;

public class DeleteLeadHandler(ILeadRepository repo) : IRequestHandler<DeleteLeadCommand>
{
    public async Task Handle(DeleteLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Lead {request.Id} not found.");

        lead.IsDeleted = true;
        lead.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(lead, cancellationToken);
    }
}
