using OnlineAttendece.ADODBFIle;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace LogisticWebApiApplication.Services
{
    public class EmailService
    {
        private readonly Office_AttendanceEntities _dbContext;
        public string EmailMessage { get; set; }
        public class ResponseData
        {
            public string Message { get; set; }
            public int StatusCode { get; set; }
            // Other properties if needed
        }

        public async Task<string> SendEmail(string emailId,  string emailMessage)
        {
            string returnMessage = string.Empty;
            try
            {
                string subject = "Reseet Password Otp";

                EmailMessage = emailMessage;
                int otp = await GenerateOTPAsync(6);
                HttpContext.Current.Session["OTP"] = otp; // Store OTP in session
                string body = "Your OTP for resetting the password is: " + otp;
                await SendEmailAsync(subject, body, emailId);
                returnMessage = "Otp sent on user email";                
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
            }
            return returnMessage;
        }
        public static async Task<int> GenerateOTPAsync(int length)
        {
            Random random = new Random();
            int min = (int)Math.Pow(10, length - 1);
            int max = (int)Math.Pow(10, length) - 1;
            return await Task.Run(() => random.Next(min, max));
        }

        private async Task SendEmailAsync(string subject, string body, string recipientEmail)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress("arnav.augurs@gmail.com");
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(recipientEmail);

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
                {
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential("arnav.augurs@gmail.com", "wghhxoaaaxohegsl");

                    await smtp.SendMailAsync(mailMessage);
                }
            }
        }
        public async Task<ResponseData> VerifiyOtp(string emailId, int otp)
        {
            ResponseData response = new ResponseData();
            try
            {
                var user = await _dbContext.AdminLogins.FirstOrDefaultAsync(u => u.UserId == emailId);

                if (user != null && user.OTP.ToString() == otp.ToString())
                {
                    response.Message = "OTP matched successfully.";
                    response.StatusCode = 200;
                }
                else
                {
                    response.Message = "OTP does not match.";
                    response.StatusCode = 400;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = 500;
            }
            return response;
        }


        private string ReplacePlaceholders(string templateBody, string firstName)
        {
            templateBody = templateBody.Replace("{UserName}", firstName);
            templateBody = templateBody.Replace("{EmailMessage}", EmailMessage);
            return templateBody;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
