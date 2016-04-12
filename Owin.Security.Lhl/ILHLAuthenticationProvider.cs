using Owin.Security.Lhl;

namespace Owin.Security.Lhl
{
    using System.Threading.Tasks;

    public interface ILHLAuthenticationProvider
    {
        Task Authenticated(LHLAuthenticatedContext context);
        
        Task ReturnEndpoint(LHLReturnEndpointContext context);
    }
}