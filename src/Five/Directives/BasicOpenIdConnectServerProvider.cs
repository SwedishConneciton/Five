using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Five.Directives
{
    /// <summary>
    /// Intentially implementing the interface rather than extending the base class
    /// to see what is actually being called.
    /// </summary>
    public class BasicOpenIdConnectServerProvider : IOpenIdConnectServerProvider
    {
        public Func<ValidateTokenRequestContext, Task> OnValidateTokenRequest { get; set; } = context =>
        {
            context.Skip();
            return Task.FromResult(0);
        };

        public Func<GrantResourceOwnerCredentialsContext, Task> OnGrantResourceOwnerCredentials { get; set; } = context => {
            var Options = context
                            .HttpContext
                            .RequestServices
                            .GetRequiredService<IOptions<IdentityOptions>>()
                            .Value;

            var Scopes = context.Request.GetScopes();

            var Identity = new ClaimsIdentity(
                OpenIdConnectServerDefaults.AuthenticationScheme,
                Options.ClaimsIdentity.UserNameClaimType,
                Options.ClaimsIdentity.RoleClaimType
            );

            Identity.AddClaim(
                ClaimTypes.Name,
                "me",
                OpenIdConnectConstants.Destinations.AccessToken,
                OpenIdConnectConstants.Destinations.IdentityToken
            );

            /// Dummy examples of scope to claim mappings
            if (Scopes.Contains(OpenIdConnectConstants.Scopes.Profile))
            {
                Identity.AddClaim(
                    ClaimTypes.NameIdentifier,
                    "12345678",
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken
                );
            }

            if (Scopes.Contains(OpenIdConnectConstants.Scopes.Email))
            {
                Identity.AddClaim(
                    ClaimTypes.Email,
                    "me",
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken
                );
            }

            // Create a new authentication ticket holding the user identity.
            var Ticket = new AuthenticationTicket(
                new ClaimsPrincipal(Identity),
                new AuthenticationProperties(),
                context.Options.AuthenticationScheme
            );

            Ticket.SetResources(
                context.Request.GetResources()
            );
            Ticket.SetScopes(
                Scopes
            );

            context.Validate(Ticket);

            return Task.FromResult(0);
        };

        public Func<MatchEndpointContext, Task> OnMatchEndpoint { get; set; } = context =>
        {
            return Task.FromResult<object>(null);
        };

        public Func<HandleTokenRequestContext, Task> OnHandleTokenRequest { get; set; } = context =>
        {
            return Task.FromResult<object>(null);
        };

        public Func<SerializeAccessTokenContext, Task> OnSerializeAccessToken { get; set; } = context =>
        {
            return Task.FromResult<object>(null);
        };

        public Func<ApplyTokenResponseContext, Task> OnApplyTokenResponse { get; set; } = context =>
        {
            return Task.FromResult<object>(null);
        };

        public Func<ValidateConfigurationRequestContext, Task> OnValidateConfigurationRequest { get; set; } = context =>
        {
            return Task.FromResult<object>(null);
        };

        public Func<HandleConfigurationRequestContext, Task> OnHandleConfigurationRequest { get; set; } = context =>
        {
            return Task.FromResult<object>(null);
        };

        public Func<ApplyConfigurationResponseContext, Task> OnApplyConfigurationResponse { get; set; } = context =>
        {
            return Task.FromResult<object>(null);
        };

        public Func<ValidateCryptographyRequestContext, Task> OnValidateCryptographyRequest { get; set; } = context =>
        {
            return Task.FromResult<object>(null);
        };

        public Func<HandleCryptographyRequestContext, Task> OnHandleCryptographyRequest { get; set; } = context =>
        {
            return Task.FromResult<object>(null);
        };

        public Func<ApplyCryptographyResponseContext, Task> OnApplyCryptographyResponse { get; set; } = context =>
        {
            return Task.FromResult<object>(null);
        };

        public Task MatchEndpoint(MatchEndpointContext context) => OnMatchEndpoint(context);

        public Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Task ValidateConfigurationRequest(ValidateConfigurationRequestContext context) => OnValidateConfigurationRequest(context);

        public Task ValidateCryptographyRequest(ValidateCryptographyRequestContext context) => OnValidateCryptographyRequest(context);

        public Task ValidateIntrospectionRequest(ValidateIntrospectionRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Task ValidateLogoutRequest(ValidateLogoutRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Task ValidateRevocationRequest(ValidateRevocationRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Task ValidateTokenRequest(ValidateTokenRequestContext context) => OnValidateTokenRequest(context);

        public Task ValidateUserinfoRequest(ValidateUserinfoRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Task GrantAuthorizationCode(GrantAuthorizationCodeContext context)
        {
            throw new NotImplementedException();
        }

        public Task GrantRefreshToken(GrantRefreshTokenContext context)
        {
            throw new NotImplementedException();
        }

        public Task GrantResourceOwnerCredentials(GrantResourceOwnerCredentialsContext context) => OnGrantResourceOwnerCredentials(context);

        public Task GrantClientCredentials(GrantClientCredentialsContext context)
        {
            throw new NotImplementedException();
        }

        public Task GrantCustomExtension(GrantCustomExtensionContext context)
        {
            throw new NotImplementedException();
        }

        public Task HandleAuthorizationRequest(HandleAuthorizationRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Task HandleConfigurationRequest(HandleConfigurationRequestContext context) => OnHandleConfigurationRequest(context);


        public Task HandleCryptographyRequest(HandleCryptographyRequestContext context) => OnHandleCryptographyRequest(context);

        public Task HandleIntrospectionRequest(HandleIntrospectionRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Task HandleLogoutRequest(HandleLogoutRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Task HandleRevocationRequest(HandleRevocationRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Task HandleTokenRequest(HandleTokenRequestContext context) => OnHandleTokenRequest(context);

        public Task HandleUserinfoRequest(HandleUserinfoRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Task ApplyAuthorizationResponse(ApplyAuthorizationResponseContext context)
        {
            throw new NotImplementedException();
        }

        public Task ApplyConfigurationResponse(ApplyConfigurationResponseContext context) => OnApplyConfigurationResponse(context);

        public Task ApplyCryptographyResponse(ApplyCryptographyResponseContext context) => OnApplyCryptographyResponse(context);

        public Task ApplyIntrospectionResponse(ApplyIntrospectionResponseContext context)
        {
            throw new NotImplementedException();
        }

        public Task ApplyLogoutResponse(ApplyLogoutResponseContext context)
        {
            throw new NotImplementedException();
        }

        public Task ApplyRevocationResponse(ApplyRevocationResponseContext context)
        {
            throw new NotImplementedException();
        }

        public Task ApplyTokenResponse(ApplyTokenResponseContext context) => OnApplyTokenResponse(context);

        public Task ApplyUserinfoResponse(ApplyUserinfoResponseContext context)
        {
            throw new NotImplementedException();
        }

        public Task SerializeAuthorizationCode(SerializeAuthorizationCodeContext context)
        {
            throw new NotImplementedException();
        }

        public Task SerializeAccessToken(SerializeAccessTokenContext context) => OnSerializeAccessToken(context);

        public Task SerializeIdentityToken(SerializeIdentityTokenContext context)
        {
            throw new NotImplementedException();
        }

        public Task SerializeRefreshToken(SerializeRefreshTokenContext context)
        {
            throw new NotImplementedException();
        }

        public Task DeserializeAuthorizationCode(DeserializeAuthorizationCodeContext context)
        {
            throw new NotImplementedException();
        }

        public Task DeserializeAccessToken(DeserializeAccessTokenContext context)
        {
            throw new NotImplementedException();
        }

        public Task DeserializeIdentityToken(DeserializeIdentityTokenContext context)
        {
            throw new NotImplementedException();
        }

        public Task DeserializeRefreshToken(DeserializeRefreshTokenContext context)
        {
            throw new NotImplementedException();
        }
    }
}
