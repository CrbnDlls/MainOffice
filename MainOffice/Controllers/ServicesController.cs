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
    public class ServicesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Services

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
			var services = db.Services.Include(s => s.ServiceVolume);
			ViewBag.ServerSide = false;
			
				if (services.Count() > 400)
                ViewBag.ServerSide = true;
			 
            ServiceFilter filter = new ServiceFilter();
			            
					List<ServiceVolume> ServiceVolumeList = db.ServiceVolumes.ToList();
						ServiceVolumeList.Insert(0, new ServiceVolume() { Name = GlobalRes.Empty });
						ViewBag.ServiceVolumesSelectList = new MultiSelectList(ServiceVolumeList, "Id", "Name");
            
						ViewBag.Filter = filter;
            			ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
			return View(new List<Service>() { });
		}

        // GET: Services/Create
        public ActionResult Create()
        {
						List<ServiceVolume> ServiceVolumeList = db.ServiceVolumes.ToList();
							ServiceVolumeList.Insert(0, new ServiceVolume() { Name = GlobalRes.Empty });
			
            ViewBag.ServiceVolumeId = new SelectList(ServiceVolumeList, "Id", "Name");
            return PartialView();
        }

        // POST: Services/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,ServiceVolumeId,RowVersion")] Service service)
        {
            if (ModelState.IsValid)
            {
							service.ServiceVolumeId = SetValueToNull(service.ServiceVolumeId);
				                db.Services.Add(service);
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				service = await db.Services.Include(s => s.ServiceVolume).FirstAsync(s => s.Id == service.Id);
					return Json(new { result = "success", data = GetJsonViewModel(service) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_ServiceUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            									ModelState.AddModelError("Name", GlobalRes.Duplicate);
																	ModelState.AddModelError("ServiceVolumeId", GlobalRes.Duplicate);
														}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            }

			List<ServiceVolume> ServiceVolumeList = db.ServiceVolumes.ToList();
							ServiceVolumeList.Insert(0, new ServiceVolume() { Name = GlobalRes.Empty });
			
            ViewBag.ServiceVolumeId = new SelectList(ServiceVolumeList, "Id", "Name");
            return PartialView(service);
        }

        // GET: Services/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = await db.Services.FindAsync(id);
            if (service == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.ServiceShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
			ViewBag.Concurrency = false;
			List<ServiceVolume> ServiceVolumeList = db.ServiceVolumes.ToList();
							ServiceVolumeList.Insert(0, new ServiceVolume() { Name = GlobalRes.Empty });
			
            ViewBag.ServiceVolumeId = new SelectList(ServiceVolumeList, "Id", "Name");
            return PartialView(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,ServiceVolumeId,RowVersion")] Service service)
        {
			ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
										service.ServiceVolumeId = SetValueToNull(service.ServiceVolumeId);
				                db.Entry(service).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				service = await db.Services.Include(s => s.ServiceVolume).FirstAsync(s => s.Id == service.Id);
					return Json(new { result = "success", data = GetJsonViewModel(service) }, JsonRequestBehavior.AllowGet);
                    }
					else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_ServiceUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            									ModelState.AddModelError("Name", GlobalRes.Duplicate);
																	ModelState.AddModelError("ServiceVolumeId", GlobalRes.Duplicate);
														}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            
			}

			List<ServiceVolume> ServiceVolumeList = db.ServiceVolumes.ToList();
							ServiceVolumeList.Insert(0, new ServiceVolume() { Name = GlobalRes.Empty });
			
            ViewBag.ServiceVolumeId = new SelectList(ServiceVolumeList, "Id", "Name");
            return PartialView(service);
        }

        // GET: Services/Delete/5
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
            
            Service service = await db.Services.Include(s => s.ServiceVolume).SingleOrDefaultAsync(s => s.Id == id.Value);
            if (service == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(service);
        }

        // POST: Services/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Service service)
        {
            
			            db.Entry(service).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);
                
			if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = service.Id, message = saveResult[1] });
                }
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = service.Id }
                };
						
        }

		[HttpPost]
		        public async Task<ActionResult> RefreshRow(int id)
		        {
		            Service service = await db.Services.Include(s => s.ServiceVolume).SingleOrDefaultAsync(s => s.Id == id);
			            if (service == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(service) }, JsonRequestBehavior.DenyGet);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		        public async Task<JsonResult> DeleteList(int[] ids)
        
        {
            List<Service> services;
            
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
                    services = await db.Services.Where(e => x.Contains(e.Id)).ToListAsync();
                    db.Services.RemoveRange(services);

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
            var services = db.Services.Include(s => s.ServiceVolume);
            
			
			int TotalNotFiltered = services.Count();
			        
            

            int Total = TotalNotFiltered;
			            if (datafilter != null)
            { 
                services = BuildFilter(services, JsonConvert.DeserializeObject<ServiceFilter>(datafilter));
                Total = services.Count();
			}
			            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

												services = services.Where(s => s.ServiceVolume.Name.Contains(search) 							|| s.Name.ToString().Contains(search)							|| s.RowVersion.ToString().Contains(search));
                Total = services.Count();
			}

             
            if (sort != null)
            {
                services = Function.OrderBy(services, sort, order);
            }
            else
            {
                services = services.OrderBy(e => e.Id);
            }
            
            if (serverSide)
            {                
                services = services.Skip(offset.Value);
								BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await services.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
				                return Json(data, JsonRequestBehavior.AllowGet);
            }
			else
			{
            			return Json(GetJsonViewModel(await services.ToListAsync()), JsonRequestBehavior.AllowGet);
			            }
}

		[HttpPost]
        [ValidateAntiForgeryToken]
		public ActionResult Filter([Bind(Include = "ServiceVolumeIdSelected")] ServiceFilter dataFilter)
		{
			if (ModelState.IsValid)
            {
									if (dataFilter.ServiceVolumeIdSelected != null && (db.ServiceVolumes.Count() + 1) == dataFilter.ServiceVolumeIdSelected.Length)
                        dataFilter.ServiceVolumeIdSelected = null;
               	
                                
                var services = db.Services.Include(s => s.ServiceVolume);

                if (dataFilter != null)
                {
                    services = BuildFilter(services, dataFilter);
                }
								
				int Count = services.Count();
				
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

		private IQueryable<Service> BuildFilter(IQueryable<Service> services, ServiceFilter filter)
        {
								if (filter.ServiceVolumeIdSelected != null)
					{
						services = services.WhereFilter("ServiceVolumeId", filter.ServiceVolumeIdSelected);
					}
			            
            return services;
        }
		private List<ServiceJsonViewModel> GetJsonViewModel(List<Service> baseResponse)
        {
            List<ServiceJsonViewModel> result = new List<ServiceJsonViewModel>();
            foreach (Service item in baseResponse)
            {
                result.Add(new ServiceJsonViewModel(item));
            }
            return result;
        }
        private ServiceJsonViewModel GetJsonViewModel(Service baseResponse)
        {
            return new ServiceJsonViewModel(baseResponse);
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
 
