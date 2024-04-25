using OfficeOpenXml;
using OnlineAttendece.ADODBFIle;
using OnlineAttendece.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Xceed.Words.NET; // For Word
using System.IO;
using System.Data.Entity;


namespace OnlineAttendece.Controllers
{

    public class HomeController : Controller
    {
        readonly Office_AttendanceEntities db = new Office_AttendanceEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Chart()
        {
            List<Attendence> AttreoList = new List<Attendence>();
            var attendanceData = db.Attendence_Master.ToList();
            foreach (var AttLoop in attendanceData)
            {
                AttreoList.Add(new Attendence
                {
                    Attendence_Id = AttLoop.Attendence_Id,
                    Attendence_Date = (DateTime)AttLoop.Attendence_Date,
                    Cheack_In_Time = (TimeSpan)AttLoop.Cheack_In_Time,
                    Cheack_Out_Time = (TimeSpan)AttLoop.Cheack_In_Time
                });
            }
            return View(AttreoList);
        }

        [HttpGet]
        public ActionResult Employee()
        {
            List<Employee> EmpMod = new List<Employee>();
            var EMPList = db.Employee_Master.ToList();
            var OfficeList = db.Office_Master.ToList();
            foreach (var EMPLoop in EMPList)
            {
                EmpMod.Add(new Employee
                {
                    EMP_Id = EMPLoop.EMP_Id,
                    EMP_Name = EMPLoop.EMP_Name,
                    Designation = EMPLoop.Designation,
                    Department = EMPLoop.Department,
                    Office_Id = EMPLoop.Office_Id,
                    Office_Name = officeName(EMPLoop.Office_Id)
                });
                string officeName(int? officeId)
                {
                    string OffiName = OfficeList.Where(x => x.Office_Id == officeId).FirstOrDefault()?.Office_Name;
                    return OffiName;
                }
            }
            return View(EmpMod);
        }
        [HttpPost]
        public ActionResult EmployeeAdd(Employee employee)
        {
            var EMPvar = new Employee_Master
            {
                EMP_Id = employee.EMP_Id,
                EMP_Name = employee.EMP_Name,
                Designation = employee.Designation,
                Department = employee.Department,
                Office_Id = employee.Office_Id
            };
            db.Employee_Master.Add(EMPvar);
            db.SaveChanges();
            return RedirectToAction("Employee");
        }


