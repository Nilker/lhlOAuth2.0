﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyOAuth2.Server.Controllers
{
    public class TokenController : Controller
    {
        public ActionResult Index()
        {
            return new EmptyResult();
        }
	}
}