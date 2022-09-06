using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using MainOffice.Functions;
using MainOffice.App_LocalResources;
using MainOffice.Models;

namespace MainOffice.Controllers
{

    [Authorize(Roles = "admin,director")]
    public class EmployeesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Employees

        public ActionResult Index(int? Edit)
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
            var employees = db.Employees.Include(e => e.BarberLevel).Include(e => e.Company).Include(e => e.Profession).Include(e => e.Salon).Include(e => e.DelayedUpdateEmployees);
            ViewBag.ServerSide = false;

            if (employees.Count() > 400)
                ViewBag.ServerSide = true;

            EmployeeFilter filter = new EmployeeFilter();

            List<BarberLevel> BarberLevelList = db.BarberLevels.ToList();
            BarberLevelList.Insert(0, new BarberLevel() { Name = GlobalRes.Empty });
            ViewBag.BarberLevelsSelectList = new MultiSelectList(BarberLevelList, "Id", "Name");

            List<Company> CompanyList = db.Companies.ToList();
            CompanyList.Insert(0, new Company() { Name = GlobalRes.Empty });
            ViewBag.CompaniesSelectList = new MultiSelectList(CompanyList, "Id", "Name");

            List<Profession> ProfessionList = db.Professions.ToList();
            ProfessionList.Insert(0, new Profession() { Name = GlobalRes.Empty });
            ViewBag.ProfessionsSelectList = new MultiSelectList(ProfessionList, "Id", "Name");

            List<Salon> SalonList = db.Salons.ToList();
            SalonList.Insert(0, new Salon() { Name = GlobalRes.Empty });
            ViewBag.SalonsSelectList = new MultiSelectList(SalonList, "Id", "Name");

            ViewBag.Filter = filter;

            ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
            
            return View(new List<Employee>() { });
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            List<BarberLevel> BarberLevelList = db.BarberLevels.ToList();
            BarberLevelList.Insert(0, new BarberLevel() { Name = GlobalRes.Empty });

            ViewBag.BarberLevelId = new SelectList(BarberLevelList, "Id", "Name");
            List<Company> CompanyList = db.Companies.ToList();
            CompanyList.Insert(0, new Company() { Name = GlobalRes.Empty });

            ViewBag.CompanyId = new SelectList(CompanyList, "Id", "Name");
            List<Profession> ProfessionList = db.Professions.ToList();
            ProfessionList.Insert(0, new Profession() { Name = GlobalRes.Empty });