        [HttpGet]
        public JsonResult EmployeeList()
        {
            List<Employee> EmpMod = new List<Employee>();
            var EMPList = db.Employee_Master.ToList();
            foreach (var EMPLoop in EMPList)
            {
                EmpMod.Add(new Employee
                {
                    EMP_Id = EMPLoop.EMP_Id,
                    EMP_Name = EMPLoop.EMP_Name,
                    Designation = EMPLoop.Designation,
                    Department = EMPLoop.Department,
                    Office_Id = EMPLoop.Office_Id

                });
            }
            return Json(EmpMod, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteEmployee(int id)
        {
            try
            {
                var employeeToDelete = db.Employee_Master.Find(id);
                if (employeeToDelete != null)
                {
                    db.Employee_Master.Remove(employeeToDelete);
                    db.SaveChanges();
                }
                return RedirectToAction("Employee");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while deleting the attendance data.");
                return RedirectToAction("Employee");
            }
        }

        [HttpGet]
        public ActionResult Office()
        {
            List<Office> OffiMod = new List<Office>();
            var OffiList = db.Office_Master.ToList();
            foreach (var OfficLoop in OffiList)
            {
                OffiMod.Add(new Office
                {
                    Office_Id = OfficLoop.Office_Id,
                    Office_Name = OfficLoop.Office_Name,
                    Location = OfficLoop.Location,
                    Cantact_Info = (int)OfficLoop.Cantact_Info
                });
            }
            return View(OffiMod);
        }
        [HttpGet]
        public JsonResult OfficeList()
        {
            List<Office> OffiMod = new List<Office>();
            var OffiList = db.Office_Master.ToList();
            foreach (var OfficLoop in OffiList)
            {
                OffiMod.Add(new Office
                {
                    Office_Id = OfficLoop.Office_Id,
                    Office_Name = OfficLoop.Office_Name,
                    Location = OfficLoop.Location,
                    Cantact_Info = (int)OfficLoop.Cantact_Info
                });
            }
            return Json(OffiMod, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OfficeAdd(Office office)
        {
            var OfficeVar = new Office_Master();
            OfficeVar.Office_Id = office.Office_Id;
            OfficeVar.Office_Name = office.Office_Name;
            OfficeVar.Location = office.Location;
            OfficeVar.Cantact_Info = office.Cantact_Info;
            db.Office_Master.Add(OfficeVar);
            db.SaveChanges();
            return RedirectToAction("Office");
        }
        public ActionResult Attendence()
        {
            List<Attendence> AttMod = new List<Attendence>();
            var AttList = db.Attendence_Master.ToList();
            var emplist = db.Employee_Master.ToList();
            foreach (var Attpara in AttList)
            {
                AttMod.Add(new Attendence
                {
                    Attendence_Id = Attpara.Attendence_Id,
                    Attendence_Date = Attpara.Attendence_Date.Value,
                    Cheack_In_Time = Attpara.Cheack_In_Time.Value,
                    Cheack_Out_Time = Attpara.Cheack_Out_Time.Value,
                    EMP_Id = Attpara.EMP_Id,
                    EMP_Name = ename(Attpara.EMP_Id)
                });

            }
            string ename(int? empid)
            {
                string name = emplist.Where(x => x.EMP_Id == empid).FirstOrDefault()?.EMP_Name;
                return name;
            }
            return View(AttMod);
        }

        [HttpGet]
        public ActionResult DeleteAttendance(int id)
        {
            try
            {
                var attendanceToDelete = db.Attendence_Master.Find(id);
                if (attendanceToDelete != null)
                {
                    db.Attendence_Master.Remove(attendanceToDelete);
                    db.SaveChanges();
                }
                return RedirectToAction("Attendence");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while deleting the attendance data.");
                return RedirectToAction("Attendence");
            }
        }
        [HttpPost]
        public ActionResult AttendenceAdd(Attendence attendence)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var Attvar = new Attendence_Master();
                    Attvar.Attendence_Date = attendence.Attendence_Date;
                    Attvar.Cheack_In_Time = attendence.Cheack_In_Time;
                    Attvar.Cheack_Out_Time = attendence.Cheack_Out_Time;
                    Attvar.EMP_Id = attendence.EMP_Id;
                    db.Attendence_Master.Add(Attvar);
                    db.SaveChanges();
                    return RedirectToAction("Attendence");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while saving the attendance data.");
                }
            }
            return View("Attendence", attendence);
        }

        [HttpGet]
        public ActionResult Holiday()
        {
            List<Models.Holiday> Holi = new List<Models.Holiday>();
            var hd = db.Holiday_Master.ToList();
            foreach (var h in hd)
            {
                Holi.Add(new Holiday
                {
                    Holiday_Id = h.Holiday_Id,
                    Holiday_Name = h.Holiday_Name,
                    Holiday_Date = h.Holiday_Date.Value
                });
            }

            return View(Holi);
        }
        public ActionResult DeleteHoliday(int id)
        {
            try
            {
                var HolidayToDelete = db.Holiday_Master.Find(id);
                if (HolidayToDelete != null)
                {
                    db.Holiday_Master.Remove(HolidayToDelete);
                    db.SaveChanges();
                }
                return RedirectToAction("Holiday");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while deleting the attendance data.");
                return RedirectToAction("Attendence");
            }
        }
        [HttpPost]
        public ActionResult HolidayAdd(Holiday holiday)
        {
            var holidays = new Holiday_Master();
            holidays.Holiday_Id = holiday.Holiday_Id;
            holidays.Holiday_Name = holiday.Holiday_Name;
            holidays.Holiday_Date = holiday.Holiday_Date;
            db.Holiday_Master.Add(holidays);
            db.SaveChanges();
            return RedirectToAction("Holiday");
        }

        public ActionResult Workingday()
        {
            List<WorkingDay> workdays = new List<WorkingDay>();
            var WorkVar = db.Working_day_Master.ToList();
            foreach (var workloop in WorkVar)
            {
                workdays.Add(item: new WorkingDay
                {
                    Working_Day_Id = workloop.Working_Day_Id,
                    Day_Of_Week = workloop.Day_Of_Week,
                    Start_Time = workloop.Start_Time,
                    End_Time = workloop.End_Time
                });
            }

            return View(workdays);
        }
        [HttpPost]
        public ActionResult WorkingdayAdd(WorkingDay workday)
        {
            try
            {
                var workdaysList = new Working_day_Master();
                workdaysList.Working_Day_Id = workday.Working_Day_Id;
                workdaysList.Day_Of_Week = workday.Day_Of_Week;
                workdaysList.Start_Time = workday.Start_Time;
                workdaysList.End_Time = workday.End_Time;
                db.Working_day_Master.Add(workdaysList);
                db.SaveChanges();
                return RedirectToAction("Workingday");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        public ActionResult DeleteWorkingDay(int id)
        {
            try
            {
                var WorkingDayToDelete = db.Working_day_Master.Find(id);
                if (WorkingDayToDelete != null)
                {
                    db.Working_day_Master.Remove(WorkingDayToDelete);
                    db.SaveChanges();
                }
                return RedirectToAction("WorkingDay");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while deleting the attendance data.");
                return RedirectToAction("Attendence");
            }
        }


        // All Master Report  here

        public ActionResult EmployeeReport()
        {
            List<Employee> EmpMod = new List<Employee>();
            var EMPList = db.Employee_Master.ToList();
            var OfficeList = db.Office_Master.ToList();
            foreach (var EMPLoop in EMPList)
            {
                EmpMod.Add(new Employee
                {
                    EMP_Id = EMPLoop.EMP_Id,
                    EMP_Name = EMPLoop.EMP_Name,
                    Designation = EMPLoop.Designation,
                    Department = EMPLoop.Department,
                    Office_Id = EMPLoop.Office_Id,
                    Office_Name = officeName(EMPLoop.Office_Id)
                });
                string officeName(int? officeId)
                {
                    string OffiName = OfficeList.Where(x => x.Office_Id == officeId).FirstOrDefault()?.Office_Name;
                    return OffiName;
                }
            }
            return View(EmpMod);
        }

        public ActionResult OfficeReport()
        {
            List<Office> OffiMod = new List<Office>();
            var OffiList = db.Office_Master.ToList();
            foreach (var OfficLoop in OffiList)
            {
                OffiMod.Add(new Office
                {
                    Office_Id = OfficLoop.Office_Id,
                    Office_Name = OfficLoop.Office_Name,
                    Location = OfficLoop.Location,
                    Cantact_Info = (int)OfficLoop.Cantact_Info
                });
            }
            return View(OffiMod);
        }

        public ActionResult AttendenceReport()
        {
            List<Attendence> AttMod = new List<Attendence>();
            var AttList = db.Attendence_Master.ToList();
            var emplist = db.Employee_Master.ToList();
            foreach (var Attpara in AttList)
            {
                AttMod.Add(new Attendence
                {
                    Attendence_Id = Attpara.Attendence_Id,
                    Attendence_Date = Attpara.Attendence_Date.Value,
                    Cheack_In_Time = Attpara.Cheack_In_Time.Value,
                    Cheack_Out_Time = Attpara.Cheack_Out_Time.Value,
                    EMP_Id = Attpara.EMP_Id,
                    EMP_Name = ename(Attpara.EMP_Id)
                });

            }
            string ename(int? empid)
            {
                string name = emplist.Where(x => x.EMP_Id == empid).FirstOrDefault().EMP_Name;
                return name;
            }
            return View(AttMod);
        }
        public ActionResult HolidayReport()
        {
            List<Models.Holiday> Holi = new List<Models.Holiday>();
            var hd = db.Holiday_Master.ToList();
            foreach (var h in hd)
            {
                Holi.Add(new Holiday
                {
                    Holiday_Id = h.Holiday_Id,
                    Holiday_Name = h.Holiday_Name,
                    Holiday_Date = h.Holiday_Date.Value
                });
            }

            return View(Holi);
        }

        public ActionResult WorkingdayReport()
        {
            List<WorkingDay> workdays = new List<WorkingDay>();
            var WorkVar = db.Working_day_Master.ToList();
            var Holiay = db.Holiday_Master.ToList();
            foreach (var workloop in WorkVar)
            {
                workdays.Add(item: new WorkingDay
                {
                    Working_Day_Id = workloop.Working_Day_Id,
                    Day_Of_Week = workloop.Day_Of_Week,
                    Start_Time = workloop.Start_Time,
                    End_Time = (DateTime)workloop.End_Time,
                    Holidaymatch = Holidaymatch(workloop.End_Time)
                });
            }
            bool Holidaymatch(DateTime? Dt)
            {
                bool result = false;
                if (Dt.HasValue)
                {
                    var HDdata = Holiay.Where(x => x.Holiday_Date.Value.Date == Dt.Value.Date).FirstOrDefault();
                    if (HDdata != null)
                    {
                        result = true;
                    }
                }
                return result;
            }

            return View(workdays);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Account");
        }

		public FileResult DownloadExcel()
		{
			List<WorkingDay> workdays = new List<WorkingDay>();
			var WorkVar = db.Working_day_Master.ToList();
			foreach (var workloop in WorkVar)
			{
				workdays.Add(new WorkingDay
				{
					Working_Day_Id = workloop.Working_Day_Id,
					Day_Of_Week = workloop.Day_Of_Week,
					Start_Time = workloop.Start_Time,
					End_Time = (DateTime)workloop.End_Time,
				});
			}

			// Generate Excel file
			using (var excelPackage = new ExcelPackage())
			{
				var worksheet = excelPackage.Workbook.Worksheets.Add("Working Day Report");

				// Add headers
				worksheet.Cells[1, 1].Value = "Working Day ID";
				worksheet.Cells[1, 2].Value = "Day Of Week";
				worksheet.Cells[1, 3].Value = "Start Time";
				worksheet.Cells[1, 4].Value = "End Time";

				// Add data
				int row = 2;
				foreach (var day in workdays)
				{
					worksheet.Cells[row, 1].Value = day.Working_Day_Id;
					worksheet.Cells[row, 2].Value = day.Day_Of_Week;
					worksheet.Cells[row, 3].Value = day.Start_Time?.ToString(@"hh\:mm");
					worksheet.Cells[row, 4].Value = day.End_Time?.ToString(@"hh\:mm");
					row++;
				}

				// Convert to byte array
				byte[] excelBytes = excelPackage.GetAsByteArray();

				// Return Excel file
				return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WorkingDayReport.xlsx");
			}
		}

	}
}