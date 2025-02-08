using System.Collections.Immutable;
using System.Net;
using System.Reflection;
using coaches.Infrastructure.Common.Extensions;
using coaches.Modules.Shared.Contracts.Common.Interfaces;
using coaches.Modules.Shared.Contracts.Common.Security;
using MassTransit;
using Shared.Application.Exceptions;

namespace coaches.Infrastructure.Messaging.Filters;

public class AuthorizationFilter<T>(
    IUser user,
    IIdentityService identityService)
    : IFilter<ConsumeContext<T>>
    where T : class
{
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var authorizeAttributes = typeof(T).GetCustomAttributes<AuthorizeAttribute>().ToList();

        if (authorizeAttributes.Count is not 0)
        {
            // Must be authenticated user
            if (user.Id == null)
            {
                await context.NotifyFilterFault("You are not authorized to access this resource.", (int)HttpStatusCode.Unauthorized);
                return;
            }

            // Role-based authorization
            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles)).ToImmutableArray();
            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;
                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var isInRole = await identityService.IsInRoleAsync(user.Id, role.Trim());
                        if (isInRole)
                        {
                            authorized = true;
                            break;
                        }
                    }

                    if (authorized) break;
                }

                // Must be a member of at least one role in roles
                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }

            // Policy-based authorization
            var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy)).ToImmutableArray();
            if (authorizeAttributesWithPolicies.Any())
            {
                foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                {
                    var authorized = await identityService.AuthorizeAsync(user.Id, policy);
                    if (!authorized)
                    {
                        throw new ForbiddenAccessException();
                    }
                }
            }
        }

        // User is authorized / authorization not required
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("authorization-filter");
    }
}
