using CRM.Application.Interfaces;
using FluentValidation;

namespace CRM.Application.Features.Accounts.Create;

public class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountValidator(IAccountRepository repo)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MustAsync(async (name, ct) => !await repo.NameExistsAsync(name, null, ct))
            .WithMessage("An account with this name already exists.");
    }
}
