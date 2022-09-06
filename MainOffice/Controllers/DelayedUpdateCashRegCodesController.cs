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
    public class DelayedUpdateCashRegCodesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: DelayedUpdateCashRegCodes

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
            var delayedUpdateCashRegCodes = db.DelayedUpdateCashRegCodes.Include(d => d.CashRegCode).Include(d => d.PriceListUnit).Include(d => d.Product).Include(d => d.Service);
            ViewBag.ServerSide = false;

            if (delayedUpdateCashRegCodes.Count() > 400)
                ViewBag.ServerSide = true;

            DelayedUpdateCashRegCodeFilter filter = new DelayedUpdateCashRegCodeFilter();

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
            return View(new List<DelayedUpdateCashRegCode>() { });
        }

        // GET: DelayedUpdateCashRegCodes/Create
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

        // POST: DelayedUpdateCashRegCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Code,PriceListUnitId,ProductId,ServiceId,Price,Price10,Price50,PriceStaff,CashRegCodeId,UpdateDate")] DelayedUpdateCashRegCode delayedUpdateCashRegCode)
        {
            delayedUpdateCashRegCode.CashRegCodeId = SetValueToNull(delayedUpdateCashRegCode.CashRegCodeId);
            delayedUpdateCashRegCode.PriceListUnitId = SetValueToNull(delayedUpdateCashRegCode.PriceListUnitId);
            delayedUpdateCashRegCode.ProductId = SetValueToNull(delayedUpdateCashRegCode.ProductId);
            delayedUpdateCashRegCode.ServiceId = SetValueToNull(delayedUpdateCashRegCode.ServiceId);
            if (delayedUpdateCashRegCode.ProductId != null & delayedUpdateCashRegCode.ServiceId != null)
            {
                ModelState.AddModelError("ProductId", GlobalRes.CannotChooseBoth);
                ModelState.AddModelError("ServiceId", GlobalRes.CannotChooseBoth);
            }
            if (delayedUpdateCashRegCode.ProductId == null & delayedUpdateCashRegCode.ServiceId == null)
            {
                ModelState.AddModelError("ProductId", GlobalRes.MustChoose);
                ModelState.AddModelError("ServiceId", GlobalRes.MustChoose);
            }
            if (ModelState.IsValid)
            {
                if (await DelayedUpdateIsValid(delayedUpdateCashRegCode))
                {
                    db.DelayedUpdateCashRegCodes.Add(delayedUpdateCashRegCode);
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    if (saveResult[0] == "success")
                    {
                        delayedUpdateCashRegCode = await db.DelayedUpdateCashRegCodes.Include(d => d.CashRegCode).Include(d => d.CashRegCode.PriceListUnit).Include(d => d.CashRegCode.Product).Include(d => d.CashRegCode.Product.ProductVolume).Include(c => c.CashRegCode.Product.Trademark).Include(c => c.CashRegCode.Product.Productline).Include(d => d.CashRegCode.Service).Include(d => d.CashRegCode.Service.ServiceVolume).Include(d => d.PriceListUnit).Include(d => d.Product).Include(d => d.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(d => d.Service).Include(d => d.Service.ServiceVolume).FirstAsync(d => d.Id == delayedUpdateCashRegCode.Id);
                        return Json(new { result = "success", data = GetJsonViewModel(delayedUpdateCashRegCode) }, JsonRequestBehavior.AllowGet);
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
                    ModelState.AddModelError(String.Empty, GlobalRes.ErrParentTableData);
                    ModelState.AddModelError("Code", GlobalRes.Duplicate);

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
            return PartialView(delayedUpdateCashRegCode);
        }

        // GET: DelayedUpdateCashRegCodes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DelayedUpdateCashRegCode delayedUpdateCashRegCode = await db.DelayedUpdateCashRegCodes.FindAsync(id);
            if (delayedUpdateCashRegCode == null)
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
            if (delayedUpdateCashRegCode.Product == null)
            {
                delayedUpdateCashRegCode.RadiosSwitch = "service";
            }
            return PartialView(delayedUpdateCashRegCode);
        }

        // POST: DelayedUpdateCashRegCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Code,PriceListUnitId,ProductId,ServiceId,Price,Price10,Price50,PriceStaff,CashRegCodeId,UpdateDate,RowVersion")] DelayedUpdateCashRegCode delayedUpdateCashRegCode)
        {
            ViewBag.Concurrency = false;
            delayedUpdateCashRegCode.CashRegCodeId = SetValueToNull(delayedUpdateCashRegCode.CashRegCodeId);
            delayedUpdateCashRegCode.PriceListUnitId = SetValueToNull(delayedUpdateCashRegCode.PriceListUnitId);
            delayedUpdateCashRegCode.ProductId = SetValueToNull(delayedUpdateCashRegCode.ProductId);
            delayedUpdateCashRegCode.ServiceId = SetValueToNull(delayedUpdateCashRegCode.ServiceId);
            if (delayedUpdateCashRegCode.ProductId != null & delayedUpdateCashRegCode.ServiceId != null)
            {
                ModelState.AddModelError("ProductId", GlobalRes.CannotChooseBoth);
                ModelState.AddModelError("ServiceId", GlobalRes.CannotChooseBoth);
            }
            if (delayedUpdateCashRegCode.ProductId == null & delayedUpdateCashRegCode.ServiceId == null)
            {
                ModelState.AddModelError("ProductId", GlobalRes.MustChoose);
                ModelState.AddModelError("ServiceId", GlobalRes.MustChoose);
            }
            if (ModelState.IsValid)
            {
                if (await DelayedUpdateIsValid(delayedUpdateCashRegCode))
                {
                    db.Entry(delayedUpdateCashRegCode).State = EntityState.Modified;
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    if (saveResult[0] == "success")
                    {
                        delayedUpdateCashRegCode = await db.DelayedUpdateCashRegCodes.Include(d => d.CashRegCode).Include(d => d.CashRegCode.PriceListUnit).Include(d => d.CashRegCode.Product).Include(d => d.CashRegCode.Product.ProductVolume).Include(c => c.CashRegCode.Product.Trademark).Include(c => c.CashRegCode.Product.Productline).Include(d => d.CashRegCode.Service).Include(d => d.CashRegCode.Service.ServiceVolume).Include(d => d.PriceListUnit).Include(d => d.Product).Include(d => d.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(d => d.Service).Include(d => d.Service.ServiceVolume).FirstAsync(d => d.Id == delayedUpdateCashRegCode.Id);
                        return Json(new { result = "success", data = GetJsonViewModel(delayedUpdateCashRegCode) }, JsonRequestBehavior.AllowGet);
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
                    ModelState.AddModelError(String.Empty, GlobalRes.ErrParentTableData);
                    ModelState.AddModelError("Code", GlobalRes.Duplicate);

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
            return PartialView(delayedUpdateCashRegCode);
        }

        // GET: DelayedUpdateCashRegCodes/Delete/5
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

            DelayedUpdateCashRegCode delayedUpdateCashRegCode = await db.DelayedUpdateCashRegCodes.Include(d => d.CashRegCode).Include(d => d.CashRegCode.PriceListUnit).Include(d => d.CashRegCode.Product).Include(d => d.CashRegCode.Product.ProductVolume).Include(c => c.CashRegCode.Product.Trademark).Include(c => c.CashRegCode.Product.Productline).Include(d => d.CashRegCode.Service).Include(d => d.CashRegCode.Service.ServiceVolume).Include(d => d.PriceListUnit).Include(d => d.Product).Include(d => d.Service).SingleOrDefaultAsync(d => d.Id == id.Value);
            if (delayedUpdateCashRegCode == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(delayedUpdateCashRegCode);
        }

        // POST: DelayedUpdateCashRegCodes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(DelayedUpdateCashRegCode delayedUpdateCashRegCode)
        {

            db.Entry(delayedUpdateCashRegCode).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);

            if (saveResult[0] == "concurrencyError")
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = delayedUpdateCashRegCode.Id, message = saveResult[1] });
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = saveResult[0], message = saveResult[1], value = delayedUpdateCashRegCode.Id }
            };

        }

        [HttpPost]
        public async Task<ActionResult> RefreshRow(int id)
        {
            DelayedUpdateCashRegCode delayedUpdateCashRegCode = await db.DelayedUpdateCashRegCodes.Include(d => d.CashRegCode).Include(d => d.CashRegCode.PriceListUnit).Include(d => d.CashRegCode.Product).Include(d => d.CashRegCode.Product.ProductVolume).Include(c => c.CashRegCode.Product.Trademark).Include(c => c.CashRegCode.Product.Productline).Include(d => d.CashRegCode.Service).Include(d => d.CashRegCode.Service.ServiceVolume).Include(d => d.PriceListUnit).Include(d => d.Product).Include(d => d.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(d => d.Service).Include(d => d.Service.ServiceVolume).SingleOrDefaultAsync(d => d.Id == id);
            if (delayedUpdateCashRegCode == null)
                return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(delayedUpdateCashRegCode) }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteList(int[] ids)

        {
            List<DelayedUpdateCashRegCode> delayedUpdateCashRegCodes;

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
                delayedUpdateCashRegCodes = await db.DelayedUpdateCashRegCodes.Where(e => x.Contains(e.Id)).ToListAsync();
                db.DelayedUpdateCashRegCodes.RemoveRange(delayedUpdateCashRegCodes);

            }
            string[] saveResult = await Function.SaveChangesToDb(db);
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = saveResult[0], message = saveResult[1] }
            };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SetUpdateDate(int[] ids, string date)
        {
            DateTime updateDate;
            bool success = DateTime.TryParse(date, out updateDate);
            string result = "fail";
            string message = null;
            List<DelayedUpdateCashRegCodeJsonViewModel> data = new List<DelayedUpdateCashRegCodeJsonViewModel>();
            if (success)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    int x = ids[i];
                    DelayedUpdateCashRegCode delayedUpdateCashRegCode = await db.DelayedUpdateCashRegCodes.FindAsync(ids[i]);
                    if (delayedUpdateCashRegCode == null)
                    {
                        continue;
                    }
                    delayedUpdateCashRegCode.UpdateDate = updateDate;
                    db.Entry(delayedUpdateCashRegCode).State = EntityState.Modified;
                }
                string[] saveResult = await Function.SaveChangesToDb(db);
                if (saveResult[0] == "success")
                {

                    List<DelayedUpdateCashRegCode> delayedUpdateCashRegCodes = new List<DelayedUpdateCashRegCode>();

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
                        delayedUpdateCashRegCodes.AddRange(await db.DelayedUpdateCashRegCodes.Include(d => d.CashRegCode).Include(d => d.CashRegCode.PriceListUnit).Include(d => d.CashRegCode.Product).Include(d => d.CashRegCode.Product.ProductVolume).Include(c => c.CashRegCode.Product.Trademark).Include(c => c.CashRegCode.Product.Productline).Include(d => d.CashRegCode.Service).Include(d => d.CashRegCode.Service.ServiceVolume).Include(d => d.CashRegCode).Include(d => d.PriceListUnit).Include(d => d.Product).Include(d => d.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(d => d.Service).Include(d => d.Service.ServiceVolume).Where(e => x.Contains(e.Id)).ToListAsync());
                    }

                    data = GetJsonViewModel(delayedUpdateCashRegCodes);
                }
                result = saveResult[0];
                message = saveResult[1];
            }
            else
            {
                message = GlobalRes.WrongDateFormat;
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result, message, data }
            };
        }

        private async Task<bool> DelayedUpdateIsValid(DelayedUpdateCashRegCode delayedUpdateCashRegCode)
        {
            try
            {
                CashRegCode checkItem = await db.CashRegCodes.SingleAsync(e => e.Code == delayedUpdateCashRegCode.Code
                                                                    );
                if (delayedUpdateCashRegCode.CashRegCodeId != null)
                {
                    if (delayedUpdateCashRegCode.CashRegCodeId == checkItem.Id)
                    {
                        return true;
                    }
                }
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
            var delayedUpdateCashRegCodes = db.DelayedUpdateCashRegCodes.Include(d => d.CashRegCode).Include(d => d.CashRegCode.PriceListUnit).Include(d => d.CashRegCode.Product).Include(d => d.CashRegCode.Product.ProductVolume).Include(c => c.CashRegCode.Product.Trademark).Include(c => c.CashRegCode.Product.Productline).Include(d => d.CashRegCode.Service).Include(d => d.CashRegCode.Service.ServiceVolume).Include(d => d.PriceListUnit).Include(d => d.Product).Include(d => d.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(d => d.Service).Include(d => d.Service.ServiceVolume);


            int TotalNotFiltered = delayedUpdateCashRegCodes.Count();



            int Total = TotalNotFiltered;
            if (datafilter != null)
            {
                delayedUpdateCashRegCodes = BuildFilter(delayedUpdateCashRegCodes, JsonConvert.DeserializeObject<DelayedUpdateCashRegCodeFilter>(datafilter));
                Total = delayedUpdateCashRegCodes.Count();
            }
            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.PriceListUnit.Name.Contains(search) || d.Product.Name.Contains(search) || d.Service.Name.Contains(search) || d.Code.ToString().Contains(search) || d.Price.ToString().Contains(search) || d.Price10.ToString().Contains(search) || d.Price50.ToString().Contains(search) || d.PriceStaff.ToString().Contains(search) || (d.UpdateDate.ToString().Substring(8, 2) + "." + d.UpdateDate.ToString().Substring(5, 2) + "." + d.UpdateDate.ToString().Substring(0, 4)).Contains(search));
                Total = delayedUpdateCashRegCodes.Count();
            }


            if (sort != null)
            {
                delayedUpdateCashRegCodes = Function.OrderBy(delayedUpdateCashRegCodes, sort, order);
            }
            else
            {
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.OrderBy(e => e.Id);
            }

            if (serverSide)
            {
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Skip(offset.Value);
                BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await delayedUpdateCashRegCodes.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(GetJsonViewModel(await delayedUpdateCashRegCodes.ToListAsync()), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter([Bind(Include = "PriceListUnitIdSelected,PriceFrom,PriceTo,Price10Buttons,Price50Buttons,PriceStaffButtons,UpdateDateButtons,UpdateDateFrom,UpdateDateTo")] DelayedUpdateCashRegCodeFilter dataFilter)
        {
            if (ModelState.IsValid)
            {
                if (dataFilter.PriceListUnitIdSelected != null && (db.PriceListUnits.Count() + 1) == dataFilter.PriceListUnitIdSelected.Length)
                    dataFilter.PriceListUnitIdSelected = null;


                var delayedUpdateCashRegCodes = db.DelayedUpdateCashRegCodes.Include(d => d.CashRegCode).Include(d => d.PriceListUnit).Include(d => d.Product).Include(d => d.Service);

                if (dataFilter != null)
                {
                    delayedUpdateCashRegCodes = BuildFilter(delayedUpdateCashRegCodes, dataFilter);
                }

                int Count = delayedUpdateCashRegCodes.Count();

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

        private IQueryable<DelayedUpdateCashRegCode> BuildFilter(IQueryable<DelayedUpdateCashRegCode> delayedUpdateCashRegCodes, DelayedUpdateCashRegCodeFilter filter)
        {
            if (filter.PriceListUnitIdSelected != null)
            {
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.WhereFilter("PriceListUnitId", filter.PriceListUnitIdSelected);
            }
            if (filter.PriceFrom != null)
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.Price >= filter.PriceFrom);
            if (filter.PriceTo != null)
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.Price <= filter.PriceTo);
            if (filter.Price10Buttons == 1)
            {
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.Price10 != null);
            }
            else if (filter.Price10Buttons == 2)
            {
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.Price10 == null);
            }
            if (filter.Price50Buttons == 1)
            {
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.Price50 != null);
            }
            else if (filter.Price50Buttons == 2)
            {
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.Price50 == null);
            }
            if (filter.PriceStaffButtons == 1)
            {
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.PriceStaff != null);
            }
            else if (filter.PriceStaffButtons == 2)
            {
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.PriceStaff == null);
            }
            if (filter.UpdateDateButtons == 1)
            {
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.UpdateDate != null);
            }
            else if (filter.UpdateDateButtons == 2)
            {
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.UpdateDate == null);
            }
            if (filter.UpdateDateFrom != null)
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.UpdateDate >= filter.UpdateDateFrom);
            if (filter.UpdateDateTo != null)
                delayedUpdateCashRegCodes = delayedUpdateCashRegCodes.Where(d => d.UpdateDate <= filter.UpdateDateTo);

            return delayedUpdateCashRegCodes;
        }
        private List<DelayedUpdateCashRegCodeJsonViewModel> GetJsonViewModel(List<DelayedUpdateCashRegCode> baseResponse)
        {
            List<DelayedUpdateCashRegCodeJsonViewModel> result = new List<DelayedUpdateCashRegCodeJsonViewModel>();
            foreach (DelayedUpdateCashRegCode item in baseResponse)
            {
                result.Add(new DelayedUpdateCashRegCodeJsonViewModel(item, true));
            }
            return result;
        }
        private DelayedUpdateCashRegCodeJsonViewModel GetJsonViewModel(DelayedUpdateCashRegCode baseResponse)
        {
            return new DelayedUpdateCashRegCodeJsonViewModel(baseResponse, true);
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

