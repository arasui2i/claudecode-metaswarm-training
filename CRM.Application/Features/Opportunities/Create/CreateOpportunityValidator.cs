using CRM.Application.Interfaces;
using FluentValidation;

namespace CRM.Application.Features.Opportunities.Create;

public class CreateOpportunityValidator : AbstractValidator<CreateOpportunityCommand>
{
    public CreateOpportunityValidator(IOpportunityRepository repo)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MustAsync(async (name, ct) => !await repo.NameExistsAsync(name, null, ct))
            .WithMessage("An opportunity with this name already exists.");
        RuleFor(x => x.Probability)
            .InclusiveBetween(0, 100)
            .WithMessage("Probability must be between 0 and 100.");
    }
}
