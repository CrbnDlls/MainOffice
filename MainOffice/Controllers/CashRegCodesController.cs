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
    public class CashRegCodesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: CashRegCodes

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
            var cashRegCodes = db.CashRegCodes.Include(c => c.PriceListUnit).Include(c => c.Product).Include(c => c.Service).Include(c => c.DelayedUpdateCashRegCodes);
            ViewBag.ServerSide = false;

            if (cashRegCodes.Count() > 400)
                ViewBag.ServerSide = true;

            CashRegCodeFilter filter = new CashRegCodeFilter();

            List<PriceListUnit> PriceListUnitList = db.PriceListUnits.ToList();
            PriceListUnitList.Insert(0, new PriceListUnit() { Name = GlobalRes.Empty });
            ViewBag.PriceListUnitsSelectList = new MultiSelectList(PriceListUnitList, "Id", "Name");

            List<Product> ProductList = db.Products.ToList();
            ProductList.Insert(0, new Product() { Name = GlobalRes.Empty });
            ViewBag.ProductsSelectList = new MultiSelectList(ProductList, "Id", "Name");

            List<Service> ServiceList = db.Services.ToList();
            ServiceList.Insert(0, new Service() { Name = GlobalRes.Empty });
            ViewBag.ServicesSelectList = new MultiSelectList(ServiceList, "Id", "Name");

            ViewBag.Filter = filter;
            ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
            return View(new List<CashRegCode>() { });
        }

        // GET: CashRegCodes/Create
        public ActionResult Create()
        {
            List<PriceListUnit> PriceListUnitList = db.PriceListUnits.ToList();
            PriceListUnitList.Insert(0, new PriceListUnit() { Name = GlobalRes.Empty });

            ViewBag.PriceListUnitId = new SelectList(PriceListUnitList, "Id", "Name");
            List<Product> ProductList = db.Products.Include(x => x.Trademark).Include(x => x.Productline).Include(x => x.ProductVolume).OrderBy(x => x.Name).ToList();
            ProductList = GetProductSelectName(ProductList);
            ProductList.Insert(0, new Product() { Name = GlobalRes.Empty });
            ViewBag.ProductId = new SelectList(ProductList, "Id", "Name");
            List<Service> ServiceList = db.Services.Include(x => x.ServiceVolume).OrderBy(x => x.Name).ToList();
            ServiceList = GetServiceSelectName(ServiceList);
            ServiceList.Insert(0, new Service() { Name = GlobalRes.Empty });
            ViewBag.ServiceId = new SelectList(ServiceList, "Id", "Name");
            return PartialView();
        }

        // POST: CashRegCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Code,PriceListUnitId,ProductId,ServiceId,Price,Price10,Price50,PriceStaff,RadiosSwitch")] CashRegCode cashRegCode)
        {
            cashRegCode.PriceListUnitId = SetValueToNull(cashRegCode.PriceListUnitId);
            cashRegCode.ProductId = SetValueToNull(cashRegCode.ProductId);
            cashRegCode.ServiceId = SetValueToNull(cashRegCode.ServiceId);
            if (cashRegCode.ProductId != null & cashRegCode.ServiceId != null)
            {
                ModelState.AddModelError("ProductId", GlobalRes.CannotChooseBoth);
                ModelState.AddModelError("ServiceId", GlobalRes.CannotChooseBoth);
            }
            if (cashRegCode.ProductId == null & cashRegCode.ServiceId == null)
            {
                ModelState.AddModelError("ProductId", GlobalRes.MustChoose);
                ModelState.AddModelError("ServiceId", GlobalRes.MustChoose);
            }
            if (ModelState.IsValid)
            {
                if (await DelayedUpdateIsValid(cashRegCode))
                {
                   db.CashRegCodes.Add(cashRegCode);
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    if (saveResult[0] == "success")
                    {
                        cashRegCode = await db.CashRegCodes.Include(c => c.PriceListUnit).Include(c => c.Product).Include(c => c.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.Service).Include(c => c.Service.ServiceVolume).Include(c => c.DelayedUpdateCashRegCodes).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.PriceListUnit)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.ProductVolume)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Trademark)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Productline)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Service)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Service.ServiceVolume)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Trademark)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Productline)).FirstAsync(c => c.Id == cashRegCode.Id);
                        return Json(new { result = "success", data = GetJsonViewModel(cashRegCode) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_CashRegCodeUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            ModelState.AddModelError("Code", GlobalRes.Duplicate);
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError(String.Empty, GlobalRes.ErrDelayedUpdateExists);
                    ModelState.AddModelError("Code", GlobalRes.DelayedUpdate);

                }
            }

            List<PriceListUnit> PriceListUnitList = db.PriceListUnits.ToList();
            PriceListUnitList.Insert(0, new PriceListUnit() { Name = GlobalRes.Empty });

            ViewBag.PriceListUnitId = new SelectList(PriceListUnitList, "Id", "Name");
            List<Product> ProductList = db.Products.Include(x => x.Trademark).Include(x => x.Productline).Include(x => x.ProductVolume).OrderBy(x => x.Name).ToList();
            ProductList = GetProductSelectName(ProductList);
            ProductList.Insert(0, new Product() { Name = GlobalRes.Empty });
            ViewBag.ProductId = new SelectList(ProductList, "Id", "Name");
            List<Service> ServiceList = db.Services.Include(x => x.ServiceVolume).OrderBy(x => x.Name).ToList();
            ServiceList = GetServiceSelectName(ServiceList);
            ServiceList.Insert(0, new Service() { Name = GlobalRes.Empty });
            ViewBag.ServiceId = new SelectList(ServiceList, "Id", "Name");
            return PartialView(cashRegCode);
        }

        // GET: CashRegCodes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashRegCode cashRegCode = await db.CashRegCodes.FindAsync(id);
            if (cashRegCode == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.CashRegCodeShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
            ViewBag.Concurrency = false;
            List<PriceListUnit> PriceListUnitList = db.PriceListUnits.ToList();
            PriceListUnitList.Insert(0, new PriceListUnit() { Name = GlobalRes.Empty });

            ViewBag.PriceListUnitId = new SelectList(PriceListUnitList, "Id", "Name");
            List<Product> ProductList = db.Products.Include(x => x.Trademark).Include(x => x.Productline).Include(x => x.ProductVolume).OrderBy(x => x.Name).ToList();
            ProductList = GetProductSelectName(ProductList);
            ProductList.Insert(0, new Product() { Name = GlobalRes.Empty });
            ViewBag.ProductId = new SelectList(ProductList, "Id", "Name");
            List<Service> ServiceList = db.Services.Include(x => x.ServiceVolume).OrderBy(x => x.Name).ToList();
            ServiceList = GetServiceSelectName(ServiceList);
            ServiceList.Insert(0, new Service() { Name = GlobalRes.Empty });
            ViewBag.ServiceId = new SelectList(ServiceList, "Id", "Name");
            if (cashRegCode.Product == null)
            {
                cashRegCode.RadiosSwitch = "service";
            }
            return PartialView(cashRegCode);
        }

        // POST: CashRegCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Code,PriceListUnitId,ProductId,ServiceId,Price,Price10,Price50,PriceStaff,RowVersion,RadiosSwitch")] CashRegCode cashRegCode)
        {
            ViewBag.Concurrency = false;
            cashRegCode.PriceListUnitId = SetValueToNull(cashRegCode.PriceListUnitId);
            cashRegCode.ProductId = SetValueToNull(cashRegCode.ProductId);
            cashRegCode.ServiceId = SetValueToNull(cashRegCode.ServiceId);
            if (cashRegCode.ProductId != null & cashRegCode.ServiceId != null)
            {
                ModelState.AddModelError("ProductId", GlobalRes.CannotChooseBoth);
                ModelState.AddModelError("ServiceId", GlobalRes.CannotChooseBoth);
            }
            if (cashRegCode.ProductId == null & cashRegCode.ServiceId == null)
            {
                ModelState.AddModelError("ProductId", GlobalRes.MustChoose);
                ModelState.AddModelError("ServiceId", GlobalRes.MustChoose);
            }
            if (ModelState.IsValid)
            {
                if (await DelayedUpdateIsValid(cashRegCode))
                {
                    db.Entry(cashRegCode).State = EntityState.Modified;
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    if (saveResult[0] == "success")
                    {
                        cashRegCode = await db.CashRegCodes.Include(c => c.PriceListUnit).Include(c => c.Product).Include(c => c.Service).Include(c => c.DelayedUpdateCashRegCodes).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.PriceListUnit)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.ProductVolume)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Trademark)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Productline)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Service)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Service.ServiceVolume)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Trademark)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Productline)).FirstAsync(c => c.Id == cashRegCode.Id);
                        return Json(new { result = "success", data = GetJsonViewModel(cashRegCode) }, JsonRequestBehavior.AllowGet);
                    }
                    else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_CashRegCodeUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            ModelState.AddModelError("Code", GlobalRes.Duplicate);
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError(String.Empty, GlobalRes.ErrDelayedUpdateExists);
                    ModelState.AddModelError("Code", GlobalRes.DelayedUpdate);

                }

            }

            List<PriceListUnit> PriceListUnitList = db.PriceListUnits.ToList();
            PriceListUnitList.Insert(0, new PriceListUnit() { Name = GlobalRes.Empty });

            ViewBag.PriceListUnitId = new SelectList(PriceListUnitList, "Id", "Name");
            List<Product> ProductList = db.Products.Include(x => x.Trademark).Include(x => x.Productline).Include(x => x.ProductVolume).OrderBy(x => x.Name).ToList();
            ProductList = GetProductSelectName(ProductList);
            ProductList.Insert(0, new Product() { Name = GlobalRes.Empty });
            ViewBag.ProductId = new SelectList(ProductList, "Id", "Name");
            List<Service> ServiceList = db.Services.Include(x => x.ServiceVolume).OrderBy(x => x.Name).ToList();
            ServiceList = GetServiceSelectName(ServiceList);
            ServiceList.Insert(0, new Service() { Name = GlobalRes.Empty });

            ViewBag.ServiceId = new SelectList(ServiceList, "Id", "Name");
            return PartialView(cashRegCode);
        }

        // GET: CashRegCodes/Delete/5
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

            CashRegCode cashRegCode = await db.CashRegCodes.Include(c => c.PriceListUnit).Include(c => c.Product).Include(c => c.Service).Include(c => c.DelayedUpdateCashRegCodes).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.PriceListUnit)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.ProductVolume)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Trademark)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Productline)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Service)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Service.ServiceVolume)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Trademark)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Productline)).SingleOrDefaultAsync(c => c.Id == id.Value);
            if (cashRegCode == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(cashRegCode);
        }

        // POST: CashRegCodes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(CashRegCode cashRegCode)
        {

            if (await DelayedUpdateIsValid(cashRegCode))
            {
                db.Entry(cashRegCode).State = EntityState.Deleted;
                string[] saveResult = await Function.SaveChangesToDb(db);

                if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = cashRegCode.Id, message = saveResult[1] });
                }
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = cashRegCode.Id }
                };
            }
            else
            {
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = "fail", message = GlobalRes.ErrDelayedUpdateExists }
                };
            }

        }

        [HttpPost]
        public async Task<ActionResult> RefreshRow(int id)
        {
            CashRegCode cashRegCode = await db.CashRegCodes.Include(c => c.PriceListUnit).Include(c => c.Product).Include(c => c.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.Service).Include(c => c.Service.ServiceVolume).Include(c => c.DelayedUpdateCashRegCodes).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.PriceListUnit)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.ProductVolume)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Trademark)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Productline)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Service)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Service.ServiceVolume)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Trademark)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Productline)).SingleOrDefaultAsync(c => c.Id == id);
            if (cashRegCode == null)
                return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(cashRegCode) }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteList(int[] ids)

        {
            List<CashRegCode> cashRegCodes;

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
                cashRegCodes = await db.CashRegCodes.Where(e => x.Contains(e.Id)).ToListAsync();
                db.CashRegCodes.RemoveRange(cashRegCodes);

            }
            string[] saveResult = await Function.SaveChangesToDb(db);
            if (saveResult[1].Contains("REFERENCE") && saveResult[1].Contains("DelayedUpdate"))
            {
                saveResult[1] = GlobalRes.ErrDelayedUpdateExists;
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = saveResult[0], message = saveResult[1] }
            };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DelayedUpdate(int[] ids)
        {
            var cashRegCodes = db.CashRegCodes.Include(c => c.PriceListUnit).Include(c => c.Product).Include(c => c.Service).Include(c => c.DelayedUpdateCashRegCodes);
            for (int i = 0; i < ids.Length; i++)
            {
                int x = ids[i];
                DelayedUpdateCashRegCode delayedUpdateCashRegCode = await db.DelayedUpdateCashRegCodes.FirstOrDefaultAsync(e => e.CashRegCodeId == x);
                if (delayedUpdateCashRegCode != null)
                {
                    continue;
                }
                CashRegCode cashRegCode;
                try
                {
                    cashRegCode = await cashRegCodes.SingleAsync(e => e.Id == x);
                }
                catch
                {
                    continue;
                }
                delayedUpdateCashRegCode = new DelayedUpdateCashRegCode(cashRegCode);

                db.DelayedUpdateCashRegCodes.Add(delayedUpdateCashRegCode);
            }
            await Function.SaveChangesToDb(db);
            string result = "success";
            string url = "/DelayedUpdateCashRegCodes/";

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result, url }
            };
        }

        private async Task<bool> DelayedUpdateIsValid(CashRegCode cashRegCode)
        {
            try
            {
                await db.DelayedUpdateCashRegCodes.SingleAsync(e => e.CashRegCodeId == cashRegCode.Id | (e.Code == cashRegCode.Code
                                                                    ));
                return false;
            }
            catch (Exception e)
            {
                if (e.HResult == -2146233079)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public async Task<JsonResult> Data(string search, string sort, string order, int? offset, int? limit, string datafilter = null)
        {
            var cashRegCodes = db.CashRegCodes.Include(c => c.PriceListUnit).Include(c => c.Product).Include(c => c.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.Service).Include(c => c.Service.ServiceVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.DelayedUpdateCashRegCodes).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.PriceListUnit)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.ProductVolume)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Trademark)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Productline)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Service)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Service.ServiceVolume)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Trademark)).Include(c => c.DelayedUpdateCashRegCodes.Select(x => x.Product.Productline));


            int TotalNotFiltered = cashRegCodes.Count();



            int Total = TotalNotFiltered;
            if (datafilter != null)
            {
                cashRegCodes = BuildFilter(cashRegCodes, JsonConvert.DeserializeObject<CashRegCodeFilter>(datafilter));
                Total = cashRegCodes.Count();
            }
            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

                cashRegCodes = cashRegCodes.Where(c => c.PriceListUnit.Name.Contains(search) || c.Product.Name.Contains(search) || c.Service.Name.Contains(search) || c.Code.ToString().Contains(search) || c.Price.ToString().Contains(search) || c.Price10.ToString().Contains(search) || c.Price50.ToString().Contains(search) || c.PriceStaff.ToString().Contains(search));
                Total = cashRegCodes.Count();
            }


            if (sort != null)
            {
                cashRegCodes = Function.OrderBy(cashRegCodes, sort, order);
            }
            else
            {
                cashRegCodes = cashRegCodes.OrderBy(e => e.Id);
            }

            if (serverSide)
            {
                cashRegCodes = cashRegCodes.Skip(offset.Value);
                BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await cashRegCodes.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(GetJsonViewModel(await cashRegCodes.ToListAsync()), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter([Bind(Include = "PriceListUnitIdSelected,PriceFrom,PriceTo,Price10Buttons,Price50Buttons,PriceStaffButtons")] CashRegCodeFilter dataFilter)
        {
            if (ModelState.IsValid)
            {
                if (dataFilter.PriceListUnitIdSelected != null && (db.PriceListUnits.Count() + 1) == dataFilter.PriceListUnitIdSelected.Length)
                    dataFilter.PriceListUnitIdSelected = null;


                var cashRegCodes = db.CashRegCodes.Include(c => c.PriceListUnit).Include(c => c.Product).Include(c => c.Service).Include(c => c.DelayedUpdateCashRegCodes);

                if (dataFilter != null)
                {
                    cashRegCodes = BuildFilter(cashRegCodes, dataFilter);
                }

                int Count = cashRegCodes.Count();

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

        private IQueryable<CashRegCode> BuildFilter(IQueryable<CashRegCode> cashRegCodes, CashRegCodeFilter filter)
        {
            if (filter.PriceListUnitIdSelected != null)
            {
                cashRegCodes = cashRegCodes.WhereFilter("PriceListUnitId", filter.PriceListUnitIdSelected);
            }
            if (filter.PriceFrom != null)
                cashRegCodes = cashRegCodes.Where(c => c.Price >= filter.PriceFrom);
            if (filter.PriceTo != null)
                cashRegCodes = cashRegCodes.Where(c => c.Price <= filter.PriceTo);
            if (filter.Price10Buttons == 1)
            {
                cashRegCodes = cashRegCodes.Where(c => c.Price10 != null);
            }
            else if (filter.Price10Buttons == 2)
            {
                cashRegCodes = cashRegCodes.Where(c => c.Price10 == null);
            }
            if (filter.Price50Buttons == 1)
            {
                cashRegCodes = cashRegCodes.Where(c => c.Price50 != null);
            }
            else if (filter.Price50Buttons == 2)
            {
                cashRegCodes = cashRegCodes.Where(c => c.Price50 == null);
            }
            if (filter.PriceStaffButtons == 1)
            {
                cashRegCodes = cashRegCodes.Where(c => c.PriceStaff != null);
            }
            else if (filter.PriceStaffButtons == 2)
            {
                cashRegCodes = cashRegCodes.Where(c => c.PriceStaff == null);
            }

            return cashRegCodes;
        }
        private List<CashRegCodeJsonViewModel> GetJsonViewModel(List<CashRegCode> baseResponse)
        {
            List<CashRegCodeJsonViewModel> result = new List<CashRegCodeJsonViewModel>();
            foreach (CashRegCode item in baseResponse)
            {
                result.Add(new CashRegCodeJsonViewModel(item, true));
            }
            return result;
        }
        private CashRegCodeJsonViewModel GetJsonViewModel(CashRegCode baseResponse)
        {
            return new CashRegCodeJsonViewModel(baseResponse, true);
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

        private List<Product> GetProductSelectName(List<Product> products)
        {
            List<Product> result = new List<Product>();
            foreach (Product product in products)
            {
                if (product.ProductVolume != null)
                {
                    product.Name = product.Name + " | " + product.ProductVolume.Name;
                }
                if (product.Trademark != null)
                {
                    product.Name = product.Name + " | " + product.Trademark.Name;
                }
                if (product.Productline != null)
                {
                    product.Name = product.Name + " | " + product.Productline.Name;
                }
                result.Add(product);
            }
            return result;
        }
        private List<Service> GetServiceSelectName(List<Service> services)
        {
            List<Service> result = new List<Service>();
            foreach (Service service in services)
            {
                if (service.ServiceVolume != null)
                {
                    service.Name = service.Name + " | " + service.ServiceVolume.Name;
                }
                result.Add(service);
            }
            return result;
        }
    }

}

