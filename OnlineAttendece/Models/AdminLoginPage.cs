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
        public int? MobileNumber { get; set; }
        public string RoleId { get; set; }
        public DateTime? LastLogin { get; set; }
        [Required]
        public string UserPassword { get; set; }

    }
}