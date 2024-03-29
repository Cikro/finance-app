using System;
using System.Linq;
using System.Threading.Tasks;
using finance_app.Types.Repositories;
using Microsoft.AspNetCore.Authorization;


//https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-3.1
namespace finance_app.Types.Services.V1.Authorization
{
    /// <summary>
    /// An implementation of Microsoft's AuthorizationHandler.
    /// Authorizes "IUserIdResources" with the "UserOwnsResource" Policy that is configured in startup.cs
    /// to be associated with an "AuthorizationPolicy".
    /// When _authorizationService.AuthorizeAsync() is called with the
    /// configured AuthorizationPolicy as a parameter, this method will be called.
    /// </summary>
    public class DatabaseObjectAuthorizationHandler : 
        AuthorizationHandler<UserOwnsResource, IUserIdResource>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                    UserOwnsResource requirement,
                                                   IUserIdResource resource)
        {
            // Parse user's ID from claims
            var userIdFromClaims = context.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            var hasUserId = int.TryParse(userIdFromClaims, out var userId);
            if (!hasUserId)  { context.Fail(); };

            if (resource?.UserId <= 0) {
                var message = "Could not Authorize Resource. Resource does not have UserId populated.";
                throw new ArgumentException(message, nameof(resource));
            }

            if (userId == resource.UserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
    public class UserOwnsResource : IAuthorizationRequirement { }
}