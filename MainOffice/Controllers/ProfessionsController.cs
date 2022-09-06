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
    public class ProfessionsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Professions

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
			var professions = db.Professions;
			ViewBag.ServerSide = false;
			
				if (professions.Count() > 400)
                ViewBag.ServerSide = true;
						ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
			return View(new List<Profession>() { });
		}

        // GET: Professions/Create
        public ActionResult Create()
        {
			            return PartialView();
        }

        // POST: Professions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,OrderNumber,Color,RowVersion")] Profession profession)
        {
            if (ModelState.IsValid)
            {
			                db.Professions.Add(profession);
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				profession = await db.Professions.FirstAsync(p => p.Id == profession.Id);
					return Json(new { result = "success", data = GetJsonViewModel(profession) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_ProfessionUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            }

            return PartialView(profession);
        }

        // GET: Professions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profession profession = await db.Professions.FindAsync(id);
            if (profession == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.ProfessionShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
			ViewBag.Concurrency = false;
            return PartialView(profession);
        }

        // POST: Professions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,OrderNumber,Color,RowVersion")] Profession profession)
        {
			ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
						                db.Entry(profession).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				profession = await db.Professions.FirstAsync(p => p.Id == profession.Id);
					return Json(new { result = "success", data = GetJsonViewModel(profession) }, JsonRequestBehavior.AllowGet);
                    }
					else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_ProfessionUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            						}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            
			}

            return PartialView(profession);
        }

        // GET: Professions/Delete/5
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
            Profession profession = await db.Professions.SingleOrDefaultAsync(p => p.Id == id.Value);
            if (profession == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(profession);
        }

        // POST: Professions/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Profession profession)
        {
            
			            db.Entry(profession).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);
                
			if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = profession.Id, message = saveResult[1] });
                }
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = profession.Id }
                };
						
        }

		[HttpPost]
		        public async Task<ActionResult> RefreshRow(int id)
		        {
		            Profession profession = await db.Professions.SingleOrDefaultAsync(p => p.Id == id);
			            if (profession == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(profession) }, JsonRequestBehavior.DenyGet);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		        public async Task<JsonResult> DeleteList(int[] ids)
        
        {
            List<Profession> professions;
            
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
                    professions = await db.Professions.Where(e => x.Contains(e.Id)).ToListAsync();
                    db.Professions.RemoveRange(professions);

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
            var professions = db.Professions.AsQueryable();
            
			
			int TotalNotFiltered = professions.Count();
			        
            

            int Total = TotalNotFiltered;
			            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

											professions = professions.Where(p => p.Name.ToString().Contains(search) 							|| p.OrderNumber.ToString().Contains(search)							|| p.Color.ToString().Contains(search)							|| p.RowVersion.ToString().Contains(search));
                Total = professions.Count();
			}

             
            if (sort != null)
            {
                professions = Function.OrderBy(professions, sort, order);
            }
            else
            {
                professions = professions.OrderBy(e => e.Id);
            }
            
            if (serverSide)
            {                
                professions = professions.Skip(offset.Value);
								BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await professions.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
				                return Json(data, JsonRequestBehavior.AllowGet);
            }
			else
			{
            			return Json(GetJsonViewModel(await professions.ToListAsync()), JsonRequestBehavior.AllowGet);
			            }
}

		private List<ProfessionJsonViewModel> GetJsonViewModel(List<Profession> baseResponse)
        {
            List<ProfessionJsonViewModel> result = new List<ProfessionJsonViewModel>();
            foreach (Profession item in baseResponse)
            {
                result.Add(new ProfessionJsonViewModel(item));
            }
            return result;
        }
        private ProfessionJsonViewModel GetJsonViewModel(Profession baseResponse)
        {
            return new ProfessionJsonViewModel(baseResponse);
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
 
