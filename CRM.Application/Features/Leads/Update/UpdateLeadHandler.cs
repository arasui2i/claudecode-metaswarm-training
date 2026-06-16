using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Leads.Update;

public class UpdateLeadHandler(ILeadRepository repo) : IRequestHandler<UpdateLeadCommand>
{
    public async Task Handle(UpdateLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Lead {request.Id} not found.");

        if (await repo.EmailExistsAsync(request.Email, request.Id, cancellationToken))
            throw new InvalidOperationException("A lead with this email already exists.");

        lead.FirstName = request.FirstName;
        lead.LastName = request.LastName;
        lead.Email = request.Email;
        lead.PhoneNumber = request.PhoneNumber;
        lead.Company = request.Company;
        lead.JobTitle = request.JobTitle;
        lead.Status = request.Status;
        lead.Source = request.Source;
        lead.Notes = request.Notes;
        lead.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(lead, cancellationToken);
    }
}
