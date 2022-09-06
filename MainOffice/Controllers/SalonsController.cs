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
    public class SalonsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Salons

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
			var salons = db.Salons.Include(s => s.SalonState).Include(s => s.SalonType);
			ViewBag.ServerSide = false;
			
				if (salons.Count() > 400)
                ViewBag.ServerSide = true;
			            
					List<SalonState> SalonStateList = db.SalonStates.ToList();
						ViewBag.SalonStatesSelectList = new MultiSelectList(SalonStateList, "Id", "Name");
            
						List<SalonType> SalonTypeList = db.SalonTypes.ToList();
						ViewBag.SalonTypesSelectList = new MultiSelectList(SalonTypeList, "Id", "Name");
            
						ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
			return View(new List<Salon>() { });
		}

        // GET: Salons/Create
        public ActionResult Create()
        {
						List<SalonState> SalonStateList = db.SalonStates.ToList();
			
            ViewBag.SalonStateId = new SelectList(SalonStateList, "Id", "Name");
			List<SalonType> SalonTypeList = db.SalonTypes.ToList();
			
            ViewBag.SalonTypeId = new SelectList(SalonTypeList, "Id", "Name");
            return PartialView();
        }

        // POST: Salons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Address,PhoneNumber1,PhoneNumber2,SalonStateId,SalonTypeId,Longitude,Latitude,IP,RowVersion")] Salon salon)
        {
            if (ModelState.IsValid)
            {
							salon.SalonStateId = salon.SalonStateId;
				salon.SalonTypeId = salon.SalonTypeId;
                db.Salons.Add(salon);
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				salon = await db.Salons.Include(s => s.SalonState).Include(s => s.SalonType).FirstAsync(s => s.Id == salon.Id);
					return Json(new { result = "success", data = GetJsonViewModel(salon) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_SalonUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            }

			List<SalonState> SalonStateList = db.SalonStates.ToList();
			
            ViewBag.SalonStateId = new SelectList(SalonStateList, "Id", "Name");
			List<SalonType> SalonTypeList = db.SalonTypes.ToList();
			
            ViewBag.SalonTypeId = new SelectList(SalonTypeList, "Id", "Name");
            return PartialView(salon);
        }

        // GET: Salons/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salon salon = await db.Salons.FindAsync(id);
            if (salon == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.SalonShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
			ViewBag.Concurrency = false;
			List<SalonState> SalonStateList = db.SalonStates.ToList();
			
            ViewBag.SalonStateId = new SelectList(SalonStateList, "Id", "Name");
			List<SalonType> SalonTypeList = db.SalonTypes.ToList();
			
            ViewBag.SalonTypeId = new SelectList(SalonTypeList, "Id", "Name");
            return PartialView(salon);
        }

        // POST: Salons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Address,PhoneNumber1,PhoneNumber2,SalonStateId,SalonTypeId,Longitude,Latitude,IP,RowVersion")] Salon salon)
        {
			ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
										salon.SalonStateId = salon.SalonStateId;
				salon.SalonTypeId = salon.SalonTypeId;
                db.Entry(salon).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				salon = await db.Salons.Include(s => s.SalonState).Include(s => s.SalonType).FirstAsync(s => s.Id == salon.Id);
					return Json(new { result = "success", data = GetJsonViewModel(salon) }, JsonRequestBehavior.AllowGet);
                    }
					else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_SalonUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            
			}

			List<SalonState> SalonStateList = db.SalonStates.ToList();
			
            ViewBag.SalonStateId = new SelectList(SalonStateList, "Id", "Name");
			List<SalonType> SalonTypeList = db.SalonTypes.ToList();
			
            ViewBag.SalonTypeId = new SelectList(SalonTypeList, "Id", "Name");
            return PartialView(salon);
        }

        // GET: Salons/Delete/5
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
            
            Salon salon = await db.Salons.Include(s => s.SalonState).Include(s => s.SalonType).SingleOrDefaultAsync(s => s.Id == id.Value);
            if (salon == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(salon);
        }

        // POST: Salons/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Salon salon)
        {
            
			            db.Entry(salon).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);
                
			if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = salon.Id, message = saveResult[1] });
                }
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = salon.Id }
                };
						
        }

		[HttpPost]
		        public async Task<ActionResult> RefreshRow(int id)
		        {
		            Salon salon = await db.Salons.Include(s => s.SalonState).Include(s => s.SalonType).SingleOrDefaultAsync(s => s.Id == id);
			            if (salon == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(salon) }, JsonRequestBehavior.DenyGet);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		        public async Task<JsonResult> DeleteList(int[] ids)
        
        {
            List<Salon> salons;
            
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
                    salons = await db.Salons.Where(e => x.Contains(e.Id)).ToListAsync();
                    db.Salons.RemoveRange(salons);

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
            var salons = db.Salons.Include(s => s.SalonState).Include(s => s.SalonType);
            
			
			int TotalNotFiltered = salons.Count();
			        
            

            int Total = TotalNotFiltered;
			            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

												salons = salons.Where(s => s.SalonState.Name.Contains(search) 								|| s.SalonType.Name.Contains(search)							|| s.Name.ToString().Contains(search)							|| s.Address.ToString().Contains(search)							|| (s.PhoneNumber1.Substring(1, 3) + s.PhoneNumber1.Substring(6, 3) + s.PhoneNumber1.Substring(10, 2) + s.PhoneNumber1.Substring(13, 2)).Contains(search)
							|| s.PhoneNumber1.Contains(search)							|| (s.PhoneNumber2.Substring(1, 3) + s.PhoneNumber2.Substring(6, 3) + s.PhoneNumber2.Substring(10, 2) + s.PhoneNumber2.Substring(13, 2)).Contains(search)
							|| s.PhoneNumber2.Contains(search)							|| s.Longitude.ToString().Contains(search)							|| s.Latitude.ToString().Contains(search)							|| s.IP.ToString().Contains(search)							|| s.RowVersion.ToString().Contains(search));
                Total = salons.Count();
			}

             
            if (sort != null)
            {
                salons = Function.OrderBy(salons, sort, order);
            }
            else
            {
                salons = salons.OrderBy(e => e.Id);
            }
            
            if (serverSide)
            {                
                salons = salons.Skip(offset.Value);
								BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await salons.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
				                return Json(data, JsonRequestBehavior.AllowGet);
            }
			else
			{
            			return Json(GetJsonViewModel(await salons.ToListAsync()), JsonRequestBehavior.AllowGet);
			            }
}

		private List<SalonJsonViewModel> GetJsonViewModel(List<Salon> baseResponse)
        {
            List<SalonJsonViewModel> result = new List<SalonJsonViewModel>();
            foreach (Salon item in baseResponse)
            {
                result.Add(new SalonJsonViewModel(item));
            }
            return result;
        }
        private SalonJsonViewModel GetJsonViewModel(Salon baseResponse)
        {
            return new SalonJsonViewModel(baseResponse);
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
 
