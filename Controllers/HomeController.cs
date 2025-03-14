﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RedirectToRegister()
        {
            return RedirectToAction("Register", "Account");
        }

        public ActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Account");
        }
    }
}
