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
    public class OperationDayArchivesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: OperationDayArchives

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
			var operationDayArchives = db.OperationDayArchives.Include(o => o.CloseEmployee).Include(o => o.OpenEmployee).Include(o => o.Salon);
			ViewBag.ServerSide = false;
			
				if (operationDayArchives.Count() > 400)
                ViewBag.ServerSide = true;
			 
            OperationDayArchiveFilter filter = new OperationDayArchiveFilter();
			            
					List<Employee> EmployeeList = db.Employees.ToList();
						ViewBag.EmployeesSelectList = new MultiSelectList(EmployeeList, "Id", "FamilyName");
            
						
						ViewBag.EmployeesSelectList = new MultiSelectList(EmployeeList, "Id", "FamilyName");
            
						List<Salon> SalonList = db.Salons.ToList();
						ViewBag.SalonsSelectList = new MultiSelectList(SalonList, "Id", "Name");
            
						ViewBag.Filter = filter;
            			ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
			return View(new List<OperationDayArchive>() { });
		}

        // GET: OperationDayArchives/Create
        public ActionResult Create()
        {
						List<Employee> EmployeeList = db.Employees.ToList();
			
            ViewBag.CloseEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			
			
            ViewBag.OpenEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			List<Salon> SalonList = db.Salons.ToList();
			
            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            return PartialView();
        }

        // POST: OperationDayArchives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,SalonId,OperationDate,OpenOperationPoint,OpenEmployeeId,OpenGeoLocation,CloseOperationPoint,CloseEmployeeId,CloseGeoLocation,RowVersion")] OperationDayArchive operationDayArchive)
        {
            if (ModelState.IsValid)
            {
							operationDayArchive.CloseEmployeeId = operationDayArchive.CloseEmployeeId;
				operationDayArchive.OpenEmployeeId = operationDayArchive.OpenEmployeeId;
				operationDayArchive.SalonId = operationDayArchive.SalonId;
                db.OperationDayArchives.Add(operationDayArchive);
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				operationDayArchive = await db.OperationDayArchives.Include(o => o.CloseEmployee).Include(o => o.OpenEmployee).Include(o => o.Salon).FirstAsync(o => o.Id == operationDayArchive.Id);
					return Json(new { result = "success", data = GetJsonViewModel(operationDayArchive) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_OperationDayArchiveUnique"))
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
			
            ViewBag.CloseEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			
			
            ViewBag.OpenEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			List<Salon> SalonList = db.Salons.ToList();
			
            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            return PartialView(operationDayArchive);
        }

        // GET: OperationDayArchives/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperationDayArchive operationDayArchive = await db.OperationDayArchives.FindAsync(id);
            if (operationDayArchive == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.OperationDayArchiveShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
			ViewBag.Concurrency = false;
			List<Employee> EmployeeList = db.Employees.ToList();
			
            ViewBag.CloseEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			
			
            ViewBag.OpenEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			List<Salon> SalonList = db.Salons.ToList();
			
            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            return PartialView(operationDayArchive);
        }

        // POST: OperationDayArchives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,SalonId,OperationDate,OpenOperationPoint,OpenEmployeeId,OpenGeoLocation,CloseOperationPoint,CloseEmployeeId,CloseGeoLocation,RowVersion")] OperationDayArchive operationDayArchive)
        {
			ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
										operationDayArchive.CloseEmployeeId = operationDayArchive.CloseEmployeeId;
				operationDayArchive.OpenEmployeeId = operationDayArchive.OpenEmployeeId;
				operationDayArchive.SalonId = operationDayArchive.SalonId;
                db.Entry(operationDayArchive).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				operationDayArchive = await db.OperationDayArchives.Include(o => o.CloseEmployee).Include(o => o.OpenEmployee).Include(o => o.Salon).FirstAsync(o => o.Id == operationDayArchive.Id);
					return Json(new { result = "success", data = GetJsonViewModel(operationDayArchive) }, JsonRequestBehavior.AllowGet);
                    }
					else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_OperationDayArchiveUnique"))
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
			
            ViewBag.CloseEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
						
            ViewBag.OpenEmployeeId = new SelectList(EmployeeList, "Id", "FamilyName");
			List<Salon> SalonList = db.Salons.ToList();
			
            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            return PartialView(operationDayArchive);
        }

        // GET: OperationDayArchives/Delete/5
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
            
            OperationDayArchive operationDayArchive = await db.OperationDayArchives.Include(o => o.CloseEmployee).Include(o => o.OpenEmployee).Include(o => o.Salon).SingleOrDefaultAsync(o => o.Id == id.Value);
            if (operationDayArchive == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(operationDayArchive);
        }

        // POST: OperationDayArchives/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(OperationDayArchive operationDayArchive)
        {
            
			            db.Entry(operationDayArchive).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);
                
			if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = operationDayArchive.Id, message = saveResult[1] });
                }
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = operationDayArchive.Id }
                };
						
        }

		[HttpPost]
		        public async Task<ActionResult> RefreshRow(int id)
		        {
		            OperationDayArchive operationDayArchive = await db.OperationDayArchives.Include(o => o.CloseEmployee).Include(o => o.OpenEmployee).Include(o => o.Salon).SingleOrDefaultAsync(o => o.Id == id);
			            if (operationDayArchive == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(operationDayArchive) }, JsonRequestBehavior.DenyGet);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		        public async Task<JsonResult> DeleteList(int[] ids)
        
        {
            List<OperationDayArchive> operationDayArchives;
            
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
                    operationDayArchives = await db.OperationDayArchives.Where(e => x.Contains(e.Id)).ToListAsync();
                    db.OperationDayArchives.RemoveRange(operationDayArchives);

                }
				            string[] saveResult = await Function.SaveChangesToDb(db);
									return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = saveResult[0], message = saveResult[1] }
            };
        }
		public async Task<JsonResult> Data(string search, string sort, string order, int? offset, int? limit, string datafilter = null)
        {
            var operationDayArchives = db.OperationDayArchives.Include(o => o.CloseEmployee).Include(o => o.OpenEmployee).Include(o => o.Salon);
            
			
			int TotalNotFiltered = operationDayArchives.Count();
			        
            

            int Total = TotalNotFiltered;
			            if (datafilter != null)
            { 
                operationDayArchives = BuildFilter(operationDayArchives, JsonConvert.DeserializeObject<OperationDayArchiveFilter>(datafilter));
                Total = operationDayArchives.Count();
			}
			            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

												operationDayArchives = operationDayArchives.Where(o => o.CloseEmployee.FamilyName.Contains(search) 								|| o.OpenEmployee.FamilyName.Contains(search)								|| o.Salon.Name.Contains(search)							|| (o.OperationDate.ToString().Substring(8, 2) + "." + o.OperationDate.ToString().Substring(5, 2) + "." + o.OperationDate.ToString().Substring(0, 4)).Contains(search)							|| o.OpenOperationPoint.ToString().Contains(search)							|| o.OpenGeoLocation.ToString().Contains(search)							|| o.CloseOperationPoint.ToString().Contains(search)							|| o.CloseGeoLocation.ToString().Contains(search)							|| o.RowVersion.ToString().Contains(search));
                Total = operationDayArchives.Count();
			}

             
            if (sort != null)
            {
                operationDayArchives = Function.OrderBy(operationDayArchives, sort, order);
            }
            else
            {
                operationDayArchives = operationDayArchives.OrderBy(e => e.Id);
            }
            
            if (serverSide)
            {                
                operationDayArchives = operationDayArchives.Skip(offset.Value);
								BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await operationDayArchives.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
				                return Json(data, JsonRequestBehavior.AllowGet);
            }
			else
			{
            			return Json(GetJsonViewModel(await operationDayArchives.ToListAsync()), JsonRequestBehavior.AllowGet);
			            }
}

		[HttpPost]
        [ValidateAntiForgeryToken]
		public ActionResult Filter([Bind(Include = "OperationDateFrom,OperationDateTo,SalonIdSelected,OpenOperationPointFrom,OpenOperationPointTo,CloseOperationPointFrom,CloseOperationPointTo")] OperationDayArchiveFilter dataFilter)
		{
			if (ModelState.IsValid)
            {
									if (dataFilter.SalonIdSelected != null && db.Salons.Count() == dataFilter.SalonIdSelected.Length)
                        dataFilter.SalonIdSelected = null;
					
                                
                var operationDayArchives = db.OperationDayArchives.Include(o => o.CloseEmployee).Include(o => o.OpenEmployee).Include(o => o.Salon);

                if (dataFilter != null)
                {
                    operationDayArchives = BuildFilter(operationDayArchives, dataFilter);
                }
								
				int Count = operationDayArchives.Count();
				
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

		private IQueryable<OperationDayArchive> BuildFilter(IQueryable<OperationDayArchive> operationDayArchives, OperationDayArchiveFilter filter)
        {
								if (filter.OperationDateFrom != null)
					operationDayArchives = operationDayArchives.Where(o => o.OperationDate >= filter.OperationDateFrom);
								if (filter.OperationDateTo != null)
					operationDayArchives = operationDayArchives.Where(o => o.OperationDate <= filter.OperationDateTo);
								if (filter.SalonIdSelected != null)
					{
						operationDayArchives = operationDayArchives.WhereFilter("SalonId", filter.SalonIdSelected);
					}
								if (filter.OpenOperationPointFrom != null)
					operationDayArchives = operationDayArchives.Where(o => o.OpenOperationPoint >= filter.OpenOperationPointFrom);
								if (filter.OpenOperationPointTo != null)
					operationDayArchives = operationDayArchives.Where(o => o.OpenOperationPoint <= filter.OpenOperationPointTo);
								if (filter.CloseOperationPointFrom != null)
					operationDayArchives = operationDayArchives.Where(o => o.CloseOperationPoint >= filter.CloseOperationPointFrom);
								if (filter.CloseOperationPointTo != null)
					operationDayArchives = operationDayArchives.Where(o => o.CloseOperationPoint <= filter.CloseOperationPointTo);
			            
            return operationDayArchives;
        }
		private List<OperationDayArchiveJsonViewModel> GetJsonViewModel(List<OperationDayArchive> baseResponse)
        {
            List<OperationDayArchiveJsonViewModel> result = new List<OperationDayArchiveJsonViewModel>();
            foreach (OperationDayArchive item in baseResponse)
            {
                result.Add(new OperationDayArchiveJsonViewModel(item));
            }
            return result;
        }
        private OperationDayArchiveJsonViewModel GetJsonViewModel(OperationDayArchive baseResponse)
        {
            return new OperationDayArchiveJsonViewModel(baseResponse);
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
 
