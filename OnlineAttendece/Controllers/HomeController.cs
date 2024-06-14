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
using Xceed.Document.NET;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using Paragraph = iTextSharp.text.Paragraph;
using System.Xml.Linq;
using System.Reflection.Metadata;
using OfficeOpenXml.Style;


namespace OnlineAttendece.Controllers
{
   
    public class HomeController : Controller
    {
        readonly Office_AttendanceEntities db = new Office_AttendanceEntities();

        public ActionResult Index()
        {
            var employeeReportResult = EmployeeReport();
            int totalEmployees = (int)((ViewResultBase)employeeReportResult).ViewBag.TotalEmployees;
            int totalOffices = db.Office_Master.Count();
            decimal totalSalaries = 0;
            foreach (var employee in db.Employee_Master)
            {
                if (decimal.TryParse(employee.Salary, out decimal salary))
                {
                    totalSalaries += salary ;
                }
            }
            int totalHolidays = db.Holiday_Master.Count();
            string formattedTotalSalaries = FormatSalary(totalSalaries);

            ViewBag.TotalEmployees = totalEmployees;
            ViewBag.TotalOffices = totalOffices;
            ViewBag.TotalHolidays = totalHolidays;
            ViewBag.TotalSalaries = formattedTotalSalaries;

            return View();
        }


        // Function to format the salary

        public string FormatSalary(decimal salary)
        {
            if (salary >= 1000)
            {
                return (salary / 1000).ToString("0.##") + "k";
            }
            else
            {
                return salary.ToString();
            }
        }


        // Emolpoyee Method

