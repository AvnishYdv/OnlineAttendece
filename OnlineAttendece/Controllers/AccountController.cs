using OnlineAttendece.ADODBFIle;
using OnlineAttendece.Models;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
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
        public ActionResult Register()
        {
            List<Regeister> Reg = new List<Regeister>();
            var regList = db.Registrations.ToList();
            foreach (var regLoop in regList)
            {
                Reg.Add(new Regeister
                {
                    UserId = regLoop.UserEmail,
                    UserName = regLoop.UserName,
                    MobileNumber = regLoop.MobileNumber,
                    UserEmail = regLoop.UserEmail,
                    UserPassword = regLoop.UserPassword,
                });
            }
            return View(Reg);
        }
        [HttpPost]
        public ActionResult Registererd(Regeister regeister)
        {
            var Regst = new Registration
            {
              UserId= regeister.UserEmail,
              UserName= regeister.UserName,
                MobileNumber = regeister.MobileNumber,
                UserEmail = regeister.UserEmail,
              UserPassword = regeister.UserPassword,

            };

            db.Registrations.Add(Regst);
            db.SaveChanges();
            var login = new AdminLogin
            {
                UserId = regeister.UserEmail,
                UserName = regeister.UserName,
                UserPassword = regeister.UserPassword
            };

            // Add to Login table
            db.AdminLogins.Add(login);
            db.SaveChanges();

            return RedirectToAction("Register");
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
    
    

