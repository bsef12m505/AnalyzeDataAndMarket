using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYPAdam.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult AIndex()
        {
            return View();
        }
        public ActionResult AdminDash()
        {
            return View();
        }
        public ActionResult Charts()
        {
            return View();

        }

        public ActionResult ProductDetails()
        {
            return View();

        }
        public ActionResult UserInfo()
        {
            return View();

        }

    }
}
