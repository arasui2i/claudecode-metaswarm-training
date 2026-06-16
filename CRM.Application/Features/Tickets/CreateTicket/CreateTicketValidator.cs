using FluentValidation;

namespace CRM.Application.Features.Tickets.CreateTicket;

public class CreateTicketValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketValidator()
    {
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Priority).IsInEnum();
    }
}
