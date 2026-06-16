using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Leads.GetById;

public class GetLeadByIdHandler(ILeadRepository repo) : IRequestHandler<GetLeadByIdQuery, LeadDetailDto>
{
    public async Task<LeadDetailDto> Handle(GetLeadByIdQuery request, CancellationToken cancellationToken)
    {
        var l = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Lead {request.Id} not found.");

        return new LeadDetailDto(
            l.Id, l.FirstName, l.LastName, l.Email, l.PhoneNumber,
            l.Company, l.JobTitle, l.Status, l.Source, l.Notes,
            l.ConvertedCustomerId, l.CreatedAt, l.UpdatedAt
        );
    }
}
