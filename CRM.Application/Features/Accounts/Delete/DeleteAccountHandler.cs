using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Accounts.Delete;

public class DeleteAccountHandler(IAccountRepository repo) : IRequestHandler<DeleteAccountCommand>
{
    public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Account {request.Id} not found.");

        account.IsDeleted = true;
        account.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(account, cancellationToken);
    }
}
