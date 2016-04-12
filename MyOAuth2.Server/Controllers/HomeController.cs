using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace MyOAuth2.Server.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public JsonResult GetUser()
        {
            var ss = this.Request.GetOwinContext().Request.User;
            var claims = ss.Identity as ClaimsIdentity;
            var strId = claims.FindFirstValue(ClaimTypes.NameIdentifier);

            return Json(new { userName = ss.Identity.Name, userId = strId }, JsonRequestBehavior.AllowGet);
        }
    }
}