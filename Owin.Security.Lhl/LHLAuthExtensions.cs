namespace Owin
{
    using System;

    using Owin.Security.Lhl;

    public static class LHLAuthExtensions
    {
        public static IAppBuilder UseLHLAuthentication(this IAppBuilder app, LHLAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (options == null)
                throw new ArgumentNullException("options");
            app.Use(typeof(LHLAuthenticationMiddleware), app, options);
            return app;
        }

        public static IAppBuilder UseLHLAuthentication(this IAppBuilder app, string appId,string appKey)
        {
            return app.UseLHLAuthentication(new LHLAuthenticationOptions { 
                AppID=appId,
                AppKey=appKey,
            });
        }
    }
}