using AdamDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Engine.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Login(string email,string password)
        {
            DbWrappers wrap = new DbWrappers();
            bool loginStatus=wrap.CheckLoginDetails(email,password);

            return this.Json(loginStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SignUp(string firstName, string lastName, string email, string password)
        {
            DbWrappers wrap = new DbWrappers();
            bool signUpStatus=wrap.AddSignUpDetail(firstName,lastName,email,password);
            return this.Json(signUpStatus, JsonRequestBehavior.AllowGet);

            
        }

    }
}
