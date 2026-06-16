using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using MediatR;

namespace CRM.Application.Features.Leads.Create;

public class CreateLeadHandler(ILeadRepository repo) : IRequestHandler<CreateLeadCommand, Guid>
{
    public async Task<Guid> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
    {
        if (await repo.EmailExistsAsync(request.Email, null, cancellationToken))
            throw new InvalidOperationException("A lead with this email already exists.");

        var lead = new Lead
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Company = request.Company,
            JobTitle = request.JobTitle,
            Status = request.Status,
            Source = request.Source,
            Notes = request.Notes,
        };

        await repo.AddAsync(lead, cancellationToken);
        return lead.Id;
    }
}
