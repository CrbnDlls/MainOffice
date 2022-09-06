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
    public class OperationDayEmployeeArchivesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: OperationDayEmployeeArchives

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
			var operationDayEmployeeArchives = db.OperationDayEmployeeArchives.Include(o => o.Employee).Include(o => o.EndEmployee).Include(o => o.OperationDayArchive).Include(o => o.StartEmployee);
			ViewBag.ServerSide = false;
			
				if (operationDayEmployeeArchives.Count() > 400)
                ViewBag.ServerSide = true;
			            
					List<Employee> EmployeeList = db.Employees.ToList();
						ViewBag.EmployeesSelectList = new MultiSelectList(EmployeeList, "Id", "FamilyName");
            
						
						ViewBag.EmployeesSelectList = new MultiSelectList(EmployeeList, "Id", "FamilyName");
            
						List<OperationDayArchive> OperationDayArchiveList = db.OperationDayArchives.ToList();
						ViewBag.OperationDayArchivesSelectList = new MultiSelectList(OperationDayArchiveList, "Id", "OpenGeoLocation");
            
						
						ViewBag.EmployeesSelectList = new MultiSelectList(EmployeeList, "Id", "FamilyName");
            
						ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
			return View(new List<OperationDayEmployeeArchive>() { });
		}

        // GET: OperationDayEmployeeArchives/Create
        public ActionResult Create()
        {
						List<Employee> EmployeeList = db.Employees.ToList();
			
            ViewBag.EmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			
			
            ViewBag.EndEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			List<OperationDayArchive> OperationDayArchiveList = db.OperationDayArchives.ToList();
			
            ViewBag.OperationDayArchiveId = new SelectList(OperationDayArchiveList, "Id", "OpenGeoLocation");
			
			
            ViewBag.StartEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
            return PartialView();
        }

        // POST: OperationDayEmployeeArchives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,OperationDayArchiveId,EmployeeId,StartPoint,StartEmployeeId,EndPoint,EndEmployeeId,RowVersion")] OperationDayEmployeeArchive operationDayEmployeeArchive)
        {
            if (ModelState.IsValid)
            {
							operationDayEmployeeArchive.EmployeeId = operationDayEmployeeArchive.EmployeeId;
				operationDayEmployeeArchive.EndEmployeeId = operationDayEmployeeArchive.EndEmployeeId;
				operationDayEmployeeArchive.OperationDayArchiveId = operationDayEmployeeArchive.OperationDayArchiveId;
				operationDayEmployeeArchive.StartEmployeeId = operationDayEmployeeArchive.StartEmployeeId;
                db.OperationDayEmployeeArchives.Add(operationDayEmployeeArchive);
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				operationDayEmployeeArchive = await db.OperationDayEmployeeArchives.Include(o => o.Employee).Include(o => o.EndEmployee).Include(o => o.OperationDayArchive.Salon).Include(o => o.StartEmployee).FirstAsync(o => o.Id == operationDayEmployeeArchive.Id);
					return Json(new { result = "success", data = GetJsonViewModel(operationDayEmployeeArchive) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_OperationDayEmployeeArchiveUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            }

			List<Employee> EmployeeList = db.Employees.ToList();
			
            ViewBag.EmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			
			
            ViewBag.EndEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			List<OperationDayArchive> OperationDayArchiveList = db.OperationDayArchives.ToList();
			
            ViewBag.OperationDayArchiveId = new SelectList(OperationDayArchiveList, "Id", "OpenGeoLocation");
			
			
            ViewBag.StartEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
            return PartialView(operationDayEmployeeArchive);
        }

        // GET: OperationDayEmployeeArchives/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperationDayEmployeeArchive operationDayEmployeeArchive = await db.OperationDayEmployeeArchives.FindAsync(id);
            if (operationDayEmployeeArchive == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.WorkSheet + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
			ViewBag.Concurrency = false;
			List<Employee> EmployeeList = db.Employees.ToList();
			
            ViewBag.EmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			
			
            ViewBag.EndEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			List<OperationDayArchive> OperationDayArchiveList = db.OperationDayArchives.ToList();
			
            ViewBag.OperationDayArchiveId = new SelectList(OperationDayArchiveList, "Id", "OpenGeoLocation");
			
			
            ViewBag.StartEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
            return PartialView(operationDayEmployeeArchive);
        }

        // POST: OperationDayEmployeeArchives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,OperationDayArchiveId,EmployeeId,StartPoint,StartEmployeeId,EndPoint,EndEmployeeId,RowVersion")] OperationDayEmployeeArchive operationDayEmployeeArchive)
        {
			ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
										operationDayEmployeeArchive.EmployeeId = operationDayEmployeeArchive.EmployeeId;
				operationDayEmployeeArchive.EndEmployeeId = operationDayEmployeeArchive.EndEmployeeId;
				operationDayEmployeeArchive.OperationDayArchiveId = operationDayEmployeeArchive.OperationDayArchiveId;
				operationDayEmployeeArchive.StartEmployeeId = operationDayEmployeeArchive.StartEmployeeId;
                db.Entry(operationDayEmployeeArchive).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				operationDayEmployeeArchive = await db.OperationDayEmployeeArchives.Include(o => o.Employee).Include(o => o.EndEmployee).Include(o => o.OperationDayArchive.Salon).Include(o => o.StartEmployee).FirstAsync(o => o.Id == operationDayEmployeeArchive.Id);
					return Json(new { result = "success", data = GetJsonViewModel(operationDayEmployeeArchive) }, JsonRequestBehavior.AllowGet);
                    }
					else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_OperationDayEmployeeArchiveUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            
			}

			List<Employee> EmployeeList = db.Employees.ToList();
			
            ViewBag.EmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			
			
            ViewBag.EndEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			List<OperationDayArchive> OperationDayArchiveList = db.OperationDayArchives.ToList();
			
            ViewBag.OperationDayArchiveId = new SelectList(OperationDayArchiveList, "Id", "OpenGeoLocation");
			
			
            ViewBag.StartEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
            return PartialView(operationDayEmployeeArchive);
        }

        // GET: OperationDayEmployeeArchives/Delete/5
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
            
            OperationDayEmployeeArchive operationDayEmployeeArchive = await db.OperationDayEmployeeArchives.Include(o => o.Employee).Include(o => o.EndEmployee).Include(o => o.OperationDayArchive.Salon).Include(o => o.StartEmployee).SingleOrDefaultAsync(o => o.Id == id.Value);
            if (operationDayEmployeeArchive == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(operationDayEmployeeArchive);
        }

        // POST: OperationDayEmployeeArchives/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(OperationDayEmployeeArchive operationDayEmployeeArchive)
        {
            
			            db.Entry(operationDayEmployeeArchive).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);
                
			if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = operationDayEmployeeArchive.Id, message = saveResult[1] });
                }
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = operationDayEmployeeArchive.Id }
                };
						
        }

		[HttpPost]
		        public async Task<ActionResult> RefreshRow(int id)
		        {
		            OperationDayEmployeeArchive operationDayEmployeeArchive = await db.OperationDayEmployeeArchives.Include(o => o.Employee).Include(o => o.EndEmployee).Include(o => o.OperationDayArchive.Salon).Include(o => o.StartEmployee).SingleOrDefaultAsync(o => o.Id == id);
			            if (operationDayEmployeeArchive == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(operationDayEmployeeArchive) }, JsonRequestBehavior.DenyGet);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		        public async Task<JsonResult> DeleteList(int[] ids)
        
        {
            List<OperationDayEmployeeArchive> operationDayEmployeeArchives;
            
                for (int i = 0; i <= ids.Length/500; i++)
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
                    operationDayEmployeeArchives = await db.OperationDayEmployeeArchives.Where(e => x.Contains(e.Id)).ToListAsync();
                    db.OperationDayEmployeeArchives.RemoveRange(operationDayEmployeeArchives);

                }
				            string[] saveResult = await Function.SaveChangesToDb(db);
									return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = saveResult[0], message = saveResult[1] }
            };
        }
		public async Task<JsonResult> Data(string search, string sort, string order, int? offset, int? limit)
        {
            var operationDayEmployeeArchives = db.OperationDayEmployeeArchives.Include(o => o.Employee).Include(o => o.EndEmployee).Include(o => o.OperationDayArchive.Salon).Include(o => o.StartEmployee);
            
			
			int TotalNotFiltered = operationDayEmployeeArchives.Count();
			        
            

            int Total = TotalNotFiltered;
			            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

												operationDayEmployeeArchives = operationDayEmployeeArchives.Where(o => o.Employee.FamilyName.Contains(search) 								|| o.EndEmployee.FamilyName.Contains(search)								|| o.OperationDayArchive.OpenGeoLocation.Contains(search)								|| o.StartEmployee.FamilyName.Contains(search)							|| o.StartPoint.ToString().Contains(search)							|| o.EndPoint.ToString().Contains(search)							|| o.RowVersion.ToString().Contains(search));
                Total = operationDayEmployeeArchives.Count();
			}

             
            if (sort != null)
            {
                operationDayEmployeeArchives = Function.OrderBy(operationDayEmployeeArchives, sort, order);
            }
            else
            {
                operationDayEmployeeArchives = operationDayEmployeeArchives.OrderBy(e => e.Id);
            }
            
            if (serverSide)
            {                
                operationDayEmployeeArchives = operationDayEmployeeArchives.Skip(offset.Value);
								BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await operationDayEmployeeArchives.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
				                return Json(data, JsonRequestBehavior.AllowGet);
            }
			else
			{
            			return Json(GetJsonViewModel(await operationDayEmployeeArchives.ToListAsync()), JsonRequestBehavior.AllowGet);
			            }
}

		private List<OperationDayEmployeeArchiveJsonViewModel> GetJsonViewModel(List<OperationDayEmployeeArchive> baseResponse)
        {
            List<OperationDayEmployeeArchiveJsonViewModel> result = new List<OperationDayEmployeeArchiveJsonViewModel>();
            foreach (OperationDayEmployeeArchive item in baseResponse)
            {
                result.Add(new OperationDayEmployeeArchiveJsonViewModel(item));
            }
            return result;
        }
        private OperationDayEmployeeArchiveJsonViewModel GetJsonViewModel(OperationDayEmployeeArchive baseResponse)
        {
            return new OperationDayEmployeeArchiveJsonViewModel(baseResponse);
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
    }
	
}
 
