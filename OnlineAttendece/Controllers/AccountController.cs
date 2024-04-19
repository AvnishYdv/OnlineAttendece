using OnlineAttendece.ADODBFIle;
using OnlineAttendece.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace OnlineAttendece.Controllers
{
    public class AccountController : Controller
    {
        Office_AttendanceEntities db = new Office_AttendanceEntities();
        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(string userId, string password)
        {
            var user = db.AdminLogins.FirstOrDefault(u => u.UserId == userId && u.UserPassword == password);
            if (user != null)
            {
                Session["UserId"] = user;
                Session["Password"] = password;
                Session["UserName"] = user.UserName;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", " Invalid user ID or password!");
                return View("Index");
            }
        }
        public ActionResult AdminLogin()
        {
            return View();
        }
   
            public ActionResult ForgetPassword()
            {
                // Display a view for entering the user ID
                return View();
            }

        public ActionResult ResetPassword()
        {
            // Display a view for entering the user ID
            return View();
        }
    }

    }
    

