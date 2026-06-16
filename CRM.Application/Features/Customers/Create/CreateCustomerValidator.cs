using CRM.Application.Interfaces;
using FluentValidation;

namespace CRM.Application.Features.Customers.Create;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator(ICustomerRepository repo)
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (email, ct) => !await repo.EmailExistsAsync(email, null, ct))
            .WithMessage("Email address is already in use.");
    }
}
