using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eproject.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult error(string msg)
        {
            ViewBag.msg = msg;
            return View();
        }
        public ActionResult Aboutus()
        {
            return View();
        }
        public ActionResult FAQ()
        {
            return View();
        }
    }
}