namespace Owin.Security.Lhl
{
    using System.Net.Http;

    using Microsoft.Owin;
    using Microsoft.Owin.Logging;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.DataHandler;
    using Microsoft.Owin.Security.DataProtection;
    using Microsoft.Owin.Security.Infrastructure;

    using Owin;

    public class LHLAuthenticationMiddleware : AuthenticationMiddleware<LHLAuthenticationOptions>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        public LHLAuthenticationMiddleware(OwinMiddleware next,IAppBuilder app, LHLAuthenticationOptions options)
            : base(next, options)
        {
            this._logger = AppBuilderLoggerExtensions.CreateLogger<LHLAuthenticationMiddleware>(app);
            if (options.Provider == null)
            {
                options.Provider = new LHLAuthenticationProvider();
            }

            if (this.Options.StateDataFormat == null)
            {
                this.Options.StateDataFormat =new PropertiesDataFormat(app.CreateDataProtector(typeof(LHLAuthenticationMiddleware).FullName,this.Options.AuthenticationType,"v1"));
            }
            if (string.IsNullOrEmpty(this.Options.SignInAsAuthenticationType))
                this.Options.SignInAsAuthenticationType = AppBuilderSecurityExtensions.GetDefaultSignInAsAuthenticationType(app);


            this._httpClient = new HttpClient(ResolveHttpMessageHandler(this.Options));
            
            this._httpClient.MaxResponseContentBufferSize = 10485760L;
        }

        protected override AuthenticationHandler<LHLAuthenticationOptions> CreateHandler()
        {
            return new LHLAuthenticationHandler(this._httpClient,this._logger);
        }

        private static HttpMessageHandler ResolveHttpMessageHandler(LHLAuthenticationOptions options)
        {
            return new WebRequestHandler();
        }
    }
}