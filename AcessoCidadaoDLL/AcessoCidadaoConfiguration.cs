using System.IdentityModel.Tokens;
using System.Web.Helpers;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Owin.Security;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Owin;
using System;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;
using AcessoCidadaoDLL.Util;

namespace AcessoCidadaoDLL
{
    
    public static class AcessoCidadaoConfiguration
    {
        public static void Configuration(IAppBuilder app)
        {
            // Define que a chave do usuário vem da Claim "sub"
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";
            // Desliga o mapeamento automático de Claims do asp.net, os nomes ficam como as
            // Claims do Acesso Cidadão..
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            
            app.UseKentorOwinCookieSaver();

            // Configurar o OWIN para gerenciar autenticação usando Cookies
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            // Configurando para fazer autenticação usando OpenID Connect
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = WebConfigUtil.BaseAddress,
                ClientId = WebConfigUtil.ClientIdCorporativo,
                RedirectUri = WebConfigUtil.RedirectUriCorporativo,
                ResponseType = "id_token code",
                // A maioria desses scopes só está disponível para sistemas corporativos do estado
                // e precisa de permissão explicita da área responsável. Fazer requisição devidamente
                // fundamentada de porque a informação é necessária
                //Scope = "openid nome email cpf dataNascimento filiacao permissoes roles offline_access api-teste",

                Scope = "openid nome email cpf dataNascimento filiacao permissoes roles",

                // Configura o middleware do openid connect para usar o middleware configurado acima
                // para controlar a autenticação usando cookies
                SignInAsAuthenticationType = "Cookies",

                // Essa parte é totalmente opcional
                // Nesse exemplo a gente vai usar ela para apagar algumas claims que vem do Acesso Cidadão
                // e não queremos guardar e para setar o nome da identity
                // Mas também pode ser usada para fazer qualquer tipo de mapeamento das claims que vem do Acesso Cidadão
                // Por exemplo: Trocar o nome, por primeiro nome e sobrenome, mapear as permissões para outra
                // representação e assim por diante
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = async n =>
                    {
                        // use the code to get the access and refresh token
                        var tokenClient = new TokenClient(
                            WebConfigUtil.TokenEndpoint,
                            WebConfigUtil.ClientIdCorporativo,
                            WebConfigUtil.ClientSecretCorporativo);

                        var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(
                            n.Code, n.RedirectUri);

                        if (tokenResponse.IsError)
                        {
                            throw new Exception(tokenResponse.Error);
                        }

                        // use the access token to retrieve claims from userinfo
                        var userInfoClient = new UserInfoClient(WebConfigUtil.UserInfoEndpoint);
                        var userInfoResponse = await userInfoClient.GetAsync(tokenResponse.AccessToken);

                        // create new identity
                        var id = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);
                        id.AddClaims(userInfoResponse.Claims);

                        id.AddClaim(new Claim("access_token", tokenResponse.AccessToken));
                        id.AddClaim(new Claim("expires_at", DateTime.Now.AddSeconds(tokenResponse.ExpiresIn).ToLocalTime().ToString()));
                        id.AddClaim(new Claim("refresh_token", (!string.IsNullOrEmpty(tokenResponse.RefreshToken) ? tokenResponse.RefreshToken : string.Empty)));
                        id.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        var sid = n.AuthenticationTicket.Identity.FindFirst("sid");

                        id.AddClaim(new Claim("sid", (sid != null ? sid.Value : string.Empty)));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            new ClaimsIdentity(id.Claims, n.AuthenticationTicket.Identity.AuthenticationType, "nome", "role"),
                            n.AuthenticationTicket.Properties);
                    },

                    RedirectToIdentityProvider = n =>
                    {
                        // if signing out, add the id_token_hint
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }

                        return Task.FromResult(0);
                    }
                }
            });
            
        }
    }
}
