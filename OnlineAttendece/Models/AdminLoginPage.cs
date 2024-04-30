using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineAttendece.Models
{
    public class AdminLoginPage
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public Nullable<System.DateTime> LastLogin { get; set; }
        public string OTP { get; internal set; }



    }
}