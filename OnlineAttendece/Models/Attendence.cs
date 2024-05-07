using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineAttendece.Models
{
    public class Attendence
    {
        public int Attendence_Id { get; set; }
        public DateTime? Attendence_Date { get; set; }
        public DateTime? Cheack_In_Time { get; set; }
        public DateTime? Cheack_Out_Time { get; set; }
        public int? EMP_Id { get; set; }
        public String EMP_Name { get; set; }  = String.Empty;
    }
}