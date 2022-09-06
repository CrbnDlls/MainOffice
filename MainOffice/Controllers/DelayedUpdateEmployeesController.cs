using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using MainOffice.Functions;
using MainOffice.App_LocalResources;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using MainOffice.Models;

namespace MainOffice.Controllers
{

    [Authorize(Roles = "admin,director")]
    public class DelayedUpdateEmployeesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: DelayedUpdateEmployees

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
            var delayedUpdateEmployees = db.DelayedUpdateEmployees.Include(d => d.BarberLevel).Include(d => d.Company).Include(d => d.Employee).Include(d => d.Profession).Include(d => d.Salon);
            ViewBag.ServerSide = false;

            if (delayedUpdateEmployees.Count() > 400)
                ViewBag.ServerSide = true;

            DelayedUpdateEmployeeFilter filter = new DelayedUpdateEmployeeFilter();

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

            return View(new List<DelayedUpdateEmployee>() { });
        }

        // GET: DelayedUpdateEmployees/Create
        public ActionResult Create()
        {
            DelayedUpdateEmployee delayedUpdateEmployee = new DelayedUpdateEmployee();
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

        // POST: DelayedUpdateEmployees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FamilyName,Name,FathersName,BirthDay,OldFamilyName,ProfessionId,BarberLevelId,SalonId,HireDate,StaffNumber,PhoneNumber1,PhoneNumber2,CompanyId,RegisterDate,DismissalDate,EmployeeId,UpdateDate")] DelayedUpdateEmployee delayedUpdateEmployee, string[] PriceListUnits)
        {
            if (ModelState.IsValid)
            {
                if (await DelayedUpdateIsValid(delayedUpdateEmployee))
                {
                    delayedUpdateEmployee.BarberLevelId = SetValueToNull(delayedUpdateEmployee.BarberLevelId);
                    delayedUpdateEmployee.CompanyId = SetValueToNull(delayedUpdateEmployee.CompanyId);
                    delayedUpdateEmployee.EmployeeId = SetValueToNull(delayedUpdateEmployee.EmployeeId);
                    delayedUpdateEmployee.ProfessionId = SetValueToNull(delayedUpdateEmployee.ProfessionId);
                    delayedUpdateEmployee.SalonId = SetValueToNull(delayedUpdateEmployee.SalonId);
                    db.DelayedUpdateEmployees.Add(delayedUpdateEmployee);
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    if (saveResult[0] == "success")
                    {
                        DelayedUpdateEmployee modifiedEmployee = await db.DelayedUpdateEmployees.Include(x => x.PriceListUnits).Where(x => x.Id == delayedUpdateEmployee.Id).SingleAsync();
                        UpdateEmployeePriceListUnits(PriceListUnits, modifiedEmployee);
                        await Function.SaveChangesToDb(db);
                        delayedUpdateEmployee = await db.DelayedUpdateEmployees.Include(d => d.BarberLevel).Include(d => d.Company).Include(d => d.Employee).Include(d => d.Employee.BarberLevel).Include(d => d.Employee.Company).Include(d => d.Employee.Profession).Include(d => d.Employee.Salon).Include(d => d.Profession).Include(d => d.Salon).FirstAsync(d => d.Id == delayedUpdateEmployee.Id);
                        return Json(new { result = "success", data = GetJsonViewModel(delayedUpdateEmployee) }, JsonRequestBehavior.AllowGet);
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
                    ModelState.AddModelError(String.Empty, GlobalRes.ErrParentTableData);
                    ModelState.AddModelError("FamilyName", GlobalRes.Duplicate);
                    ModelState.AddModelError("Name", GlobalRes.Duplicate);
                    ModelState.AddModelError("FathersName", GlobalRes.Duplicate);
                    ModelState.AddModelError("BirthDay", GlobalRes.Duplicate);

                }
            }

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
            return PartialView(delayedUpdateEmployee);
        }

        // GET: DelayedUpdateEmployees/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DelayedUpdateEmployee delayedUpdateEmployee = await db.DelayedUpdateEmployees.FindAsync(id);
            if (delayedUpdateEmployee == null)
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
            return PartialView(delayedUpdateEmployee);
        }

        // POST: DelayedUpdateEmployees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FamilyName,Name,FathersName,BirthDay,OldFamilyName,ProfessionId,BarberLevelId,SalonId,HireDate,StaffNumber,PhoneNumber1,PhoneNumber2,CompanyId,RegisterDate,DismissalDate,EmployeeId,UpdateDate,RowVersion")] DelayedUpdateEmployee delayedUpdateEmployee, string[] PriceListUnits)
        {
            ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
                if (await DelayedUpdateIsValid(delayedUpdateEmployee))
                {
                    delayedUpdateEmployee.BarberLevelId = SetValueToNull(delayedUpdateEmployee.BarberLevelId);
                    delayedUpdateEmployee.CompanyId = SetValueToNull(delayedUpdateEmployee.CompanyId);
                    delayedUpdateEmployee.EmployeeId = SetValueToNull(delayedUpdateEmployee.EmployeeId);
                    delayedUpdateEmployee.ProfessionId = SetValueToNull(delayedUpdateEmployee.ProfessionId);
                    delayedUpdateEmployee.SalonId = SetValueToNull(delayedUpdateEmployee.SalonId);
                    db.Entry(delayedUpdateEmployee).State = EntityState.Modified;
                    DelayedUpdateEmployee modifiedEmployee = await db.DelayedUpdateEmployees.Include(x => x.PriceListUnits).Where(x => x.Id == delayedUpdateEmployee.Id).SingleOrDefaultAsync();
                    if (modifiedEmployee != null)
                        UpdateEmployeePriceListUnits(PriceListUnits, modifiedEmployee);
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    if (saveResult[0] == "success")
                    {
                        delayedUpdateEmployee = await db.DelayedUpdateEmployees.Include(d => d.BarberLevel).Include(d => d.Company).Include(d => d.Employee).Include(d => d.Employee.BarberLevel).Include(d => d.Employee.Company).Include(d => d.Employee.Profession).Include(d => d.Employee.Salon).Include(d => d.Profession).Include(d => d.Salon).FirstAsync(d => d.Id == delayedUpdateEmployee.Id);
                        return Json(new { result = "success", data = GetJsonViewModel(delayedUpdateEmployee) }, JsonRequestBehavior.AllowGet);
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
                    ModelState.AddModelError(String.Empty, GlobalRes.ErrParentTableData);
                    ModelState.AddModelError("FamilyName", GlobalRes.Duplicate);
                    ModelState.AddModelError("Name", GlobalRes.Duplicate);
                    ModelState.AddModelError("FathersName", GlobalRes.Duplicate);
                    ModelState.AddModelError("BirthDay", GlobalRes.Duplicate);

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
            delayedUpdateEmployee.PriceListUnits = unitList;
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
            return PartialView(delayedUpdateEmployee);
        }

        // GET: DelayedUpdateEmployees/Delete/5
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

            
            DelayedUpdateEmployee delayedUpdateEmployee = await db.DelayedUpdateEmployees.Include(d => d.BarberLevel).Include(d => d.Company).Include(d => d.Employee).Include(d => d.Employee.BarberLevel).Include(d => d.Employee.Company).Include(d => d.Employee.Profession).Include(d => d.Employee.Salon).Include(d => d.Profession).Include(d => d.Salon).SingleOrDefaultAsync(d => d.Id == id.Value);
            if (delayedUpdateEmployee == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(delayedUpdateEmployee);
        }

        // POST: DelayedUpdateEmployees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(DelayedUpdateEmployee delayedUpdateEmployee)
        {
            db.Entry(delayedUpdateEmployee).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);

            if (saveResult[0] == "concurrencyError")
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = delayedUpdateEmployee.Id, message = saveResult[1] });
            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = saveResult[0], message = saveResult[1], value = delayedUpdateEmployee.Id }
            };
        }

        [HttpPost]
        public async Task<ActionResult> RefreshRow(int id)
        {
            DelayedUpdateEmployee delayedUpdateEmployee = await db.DelayedUpdateEmployees.Include(d => d.BarberLevel).Include(d => d.Company).Include(d => d.Employee).Include(d => d.Employee.BarberLevel).Include(d => d.Employee.Company).Include(d => d.Employee.Profession).Include(d => d.Employee.Salon).Include(d => d.Profession).Include(d => d.Salon).SingleOrDefaultAsync(e => e.Id == id);
            if (delayedUpdateEmployee == null)
                return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(delayedUpdateEmployee) }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteList(int[] ids)

        {
            List<DelayedUpdateEmployee> delayedUpdateEmployees;

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
                delayedUpdateEmployees = await db.DelayedUpdateEmployees.Include(e => e.PriceListUnits).Where(e => x.Contains(e.Id)).ToListAsync();
                db.DelayedUpdateEmployees.RemoveRange(delayedUpdateEmployees);

            }
            string[] saveResult = await Function.SaveChangesToDb(db);
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = saveResult[0], message = saveResult[1] }
            };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SetUpdateDate(int[] ids, string date)
        {
            DateTime updateDate;
            bool success = DateTime.TryParse(date, out updateDate);
            string result = "fail";
            string message = null;
            List<DelayedUpdateEmployeeJsonViewModel> data = new List<DelayedUpdateEmployeeJsonViewModel>();
            if (success)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    int x = ids[i];
                    DelayedUpdateEmployee delayedUpdateEmployee = await db.DelayedUpdateEmployees.FindAsync(ids[i]);
                    if (delayedUpdateEmployee == null)
                    {
                        continue;
                    }
                    delayedUpdateEmployee.UpdateDate = updateDate;
                    db.Entry(delayedUpdateEmployee).State = EntityState.Modified;
                }
                string[] saveResult = await Function.SaveChangesToDb(db);
                if (saveResult[0]=="success")
                {
                    List<DelayedUpdateEmployee> delayedUpdateEmployees = new List<DelayedUpdateEmployee>();

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
                        delayedUpdateEmployees.AddRange(await db.DelayedUpdateEmployees.Include(d => d.BarberLevel).Include(d => d.Company).Include(d => d.Employee).Include(d => d.Employee.BarberLevel).Include(d => d.Employee.Company).Include(d => d.Employee.Profession).Include(d => d.Employee.Salon).Include(d => d.Profession).Include(d => d.Salon).Where(e => x.Contains(e.Id)).ToListAsync());
                    }

                    data = GetJsonViewModel(delayedUpdateEmployees);
                }
                result = saveResult[0];
                message = saveResult[1];
            }
            else
            {
                message = GlobalRes.WrongDateFormat;
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result, message, data }
            };
        }

        private async Task<bool> DelayedUpdateIsValid(DelayedUpdateEmployee delayedUpdateEmployee)
        {
            try
            {
                Employee checkItem = await db.Employees.SingleAsync(e => e.FamilyName == delayedUpdateEmployee.FamilyName
                                                                                                        & e.Name == delayedUpdateEmployee.Name
                                                                                                        & e.FathersName == delayedUpdateEmployee.FathersName
                                                                                                        & e.BirthDay == delayedUpdateEmployee.BirthDay
                                                                    );
                if (delayedUpdateEmployee.EmployeeId != null)
                {
                    if (delayedUpdateEmployee.EmployeeId == checkItem.Id)
                    {
                        return true;
                    }
                }
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
            var delayedUpdateEmployees = db.DelayedUpdateEmployees.Include(d => d.BarberLevel).Include(d => d.Company).Include(d => d.Profession).Include(d => d.Salon).Include(d => d.Employee).Include(d => d.Employee.BarberLevel).Include(d => d.Employee.Company).Include(d => d.Employee.Profession).Include(d => d.Employee.Salon);


            int TotalNotFiltered = delayedUpdateEmployees.Count();



            int Total = TotalNotFiltered;
            if (datafilter != null)
            {
                delayedUpdateEmployees = BuildFilter(delayedUpdateEmployees, JsonConvert.DeserializeObject<DelayedUpdateEmployeeFilter>(datafilter));
                Total = delayedUpdateEmployees.Count();
            }
            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.BarberLevel.Name.Contains(search) || d.Company.Name.Contains(search) || d.Profession.Name.Contains(search) || d.Salon.Name.Contains(search) || d.FamilyName.ToString().Contains(search) || d.Name.ToString().Contains(search) || d.FathersName.ToString().Contains(search) || (d.BirthDay.ToString().Substring(8, 2) + "." + d.BirthDay.ToString().Substring(5, 2) + "." + d.BirthDay.ToString().Substring(0, 4)).Contains(search) || d.OldFamilyName.ToString().Contains(search) || (d.HireDate.ToString().Substring(8, 2) + "." + d.HireDate.ToString().Substring(5, 2) + "." + d.HireDate.ToString().Substring(0, 4)).Contains(search) || d.StaffNumber.ToString().Contains(search) || (d.PhoneNumber1.Substring(1, 3) + d.PhoneNumber1.Substring(6, 3) + d.PhoneNumber1.Substring(10, 2) + d.PhoneNumber1.Substring(13, 2)).Contains(search)
|| d.PhoneNumber1.Contains(search) || (d.PhoneNumber2.Substring(1, 3) + d.PhoneNumber2.Substring(6, 3) + d.PhoneNumber2.Substring(10, 2) + d.PhoneNumber2.Substring(13, 2)).Contains(search)
|| d.PhoneNumber2.Contains(search) || (d.RegisterDate.ToString().Substring(8, 2) + "." + d.RegisterDate.ToString().Substring(5, 2) + "." + d.RegisterDate.ToString().Substring(0, 4)).Contains(search) || (d.DismissalDate.ToString().Substring(8, 2) + "." + d.DismissalDate.ToString().Substring(5, 2) + "." + d.DismissalDate.ToString().Substring(0, 4)).Contains(search) || (d.UpdateDate.ToString().Substring(8, 2) + "." + d.UpdateDate.ToString().Substring(5, 2) + "." + d.UpdateDate.ToString().Substring(0, 4)).Contains(search));
                Total = delayedUpdateEmployees.Count();
            }


            if (sort != null)
            {
                delayedUpdateEmployees = Function.OrderBy(delayedUpdateEmployees, sort, order);
            }
            else
            {
                delayedUpdateEmployees = delayedUpdateEmployees.OrderBy(e => e.Id);
            }

            if (serverSide)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Skip(offset.Value);
                BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await delayedUpdateEmployees.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(GetJsonViewModel(await delayedUpdateEmployees.ToListAsync()), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter([Bind(Include = "BirthDayFrom,BirthDayTo,OldFamilyNameButtons,ProfessionIdSelected,BarberLevelIdSelected,SalonIdSelected,HireDateButtons,HireDateFrom,HireDateTo,StaffNumberButtons,StaffNumberFrom,StaffNumberTo,PhoneNumber1Buttons,PhoneNumber2Buttons,CompanyIdSelected,RegisterDateButtons,RegisterDateFrom,RegisterDateTo,DismissalDateButtons,DismissalDateFrom,DismissalDateTo,UpdateDateButtons,UpdateDateFrom,UpdateDateTo")] DelayedUpdateEmployeeFilter dataFilter)
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


                var delayedUpdateEmployees = db.DelayedUpdateEmployees.Include(d => d.BarberLevel).Include(d => d.Company).Include(d => d.Employee).Include(d => d.Profession).Include(d => d.Salon);

                if (dataFilter != null)
                {
                    delayedUpdateEmployees = BuildFilter(delayedUpdateEmployees, dataFilter);
                }

                int Count = delayedUpdateEmployees.Count();

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

        private IQueryable<DelayedUpdateEmployee> BuildFilter(IQueryable<DelayedUpdateEmployee> delayedUpdateEmployees, DelayedUpdateEmployeeFilter filter)
        {
            if (filter.BirthDayFrom != null)
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.BirthDay >= filter.BirthDayFrom);
            if (filter.BirthDayTo != null)
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.BirthDay <= filter.BirthDayTo);
            if (filter.OldFamilyNameButtons == 1)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.OldFamilyName != null);
            }
            else if (filter.OldFamilyNameButtons == 2)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.OldFamilyName == null);
            }
            if (filter.ProfessionIdSelected != null)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.WhereFilter("ProfessionId", filter.ProfessionIdSelected);
            }
            if (filter.BarberLevelIdSelected != null)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.WhereFilter("BarberLevelId", filter.BarberLevelIdSelected);
            }
            if (filter.SalonIdSelected != null)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.WhereFilter("SalonId", filter.SalonIdSelected);
            }
            if (filter.HireDateButtons == 1)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.HireDate != null);
            }
            else if (filter.HireDateButtons == 2)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.HireDate == null);
            }
            if (filter.HireDateFrom != null)
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.HireDate >= filter.HireDateFrom);
            if (filter.HireDateTo != null)
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.HireDate <= filter.HireDateTo);
            if (filter.StaffNumberButtons == 1)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.StaffNumber != null);
            }
            else if (filter.StaffNumberButtons == 2)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.StaffNumber == null);
            }
            if (filter.StaffNumberFrom != null)
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.StaffNumber >= filter.StaffNumberFrom);
            if (filter.StaffNumberTo != null)
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.StaffNumber <= filter.StaffNumberTo);
            if (filter.PhoneNumber1Buttons == 1)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.PhoneNumber1 != null);
            }
            else if (filter.PhoneNumber1Buttons == 2)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.PhoneNumber1 == null);
            }
            if (filter.PhoneNumber2Buttons == 1)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.PhoneNumber2 != null);
            }
            else if (filter.PhoneNumber2Buttons == 2)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.PhoneNumber2 == null);
            }
            if (filter.CompanyIdSelected != null)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.WhereFilter("CompanyId", filter.CompanyIdSelected);
            }
            if (filter.RegisterDateButtons == 1)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.RegisterDate != null);
            }
            else if (filter.RegisterDateButtons == 2)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.RegisterDate == null);
            }
            if (filter.RegisterDateFrom != null)
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.RegisterDate >= filter.RegisterDateFrom);
            if (filter.RegisterDateTo != null)
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.RegisterDate <= filter.RegisterDateTo);
            if (filter.DismissalDateButtons == 1)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.DismissalDate != null);
            }
            else if (filter.DismissalDateButtons == 2)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.DismissalDate == null);
            }
            if (filter.DismissalDateFrom != null)
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.DismissalDate >= filter.DismissalDateFrom);
            if (filter.DismissalDateTo != null)
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.DismissalDate <= filter.DismissalDateTo);
            if (filter.UpdateDateButtons == 1)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.UpdateDate != null);
            }
            else if (filter.UpdateDateButtons == 2)
            {
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.UpdateDate == null);
            }
            if (filter.UpdateDateFrom != null)
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.UpdateDate >= filter.UpdateDateFrom);
            if (filter.UpdateDateTo != null)
                delayedUpdateEmployees = delayedUpdateEmployees.Where(d => d.UpdateDate <= filter.UpdateDateTo);

            return delayedUpdateEmployees;
        }
        
        private List<DelayedUpdateEmployeeJsonViewModel> GetJsonViewModel(List<DelayedUpdateEmployee> baseResponse)
        {
            List<DelayedUpdateEmployeeJsonViewModel> result = new List<DelayedUpdateEmployeeJsonViewModel>();
            foreach (DelayedUpdateEmployee item in baseResponse)
            {
                result.Add(new DelayedUpdateEmployeeJsonViewModel(item, true));
            }
            return result;
        }
        private DelayedUpdateEmployeeJsonViewModel GetJsonViewModel(DelayedUpdateEmployee baseResponse)
        {
            return new DelayedUpdateEmployeeJsonViewModel(baseResponse, true);
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
        private void UpdateEmployeePriceListUnits(string[] selectedPriceListUnit, DelayedUpdateEmployee employee)
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

