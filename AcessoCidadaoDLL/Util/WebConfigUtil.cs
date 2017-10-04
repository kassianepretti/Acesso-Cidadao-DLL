using System.Configuration;

namespace AcessoCidadaoDLL.Util
{
    public static class WebConfigUtil
    {
        private static ConnectionStringSettings getConcectionString(string chave)
        {
            return ConfigurationManager.ConnectionStrings[chave];
        }

        private static string getParametroWebConfig(string chave)
        {
            return ConfigurationManager.AppSettings[chave];
        }

        public static string AuthorizeEndpoint
        {
            get
            {
                return BaseAddress + "/connect/authorize";
            }
        }

        public static string LogoutEndpoint
        {
            get
            {
                return BaseAddress + "/connect/endsession";
            }
        }

        public static string TokenEndpoint
        {
            get
            {
                return BaseAddress + "/connect/token";
            }
        }

        public static string UserInfoEndpoint
        {
            get
            {
                return BaseAddress + "/connect/userinfo";
            }
        }

        public static string IdentityTokenValidationEndpoint
        {
            get
            {
                return BaseAddress + "/connect/identitytokenvalidation";
            }
        }

        public static string BaseAddress
        {
            get
            {
                return getParametroWebConfig("BaseAddress").ToString();
            }
        }

        public static string RedirectUriCorporativo
        {
            get
            {
                return getParametroWebConfig("RedirectUriCorporativo").ToString();
            }
        }

        public static string RedirectUriImplicit
        {
            get
            {
                return getParametroWebConfig("RedirectUriImplicit").ToString();
            }
        }

        public static string ClientIdCorporativo
        {
            get
            {
                return getParametroWebConfig("ClientIdCorporativo").ToString();
            }
        }

        public static string ClientSecretCorporativo
        {
            get
            {
                return getParametroWebConfig("ClientSecretCorporativo").ToString().Replace("&amp;", "&");
            }
        }

        public static string ClientIdImplicit
        {
            get
            {
                return getParametroWebConfig("ClientIdImplicit").ToString();
            }
        }
    }
}
