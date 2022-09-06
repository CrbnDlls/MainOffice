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
    public class ClientsControllerOld : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Clients

        public ActionResult Index()
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
                string[] saveResult = await SaveChangesToDb();
                if (saveResult[0] == "success")
                {
                    client = await db.Clients.Include(x => x.AdditionalPhones).Where(c => c.Id == client.Id).SingleAsync();
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
                return HttpNotFound();
            }
            return PartialView(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,FamilyName,FathersName,BirthDay,PhoneNumber,Email,AdditionalPhones")] Client client)
        {
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


                string[] saveResult = await SaveChangesToDb();
                if (saveResult[0] == "success")
                {
                    client = await db.Clients.Include(x => x.AdditionalPhones).Where(c => c.Id == client.Id).SingleAsync();
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

        // GET: Clients/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = await db.Clients.FindAsync(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return PartialView(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteConfirmed(int id)
        {
            Client client = await db.Clients.Include(x => x.AdditionalPhones).Where(c => c.Id == id).SingleAsync();
            db.Clients.Remove(client);
            string[] saveResult = await SaveChangesToDb();

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = saveResult[0], message = saveResult[1], value = id }
            };

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteList(int[] ids)

        {
            List<Client> clients;

            for (int i = 0; i <= ids.Length / 500; i++)
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
            string[] saveResult = await SaveChangesToDb();
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

                clients = clients.Where(c => c.Name.ToString().Contains(search) || c.FamilyName.ToString().Contains(search) || c.FathersName.ToString().Contains(search) || (c.BirthDay.ToString().Substring(8, 2) + "." + c.BirthDay.ToString().Substring(5, 2) + "." + c.BirthDay.ToString().Substring(0, 4)).Contains(search) || (c.PhoneNumber.Substring(1, 3) + c.PhoneNumber.Substring(6, 3) + c.PhoneNumber.Substring(10, 2) + c.PhoneNumber.Substring(13, 2)).Contains(search)
|| c.PhoneNumber.Contains(search) || c.Email.ToString().Contains(search));
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

        private async Task<string[]> SaveChangesToDb()
        {
            string[] result = new string[2];
            result[0] = "error";

            try
            {
                await db.SaveChangesAsync();
                result[0] = "success";
            }
            catch (DbEntityValidationException e)
            {
                result[1] = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
            }
            catch (DbUpdateConcurrencyException e)
            {
                result[1] = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
            }
            catch (DbUpdateException e)
            {
                result[1] = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
            }
            return result;
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

