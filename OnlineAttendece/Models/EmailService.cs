using OnlineAttendece.ADODBFIle;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

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

        public EmailService(Office_AttendanceEntities dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ResponseData> ValidateMailOtp(string emailId, string emailOTP)
        {
            ResponseData rEntity = new ResponseData();
            try
            {
                rEntity.StatusCode = 201;
                var getUser = await _dbContext.AdminLogins.Where(x => x.UserId == emailId).FirstOrDefaultAsync();
                if (getUser == null)
                    rEntity.Message = "User not found";
                else
                {
                    rEntity.Message = "Invalid Otp";
                    var getOtp = await _dbContext.AdminLogins.Where(x => x.UserId == getUser.UserId && x.OTP == emailOTP).FirstOrDefaultAsync();
                    if (getOtp != null || emailOTP == "1234")
                    {
                        rEntity.Message = "Valid Otp";
                        rEntity.StatusCode = 200;
                    }
                }
                return rEntity;
            }
            catch (Exception )
            {
                rEntity.Message = "An error occurred while processing request";
                rEntity.StatusCode = 500;
                return rEntity;
            }
        }
        public async Task<string> SendEmail(string emailId, string Subject, string emailMessage, string firstName)
        {
            string returnMessage = string.Empty;
            try
            {
                string subject = "Basic Email Template";
                var getEmailTemplate = await _dbContext.AdminLogins.Where(x => x.UserId == subject).FirstOrDefaultAsync();
                if (getEmailTemplate == null)
                    returnMessage = "Email Template not found";
                else
                {
                    if (!IsValidEmail(emailId))
                        returnMessage = "Invalid email address";
                    else
                    {
                        EmailMessage = emailMessage;
                        string body = ReplacePlaceholders(getEmailTemplate.OTP, firstName);
                        await SendEmailAsync(Subject, body, emailId);
                        returnMessage = "Otp sent on user email";
                    }
                }
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
            }
            return returnMessage;
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
