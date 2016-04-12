using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace MyOAuth2.Server.Controllers
{
    public class AuthorizeController : Controller
    {
        public ActionResult Index()
        {
            if (Response.StatusCode != 200)
            {
                return View("Error");
            }
            IAuthenticationManager authentication = HttpContext.GetOwinContext().Authentication;
            AuthenticateResult ticket = authentication.AuthenticateAsync(Startup.AuthenticationType).Result;
            ClaimsIdentity identity = ticket != null ? ticket.Identity : null;
            if (identity == null)
            {
                authentication.Challenge(Startup.AuthenticationType);
                return new HttpUnauthorizedResult();
            }

            string[] scopes = (Request.QueryString.Get("scope") ?? "").Split(' ');



            identity = new ClaimsIdentity(identity.Claims, "Bearer", identity.NameClaimType,
                identity.RoleClaimType);
            foreach (string scope in scopes)
            {
                identity.AddClaim(new Claim("urn:oauth:scope", scope));
            }
            authentication.SignIn(identity);



            return View();
        }
    }
}