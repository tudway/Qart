﻿using Microsoft.Owin.Security.OAuth;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Qart.Rest.Core.Auth
{

    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IAuthorisationManager _authorisationManager;

        public SimpleAuthorizationServerProvider(IAuthorisationManager authorisationManager)
        {
            _authorisationManager = authorisationManager;
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.CompletedTask;
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                if (!_authorisationManager.Authorise(context))
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                }
                else
                {
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("sub", context.UserName));
                    identity.AddClaim(new Claim("role", "user"));

                    context.Validated(identity);
                }
            }
            catch (Exception)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
            }
            return Task.CompletedTask;
        }
    }
}