        public ActionResult EmployeeDashbord()
        {
            var employeeReportResult = EmployeeReport();
            int totalEmployees = (int)((ViewResultBase)employeeReportResult).ViewBag.TotalEmployees;
            int totalOffices = db.Office_Master.Count();
            int totalHolidays = db.Holiday_Master.Count();
            decimal totalSalaries = 0;
            foreach (var employee in db.Employee_Master)
            {
                if (decimal.TryParse(employee.Salary, out decimal salary))
                {
                    totalSalaries += salary;
                }
            }
            string formattedTotalSalaries = FormatSalary(totalSalaries);
            ViewBag.TotalEmployees = totalEmployees;
            ViewBag.TotalOffices = totalOffices;
            ViewBag.TotalHolidays = totalHolidays;
            ViewBag.TotalSalaries = formattedTotalSalaries;
            return View();
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
                    Office_Name = officeName(EMPLoop.Office_Id),
                    Salary = EMPLoop.Salary,
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
                Office_Id = employee.Office_Id,
                Salary  = employee.Salary,
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

        // Office Methods


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
                    Cantact_Info = OfficLoop.Cantact_Info
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
                    Cantact_Info = OfficLoop.Cantact_Info
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

        // Attendence Methods
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


        // Holiday Methods 


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

        // Working Day Methods 


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
                    Office_Name = officeName(EMPLoop.Office_Id),
                    Salary = EMPLoop.Salary
                });
                string officeName(int? officeId)
                {
                    string OffiName = OfficeList.Where(x => x.Office_Id == officeId).FirstOrDefault()?.Office_Name;
                    return OffiName;
                }
            }
            int totalEmployees = EmpMod.Count;
            ViewBag.TotalEmployees = totalEmployees;
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
                    Cantact_Info = OfficLoop.Cantact_Info
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
                string name = emplist.Where(x => x.EMP_Id == empid).FirstOrDefault()?.EMP_Name;
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


        // All Reports Excele File Download Mathods


		public FileResult DownloadExcelHolidays()
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

            using (var excelPackage = new ExcelPackage())
			{
				var worksheet = excelPackage.Workbook.Worksheets.Add("Working Day Report");

                worksheet.Cells["A1"].Value = "Holiday Report";
                worksheet.Cells["A1:E1"].Merge = true; // Merge cells for the heading
                worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center align the heading
                worksheet.Cells["A1:E1"].Style.Font.Bold = true; // Bold the heading

                worksheet.InsertRow(2, 1);

                worksheet.Cells[1, 1].Value = "Holiday Id";
				worksheet.Cells[1, 2].Value = "Holiday Name";
				worksheet.Cells[1, 3].Value = "Holiday Date";

				int row = 2;
				foreach (var day in Holi)
				{
					worksheet.Cells[row, 1].Value = day.Holiday_Id;
					worksheet.Cells[row, 2].Value = day.Holiday_Name;
					worksheet.Cells[row, 3].Value = day.Holiday_Date.ToString(@"yyyy-MM-dd");
					row++;
				}
				byte[] excelBytes = excelPackage.GetAsByteArray();
				return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "HolidayReport.xlsx");
			}
		}
		public FileResult DownloadExcelWorkingDay()
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

			using (var excelPackage = new ExcelPackage())
			{
				var worksheet = excelPackage.Workbook.Worksheets.Add("Working Day Report");

                worksheet.Cells["A1"].Value = "Working Day Report";
                worksheet.Cells["A1:E1"].Merge = true; // Merge cells for the heading
                worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center align the heading
                worksheet.Cells["A1:E1"].Style.Font.Bold = true; // Bold the heading

                worksheet.InsertRow(2, 1);

                worksheet.Cells[1, 1].Value = "Working Day ID";
				worksheet.Cells[1, 2].Value = "Day Of Week";
				worksheet.Cells[1, 3].Value = "Start Time";
				worksheet.Cells[1, 4].Value = "End Time";

				int row = 2;
				foreach (var day in workdays)
				{
					worksheet.Cells[row, 1].Value = day.Working_Day_Id;
					worksheet.Cells[row, 2].Value = day.Day_Of_Week;
					worksheet.Cells[row, 3].Value = day.Start_Time?.ToString(@"hh\:mm");
					worksheet.Cells[row, 4].Value = day.End_Time?.ToString(@"hh\:mm");
					row++;
				}
				byte[] excelBytes = excelPackage.GetAsByteArray();
				return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WorkingDayReport.xlsx");
			}
		}
		public FileResult DownloadExcelOffice()
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
                    Cantact_Info = OfficLoop.Cantact_Info
                });
            }

            using (var excelPackage = new ExcelPackage())
			{
				var worksheet = excelPackage.Workbook.Worksheets.Add("Office Report");

                worksheet.Cells["A1"].Value = "Office Report";
                worksheet.Cells["A1:E1"].Merge = true; // Merge cells for the heading
                worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center align the heading
                worksheet.Cells["A1:E1"].Style.Font.Bold = true; // Bold the heading

                worksheet.InsertRow(3, 1);

                worksheet.Cells[1, 1].Value = "Office ID";
				worksheet.Cells[1, 2].Value = "Office Name";
				worksheet.Cells[1, 3].Value = "Location";
				worksheet.Cells[1, 4].Value = "Contact Info";

				int row = 2;
				foreach (var day in OffiMod)
				{
					worksheet.Cells[row, 1].Value = day.Office_Id;
					worksheet.Cells[row, 2].Value = day.Office_Name;
					worksheet.Cells[row, 3].Value = day.Location;
                    worksheet.Cells[row, 4].Value = day.Cantact_Info;
					row++;
				}
				byte[] excelBytes = excelPackage.GetAsByteArray();
				return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OfficeReport.xlsx");
			}
		}
        [HttpGet]
        public ActionResult Calender()
        {
            var events = new List<CalendarEvent>
            {
                new CalendarEvent { Title = "Event 1", StartDate = new DateTime(2024, 06, 01) },
                new CalendarEvent { Title = "Event 2", StartDate = new DateTime(2024, 06, 10) }
            };

            ViewBag.Events = events;

            return View();
        }

        // All Reports PDF File Downloads Mathods 
        public FileResult DownloadExcelAttendence()
        {
            var AttList = db.Attendence_Master.ToList();
            var emplist = db.Employee_Master.ToDictionary(x => x.EMP_Id, x => x.EMP_Name);

            var excelData = AttList.Select(att => new
            {
                Attendence_Id = att.Attendence_Id,
                Attendence_Date = att.Attendence_Date.Value.ToString("yyyy-MM-dd"), // Format date
                Cheack_In_Time = att.Cheack_In_Time?.ToString(@"hh\:mm"),
                Cheack_Out_Time = att.Cheack_Out_Time?.ToString(@"hh\:mm"),
                EMP_Name = emplist.ContainsKey((int)att.EMP_Id) ? emplist[(int)att.EMP_Id] : "N/A"
            }).ToList();

            var memoryStream = new MemoryStream();
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Attendance Report");

                worksheet.Cells["A1"].Value = "Attendence Report";
                worksheet.Cells["A1:E1"].Merge = true; // Merge cells for the heading
                worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center align the heading
                worksheet.Cells["A1:E1"].Style.Font.Bold = true; // Bold the heading

                worksheet.InsertRow(2, 1);

                worksheet.Cells["A3"].LoadFromCollection(excelData, true);
                package.Save();
            }

            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AttendanceReport.xlsx");
        }
        public FileResult DownloadExcelEmployee()
        {
            List<Employee> EmpMod = new List<Employee>();
            var EMPList = db.Employee_Master.ToList();
            var OfficeList = db.Office_Master.ToDictionary(x => x.Office_Id, x => x.Office_Name);

            var excelData = EMPList.Select(emp => new
            {
                Emp_Id = emp.EMP_Id,
                EMP_Name = emp.EMP_Name,
                Designation = emp.Designation,
                Department = emp.Department,
                Office_Name = OfficeList.ContainsKey((int)emp.Office_Id) ? OfficeList[(int)emp.Office_Id] : "N/A",
                Salary = emp.Salary
            }).ToList();

            var memoryStream = new MemoryStream();
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Office Report");

                worksheet.Cells["A1"].Value = "Office Report";
                worksheet.Cells["A1:E1"].Merge = true; // Merge cells for the heading
                worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center align the heading
                worksheet.Cells["A1:E1"].Style.Font.Bold = true; // Bold the heading

                worksheet.InsertRow(2, 1);

                worksheet.Cells["A3"].LoadFromCollection(excelData, true);
                package.Save();
            }

            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "officeReport.xlsx");
        }

        [Obsolete]
        public FileResult DownloadPDFAttendence()
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

            using (MemoryStream memoryStream = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document();
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                document.Add(new Paragraph(" "));

                Paragraph heading = new Paragraph("Attendence Report");
                heading.Alignment = Element.ALIGN_CENTER;
                heading.SpacingAfter = 20f; // Add bottom margin
                document.Add(heading);


                PdfPTable table = new PdfPTable(5);
                table.AddCell("Attendence ID");
                table.AddCell("Attendamce Date");
                table.AddCell("Cheack_In_Time");
                table.AddCell("Cheack_Out_Time");
                table.AddCell("EMP_Name");

                foreach (var Att in AttMod)
                {
                    table.AddCell(Att.Attendence_Id.ToString());
                    table.AddCell(Att.Attendence_Date?.ToString("yyyy-MM-dd"));
                    table.AddCell(Att.Cheack_In_Time?.ToString(@"hh\:mm"));
                    table.AddCell(Att.Cheack_Out_Time?.ToString(@"hh\:mm"));
                    table.AddCell(Att.EMP_Name);
                }

                document.Add(table);
                document.Close();

                // Convert to byte array
                byte[] pdfBytes = memoryStream.ToArray();

                // Return PDF file
                return File(pdfBytes, "application/pdf", "AttendeceReport.pdf");
            }


        }
        public FileResult DownloadPDFEmployee()
        {


            List<Employee> EmpMod = new List<Employee>();
            var EMPList = db.Employee_Master.ToList();
            var OfficeList = db.Office_Master.ToList();

            foreach (var Emppara in EMPList)
            {
                EmpMod.Add(new Employee
                {
                    EMP_Id = Emppara.EMP_Id,
                    EMP_Name = Emppara.EMP_Name,
                    Designation = Emppara.Designation,
                    Department = Emppara.Department,
                    Office_Id = Emppara.Office_Id,
                    Office_Name = ename(Emppara.Office_Id),
                });

            }
            string ename(int? empid)
            {
                string name = OfficeList.Where(x => x.Office_Id == empid).FirstOrDefault()?.Office_Name;
                return name;
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document();
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                document.Add(new Paragraph(" "));

                Paragraph heading = new Paragraph("Employee Report");
                heading.Alignment = Element.ALIGN_CENTER;
                heading.SpacingAfter = 20f; // Add bottom margin
                document.Add(heading);


                PdfPTable table = new PdfPTable(5);
                table.AddCell("EMP ID");
                table.AddCell("EMP Name");
                table.AddCell("Designation");
                table.AddCell("Department");
                table.AddCell("Office Name");

                foreach (var Att in EmpMod)
                {
                    table.AddCell(Att.EMP_Id.ToString());
                    table.AddCell(Att.EMP_Name);
                    table.AddCell(Att.Designation);
                    table.AddCell(Att.Department);
                    table.AddCell(Att.Office_Name);
                }

                document.Add(table);
                document.Close();

                // Convert to byte array
                byte[] pdfBytes = memoryStream.ToArray();

                // Return PDF file
                return File(pdfBytes, "application/pdf", "EmployeeReport.pdf");
            }


        }
        public ActionResult DownloadPDF()
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

            using (MemoryStream memoryStream = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document();
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                document.Add(new Paragraph(" "));

                Paragraph heading = new Paragraph("Working Day Report");
                heading.Alignment = Element.ALIGN_CENTER;
                heading.SpacingAfter = 20f; // Add bottom margin
                document.Add(heading);

                // Add headers
                PdfPTable table = new PdfPTable(4);
                table.AddCell("Working Day ID");
                table.AddCell("Day Of Week");
                table.AddCell("Start Time");
                table.AddCell("End Time");

                // Add data
                foreach (var day in workdays)
                {
                    table.AddCell(day.Working_Day_Id.ToString());
                    table.AddCell(day.Day_Of_Week);
                    table.AddCell(day.Start_Time?.ToString(@"hh\:mm"));
                    table.AddCell(day.End_Time?.ToString(@"hh\:mm"));
                }

                document.Add(table);
                document.Close();

                // Convert to byte array
                byte[] pdfBytes = memoryStream.ToArray();

                // Return PDF file
                return File(pdfBytes, "application/pdf", "WorkingDayReport.pdf");
            }
        }
        public ActionResult DownloadPDFOffice()
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
                    Cantact_Info = OfficLoop.Cantact_Info
                });
            }


            using (MemoryStream memoryStream = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document();
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                document.Add(new Paragraph(" "));

                Paragraph heading = new Paragraph("Office Report");
                heading.Alignment = Element.ALIGN_CENTER;
                heading.SpacingAfter = 20f; // Add bottom margin
                document.Add(heading);

                // Add headers
                PdfPTable table = new PdfPTable(4);
                table.AddCell("Office ID");
                table.AddCell("Office Name");
                table.AddCell("Location");
                table.AddCell("Contact Info");

                // Add data
                foreach (var day in OffiMod)
                {
                    table.AddCell(day.Office_Id.ToString());
                    table.AddCell(day.Office_Name);
                    table.AddCell(day.Location);
                    table.AddCell(day.Cantact_Info);
                }

                document.Add(table);
                document.Close();

                // Convert to byte array
                byte[] pdfBytes = memoryStream.ToArray();

                // Return PDF file
                return File(pdfBytes, "application/pdf", "OfficeReport.pdf");
            }
        }
        public ActionResult DownloadPDFHoliday()
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
            using (MemoryStream memoryStream = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document();
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                document.Add(new Paragraph(" "));

                Paragraph heading = new Paragraph("Holiday Report");
                heading.Alignment = Element.ALIGN_CENTER;
                heading.SpacingAfter = 20f; // Add bottom margin
                document.Add(heading);

                // Add headers
                PdfPTable table = new PdfPTable(3);
                table.AddCell("Holiday Id");
                table.AddCell("Holiday Name");
                table.AddCell("Holiday Date");

                // Add data
                foreach (var day in Holi)
                {
                    table.AddCell(day.Holiday_Id.ToString());
                    table.AddCell(day.Holiday_Name);
                    table.AddCell(day.Holiday_Date.ToString(@"yyyy-MM-dd"));
                }

                document.Add(table);
                document.Close();

                // Convert to byte array
                byte[] pdfBytes = memoryStream.ToArray();

                // Return PDF file
                return File(pdfBytes, "application/pdf", "HolidayReport.pdf");
            }
        }
    }
    public class CalendarEvent
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
    }
}