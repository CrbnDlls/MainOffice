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
    public class ClientsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Clients

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
			var clients = db.Clients;
			ViewBag.ServerSide = false;
			
				if (clients.Count() > 400)
                ViewBag.ServerSide = true;
						ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
			return View(new List<Client>() { });
		}

        // GET: Clients/Create
        public ActionResult Create()
        {
			            return PartialView();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,FamilyName,FathersName,BirthDay,PhoneNumber,Email,AdditionalPhones")] Client client)
        {
            if (ModelState.IsValid)
            {
			                db.Clients.Add(client);
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				client = await db.Clients.Include(x => x.AdditionalPhones).FirstAsync(c => c.Id == client.Id);
					return Json(new { result = "success", data = GetJsonViewModel(client) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_ClientUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            									ModelState.AddModelError("PhoneNumber", GlobalRes.Duplicate);
														}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            }

            return PartialView(client);
        }

        // GET: Clients/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = await db.Clients.FindAsync(id);
            if (client == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.ClientShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
			ViewBag.Concurrency = false;
            return PartialView(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,FamilyName,FathersName,BirthDay,PhoneNumber,Email,AdditionalPhones,RowVersion")] Client client)
        {
			ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
                for (int i = 0; i < client.AdditionalPhones.Count; i++)
                {
                    if (client.AdditionalPhones[i].Id == 0)
                    {
                        client.AdditionalPhones[i].ClientId = client.Id;
                        client.AdditionalPhones[i] = db.ClientPhones.Add(client.AdditionalPhones[i]);
                    }
                    else
                    {
                        db.Entry(client.AdditionalPhones[i]).State = EntityState.Modified;
                    }
                }
                List<ClientPhone> dbphones = await db.ClientPhones.Where(x => x.ClientId == client.Id).ToListAsync();
                dbphones = dbphones.Except(client.AdditionalPhones).ToList();
                db.ClientPhones.RemoveRange(dbphones);
                db.Entry(client).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				client = await db.Clients.Include(x => x.AdditionalPhones).FirstAsync(c => c.Id == client.Id);
					return Json(new { result = "success", data = GetJsonViewModel(client) }, JsonRequestBehavior.AllowGet);
                    }
					else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_ClientUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            									ModelState.AddModelError("PhoneNumber", GlobalRes.Duplicate);
														}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            
			}

            return PartialView(client);
        }

        // GET: Clients/Delete/5
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
            Client client = await db.Clients.Include(x => x.AdditionalPhones).SingleOrDefaultAsync(c => c.Id == id.Value);
            if (client == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(client);
        }

        // POST: Clients/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Client client)
        {
            
			            db.Entry(client).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);
                
			if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = client.Id, message = saveResult[1] });
                }
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = client.Id }
                };
						
        }

		[HttpPost]
		        public async Task<ActionResult> RefreshRow(int id)
		        {
		            Client client = await db.Clients.Include(x => x.AdditionalPhones).SingleOrDefaultAsync(c => c.Id == id);
			            if (client == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(client) }, JsonRequestBehavior.DenyGet);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		        public async Task<JsonResult> DeleteList(int[] ids)
        
        {
            List<Client> clients;
            
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
                    clients = await db.Clients.Include(e => e.AdditionalPhones).Where(e => x.Contains(e.Id)).ToListAsync();
                    db.Clients.RemoveRange(clients);

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
            var clients = db.Clients.AsQueryable();
            
			
			int TotalNotFiltered = clients.Count();
			        
            

            int Total = TotalNotFiltered;
			            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

											clients = clients.Where(c => c.Name.ToString().Contains(search) 							|| c.FamilyName.ToString().Contains(search)							|| c.FathersName.ToString().Contains(search)							|| (c.BirthDay.ToString().Substring(8, 2) + "." + c.BirthDay.ToString().Substring(5, 2) + "." + c.BirthDay.ToString().Substring(0, 4)).Contains(search)							|| (c.PhoneNumber.Substring(1, 3) + c.PhoneNumber.Substring(6, 3) + c.PhoneNumber.Substring(10, 2) + c.PhoneNumber.Substring(13, 2)).Contains(search)
							|| c.PhoneNumber.Contains(search)							|| c.Email.ToString().Contains(search));
                Total = clients.Count();
			}

             
            if (sort != null)
            {
                clients = Function.OrderBy(clients, sort, order);
            }
            else
            {
                clients = clients.OrderBy(e => e.Id);
            }
            
            if (serverSide)
            {                
                clients = clients.Skip(offset.Value);
								BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await clients.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
				                return Json(data, JsonRequestBehavior.AllowGet);
            }
			else
			{
            			return Json(GetJsonViewModel(await clients.ToListAsync()), JsonRequestBehavior.AllowGet);
			            }
}

		private List<ClientJsonViewModel> GetJsonViewModel(List<Client> baseResponse)
        {
            List<ClientJsonViewModel> result = new List<ClientJsonViewModel>();
            foreach (Client item in baseResponse)
            {
                result.Add(new ClientJsonViewModel(item));
            }
            return result;
        }
        private ClientJsonViewModel GetJsonViewModel(Client baseResponse)
        {
            return new ClientJsonViewModel(baseResponse);
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
 
