using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineAttendece.Models
{
    public class Regeister
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public string RoleId { get; set; }
        public string UserPassword { get; set; }
        public string UserEmail { get; set; }
    }
}