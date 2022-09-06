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
    public class TrademarksController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Trademarks

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
			var trademarks = db.Trademarks;
			ViewBag.ServerSide = false;
			
				if (trademarks.Count() > 400)
                ViewBag.ServerSide = true;
						ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
			return View(new List<Trademark>() { });
		}

        // GET: Trademarks/Create
        public ActionResult Create()
        {
			            return PartialView();
        }

        // POST: Trademarks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,RowVersion")] Trademark trademark)
        {
            if (ModelState.IsValid)
            {
			                db.Trademarks.Add(trademark);
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				trademark = await db.Trademarks.FirstAsync(t => t.Id == trademark.Id);
					return Json(new { result = "success", data = GetJsonViewModel(trademark) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_TrademarkUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            }

            return PartialView(trademark);
        }

        // GET: Trademarks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trademark trademark = await db.Trademarks.FindAsync(id);
            if (trademark == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.TrademarkShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
			ViewBag.Concurrency = false;
            return PartialView(trademark);
        }

        // POST: Trademarks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,RowVersion")] Trademark trademark)
        {
			ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
						                db.Entry(trademark).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				trademark = await db.Trademarks.FirstAsync(t => t.Id == trademark.Id);
					return Json(new { result = "success", data = GetJsonViewModel(trademark) }, JsonRequestBehavior.AllowGet);
                    }
					else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_TrademarkUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            
			}

            return PartialView(trademark);
        }

        // GET: Trademarks/Delete/5
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
            Trademark trademark = await db.Trademarks.SingleOrDefaultAsync(t => t.Id == id.Value);
            if (trademark == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(trademark);
        }

        // POST: Trademarks/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Trademark trademark)
        {
            
			            db.Entry(trademark).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);
                
			if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = trademark.Id, message = saveResult[1] });
                }
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = trademark.Id }
                };
						
        }

		[HttpPost]
		        public async Task<ActionResult> RefreshRow(int id)
		        {
		            Trademark trademark = await db.Trademarks.SingleOrDefaultAsync(t => t.Id == id);
			            if (trademark == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(trademark) }, JsonRequestBehavior.DenyGet);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		        public async Task<JsonResult> DeleteList(int[] ids)
        
        {
            List<Trademark> trademarks;
            
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
                    trademarks = await db.Trademarks.Where(e => x.Contains(e.Id)).ToListAsync();
                    db.Trademarks.RemoveRange(trademarks);

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
            var trademarks = db.Trademarks.AsQueryable();
            
			
			int TotalNotFiltered = trademarks.Count();
			        
            

            int Total = TotalNotFiltered;
			            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

											trademarks = trademarks.Where(t => t.Name.ToString().Contains(search) 							|| t.RowVersion.ToString().Contains(search));
                Total = trademarks.Count();
			}

             
            if (sort != null)
            {
                trademarks = Function.OrderBy(trademarks, sort, order);
            }
            else
            {
                trademarks = trademarks.OrderBy(e => e.Id);
            }
            
            if (serverSide)
            {                
                trademarks = trademarks.Skip(offset.Value);
								BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await trademarks.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
				                return Json(data, JsonRequestBehavior.AllowGet);
            }
			else
			{
            			return Json(GetJsonViewModel(await trademarks.ToListAsync()), JsonRequestBehavior.AllowGet);
			            }
}

		private List<TrademarkJsonViewModel> GetJsonViewModel(List<Trademark> baseResponse)
        {
            List<TrademarkJsonViewModel> result = new List<TrademarkJsonViewModel>();
            foreach (Trademark item in baseResponse)
            {
                result.Add(new TrademarkJsonViewModel(item));
            }
            return result;
        }
        private TrademarkJsonViewModel GetJsonViewModel(Trademark baseResponse)
        {
            return new TrademarkJsonViewModel(baseResponse);
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
 
