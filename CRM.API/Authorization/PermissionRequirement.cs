using Microsoft.AspNetCore.Authorization;

namespace CRM.API.Authorization;

public class PermissionRequirement(string actionKey) : IAuthorizationRequirement
{
    public string ActionKey { get; } = actionKey;
}
