// A handler that can determine whether a DownloadNumberRequirement is satisfied
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

internal class DownloadAuthorizationHandler : AuthorizationHandler<DownloadRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DownloadRequirement requirement)
    {
        // Check that the JWT token provided has been provisioned for download use
        if (context.User.HasClaim(c => c.Value == "Download" && c.Type == "Use"))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

internal class DownloadRequirement : IAuthorizationRequirement
{
    public DownloadRequirement()
    {
        
    }
}