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
    public class ServiceVolumesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: ServiceVolumes

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
			var serviceVolumes = db.ServiceVolumes;
			ViewBag.ServerSide = false;
			
				if (serviceVolumes.Count() > 400)
                ViewBag.ServerSide = true;
						ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
			return View(new List<ServiceVolume>() { });
		}

        // GET: ServiceVolumes/Create
        public ActionResult Create()
        {
			            return PartialView();
        }

        // POST: ServiceVolumes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,RowVersion")] ServiceVolume serviceVolume)
        {
            if (ModelState.IsValid)
            {
			                db.ServiceVolumes.Add(serviceVolume);
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				serviceVolume = await db.ServiceVolumes.FirstAsync(s => s.Id == serviceVolume.Id);
					return Json(new { result = "success", data = GetJsonViewModel(serviceVolume) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_ServiceVolumeUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            }

            return PartialView(serviceVolume);
        }

        // GET: ServiceVolumes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceVolume serviceVolume = await db.ServiceVolumes.FindAsync(id);
            if (serviceVolume == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.ServiceVolumeShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
			ViewBag.Concurrency = false;
            return PartialView(serviceVolume);
        }

        // POST: ServiceVolumes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,RowVersion")] ServiceVolume serviceVolume)
        {
			ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
						                db.Entry(serviceVolume).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				serviceVolume = await db.ServiceVolumes.FirstAsync(s => s.Id == serviceVolume.Id);
					return Json(new { result = "success", data = GetJsonViewModel(serviceVolume) }, JsonRequestBehavior.AllowGet);
                    }
					else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_ServiceVolumeUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            
			}

            return PartialView(serviceVolume);
        }

        // GET: ServiceVolumes/Delete/5
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
            ServiceVolume serviceVolume = await db.ServiceVolumes.SingleOrDefaultAsync(s => s.Id == id.Value);
            if (serviceVolume == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(serviceVolume);
        }

        // POST: ServiceVolumes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(ServiceVolume serviceVolume)
        {
            
			            db.Entry(serviceVolume).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);
                
			if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = serviceVolume.Id, message = saveResult[1] });
                }
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = serviceVolume.Id }
                };
						
        }

		[HttpPost]
		        public async Task<ActionResult> RefreshRow(int id)
		        {
		            ServiceVolume serviceVolume = await db.ServiceVolumes.SingleOrDefaultAsync(s => s.Id == id);
			            if (serviceVolume == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(serviceVolume) }, JsonRequestBehavior.DenyGet);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		        public async Task<JsonResult> DeleteList(int[] ids)
        
        {
            List<ServiceVolume> serviceVolumes;
            
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
                    serviceVolumes = await db.ServiceVolumes.Where(e => x.Contains(e.Id)).ToListAsync();
                    db.ServiceVolumes.RemoveRange(serviceVolumes);

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
            var serviceVolumes = db.ServiceVolumes.AsQueryable();
            
			
			int TotalNotFiltered = serviceVolumes.Count();
			        
            

            int Total = TotalNotFiltered;
			            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

											serviceVolumes = serviceVolumes.Where(s => s.Name.ToString().Contains(search) 							|| s.RowVersion.ToString().Contains(search));
                Total = serviceVolumes.Count();
			}

             
            if (sort != null)
            {
                serviceVolumes = Function.OrderBy(serviceVolumes, sort, order);
            }
            else
            {
                serviceVolumes = serviceVolumes.OrderBy(e => e.Id);
            }
            
            if (serverSide)
            {                
                serviceVolumes = serviceVolumes.Skip(offset.Value);
								BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await serviceVolumes.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
				                return Json(data, JsonRequestBehavior.AllowGet);
            }
			else
			{
            			return Json(GetJsonViewModel(await serviceVolumes.ToListAsync()), JsonRequestBehavior.AllowGet);
			            }
}

		private List<ServiceVolumeJsonViewModel> GetJsonViewModel(List<ServiceVolume> baseResponse)
        {
            List<ServiceVolumeJsonViewModel> result = new List<ServiceVolumeJsonViewModel>();
            foreach (ServiceVolume item in baseResponse)
            {
                result.Add(new ServiceVolumeJsonViewModel(item));
            }
            return result;
        }
        private ServiceVolumeJsonViewModel GetJsonViewModel(ServiceVolume baseResponse)
        {
            return new ServiceVolumeJsonViewModel(baseResponse);
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
 
