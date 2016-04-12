using Microsoft.Owin;
using Owin;
using Owin.Security.Lhl;

[assembly: OwinStartupAttribute(typeof(MyOAuth2.Client.Startup))]
namespace MyOAuth2.Client
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.UseLHLAuthentication(new LHLAuthenticationOptions()
            {
                AppID = MyOAuth2.Consts.Paths.AppId,
                AppKey = MyOAuth2.Consts.Paths.AppSecret,
                CallbackPath = MyOAuth2.Consts.Paths.AuthorizeCodeCallBackPath,
            });
        }
    }
}
