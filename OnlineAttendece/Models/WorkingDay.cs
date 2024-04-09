using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineAttendece.Models
{
    public class WorkingDay
    {
        public int Working_Day_Id { get; set; }
        public string Day_Of_Week { get; set; }
        public DateTime? Start_Time { get;  set; }
        public DateTime? End_Time { get; set; }
        public  bool Holidaymatch { get; set; }
    }
}