            ViewBag.ProfessionId = new SelectList(ProfessionList, "Id", "Name");
            List<Salon> SalonList = db.Salons.ToList();
            SalonList.Insert(0, new Salon() { Name = GlobalRes.Empty });

            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            ViewBag.PriceListUnits = db.PriceListUnits.OrderBy(e => e.Name).ToList();
            return PartialView();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FamilyName,Name,FathersName,BirthDay,OldFamilyName,ProfessionId,BarberLevelId,SalonId,HireDate,StaffNumber,PhoneNumber1,PhoneNumber2,CompanyId,RegisterDate,DismissalDate")] Employee employee, string[] PriceListUnits)
        {
            if (ModelState.IsValid)
            {
                if (await DelayedUpdateIsValid(employee))
                {
                    employee.BarberLevelId = SetValueToNull(employee.BarberLevelId);
                    employee.CompanyId = SetValueToNull(employee.CompanyId);
                    employee.ProfessionId = SetValueToNull(employee.ProfessionId);
                    employee.SalonId = SetValueToNull(employee.SalonId);
                    db.Employees.Add(employee);
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    if (saveResult[0] == "success")
                    {
                        Employee modifiedEmployee = await db.Employees.Include(x => x.PriceListUnits).Where(x => x.Id == employee.Id).SingleAsync();
                        UpdateEmployeePriceListUnits(PriceListUnits, modifiedEmployee);
                        await Function.SaveChangesToDb(db);
                        employee = await db.Employees.Include(e => e.BarberLevel).Include(e => e.Company).Include(e => e.Profession).Include(e => e.Salon).Include(e => e.DelayedUpdateEmployees).Include(e => e.DelayedUpdateEmployees.Select(x => x.BarberLevel)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Company)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Profession)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Salon)).FirstAsync(e => e.Id == employee.Id);
                        return Json(new { result = "success", data = GetJsonViewModel(employee) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_EmployeeUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            ModelState.AddModelError("FamilyName", GlobalRes.Duplicate);
                            ModelState.AddModelError("Name", GlobalRes.Duplicate);
                            ModelState.AddModelError("FathersName", GlobalRes.Duplicate);
                            ModelState.AddModelError("BirthDay", GlobalRes.Duplicate);
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError(String.Empty, GlobalRes.ErrDelayedUpdateExists);
                    ModelState.AddModelError("FamilyName", GlobalRes.DelayedUpdate);
                    ModelState.AddModelError("Name", GlobalRes.DelayedUpdate);
                    ModelState.AddModelError("FathersName", GlobalRes.DelayedUpdate);
                    ModelState.AddModelError("BirthDay", GlobalRes.DelayedUpdate);

                }
            }
            List<PriceListUnit> unitList = new List<PriceListUnit>();
            if (PriceListUnits != null)
            {
                foreach (string id in PriceListUnits)
                {
                    if (id != "false")
                    {
                        PriceListUnit unit = new PriceListUnit() { Id = int.Parse(id) };
                        unitList.Add(unit);
                    }
                }
            }
            employee.PriceListUnits = unitList;
            List<BarberLevel> BarberLevelList = db.BarberLevels.ToList();
            BarberLevelList.Insert(0, new BarberLevel() { Name = GlobalRes.Empty });

            ViewBag.BarberLevelId = new SelectList(BarberLevelList, "Id", "Name");
            List<Company> CompanyList = db.Companies.ToList();
            CompanyList.Insert(0, new Company() { Name = GlobalRes.Empty });

            ViewBag.CompanyId = new SelectList(CompanyList, "Id", "Name");
            List<Profession> ProfessionList = db.Professions.ToList();
            ProfessionList.Insert(0, new Profession() { Name = GlobalRes.Empty });

            ViewBag.ProfessionId = new SelectList(ProfessionList, "Id", "Name");
            List<Salon> SalonList = db.Salons.ToList();
            SalonList.Insert(0, new Salon() { Name = GlobalRes.Empty });

            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            ViewBag.PriceListUnits = db.PriceListUnits.OrderBy(e => e.Name).ToList();
            return PartialView(employee);
        }

        // GET: Employees/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.EmployeeShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
            ViewBag.Concurrency = false;
            List<BarberLevel> BarberLevelList = db.BarberLevels.ToList();
            BarberLevelList.Insert(0, new BarberLevel() { Name = GlobalRes.Empty });

            ViewBag.BarberLevelId = new SelectList(BarberLevelList, "Id", "Name");
            List<Company> CompanyList = db.Companies.ToList();
            CompanyList.Insert(0, new Company() { Name = GlobalRes.Empty });

            ViewBag.CompanyId = new SelectList(CompanyList, "Id", "Name");
            List<Profession> ProfessionList = db.Professions.ToList();
            ProfessionList.Insert(0, new Profession() { Name = GlobalRes.Empty });

            ViewBag.ProfessionId = new SelectList(ProfessionList, "Id", "Name");
            List<Salon> SalonList = db.Salons.ToList();
            SalonList.Insert(0, new Salon() { Name = GlobalRes.Empty });

            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            ViewBag.PriceListUnits = db.PriceListUnits.OrderBy(e => e.Name).ToList();
            
