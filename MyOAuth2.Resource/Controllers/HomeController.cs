using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.Owin.Host.SystemWeb;

namespace MyOAuth2.Resource.Controllers
{
    public class HomeController : ApiController
    {
        [Authorize]
        [Route("api/Home/GetValues")]
        public object GetValues()
        {
            var identity = System.Web.HttpContext.Current.GetOwinContext().Request.User.Identity;
            var claims = identity as ClaimsIdentity;
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userName = identity.Name;

            return Json(new { userName = userName, userId = userId });
        }
    }
}
