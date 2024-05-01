using LogisticWebApiApplication.Services;
using OnlineAttendece.ADODBFIle;
using OnlineAttendece.Models;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
namespace OnlineAttendece.Controllers
{
    public class AccountController : Controller
    {
      readonly  Office_AttendanceEntities db = new Office_AttendanceEntities();
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
            var existingEmail = db.Registrations.Any(r => r.UserEmail == regeister.UserEmail);
            if (existingEmail)
            {
                ViewBag.EmailError = "Email already exists.";
                return View("Register");
            }
            var Regst = new Registration
            {
                UserId = regeister.UserEmail,
                UserName = regeister.UserName,
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

            db.AdminLogins.Add(login);
            db.SaveChanges();

            TempData["RegistrationSuccess"] = true; // Set a flag to indicate successful registration
            return RedirectToAction("Register");
        }


        [HttpGet]
        public async Task<ActionResult> ForgetPassword(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var user = db.AdminLogins.FirstOrDefault(u => u.UserId == email);
                if (user != null)
                {
                    Session["Email"] = email;

                    EmailService emailService = new EmailService();
                  await  emailService.SendEmail(email, "Otp for Forget Password");
                    return RedirectToAction("VerifiyOtp");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email!");
                    return Json(new { success = false, message = "Invalid email!" }, JsonRequestBehavior.AllowGet);
                }
            }
            // Return the view for the initial GET request
            return View();
        }
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            var email = Session["Email"]?.ToString();
            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match.";
                return View();
            }
            var adminUser = db.AdminLogins.FirstOrDefault(u => u.UserId == email);
            if (adminUser != null)
            {
                adminUser.UserPassword = newPassword;
                db.SaveChanges();

                ViewBag.SuccessMessage = "Password updated successfully.";

                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "User with the provided email does not exist.";
                return View();
            }
        }

        public ActionResult VerifiyOtp()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> VerifiyOtp(string otp)
        {
            var storedOtp = Session["OTP"]?.ToString();

            if (otp == storedOtp)
            {
               return RedirectToAction("ResetPassword");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid OTP. Please enter the correct OTP.";
                return View("VerifiyOtp"); 
            }
        }
    }
}
    
    