            return PartialView(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FamilyName,Name,FathersName,BirthDay,OldFamilyName,ProfessionId,BarberLevelId,SalonId,HireDate,StaffNumber,PhoneNumber1,PhoneNumber2,CompanyId,RegisterDate,DismissalDate,RowVersion")] Employee employee, string[] PriceListUnits)
        {
            ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
                if (await DelayedUpdateIsValid(employee))
                {
                    employee.BarberLevelId = SetValueToNull(employee.BarberLevelId);
                    employee.CompanyId = SetValueToNull(employee.CompanyId);
                    employee.ProfessionId = SetValueToNull(employee.ProfessionId);
                    employee.SalonId = SetValueToNull(employee.SalonId);
                    db.Entry(employee).State = EntityState.Modified;
                    Employee modifiedEmployee = await db.Employees.Include(x => x.PriceListUnits).Where(x => x.Id == employee.Id).SingleOrDefaultAsync();
                    if (modifiedEmployee != null)
                        UpdateEmployeePriceListUnits(PriceListUnits, modifiedEmployee);
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    if (saveResult[0] == "success")
                    {
                        employee = await db.Employees.Include(e => e.BarberLevel).Include(e => e.Company).Include(e => e.Profession).Include(e => e.Salon).Include(e => e.DelayedUpdateEmployees).Include(e => e.DelayedUpdateEmployees.Select(x => x.BarberLevel)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Company)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Profession)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Salon)).FirstAsync(e => e.Id == employee.Id);
                        return Json(new { result = "success", data = GetJsonViewModel(employee) }, JsonRequestBehavior.AllowGet);
                    }
                    else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_EmployeeUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            ModelState.AddModelError("FamilyName", GlobalRes.Duplicate);
                            ModelState.AddModelError("Name", GlobalRes.Duplicate);
                            ModelState.AddModelError("FathersName", GlobalRes.Duplicate);
                            ModelState.AddModelError("BirthDay", GlobalRes.Duplicate);
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError(String.Empty, GlobalRes.ErrDelayedUpdateExists);
                    ModelState.AddModelError("FamilyName", GlobalRes.DelayedUpdate);
                    ModelState.AddModelError("Name", GlobalRes.DelayedUpdate);
                    ModelState.AddModelError("FathersName", GlobalRes.DelayedUpdate);
                    ModelState.AddModelError("BirthDay", GlobalRes.DelayedUpdate);

                }

            }
            
            List<PriceListUnit> unitList = new List<PriceListUnit>();
            if (PriceListUnits != null)
            {
                foreach (string id in PriceListUnits)
                {
                    if (id != "false")
                    {
                        PriceListUnit unit = new PriceListUnit() { Id = int.Parse(id) };
                        unitList.Add(unit);
                    }
                }
            }
            employee.PriceListUnits = unitList;
            
            List<BarberLevel> BarberLevelList = db.BarberLevels.ToList();
            BarberLevelList.Insert(0, new BarberLevel() { Name = GlobalRes.Empty });

            ViewBag.BarberLevelId = new SelectList(BarberLevelList, "Id", "Name");
            List<Company> CompanyList = db.Companies.ToList();
            CompanyList.Insert(0, new Company() { Name = GlobalRes.Empty });

            ViewBag.CompanyId = new SelectList(CompanyList, "Id", "Name");
            List<Profession> ProfessionList = db.Professions.ToList();
            ProfessionList.Insert(0, new Profession() { Name = GlobalRes.Empty });

            ViewBag.ProfessionId = new SelectList(ProfessionList, "Id", "Name");
            List<Salon> SalonList = db.Salons.ToList();
            SalonList.Insert(0, new Salon() { Name = GlobalRes.Empty });

            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            ViewBag.PriceListUnits = db.PriceListUnits.OrderBy(e => e.Name).ToList();
            return PartialView(employee);
        }

        // GET: Employees/Delete/5
        public async Task<ActionResult> Delete(int? id, bool? concurrencyError, string message)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyError = message;
            }
            else
            {
                message = "404. Данная запись отсутствует.";
            }
            Employee employee = await db.Employees.Include(e => e.BarberLevel).Include(e => e.Company).Include(e => e.Profession).Include(e => e.Salon).Include(e => e.DelayedUpdateEmployees).Include(e => e.DelayedUpdateEmployees.Select(x => x.BarberLevel)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Company)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Profession)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Salon)).SingleOrDefaultAsync(e => e.Id == id.Value);
             
            if (employee == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            
           
            return PartialView(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Employee employee)
        {
            if (await DelayedUpdateIsValid(employee))
            {
                db.Entry(employee).State = EntityState.Deleted;
                string[] saveResult = await Function.SaveChangesToDb(db);

                if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = employee.Id, message = saveResult[1] });
                }

                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = employee.Id }
                };
            }
            else
            {
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = "fail", message = GlobalRes.ErrDelayedUpdateExists }
                };
            }

        }
        [HttpPost]
        public async Task<ActionResult> RefreshRow(int id)
        {
            Employee employee = await db.Employees.Include(e => e.BarberLevel).Include(e => e.Company).Include(e => e.Profession).Include(e => e.Salon).Include(e => e.DelayedUpdateEmployees).Include(e => e.DelayedUpdateEmployees.Select(x => x.BarberLevel)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Company)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Profession)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Salon)).SingleOrDefaultAsync(e => e.Id == id);
            if (employee == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(employee) }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteList(int[] ids)

        {
            List<Employee> employees;

            for (int i = 0; i <= ids.Length / 500; i++)
            {
                int[] x;
                if (i == 0)
                {
                    x = ids.Take(500).ToArray();
                }
                else
                {
                    x = ids.Skip(500 * i).Take(500).ToArray();
                }
                employees = await db.Employees.Include(e => e.PriceListUnits).Where(e => x.Contains(e.Id)).ToListAsync();
                db.Employees.RemoveRange(employees);

            }
            string[] saveResult = await Function.SaveChangesToDb(db);
            if (saveResult[1].Contains("REFERENCE") && saveResult[1].Contains("DelayedUpdate"))
            {
                saveResult[1] = GlobalRes.ErrDelayedUpdateExists;
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = saveResult[0], message = saveResult[1] }
            };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DelayedUpdate(int[] ids)
        {
            var employees = db.Employees.Include(e => e.BarberLevel).Include(e => e.Company).Include(e => e.Profession).Include(e => e.Salon).Include(e => e.DelayedUpdateEmployees);
            for (int i = 0; i < ids.Length; i++)
            {
                int x = ids[i];
                DelayedUpdateEmployee delayedUpdateEmployee = await db.DelayedUpdateEmployees.FirstOrDefaultAsync(e => e.EmployeeId == x);
                if (delayedUpdateEmployee != null)
                {
                    continue;
                }
                Employee employee;
                try
                {
                    employee = await employees.SingleAsync(e => e.Id == x);
                }
                catch
                {
                    continue;
                }
                delayedUpdateEmployee = new DelayedUpdateEmployee(employee);

                db.DelayedUpdateEmployees.Add(delayedUpdateEmployee);
            }
            await Function.SaveChangesToDb(db);
            string result = "success";
            string url = "/DelayedUpdateEmployees/";

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result, url }
            };
        }

        private async Task<bool> DelayedUpdateIsValid(Employee employee)
        {
            try
            {
                DelayedUpdateEmployee checkItem = await db.DelayedUpdateEmployees.SingleAsync(e => e.EmployeeId == employee.Id | (e.FamilyName == employee.FamilyName & e.Name == employee.Name & e.FathersName == employee.FathersName & e.BirthDay == employee.BirthDay));
                
                return false;
            }
            catch (Exception e)
            {
                if (e.HResult == -2146233079)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public async Task<JsonResult> Data(string search, string sort, string order, int? offset, int? limit, string datafilter = null)
        {
            var employees = db.Employees.Include(e => e.BarberLevel).Include(e => e.Company).Include(e => e.Profession).Include(e => e.Salon).Include(e => e.DelayedUpdateEmployees).Include(e => e.DelayedUpdateEmployees.Select(x => x.BarberLevel)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Company)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Profession)).Include(e => e.DelayedUpdateEmployees.Select(x => x.Salon));


            int TotalNotFiltered = employees.Count();



            int Total = TotalNotFiltered;
            if (datafilter != null)
            {
                employees = BuildFilter(employees, JsonConvert.DeserializeObject<EmployeeFilter>(datafilter));
                Total = employees.Count();
            }
            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

                employees = employees.Where(e => e.BarberLevel.Name.Contains(search) || e.Company.Name.Contains(search) || e.Profession.Name.Contains(search) || e.Salon.Name.Contains(search) || e.FamilyName.ToString().Contains(search) || e.Name.ToString().Contains(search) || e.FathersName.ToString().Contains(search) || (e.BirthDay.ToString().Substring(8, 2) + "." + e.BirthDay.ToString().Substring(5, 2) + "." + e.BirthDay.ToString().Substring(0, 4)).Contains(search) || e.OldFamilyName.ToString().Contains(search) || (e.HireDate.ToString().Substring(8, 2) + "." + e.HireDate.ToString().Substring(5, 2) + "." + e.HireDate.ToString().Substring(0, 4)).Contains(search) || e.StaffNumber.ToString().Contains(search) || (e.PhoneNumber1.Substring(1, 3) + e.PhoneNumber1.Substring(6, 3) + e.PhoneNumber1.Substring(10, 2) + e.PhoneNumber1.Substring(13, 2)).Contains(search)
|| e.PhoneNumber1.Contains(search) || (e.PhoneNumber2.Substring(1, 3) + e.PhoneNumber2.Substring(6, 3) + e.PhoneNumber2.Substring(10, 2) + e.PhoneNumber2.Substring(13, 2)).Contains(search)
|| e.PhoneNumber2.Contains(search) || (e.RegisterDate.ToString().Substring(8, 2) + "." + e.RegisterDate.ToString().Substring(5, 2) + "." + e.RegisterDate.ToString().Substring(0, 4)).Contains(search) || (e.DismissalDate.ToString().Substring(8, 2) + "." + e.DismissalDate.ToString().Substring(5, 2) + "." + e.DismissalDate.ToString().Substring(0, 4)).Contains(search));
                Total = employees.Count();
            }


            if (sort != null)
            {
                employees = Function.OrderBy(employees, sort, order);
            }
            else
            {
                employees = employees.OrderBy(e => e.Id);
            }

            if (serverSide)
            {
                employees = employees.Skip(offset.Value);
                BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await employees.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(GetJsonViewModel(await employees.ToListAsync()), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter([Bind(Include = "BirthDayFrom,BirthDayTo,OldFamilyNameButtons,ProfessionIdSelected,BarberLevelIdSelected,SalonIdSelected,HireDateButtons,HireDateFrom,HireDateTo,StaffNumberButtons,StaffNumberFrom,StaffNumberTo,PhoneNumber1Buttons,PhoneNumber2Buttons,CompanyIdSelected,RegisterDateButtons,RegisterDateFrom,RegisterDateTo,DismissalDateButtons,DismissalDateFrom,DismissalDateTo")] EmployeeFilter dataFilter)
        {
            if (ModelState.IsValid)
            {
                if (dataFilter.BarberLevelIdSelected != null && (db.BarberLevels.Count() + 1) == dataFilter.BarberLevelIdSelected.Length)
                    dataFilter.BarberLevelIdSelected = null;
                if (dataFilter.CompanyIdSelected != null && (db.Companies.Count() + 1) == dataFilter.CompanyIdSelected.Length)
                    dataFilter.CompanyIdSelected = null;
                if (dataFilter.ProfessionIdSelected != null && (db.Professions.Count() + 1) == dataFilter.ProfessionIdSelected.Length)
                    dataFilter.ProfessionIdSelected = null;
                if (dataFilter.SalonIdSelected != null && (db.Salons.Count() + 1) == dataFilter.SalonIdSelected.Length)
                    dataFilter.SalonIdSelected = null;


                var employees = db.Employees.Include(e => e.BarberLevel).Include(e => e.Company).Include(e => e.Profession).Include(e => e.Salon).Include(e => e.DelayedUpdateEmployees);

                if (dataFilter != null)
                {
                    employees = BuildFilter(employees, dataFilter);
                }

                int Count = employees.Count();

                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { result = "success", ServerSide = Count > 400 ? true : false, dataFilter = JsonConvert.SerializeObject(dataFilter) }
                };
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result = "notValid", data = ModelState }
            };
        }

        private IQueryable<Employee> BuildFilter(IQueryable<Employee> employees, EmployeeFilter filter)
        {
            if (filter.BirthDayFrom != null)
                employees = employees.Where(e => e.BirthDay >= filter.BirthDayFrom);
            if (filter.BirthDayTo != null)
                employees = employees.Where(e => e.BirthDay <= filter.BirthDayTo);
            if (filter.OldFamilyNameButtons == 1)
            {
                employees = employees.Where(e => e.OldFamilyName != null);
            }
            else if (filter.OldFamilyNameButtons == 2)
            {
                employees = employees.Where(e => e.OldFamilyName == null);
            }
            if (filter.ProfessionIdSelected != null)
            {
                employees = employees.WhereFilter("ProfessionId", filter.ProfessionIdSelected);
            }
            if (filter.BarberLevelIdSelected != null)
            {
                employees = employees.WhereFilter("BarberLevelId", filter.BarberLevelIdSelected);
            }
            if (filter.SalonIdSelected != null)
            {
                employees = employees.WhereFilter("SalonId", filter.SalonIdSelected);
            }
            if (filter.HireDateButtons == 1)
            {
                employees = employees.Where(e => e.HireDate != null);
            }
            else if (filter.HireDateButtons == 2)
            {
                employees = employees.Where(e => e.HireDate == null);
            }
            if (filter.HireDateFrom != null)
                employees = employees.Where(e => e.HireDate >= filter.HireDateFrom);
            if (filter.HireDateTo != null)
                employees = employees.Where(e => e.HireDate <= filter.HireDateTo);
            if (filter.StaffNumberButtons == 1)
            {
                employees = employees.Where(e => e.StaffNumber != null);
            }
            else if (filter.StaffNumberButtons == 2)
            {
                employees = employees.Where(e => e.StaffNumber == null);
            }
            if (filter.StaffNumberFrom != null)
                employees = employees.Where(e => e.StaffNumber >= filter.StaffNumberFrom);
            if (filter.StaffNumberTo != null)
                employees = employees.Where(e => e.StaffNumber <= filter.StaffNumberTo);
            if (filter.PhoneNumber1Buttons == 1)
            {
                employees = employees.Where(e => e.PhoneNumber1 != null);
            }
            else if (filter.PhoneNumber1Buttons == 2)
            {
                employees = employees.Where(e => e.PhoneNumber1 == null);
            }
            if (filter.PhoneNumber2Buttons == 1)
            {
                employees = employees.Where(e => e.PhoneNumber2 != null);
            }
            else if (filter.PhoneNumber2Buttons == 2)
            {
                employees = employees.Where(e => e.PhoneNumber2 == null);
            }
            if (filter.CompanyIdSelected != null)
            {
                employees = employees.WhereFilter("CompanyId", filter.CompanyIdSelected);
            }
            if (filter.RegisterDateButtons == 1)
            {
                employees = employees.Where(e => e.RegisterDate != null);
            }
            else if (filter.RegisterDateButtons == 2)
            {
                employees = employees.Where(e => e.RegisterDate == null);
            }
            if (filter.RegisterDateFrom != null)
                employees = employees.Where(e => e.RegisterDate >= filter.RegisterDateFrom);
            if (filter.RegisterDateTo != null)
                employees = employees.Where(e => e.RegisterDate <= filter.RegisterDateTo);
            if (filter.DismissalDateButtons == 1)
            {
                employees = employees.Where(e => e.DismissalDate != null);
            }
            else if (filter.DismissalDateButtons == 2)
            {
                employees = employees.Where(e => e.DismissalDate == null);
            }
            if (filter.DismissalDateFrom != null)
                employees = employees.Where(e => e.DismissalDate >= filter.DismissalDateFrom);
            if (filter.DismissalDateTo != null)
                employees = employees.Where(e => e.DismissalDate <= filter.DismissalDateTo);

            return employees;
        }
        //private async Task<string[]> SaveChangesToDb()
        //{
        //    string[] result = new string[2];
        //    result[0] = "error";

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //        result[0] = "success";
        //    }
        //    catch (DbEntityValidationException e)
        //    {
        //        result[1] = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
        //    }
        //    catch (DbUpdateConcurrencyException e)
        //    {
        //        result[1] = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
        //    }
        //    catch (DbUpdateException e)
        //    {
        //        result[1] = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
        //    }
        //    return result;
        //}
        private List<EmployeeJsonViewModel> GetJsonViewModel(List<Employee> baseResponse)
        {
            List<EmployeeJsonViewModel> result = new List<EmployeeJsonViewModel>();
            foreach (Employee item in baseResponse)
            {
                result.Add(new EmployeeJsonViewModel(item, true));
            }
            return result;
        }
        private EmployeeJsonViewModel GetJsonViewModel(Employee baseResponse)
        {
            return new EmployeeJsonViewModel(baseResponse, true);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private int? SetValueToNull(int? value)
        {
            if (IsOfNullableType(value))
            {
                if (value == 0)
                {
                    return null;
                }
            }
            return value;
        }

        private bool IsOfNullableType<T>(T o)
        {
            var type = typeof(T);
            return Nullable.GetUnderlyingType(type) != null;
        }
        private void UpdateEmployeePriceListUnits(string[] selectedPriceListUnit, Employee employee)
        {
            if (selectedPriceListUnit == null)
            {
                employee.PriceListUnits = new List<PriceListUnit>();
                return;
            }

            var selectedPriceListUnitHS = new HashSet<string>(selectedPriceListUnit);
            var employeePriceListUnits = new HashSet<int>(employee.PriceListUnits.Select(c => c.Id));
            foreach (var unit in db.PriceListUnits)
            {
                if (selectedPriceListUnitHS.Contains(unit.Id.ToString()))
                {
                    if (!employeePriceListUnits.Contains(unit.Id))
                    {
                        employee.PriceListUnits.Add(unit);
                    }
                }
                else
                {
                    if (employeePriceListUnits.Contains(unit.Id))
                    {
                        employee.PriceListUnits.Remove(unit);
                    }
                }
            }
        }
    }

}

