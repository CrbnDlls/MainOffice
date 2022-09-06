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
    public class SalonStatesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: SalonStates

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
			var salonStates = db.SalonStates;
			ViewBag.ServerSide = false;
			
				if (salonStates.Count() > 400)
                ViewBag.ServerSide = true;
						ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
			return View(new List<SalonState>() { });
		}

        // GET: SalonStates/Create
        public ActionResult Create()
        {
			            return PartialView();
        }

        // POST: SalonStates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,RowVersion")] SalonState salonState)
        {
            if (ModelState.IsValid)
            {
			                db.SalonStates.Add(salonState);
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				salonState = await db.SalonStates.FirstAsync(s => s.Id == salonState.Id);
					return Json(new { result = "success", data = GetJsonViewModel(salonState) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_SalonStateUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            }

            return PartialView(salonState);
        }

        // GET: SalonStates/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalonState salonState = await db.SalonStates.FindAsync(id);
            if (salonState == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.SalonStateShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
			ViewBag.Concurrency = false;
            return PartialView(salonState);
        }

        // POST: SalonStates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,RowVersion")] SalonState salonState)
        {
			ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
						                db.Entry(salonState).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				salonState = await db.SalonStates.FirstAsync(s => s.Id == salonState.Id);
					return Json(new { result = "success", data = GetJsonViewModel(salonState) }, JsonRequestBehavior.AllowGet);
                    }
					else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_SalonStateUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            
			}

            return PartialView(salonState);
        }

        // GET: SalonStates/Delete/5
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
            SalonState salonState = await db.SalonStates.SingleOrDefaultAsync(s => s.Id == id.Value);
            if (salonState == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(salonState);
        }

        // POST: SalonStates/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(SalonState salonState)
        {
            
			            db.Entry(salonState).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);
                
			if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = salonState.Id, message = saveResult[1] });
                }
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = salonState.Id }
                };
						
        }

		[HttpPost]
		        public async Task<ActionResult> RefreshRow(int id)
		        {
		            SalonState salonState = await db.SalonStates.SingleOrDefaultAsync(s => s.Id == id);
			            if (salonState == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(salonState) }, JsonRequestBehavior.DenyGet);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		        public async Task<JsonResult> DeleteList(int[] ids)
        
        {
            List<SalonState> salonStates;
            
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
                    salonStates = await db.SalonStates.Where(e => x.Contains(e.Id)).ToListAsync();
                    db.SalonStates.RemoveRange(salonStates);

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
            var salonStates = db.SalonStates.AsQueryable();
            
			
			int TotalNotFiltered = salonStates.Count();
			        
            

            int Total = TotalNotFiltered;
			            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

											salonStates = salonStates.Where(s => s.Name.ToString().Contains(search) 							|| s.RowVersion.ToString().Contains(search));
                Total = salonStates.Count();
			}

             
            if (sort != null)
            {
                salonStates = Function.OrderBy(salonStates, sort, order);
            }
            else
            {
                salonStates = salonStates.OrderBy(e => e.Id);
            }
            
            if (serverSide)
            {                
                salonStates = salonStates.Skip(offset.Value);
								BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await salonStates.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
				                return Json(data, JsonRequestBehavior.AllowGet);
            }
			else
			{
            			return Json(GetJsonViewModel(await salonStates.ToListAsync()), JsonRequestBehavior.AllowGet);
			            }
}

		private List<SalonStateJsonViewModel> GetJsonViewModel(List<SalonState> baseResponse)
        {
            List<SalonStateJsonViewModel> result = new List<SalonStateJsonViewModel>();
            foreach (SalonState item in baseResponse)
            {
                result.Add(new SalonStateJsonViewModel(item));
            }
            return result;
        }
        private SalonStateJsonViewModel GetJsonViewModel(SalonState baseResponse)
        {
            return new SalonStateJsonViewModel(baseResponse);
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
 
