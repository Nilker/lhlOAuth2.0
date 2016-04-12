using MyOAuth2.Consts;

namespace Owin.Security.Lhl
{
    using Microsoft.Owin.Security;

    public class LHLAuthenticationOptions:AuthenticationOptions
    {
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
        public string AppID { get; set; }

        public string AppKey { get; set; }

        public string CallbackPath { get; set; }

        public string Host { get; set; }

        public string SignInAsAuthenticationType { get; set; }

        public ILHLAuthenticationProvider Provider { get; set; }

        public LHLAuthenticationOptions()
            : this("LHL")
        {
            
        }

        public LHLAuthenticationOptions(string authenticationType)
            : base(authenticationType)
        {
            this.Description.Caption = "LHL User";
            CallbackPath = Paths.AuthorizeCodeCallBackPath;
        }
    }
}