using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ImageShare.Data;

namespace MvcApplication29.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signin(string emailAddress, string password)
        {
            var manager = new UserManager(Properties.Settings.Default.ConStr);
            var user = manager.Login(emailAddress, password);
            if (user == null)
            {
                return Redirect("/account/signin");
            }

            FormsAuthentication.SetAuthCookie(emailAddress, true);
            return Redirect("/");
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(string firstName, string lastName, string emailAddress, string password)
        {
            var manager = new UserManager(Properties.Settings.Default.ConStr);
            manager.AddUser(firstName, lastName, emailAddress, password);
            FormsAuthentication.SetAuthCookie(emailAddress, true);
            return Redirect("/");
        }

        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }

    }
}
