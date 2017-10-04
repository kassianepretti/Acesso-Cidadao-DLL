# Acesso-Cidadao-DLL
Projeto de DLL para integração com o acesso cidadão.

Modo de usar:

1. Adicionar a DLL no novo projeto;
2. Realizar a instalação dos pacotes NuGet necessários;
3. Configurar o web.config com os dados da aplicação. Lembrando que as Keys devem ser iguais do exemplo abaixo:

    <add key="BaseAddress" value="xxx" />
    <add key="RedirectUriCorporativo" value="xxx" />
    <add key="RedirectUriImplicit" value=" xxx " />
    <add key="ClientIdCorporativo" value=" xxx " />
    <add key="ClientSecretCorporativo" value=" xxx " />
    <add key="ClientIdImplicit" value=" xxx " />

4. Código do arquivo Startup.cs:

using Microsoft.Owin;
using Owin;
using AcessoCidadaoDLL;

[assembly: OwinStartup(typeof(VolumeOferta.MVC.Startup))]
namespace VolumeOferta.MVC
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AcessoCidadaoConfiguration.Configuration(app);
        }
    }
}
