using FluentValidation;

namespace CRM.Application.Features.Activities.Create;

public class CreateActivityValidator : AbstractValidator<CreateActivityCommand>
{
    public CreateActivityValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}
