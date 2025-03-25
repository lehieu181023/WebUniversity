using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

public class AdminAccessHandler : AuthorizationHandler<IAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        // Nếu user có role "Admin", cho phép truy cập luôn
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

