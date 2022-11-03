using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace WebApplication1.Provider
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); //   
        }
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            /*  var originalClient = context.Ticket.Properties.Dictionary["client_id"];
              var currentClient = context.ClientId;
              if (originalClient != currentClient)
              {
                  context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                  return Task.FromResult<object>(null);
              }*/
            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));
            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);
            return Task.FromResult<object>(null);
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            try
            { 
                if (context.UserName == "a4admin" &&  context.Password == "p4pass")
                {

                    identity.AddClaim(new Claim(ClaimTypes.Role, "USER"));
                    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    //  identity.AddClaim(new Claim("Email", user.UserEmailID));


                    context.Validated(identity);
                } 
                else
                {
                    context.SetError("invalid_grant", "Provided username and password is incorrect");
                    context.Rejected();
                }
                         
            return; 
            }
            catch (Exception ex)
            {
                context.SetError("invalid_grant", ex.ToString() + ex.Message + ex.InnerException.ToString());
                context.Rejected();
            }
        }
    }
}