namespace Owin.Security.Lhl
{
    using Microsoft.Owin;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Provider;

    public class LHLReturnEndpointContext : ReturnEndpointContext
    {
        public LHLReturnEndpointContext(IOwinContext context, AuthenticationTicket ticket)
            : base(context, ticket)
        {
        }
    }
}