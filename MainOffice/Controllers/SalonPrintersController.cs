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
    public class SalonPrintersController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: SalonPrinters

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
			var salonPrinters = db.SalonPrinters.Include(s => s.Salon);
			ViewBag.ServerSide = false;
			
				if (salonPrinters.Count() > 400)
                ViewBag.ServerSide = true;
			            
					List<Salon> SalonList = db.Salons.ToList();
						ViewBag.SalonsSelectList = new MultiSelectList(SalonList, "Id", "Name");
            
						ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
			return View(new List<SalonPrinter>() { });
		}

        // GET: SalonPrinters/Create
        public ActionResult Create()
        {
						List<Salon> SalonList = db.Salons.ToList();
			
            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            return PartialView();
        }

        // POST: SalonPrinters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,SystemPrinterName,SalonId,RowVersion")] SalonPrinter salonPrinter)
        {
            if (ModelState.IsValid)
            {
							salonPrinter.SalonId = salonPrinter.SalonId;
                db.SalonPrinters.Add(salonPrinter);
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				salonPrinter = await db.SalonPrinters.Include(s => s.Salon).FirstAsync(s => s.Id == salonPrinter.Id);
					return Json(new { result = "success", data = GetJsonViewModel(salonPrinter) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_SalonPrinterUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            									ModelState.AddModelError("Name", GlobalRes.Duplicate);
																	ModelState.AddModelError("SystemPrinterName", GlobalRes.Duplicate);
																	ModelState.AddModelError("SalonId", GlobalRes.Duplicate);
														}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            }

			List<Salon> SalonList = db.Salons.ToList();
			
            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            return PartialView(salonPrinter);
        }

        // GET: SalonPrinters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalonPrinter salonPrinter = await db.SalonPrinters.FindAsync(id);
            if (salonPrinter == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.SalonPrinterShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
			ViewBag.Concurrency = false;
			List<Salon> SalonList = db.Salons.ToList();
			
            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            return PartialView(salonPrinter);
        }

        // POST: SalonPrinters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,SystemPrinterName,SalonId,RowVersion")] SalonPrinter salonPrinter)
        {
			ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
										salonPrinter.SalonId = salonPrinter.SalonId;
                db.Entry(salonPrinter).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				salonPrinter = await db.SalonPrinters.Include(s => s.Salon).FirstAsync(s => s.Id == salonPrinter.Id);
					return Json(new { result = "success", data = GetJsonViewModel(salonPrinter) }, JsonRequestBehavior.AllowGet);
                    }
					else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_SalonPrinterUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            									ModelState.AddModelError("Name", GlobalRes.Duplicate);
																	ModelState.AddModelError("SystemPrinterName", GlobalRes.Duplicate);
																	ModelState.AddModelError("SalonId", GlobalRes.Duplicate);
														}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            
			}

			List<Salon> SalonList = db.Salons.ToList();
			
            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            return PartialView(salonPrinter);
        }

        // GET: SalonPrinters/Delete/5
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
            
            SalonPrinter salonPrinter = await db.SalonPrinters.Include(s => s.Salon).SingleOrDefaultAsync(s => s.Id == id.Value);
            if (salonPrinter == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(salonPrinter);
        }

        // POST: SalonPrinters/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(SalonPrinter salonPrinter)
        {
            
			            db.Entry(salonPrinter).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);
                
			if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = salonPrinter.Id, message = saveResult[1] });
                }
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = salonPrinter.Id }
                };
						
        }

		[HttpPost]
		        public async Task<ActionResult> RefreshRow(int id)
		        {
		            SalonPrinter salonPrinter = await db.SalonPrinters.Include(s => s.Salon).SingleOrDefaultAsync(s => s.Id == id);
			            if (salonPrinter == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(salonPrinter) }, JsonRequestBehavior.DenyGet);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		        public async Task<JsonResult> DeleteList(int[] ids)
        
        {
            List<SalonPrinter> salonPrinters;
            
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
                    salonPrinters = await db.SalonPrinters.Where(e => x.Contains(e.Id)).ToListAsync();
                    db.SalonPrinters.RemoveRange(salonPrinters);

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
            var salonPrinters = db.SalonPrinters.Include(s => s.Salon);
            
			
			int TotalNotFiltered = salonPrinters.Count();
			        
            

            int Total = TotalNotFiltered;
			            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

												salonPrinters = salonPrinters.Where(s => s.Salon.Name.Contains(search) 							|| s.Name.ToString().Contains(search)							|| s.SystemPrinterName.ToString().Contains(search)							|| s.RowVersion.ToString().Contains(search));
                Total = salonPrinters.Count();
			}

             
            if (sort != null)
            {
                salonPrinters = Function.OrderBy(salonPrinters, sort, order);
            }
            else
            {
                salonPrinters = salonPrinters.OrderBy(e => e.Id);
            }
            
            if (serverSide)
            {                
                salonPrinters = salonPrinters.Skip(offset.Value);
								BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await salonPrinters.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
				                return Json(data, JsonRequestBehavior.AllowGet);
            }
			else
			{
            			return Json(GetJsonViewModel(await salonPrinters.ToListAsync()), JsonRequestBehavior.AllowGet);
			            }
}

		private List<SalonPrinterJsonViewModel> GetJsonViewModel(List<SalonPrinter> baseResponse)
        {
            List<SalonPrinterJsonViewModel> result = new List<SalonPrinterJsonViewModel>();
            foreach (SalonPrinter item in baseResponse)
            {
                result.Add(new SalonPrinterJsonViewModel(item));
            }
            return result;
        }
        private SalonPrinterJsonViewModel GetJsonViewModel(SalonPrinter baseResponse)
        {
            return new SalonPrinterJsonViewModel(baseResponse);
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
 
