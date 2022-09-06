using MainOffice.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MainOffice.App_LocalResources;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using MainOffice.Functions;
using System.Threading;
using System.Net.Http;
using System.Drawing;
using Newtonsoft.Json;
using System.Text;

namespace MainOffice.Controllers
{
    [Authorize(Roles = "admin,director,salonadmin")]
    public class OperationDaysController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private ApplicationDbContext db1 = new ApplicationDbContext();
        
        // GET: OperationDays
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            int? employeeId = db1.Users.Single(x => x.Id == userId).EmployeeId;
            Salon salon = null;
            OperationDay operationDay = null;
            
            if (employeeId.HasValue)
            {
                OperationDayEmployee operdayEmployee = db.OperationDayEmployees.Include(v => v.OperationDay).SingleOrDefault(y => (y.EmployeeId == employeeId.Value) & (y.EndPoint == null));
                if (operdayEmployee != null)
                {
                    operationDay = db.OperationDays.Include(i => i.Salon).Include(i=> i.OperationDayEmployees.Select(x=> x.Employee.Profession)).Include(i => i.OperationDayEmployees.Select(x => x.Employee.BarberLevel)).Single(x => x.Id == operdayEmployee.OperationDay.Id);
                    operationDay.OperationDayEmployees = operationDay.OperationDayEmployees != null ? operationDay.OperationDayEmployees.OrderBy(x => x.Employee.Profession.OrderNumber).ThenBy(x => x.Employee.BarberLevel != null ? x.Employee.BarberLevel.OrderNumber : 0).ThenBy(x => x.Employee.FamilyName).ToList() : null;


                }

                if (operationDay == null)
                {
                    if (Session["GeoError"] == null)
                    {
                        if (Session["Longitude"] == null)
                        {
                            return RedirectToAction("GetGeoLocation", "Home", new { returncontroller = RouteData.Values["controller"].ToString(), returnaction = RouteData.Values["action"].ToString() });
                        }

                    }
                    else
                    {
                        ViewBag.GeoError = Session["GeoError"];
                    }

                    try
                    {
                        salon = db.Salons.Single(x => Request.UserHostAddress == "192.168.1.1" ? x.IP == "195.177.73.220" : x.IP == Request.UserHostAddress);
                    }
                    catch
                    { }
                    if (salon == null & Session["GeoError"] == null)
                    {
                        salon = FindSalon(Session["Longitude"].ToString(), Session["Latitude"].ToString());
                        if (salon == null)
                        {
                            ViewBag.GeoError = GlobalRes.FarAwayFromSalon;
                        }
                    }
                    Session["GeoError"] = null;
                    Session["Longitude"] = null;

                    if (salon != null)
                    {
                        try
                        {
                            operationDay = db.OperationDays.Single(x => x.SalonId == salon.Id);
                        }
                        catch
                        {
                            operationDay = new OperationDay() { OperationDate = DateTime.Now.Date, Salon = salon };
                        }
                    }
                    else
                    {
                        operationDay = new OperationDay();
                    }
                }
            }
            return View(operationDay);
        }

        
        private Salon FindSalon(string longitude, string latitude)
        {
            double longd;
            double latid;
            if (!double.TryParse(longitude,out longd))
            { 
                longd = double.Parse(longitude,CultureInfo.InvariantCulture);
            }
            if (!double.TryParse(latitude, out latid))
            {
                latid = double.Parse(latitude, CultureInfo.InvariantCulture);
            }
            List<Salon> salons = db.Salons.ToList();
            foreach (Salon salon in salons)
            {
                if ((salon.Longitude + 0.0018) > longd & (salon.Longitude - 0.0018) < longd)
                {
                    if ((salon.Latitude + 0.0018) > latid & (salon.Latitude - 0.0018) < latid)
                    {
                        return salon;
                    }
                }
            }
            return null;
        }
        
        [HttpPost]
        public async Task<ActionResult> ChangeOperDayStatus(string action, string message, string latitude, string longitude)
        {
            if (!String.IsNullOrEmpty(action))
            {
                string userId = User.Identity.GetUserId();
                int? employeeId = db1.Users.Single(x => x.Id == userId).EmployeeId;
                Salon salon = null;
                OperationDay operationDay = null;
                string geoLocation = String.IsNullOrEmpty(message) ? latitude + ", " + longitude : message;
                try
                {
                    salon = db.Salons.Single(x => Request.UserHostAddress == "192.168.1.1" ? x.IP == "195.177.73.220" : x.IP == Request.UserHostAddress);
                    geoLocation = salon.Latitude + ", " + salon.Longitude;
                }
                catch
                { }
                if (employeeId.HasValue)
                {
                    OperationDayEmployee operdayEmployee = db.OperationDayEmployees.Include(v => v.OperationDay).SingleOrDefault(y => (y.EmployeeId == employeeId.Value) & (y.EndPoint == null));
                    if (operdayEmployee != null)
                    {
                        if (action == "open" | action == "register")
                        {
                            return new JsonResult()
                            {
                                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                Data = new { result = "success" }
                            };
                        }
                        operationDay = db.OperationDays.Single(x => x.Id == operdayEmployee.OperationDay.Id);
                        if (operationDay.OperationDayEmployees.Where(x => x.EndPoint == null).ToList().Count == 1)
                        {
                            if (operdayEmployee.OperDayBills.Any(x => x.EndTime == null))
                            {
                                return new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                    Data = new { result = "error", message = "Error. У вас есть открытый счет." }
                                };
                            }
                            //Закрыть опер день

                            operdayEmployee.EndPoint = DateTime.Now;
                            operdayEmployee.EndEmployeeId = operdayEmployee.EmployeeId;
                            db.Entry(operdayEmployee).State = EntityState.Modified;
                            operationDay.CloseOperationPoint = DateTime.Now;
                            operationDay.CloseEmployeeId = operdayEmployee.EmployeeId;
                            operationDay.CloseGeoLocation = geoLocation;
                            db.Entry(operationDay).State = EntityState.Modified;
                            OperationDayArchive dayArchive = new OperationDayArchive(operationDay);
                            dayArchive.OperationDayEmployeeArchives = new List<OperationDayEmployeeArchive>();
                            foreach (OperationDayEmployee employee in operationDay.OperationDayEmployees)
                            {
                                OperationDayEmployeeArchive employeeArchive = new OperationDayEmployeeArchive(employee);
                                dayArchive.OperationDayEmployeeArchives.Add(employeeArchive);
                                foreach(OperDayBill operDayBill in employee.OperDayBills)
                                {
                                    Bill bill = new Bill(operDayBill, operationDay.OperationDate.Value,operationDay.SalonId,employee.EmployeeId);
                                    db.Bills.Add(bill);
                                }
                            }
                            dayArchive = db.OperationDayArchives.Add(dayArchive);
                            db.OperationDays.Remove(operationDay);
                            string[] saveResult = await Function.SaveChangesToDb(db);
                            if (saveResult[0] == "success")
                            {
                                return new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                    Data = new { result = saveResult[0] }
                                };
                            }
                            else
                            {
                                return new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                    Data = new { result = saveResult[0], message = saveResult[1] }
                                };
                            }
                            
                        }
                        else
                        {
                            return new JsonResult()
                            {
                                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                Data = new { result = "error", message = "Error. There are more then one opened worksheets." }
                            };
                        }
                    }

                    if (operationDay == null)
                    {
                        if (action == "close")
                        {
                            return new JsonResult()
                            {
                                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                Data = new { result = "success" }
                            };
                        }
                        if (salon == null)
                        {
                            if (String.IsNullOrEmpty(message))
                            {
                                salon = FindSalon(longitude, latitude);

                                if (salon == null)
                                {
                                    return new JsonResult()
                                    {
                                        JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                        Data = new { result = "error", message = GlobalRes.FarAwayFromSalon }
                                    };
                                }
                            }
                            else
                            {
                                return new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                    Data = new { result = "error", message = message }
                                };
                            }
                            
                        }
                        

                        if (salon != null)
                        {
                            try
                            {
                                operationDay = db.OperationDays.Single(x => x.SalonId == salon.Id);
                                //Зарегестрировать еще одного администратора
                                if (User.IsInRole("salonadmin"))
                                {
                                    OperationDayEmployee worker = new OperationDayEmployee() { OperationDayId = operationDay.Id, EmployeeId = employeeId.Value, StartPoint = DateTime.Now, StartEmployeeId = employeeId.Value };
                                    db.OperationDayEmployees.Add(worker);
                                    await Function.SaveChangesToDb(db);
                                    return new JsonResult()
                                    {
                                        JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                        Data = new { result = "success" }
                                    };
                                }
                            }
                            catch
                            {
                                if (action == "register")
                                {
                                    return new JsonResult()
                                    {
                                        JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                        Data = new { result = "success" }
                                    };
                                }
                                if (User.IsInRole("salonadmin"))
                                {
                                    operationDay = new OperationDay() { OperationDate = DateTime.Now.Date, SalonId = salon.Id, OpenEmployeeId = employeeId, OpenOperationPoint = DateTime.Now, OpenGeoLocation = geoLocation };
                                    operationDay = db.OperationDays.Add(operationDay);
                                    OperationDayEmployee worker = new OperationDayEmployee() { OperationDayId = operationDay.Id, EmployeeId = employeeId.Value, StartPoint = DateTime.Now, StartEmployeeId = employeeId.Value };
                                    db.OperationDayEmployees.Add(worker);
                                    string[] saveResult = await Function.SaveChangesToDb(db);
                                    if (saveResult[0] != "success")
                                    {
                                        return new JsonResult()
                                        {
                                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                            Data = new { result = saveResult[0], message = saveResult[1] }
                                        };
                                    }
                                }
                            }
                            return new JsonResult()
                            {
                                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                Data = new { result = "success" }
                            };
                        }
                        
                    }
                }
                
            }
            return null;
        }

        public ActionResult FindEmployee()
        {
            if (Session["ScreenResolution"] != null)
            {
                double screenHeight = double.Parse(Session["ScreenResolution"].ToString().Substring(Session["ScreenResolution"].ToString().IndexOf("x") + 1));
                double screenWidth = double.Parse(Session["ScreenResolution"].ToString().Substring(0, Session["ScreenResolution"].ToString().IndexOf("x")));
                double tableHeight = screenHeight - (screenHeight / 100 * 19);
                ViewBag.TableHeight = (int)tableHeight;
            }
            else
            {
                return RedirectToAction("GetResolution", "Home", new { returncontroller = RouteData.Values["controller"].ToString(), returnaction = RouteData.Values["action"].ToString() });
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddWorkSheet(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                int id;
                if (int.TryParse(message, out id))
                {
                    if (!db.OperationDayEmployees.Any(y => (y.EmployeeId == id) & (y.EndPoint == null)))
                    {
                        OperationDayEmployee operdayEmployee = await GetOPEmployeeFromUser();
                        if (operdayEmployee != null)
                        {
                            OperationDayEmployee worker = new OperationDayEmployee() { OperationDayId = operdayEmployee.OperationDay.Id, EmployeeId = id, StartPoint = DateTime.Now, StartEmployeeId = operdayEmployee.EmployeeId };
                            db.OperationDayEmployees.Add(worker);
                            string[] saveResult = await Function.SaveChangesToDb(db);
                            if (saveResult[0] != "success")
                            {
                                return new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                    Data = new { result = saveResult[0], message = saveResult[1] }
                                };
                            }
                            RegisterEmployeeViewModel employee = new RegisterEmployeeViewModel(await db.Employees.Include(e => e.BarberLevel).Include(e => e.Profession).SingleAsync(x => x.Id == id));
                            employee.Salon = operdayEmployee.OperationDay.Salon.Name + " " + operdayEmployee.OperationDay.Salon.Address;
                            return new JsonResult()
                            {
                                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                Data = new { result = "success", data = employee }
                            };
                        }
                        else
                        {
                            return new JsonResult()
                            {
                                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                Data = new { result = "error", message = "Error. You are not registered in any salon." }
                            };
                        }
                    }
                    else
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = "Error. Allready registered." }
                        };
                    }
                }
            }
            return null;
        }

        [HttpPost]
        public async Task<ActionResult> CloseWorkSheet(int? EmployeeId)
        {
            if (EmployeeId != null)
            {
                OperationDayEmployee operdayEmployee = await GetOPEmployeeFromUser();
                if (operdayEmployee != null)
                {
                    try
                    {
                        OperationDayEmployee worker = await db.OperationDayEmployees.Include(i => i.Employee.Profession).SingleAsync(x => x.Id == EmployeeId.Value & x.EndPoint == null);

                        if (operdayEmployee.OperationDayId == worker.OperationDayId)
                        {
                            if (worker.Employee.ProfessionId == 1)
                            {
                                if (await db.OperationDayEmployees.Include(i => i.Employee.Profession).CountAsync(x => x.OperationDayId == worker.OperationDayId & x.Employee.ProfessionId == 1 & x.EndPoint != null) <= 1)
                                {
                                    return new JsonResult()
                                    {
                                        JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                        Data = new { result = "error", message = "Error. Последний из администраторов." }
                                    };
                                }
                            }
                            if (worker.OperDayBills.Any(x => x.EndTime == null))
                            {
                                return new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                    Data = new { result = "error", message = "Error. У сотрудника есть открытый счет." }
                                };
                            }
                            worker.EndPoint = DateTime.Now;
                            worker.EndEmployeeId = operdayEmployee.EmployeeId;
                            db.Entry(worker).State = EntityState.Modified;
                            string[] saveResult = await Function.SaveChangesToDb(db);
                            if (saveResult[0] == "success")
                            {
                                return Json(new { result = "success", data = worker.EndPoint.Value.ToString(), id = worker.Id }, JsonRequestBehavior.DenyGet);
                            }
                            else
                            {
                                return new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                    Data = new { result = "error", message = saveResult[1] }
                                };
                            }
                        }
                    }
                    catch
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = "Error. Insufficient privileges." }
                        };
                    }
                    
                }
                
            }
            return RedirectToAction("Index");
        }

        public async Task<JsonResult> FindEmployeeData(string search, string sort, string order, int? offset, int? limit)
        {
            if (!String.IsNullOrEmpty(search))
            {
                var employees = db.Employees.Include(e => e.BarberLevel).Include(e => e.Profession);
                int TotalNotFiltered = employees.Count();
                int Total = TotalNotFiltered;

                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

                employees = employees.Where(e => ( e.BarberLevel.Name.Contains(search) || e.Profession.Name.Contains(search) || e.FamilyName.ToString().Contains(search) || e.Name.ToString().Contains(search) || e.StaffNumber.ToString().Contains(search)) 
                & e.StaffNumber != null & e.DismissalDate == null);
                Total = employees.Count();

                if (sort != null)
                {
                    employees = Function.OrderBy(employees, sort, order);
                }
                else
                {
                    employees = employees.OrderBy(e => e.StaffNumber);
                }
                employees = employees.Skip(offset.Value);
                List<OperationDayEmployee> registered = await db.OperationDayEmployees.Include(i => i.OperationDay.Salon).Where(x => x.EndPoint == null).ToListAsync();
                List<RegisterEmployeeViewModel> employeeList = new List<RegisterEmployeeViewModel>();
                foreach (Employee employee in await employees.Take(limit.Value).ToListAsync())
                {
                    employeeList.Add(new RegisterEmployeeViewModel(employee));
                    try
                    {
                        OperationDay operationDay = registered.Single(x => x.EmployeeId == employee.Id).OperationDay;
                        employeeList.Last().Salon = operationDay.Salon.Name + " " + operationDay.Salon.Address;
                    }
                    catch
                    { }
                }
                BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(employeeList, Total, TotalNotFiltered);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public async Task<ActionResult> GeneratePin(int Id)
        {
            try
            {
                OperationDayEmployee employee = await db.OperationDayEmployees.SingleAsync(x => x.Id == Id);
                if (!employee.pin.HasValue)
                {
                    Random random = new Random();
                    employee.pin = random.Next(1000, 9999);
                    while (await db.OperationDayEmployees.AnyAsync(x => x.pin == employee.pin))
                    {
                        employee.pin = random.Next(1000, 9999);
                    }
                    db.Entry(employee).State = EntityState.Modified;
                    await Function.SaveChangesToDb(db);
                }
            }
            catch
            { }
            return RedirectToAction("Index");
        }
        public ActionResult Status()
        {
            return View();
        }
        public async Task<ActionResult> StatusData()
        {
            OperationDayEmployee operDayEmployee = await GetOPEmployeeFromUser();
            if (operDayEmployee != null)
            {
                OperDayStatisticsViewModel viewModel = null;
                try
                {
                    OperationDay operationDay = await db.OperationDays.Include(i => i.Salon).Include(i => i.OpenEmployee).Include(i => i.OperationDayEmployees.Select(s => s.CloseEmployee)).Include(i => i.OperationDayEmployees.Select(s => s.StartEmployee)).Include(i => i.OperationDayEmployees.Select(s => s.Employee.BarberLevel)).Include(i => i.OperationDayEmployees.Select(s => s.Employee.Profession)).SingleAsync(x => x.Id == operDayEmployee.OperationDayId);
                    viewModel = new OperDayStatisticsViewModel(operationDay, true, false);
                }
                catch (Exception e)
                {
                    ViewBag.errorMessage = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
                    return PartialView("Error");
                }

                return PartialView("_StatusData", viewModel);
            }
            return null;
        }
        
        [HttpPost]
        public async Task<ActionResult> Sign(int id, int card, byte[] version)
        {
            OperationDayEmployee operDayEmployee = await GetOPEmployeeFromUser();
            if (operDayEmployee != null)
            {
                OperationDayEmployee worker = await db.OperationDayEmployees.Where(x => x.OperDayBills.Any(y => y.Id == id & y.RowVersion == version)).SingleOrDefaultAsync();
                if (worker != null)
                {
                    if (operDayEmployee.OperationDayId == worker.OperationDayId)
                    {
                        OperDayBill bill = worker.OperDayBills.SingleOrDefault(x => x.Id == id);
                        if (bill != null & bill.BillLines != null && bill.BillLines.Count > 0)
                        {
                            if (!bill.VisaPromoId.HasValue)
                            {
                                if (bill.BillLines.Any(x=>!x.Cancel & x.Promotion != bill.InitialPromo))
                                {
                                    bill.VisaPromoId = operDayEmployee.EmployeeId;
                                    db.Entry(bill).State = EntityState.Modified;
                                }
                            }
                            List<OperDayBillLine> billLines = bill.BillLines.Where(y => y.Cancel & !y.AdminVisaId.HasValue).ToList();
                            foreach (OperDayBillLine line in billLines)
                            {
                                line.AdminVisaId = operDayEmployee.EmployeeId;
                                db.Entry(line).State = EntityState.Modified;
                            }
                            string[] saveResult = await Function.SaveChangesToDb(db);
                            if (saveResult[0] == "success")
                            {
                                return Json(new { result = "success", id, card }, JsonRequestBehavior.DenyGet);
                            }
                            else
                            {
                                return new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                    Data = new { result = "error", message = saveResult[1] }
                                };
                            }
                        }
                    }
                }
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error", message = "Error. Insufficient privileges." }
            };
        }
        [HttpPost]
        public async Task<ActionResult> Bills(int id)
        {
            OperationDayEmployee operDayEmployee = await GetOPEmployeeFromUser();
            if (operDayEmployee != null)
            {
                OperationDayEmployee worker = await db.OperationDayEmployees.Include(i=>i.Employee).SingleOrDefaultAsync(x => x.Id == id);
                if (worker != null)
                {
                    
                    if (operDayEmployee.OperationDayId == worker.OperationDayId)
                    {
                        Session["Worker"] = worker.Id;
                        ViewBag.Name = "№" + worker.Employee.StaffNumber + " " + worker.Employee.FamilyName + " " + worker.Employee.Name;
                        return View(new List<MyBillsViewModel>());
                    }
                }
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error", message = "Error. Insufficient privileges." }
            };
        }

        public async Task<ActionResult> BillsData()
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromSession();
            if (opEmployee != null)
            {
                List<MyBillsViewModel> myBills = new List<MyBillsViewModel>();
                foreach (OperDayBill bill in opEmployee.OperDayBills)
                {
                    if (opEmployee.OperationDay.Alarm & bill.IsHidden())
                        continue;
                    MyBillsViewModel myBill = new MyBillsViewModel(bill);
                    myBills.Add(myBill);
                }
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { result = "success", myBills }
                };
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result = "error", message = "You are not registered" }
            };
        }

        private async Task<OperationDayEmployee> GetOPEmployeeFromUser()
        {
            string userId = User.Identity.GetUserId();
            int? employeeId = db1.Users.SingleOrDefault(x => x.Id == userId).EmployeeId;

            if (employeeId.HasValue)
            {
                return await db.OperationDayEmployees.Include(v => v.OperationDay.Salon).SingleOrDefaultAsync(y => (y.EmployeeId == employeeId.Value) & (y.EndPoint == null));
            }
            return null;
        }
        private async Task<OperationDayEmployee> GetOPEmployeeFromSession()
        {
            int Id = (int)Session["Worker"];
            return await db.OperationDayEmployees.Include(i=>i.OperationDay).Include(i => i.OperDayBills.Select(s=>s.WhoLocked)).Include(i => i.Employee.BarberLevel).Include(i => i.Employee.Profession).SingleOrDefaultAsync(x => x.Id == Id);
        }
        
        
        public async Task<ActionResult> BillsInfo(int? Id)
        {
            if (Id != null & Id != 0)
            {
                OperationDayEmployee opEmployee = await GetOPEmployeeFromSession();
                if (opEmployee != null)
                {
                    return PartialView(opEmployee.OperDayBills.Single(x => x.Id == Id));
                }
            }
            return null;
        }

        public async Task<ActionResult> BillsCreate()
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromSession();
            Random random = new Random();

            OperDayBill bill = new OperDayBill() { BillNumber = random.Next(10000, 99999) };
            bill.OperationDayEmployee = opEmployee;
            return PartialView(bill);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BillsCreate(int BillNumber, CodeQuantity[] list, string discount, bool BO, int? Client)
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromSession();
            if (opEmployee != null)
            {
                if (discount == "staff")
                    discount = "Сотрудник";
                OperDayBill bill = new OperDayBill();
                bill.BillNumber = BillNumber;
                if (list != null)
                {
                    bill.BillLines = new List<OperDayBillLine>();
                    foreach (CodeQuantity code in list)
                    {
                        CashRegCode regCode = await db.CashRegCodes.Include(c => c.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.Service.ServiceVolume).SingleAsync(x => x.Code == code.Code);
                        OperDayBillLine line = new OperDayBillLine() { CashRegCode = regCode.Code };
                        line.ProductOrServiceName = regCode.GetCodeName();
                        line.MaxPrice = regCode.Price;
                        line.SellPrice = regCode.GetPriceAccordingToDiscount(discount);
                        line.InsertDateTime = DateTime.Now;
                        line.Promotion = discount;
                        line.Quantity = code.Quantity;
                        bill.BillLines.Add(line);
                    }
                }
                bill.ClientId = Client;
                bill.BO = BO;
                bill.InitialPromo = discount;
                bill.OperationDayEmployeeId = opEmployee.Id;
                bill.StartTime = DateTime.Now;
                db.OperDayBills.Add(bill);
                string[] saveResult = await Function.SaveChangesToDb(db);
                if (saveResult[0] == "success")
                {

                    return Json(new { result = "success", bill = new MyBillsViewModel(bill) }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                        Data = new { result = "error", message = saveResult[1] }
                    };
                }
            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error", message = "" }
            };
        }
        [HttpPost]
        public async Task<ActionResult> GetLinePrice(int codeId, string discount)
        {
            if (discount == "staff")
                discount = "Сотрудник";
            try
            {
                CashRegCode regCode = await db.CashRegCodes.Include(c => c.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.Service.ServiceVolume).SingleAsync(x => x.Id == codeId);
                OperDayBillLine line = new OperDayBillLine() { CashRegCode = regCode.Code };
                line.ProductOrServiceName = regCode.GetCodeName();
                line.SellPrice = regCode.GetPriceAccordingToDiscount(discount);
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = "success", data = line }
                };
            }
            catch { }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error", message = "" }
            };
        }
        [HttpPost]
        public async Task<ActionResult> GetBillPrices(OperDayBill bill, string discount)
        {
            if (discount == "staff")
                discount = "Сотрудник";
            for (int i = 0; i < bill.BillLines.Count; i++)
            {
                try
                {
                    int code = bill.BillLines[i].CashRegCode;
                    CashRegCode regCode = await db.CashRegCodes.SingleAsync(x => x.Code == code);
                    bill.BillLines[i].SellPrice = regCode.GetPriceAccordingToDiscount(discount);

                }
                catch (Exception e)
                {
                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                        Data = new { result = "error", message = e.Message }
                    };
                }
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "success", bill }
            };

        }

        public async Task<ActionResult> BillsEdit(int? id)
        {
            OperationDayEmployee worker = await GetOPEmployeeFromSession();
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            if (worker != null & id.HasValue)
            {
                if (opEmployee.OperationDayId == worker.OperationDayId)
                {
                    try
                    {
                        OperDayBill bill = await db.OperDayBills.Include(i => i.WhoLocked).SingleAsync(x => x.Id == id.Value & x.OperationDayEmployeeId == worker.Id);
                        if (bill.Locked)
                        {
                            if (bill.WhoLockedId != opEmployee.EmployeeId)

                                return PartialView("_UnlockEdit",new UnlockEditViewModel() {WhoLocked = bill.WhoLocked.FamilyName + " " + bill.WhoLocked.Name.Substring(0,1) + "." + bill.WhoLocked.FathersName.Substring(0,1) + ".", MayUnlock = User.IsInRole("admin") | User.IsInRole("director") | User.IsInRole("salonadmin") ? true : false, Id = bill.Id, Version = Convert.ToBase64String(bill.RowVersion) });
                        }
                        bill.Locked = true;
                        bill.WhoLockedId = opEmployee.EmployeeId;
                        db.Entry(bill).State = EntityState.Modified;
                        string[] result = await Function.SaveChangesToDb(db);
                        if (result[0] == "success")
                            return PartialView(bill);
                    }
                    catch
                    { }
                }
            }
            return null;
        }
        public async Task<ActionResult> BillsEditData(int? id)
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromSession();
            if (opEmployee != null & id.HasValue)
            {
                try
                {
                    MyBillsViewModel bill = new MyBillsViewModel(await db.OperDayBills.Include(i => i.WhoLocked).SingleAsync(x => x.Id == id.Value & x.OperationDayEmployeeId == opEmployee.Id));

                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new { result = "success", bill }
                    };
                }
                catch
                { }

            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result = "error", message = "You are not registered" }
            };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BillsEdit(int Id, List<CodeQuantity> list, string discount, bool BO, int? Client, int? pay, byte[] version)
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            OperationDayEmployee worker = await GetOPEmployeeFromSession();
            if (worker != null)
            {
                if (discount == "staff")
                    discount = "Сотрудник";
                if (opEmployee.OperationDayId == worker.OperationDayId)
                {
                    if (opEmployee.OperationDay.Alarm & pay.HasValue && pay.Value == 3)
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = "Недопустимый метод оплаты" }
                        };
                    }
                    try
                    {
                        OperDayBill bill = worker.OperDayBills.Single(x => x.Id == Id & x.RowVersion.SequenceEqual(version));
                        if (bill.WhoLockedId == opEmployee.EmployeeId)
                        {
                            bill.BO = BO;
                            bill.ClientId = Client;
                            bill.InitialPromo = discount;
                            bill.Locked = false;
                            bill.WhoLockedId = null;
                            if (bill.EndTime.HasValue)
                            {
                                bill.PStatusId = pay;
                                bill.PayVisaId = opEmployee.EmployeeId;
                            }
                            db.Entry(bill).State = EntityState.Modified;
                            if (bill.BillLines != null)
                            {
                                foreach (OperDayBillLine line in bill.BillLines.ToList())
                                {
                                    CodeQuantity editData;
                                    try
                                    {
                                        editData = list.Single(x => x.Id == line.Id);
                                    }
                                    catch
                                    {
                                        line.Cancel = true;
                                        line.CancelDateTime = DateTime.Now;
                                        line.AdminVisaId = opEmployee.EmployeeId;
                                        db.Entry(line).State = EntityState.Modified;
                                        continue;
                                    }

                                    if (editData.Quantity != line.Quantity | line.Promotion != discount)
                                    {
                                        line.Cancel = true;
                                        line.CancelDateTime = DateTime.Now;
                                        line.AdminVisaId = opEmployee.EmployeeId;
                                        db.Entry(line).State = EntityState.Modified;

                                        CashRegCode regCode = await db.CashRegCodes.Include(c => c.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.Service.ServiceVolume).SingleAsync(x => x.Code == line.CashRegCode);
                                        OperDayBillLine line1 = new OperDayBillLine() { CashRegCode = regCode.Code };
                                        line1.ProductOrServiceName = regCode.GetCodeName();
                                        line1.MaxPrice = regCode.Price;
                                        line1.SellPrice = regCode.GetPriceAccordingToDiscount(discount);
                                        line1.InsertDateTime = DateTime.Now;
                                        line1.Promotion = discount;
                                        line1.Quantity = editData.Quantity;
                                        line1.OperDayBillId = line.OperDayBillId;
                                        db.OperDayBillLines.Add(line1);
                                    }
                                }
                            }
                            if (list != null)
                            {
                                foreach (CodeQuantity code in list)
                                {
                                    if (code.Id == 0)
                                    {
                                        CashRegCode regCode = await db.CashRegCodes.Include(c => c.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.Service.ServiceVolume).SingleAsync(x => x.Code == code.Code);
                                        OperDayBillLine line = new OperDayBillLine() { CashRegCode = regCode.Code };
                                        line.ProductOrServiceName = regCode.GetCodeName();
                                        line.MaxPrice = regCode.Price;
                                        line.SellPrice = regCode.GetPriceAccordingToDiscount(discount);
                                        line.InsertDateTime = DateTime.Now;
                                        line.Promotion = discount;
                                        line.Quantity = code.Quantity;
                                        line.OperDayBillId = bill.Id;
                                        db.OperDayBillLines.Add(line);
                                    }
                                }
                            }

                            string[] saveResult = await Function.SaveChangesToDb(db);
                            bill = await db.OperDayBills.Include(i => i.WhoLocked).SingleAsync(x => x.Id == bill.Id);
                            if (saveResult[0] == "success")
                            {

                                return Json(new { result = "success", bill = new MyBillsViewModel(bill) }, JsonRequestBehavior.DenyGet);
                            }
                            else
                            {
                                return new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                    Data = new { result = "error", message = saveResult[1] }
                                };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = e.Message }
                        };
                    }
                }

            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error", message = "" }
            };

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FinishEdit(int Id, byte[] version)
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            OperationDayEmployee worker = await GetOPEmployeeFromSession();
            if (worker != null)
            {
                try
                {
                    OperDayBill bill = worker.OperDayBills.Single(x => x.Id == Id & x.RowVersion.SequenceEqual(version));
                    if (bill.Locked && bill.WhoLockedId == opEmployee.EmployeeId)
                    {
                        bill.Locked = false;
                        bill.WhoLockedId = null;
                        db.Entry(bill).State = EntityState.Modified;
                        await Function.SaveChangesToDb(db);
                    }
                }
                catch (Exception e)
                {
                    
                }
            }
            return null;
        }
        [HttpPost]
        public async Task<ActionResult> UnlockEdit(int? Id, byte[] version)
        {
            OperationDayEmployee worker = await GetOPEmployeeFromSession();
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            if (worker != null & Id.HasValue)
            {
                if (opEmployee.OperationDayId == worker.OperationDayId)
                {
                    try
                    {
                        OperDayBill bill = worker.OperDayBills.Single(x => x.Id == Id.Value & x.RowVersion.SequenceEqual(version));
                        bill.Locked = false;
                        bill.WhoLockedId = null;
                        db.Entry(bill).State = EntityState.Modified;
                        await Function.SaveChangesToDb(db);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CloseBill(int Id)
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            OperationDayEmployee worker = await GetOPEmployeeFromSession();
            
            if (worker != null)
            {
                try
                {
                    OperDayBill bill = worker.OperDayBills.Single(x => x.Id == Id & x.EndTime == null);
                    
                    if (bill.Locked && bill.WhoLockedId != opEmployee.EmployeeId)
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = "Счет №" + bill.BillNumber + " сейчас редактирует " + bill.WhoLocked.FamilyName + " " + bill.WhoLocked.Name.Substring(0, 1) + "." + bill.WhoLocked.FathersName.Substring(0, 1) + "." }
                        };
                    }
                    if (bill.RequireAdminVisa())
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = "Счет №" + bill.BillNumber + " требует подписи администратора." }
                        };
                    }
                    bill.Locked = false;
                    bill.WhoLockedId = null;
                    bill.EndTime = DateTime.Now;
                    db.Entry(bill).State = EntityState.Modified;
                    OperDayBillPrint print = new OperDayBillPrint() { Id = bill.Id, Count = 0 };
                    db.OperDayBillPrints.Add(print);
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    
                    if (saveResult[0] == "success")
                    {
                        if (bill.BillLines != null && bill.BillLines.Count > 0 & bill.BillLines.Any(x=>x.Cancel == false))
                            await PrintBill(bill);
                        
                        try
                        {
                            bill = await db.OperDayBills.Include(i => i.PrintOperDayBill).SingleAsync(x => x.Id == bill.Id);
                        }
                        catch(Exception e)
                        {
                            return new JsonResult()
                            {
                                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                Data = new { result = "error", message = "1 " + e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message }
                            };
                        }
                        
                        return Json(new { result = "success", bill = new MyBillsViewModel(bill) }, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = saveResult[1] }
                        };
                    }
                }
                catch (Exception e)
                {
                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                        Data = new { result = "error", message = "2 " + e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message }
                    };
                }

            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error", message = "You are not registered" }
            };

        }
       

        public class CodeQuantity
        {
            public int Id { get; set; }
            public int Code { get; set; }
            public int Quantity { get; set; }
        }
        private async Task<string> PrintBill(OperDayBill bill)
        {
            string responseString = null;
            using (HttpClient httpClient = new HttpClient() { })
            {
                httpClient.Timeout = new TimeSpan(0, 0, 10);
                if (!bill.OperationDayEmployee.OperationDay.Salon.SalonPrinters.Any())
                {
                    return "В базе нет записей с описанием принтера в данном салоне";
                }
                PrintBillContent printBill = new PrintBillContent() { action = "print", bill = Convert.ToBase64String((byte[])Drawing.DrawBill(bill)), printerName = bill.OperationDayEmployee.OperationDay.Salon.SalonPrinters.First().SystemPrinterName };
                try
                {
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(printBill), Encoding.UTF8, "application/json");
                    try
                    {
                        var response = await httpClient.PostAsync("http://" + bill.OperationDayEmployee.OperationDay.Salon.IP + ":1522", content);

                        responseString = await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception e)
                    {
                        responseString = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
                    }
                }
                catch (Exception e)
                {
                    responseString = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
                }
            }
            if (responseString == "OK")
            {
                OperDayBillPrint operDayBillPrint = db.OperDayBillPrints.Single(x => x.Id == bill.Id);
                operDayBillPrint.Count = operDayBillPrint.Count + 1;
                db.Entry(operDayBillPrint).State = EntityState.Modified;
                await Function.SaveChangesToDb(db);
            }
            return responseString;
        }
        public async Task<ActionResult> AdditionalPrint(int id, bool auto)
        {
            OperationDayEmployee worker = await GetOPEmployeeFromSession();
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            string message = "worker null";
            if (worker != null)
            {
                message = "operday != operday";
                if (opEmployee.OperationDayId == worker.OperationDayId)
                {
                    try
                    {
                        OperDayBill bill = await db.OperDayBills.Include(i => i.WhoLocked).SingleAsync(x => x.Id == id & x.OperationDayEmployeeId == worker.Id);
                        if (auto)
                        {
                            if (bill.BillLines != null && bill.BillLines.Count > 0 & bill.BillLines.Any(x => x.Cancel == false))
                            {
                                message = await PrintBill(bill);
                                if (message == "OK")
                                {
                                    return new JsonResult()
                                    {
                                        JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                        Data = new { result = "success", message = "Счет напечатан" }
                                    };
                                }
                                else
                                {
                                    return new JsonResult()
                                    {
                                        JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                        Data = new { result = "error", message = "Печать не удалась " + message }
                                    };
                                }
                            }
                            else
                            {
                                return new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                    Data = new { result = "error", message = "Печать не удалась. В счете нет позиций" }
                                };
                            }
                        }
                        else
                        {
                            bill.PrintOperDayBill.Count = bill.PrintOperDayBill.Count + 1;
                            db.Entry(bill.PrintOperDayBill).State = EntityState.Modified;
                            await Function.SaveChangesToDb(db);
                            ViewBag.Image = Convert.ToBase64String((byte[])Drawing.DrawBill(bill));
                            return PartialView("_BillPicture");
                        }

                    }
                    catch (Exception e)
                    {
                        message = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message + " worker " + worker.Id;
                    }
                }
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error", message = "Печать не удалась " + message }
            };
        }
        public async Task<ActionResult> GetDaySumm()
        {
            OperationDayEmployee operDayEmployee = await GetOPEmployeeFromUser();
            if (operDayEmployee != null)
            {
                OperDayStatisticsViewModel viewModel = null;
                try
                {
                    OperationDay operationDay = await db.OperationDays.Include(i => i.Salon).Include(i => i.OpenEmployee).Include(i => i.OperationDayEmployees.Select(s => s.CloseEmployee)).Include(i => i.OperationDayEmployees.Select(s => s.StartEmployee)).Include(i => i.OperationDayEmployees.Select(s => s.Employee.BarberLevel)).Include(i => i.OperationDayEmployees.Select(s => s.Employee.Profession)).SingleAsync(x => x.Id == operDayEmployee.OperationDayId);
                    operationDay.Alarm = true;
                    db.Entry(operationDay).State = EntityState.Modified;
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    if (saveResult[0] == "success")
                    {
                        viewModel = new OperDayStatisticsViewModel(operationDay, false, false);
                        return View(viewModel);
                    }
                    else
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = saveResult[1] }
                        };
                    }
                    
                }
                catch (Exception e)
                {
                    ViewBag.errorMessage = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
                    return PartialView("Error");
                }
            }
            return null;
        }

        [HttpPost]
        public async Task<ActionResult> Pay(int id, int card, byte[] version, int param)
        {
            OperationDayEmployee operDayEmployee = await GetOPEmployeeFromUser();
            if (operDayEmployee != null)
            {
                OperationDayEmployee worker = await db.OperationDayEmployees.Where(x => x.OperDayBills.Any(y => y.Id == id & y.RowVersion == version)).SingleOrDefaultAsync();
                if (worker != null)
                {
                    if (operDayEmployee.OperationDayId == worker.OperationDayId)
                    {
                        if (operDayEmployee.OperationDay.Alarm & param == 3)
                        {
                            return new JsonResult()
                            {
                                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                Data = new { result = "error", message = "Недопустимый метод оплаты" }
                            };
                        }
                        OperDayBill bill = worker.OperDayBills.SingleOrDefault(x => x.Id == id);
                        if (bill != null & bill.RequirePayment())
                        {
                            bill.PStatusId = param;
                            bill.PayVisaId = operDayEmployee.EmployeeId;
                            db.Entry(bill).State = EntityState.Modified;
                            string[] saveResult = await Function.SaveChangesToDb(db);
                            if (saveResult[0] == "success")
                            {
                                List<OperDayBill> bills = await db.OperDayBills.Where(x => x.OperationDayEmployeeId == worker.Id).ToListAsync();
                                int count = bills.Count(x => x.RequirePayment());
                                OperationDay operationDay = await db.OperationDays.Include(i => i.Salon).Include(i => i.OpenEmployee).Include(i => i.OperationDayEmployees.Select(s => s.CloseEmployee)).Include(i => i.OperationDayEmployees.Select(s => s.StartEmployee)).Include(i => i.OperationDayEmployees.Select(s => s.Employee.BarberLevel)).Include(i => i.OperationDayEmployees.Select(s => s.Employee.Profession)).SingleAsync(x => x.Id == operDayEmployee.OperationDayId);
                                OperDayStatisticsViewModel viewModel = new OperDayStatisticsViewModel(operationDay, false, false);
                                
                                return Json(new { result = "success", id, card, count, kasa = viewModel.KasaSumm, terminal = viewModel.TerminalSumm, deposit = viewModel.DepositSumm, closed = viewModel.SummClosed, unpaid = viewModel.UnpaidSumm }, JsonRequestBehavior.DenyGet);
                            }
                            else
                            {
                                return new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                                    Data = new { result = "error", message = saveResult[1] }
                                };
                            }
                        }
                    }
                }
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error", message = "Error. Insufficient privileges." }
            };
        }

    }
}
