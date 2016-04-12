
namespace Owin.Security.Lhl
{
    using System;
    using System.Threading.Tasks;

    public class LHLAuthenticationProvider : ILHLAuthenticationProvider
    {
        public Func<LHLAuthenticatedContext, Task> OnAuthenticated { get; set; }
        public Func<LHLReturnEndpointContext, Task> OnReturnEndpoint { get; set; }

        public LHLAuthenticationProvider()
        {
            this.OnAuthenticated = (context => Task.FromResult((object)null));
            this.OnReturnEndpoint = (context => Task.FromResult((object)null));
        }

        public Task Authenticated(LHLAuthenticatedContext context)
        {
            return this.OnAuthenticated(context);
        }

        public Task ReturnEndpoint(LHLReturnEndpointContext context)
        {
            return this.OnReturnEndpoint(context);
        }
    }
}