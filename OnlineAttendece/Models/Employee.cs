using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineAttendece.Models
{
    public class Employee
    {
        public int EMP_Id { get; set; }
        public string EMP_Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public int? Office_Id { get; set; }
        public String Office_Name { get; set; } = String.Empty;
    }
}