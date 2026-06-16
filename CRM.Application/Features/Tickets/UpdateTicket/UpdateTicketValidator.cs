using FluentValidation;

namespace CRM.Application.Features.Tickets.UpdateTicket;

public class UpdateTicketValidator : AbstractValidator<UpdateTicketCommand>
{
    public UpdateTicketValidator()
    {
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Priority).IsInEnum();
    }
}
