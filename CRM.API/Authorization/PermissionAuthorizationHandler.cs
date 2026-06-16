using Microsoft.AspNetCore.Authorization;

namespace CRM.API.Authorization;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Permissions are stored as role claims; roles map to permissions via seed data.
        // A future enhancement can load permissions from DB per request for real-time revocation.
        var roles = context.User.Claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .ToHashSet();

        // Role-to-permission mapping (mirrors seed data in AppDbContext)
        var rolePermissions = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Admin"]  = ["customers.view", "customers.edit", "customers.delete"],
            ["Sales"]  = ["customers.view", "customers.edit"],
            ["Viewer"] = ["customers.view"],
        };

        var hasPermission = roles.Any(role =>
            rolePermissions.TryGetValue(role, out var perms) &&
            perms.Contains(requirement.ActionKey));

        if (hasPermission)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
