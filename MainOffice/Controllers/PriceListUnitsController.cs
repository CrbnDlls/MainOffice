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
    public class PriceListUnitsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: PriceListUnits

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
			var priceListUnits = db.PriceListUnits;
			ViewBag.ServerSide = false;
			
				if (priceListUnits.Count() > 400)
                ViewBag.ServerSide = true;
						ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
			return View(new List<PriceListUnit>() { });
		}

        // GET: PriceListUnits/Create
        public ActionResult Create()
        {
			            return PartialView();
        }

        // POST: PriceListUnits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,RowVersion")] PriceListUnit priceListUnit)
        {
            if (ModelState.IsValid)
            {
			                db.PriceListUnits.Add(priceListUnit);
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				priceListUnit = await db.PriceListUnits.FirstAsync(p => p.Id == priceListUnit.Id);
					return Json(new { result = "success", data = GetJsonViewModel(priceListUnit) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_PriceListUnitUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            }

            return PartialView(priceListUnit);
        }

        // GET: PriceListUnits/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PriceListUnit priceListUnit = await db.PriceListUnits.FindAsync(id);
            if (priceListUnit == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.PriceListUnitShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
			ViewBag.Concurrency = false;
            return PartialView(priceListUnit);
        }

        // POST: PriceListUnits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,RowVersion")] PriceListUnit priceListUnit)
        {
			ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
						                db.Entry(priceListUnit).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				priceListUnit = await db.PriceListUnits.FirstAsync(p => p.Id == priceListUnit.Id);
					return Json(new { result = "success", data = GetJsonViewModel(priceListUnit) }, JsonRequestBehavior.AllowGet);
                    }
					else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_PriceListUnitUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            
			}

            return PartialView(priceListUnit);
        }

        // GET: PriceListUnits/Delete/5
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
            PriceListUnit priceListUnit = await db.PriceListUnits.SingleOrDefaultAsync(p => p.Id == id.Value);
            if (priceListUnit == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(priceListUnit);
        }

        // POST: PriceListUnits/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(PriceListUnit priceListUnit)
        {
            
			            db.Entry(priceListUnit).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);
                
			if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = priceListUnit.Id, message = saveResult[1] });
                }
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = priceListUnit.Id }
                };
						
        }

		[HttpPost]
		        public async Task<ActionResult> RefreshRow(int id)
		        {
		            PriceListUnit priceListUnit = await db.PriceListUnits.SingleOrDefaultAsync(p => p.Id == id);
			            if (priceListUnit == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(priceListUnit) }, JsonRequestBehavior.DenyGet);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		        public async Task<JsonResult> DeleteList(int[] ids)
        
        {
            List<PriceListUnit> priceListUnits;
            
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
                    priceListUnits = await db.PriceListUnits.Where(e => x.Contains(e.Id)).ToListAsync();
                    db.PriceListUnits.RemoveRange(priceListUnits);

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
            var priceListUnits = db.PriceListUnits.AsQueryable();
            
			
			int TotalNotFiltered = priceListUnits.Count();
			        
            

            int Total = TotalNotFiltered;
			            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

											priceListUnits = priceListUnits.Where(p => p.Name.ToString().Contains(search) 							|| p.RowVersion.ToString().Contains(search));
                Total = priceListUnits.Count();
			}

             
            if (sort != null)
            {
                priceListUnits = Function.OrderBy(priceListUnits, sort, order);
            }
            else
            {
                priceListUnits = priceListUnits.OrderBy(e => e.Id);
            }
            
            if (serverSide)
            {                
                priceListUnits = priceListUnits.Skip(offset.Value);
								BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await priceListUnits.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
				                return Json(data, JsonRequestBehavior.AllowGet);
            }
			else
			{
            			return Json(GetJsonViewModel(await priceListUnits.ToListAsync()), JsonRequestBehavior.AllowGet);
			            }
}

		private List<PriceListUnitJsonViewModel> GetJsonViewModel(List<PriceListUnit> baseResponse)
        {
            List<PriceListUnitJsonViewModel> result = new List<PriceListUnitJsonViewModel>();
            foreach (PriceListUnit item in baseResponse)
            {
                result.Add(new PriceListUnitJsonViewModel(item));
            }
            return result;
        }
        private PriceListUnitJsonViewModel GetJsonViewModel(PriceListUnit baseResponse)
        {
            return new PriceListUnitJsonViewModel(baseResponse);
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
 
