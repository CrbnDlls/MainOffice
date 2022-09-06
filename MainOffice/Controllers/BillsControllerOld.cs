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
using Microsoft.AspNet.Identity;

namespace MainOffice.Controllers
{

    [Authorize(Roles = "admin,director")]
    public class BillsControllerOld : Controller
    {
        private AppDbContext db = new AppDbContext();
        private ApplicationDbContext db1 = new ApplicationDbContext();
        // GET: Bills

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
            var bills = db.Bills.Include(b => b.Client).Include(b => b.Employee).Include(b => b.Salon).Include(b => b.BillLines);
            ViewBag.ServerSide = false;

            if (bills.Count() > 400)
                ViewBag.ServerSide = true;

            BillFilter filter = new BillFilter();
            filter.IssueDateFrom = DateTime.Now.AddDays(-7).Date;
            List<Client> ClientList = db.Clients.ToList();
            ClientList = GetClientSelectName(ClientList);
            ClientList.Insert(0, new Client() { Name = GlobalRes.Empty });
            ViewBag.ClientsSelectList = new MultiSelectList(ClientList, "Id", "Name");

            List<Employee> EmployeeList = db.Employees.ToList();
            EmployeeList = GetEmployeeSelectName(EmployeeList);
            ViewBag.EmployeesSelectList = new MultiSelectList(EmployeeList, "Id", "FamilyName");

            List<Salon> SalonList = db.Salons.ToList();
            ViewBag.SalonsSelectList = new MultiSelectList(SalonList, "Id", "Name");

            ViewBag.Filter = filter;
            ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
            return View(new List<Bill>() { });
        }

        // GET: Bills/Create
        public ActionResult Create()
        {
            List<Client> ClientList = db.Clients.ToList();
            ClientList = GetClientSelectName(ClientList);
            ClientList.Insert(0, new Client() { Name = GlobalRes.Empty });

            ViewBag.ClientId = new SelectList(ClientList, "Id", "Name");
            List<Employee> EmployeeList = db.Employees.ToList();
            EmployeeList = GetEmployeeSelectName(EmployeeList);
            ViewBag.EmployeeId = new SelectList(EmployeeList, "Id", "Name");
            List<Salon> SalonList = db.Salons.ToList();
            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");

            List<Product> ProductList = db.Products.Include(x => x.ProductVolume).Include(x => x.Productline).Include(x => x.Trademark).ToList();
            ProductList = GetProductSelectName(ProductList);

            List<Service> ServiceList = db.Services.Include(x => x.ServiceVolume).ToList();
            ServiceList = GetServiceSelectName(ServiceList);

            List<string> servprodNames = new List<string>();

            foreach (Product product in ProductList)
            {
                servprodNames.Add(product.Name);
            }

            foreach (Service service in ServiceList)
            {
                servprodNames.Add(service.Name);
            }
            ViewBag.Services = servprodNames;

            List<Promotion> promotions = db.Promotions.ToList();
            List<string> promoNames = new List<string>();
            foreach (Promotion promotion in promotions)
            {
                promoNames.Add(promotion.Name);
            }

            ViewBag.Promotion = promoNames;

            Random number = new Random();

            return PartialView(new Bill() { BillNumber = number.Next(10000, 99999), BillLines = new List<BillLine>() { new BillLine() } });
        }

        // POST: Bills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,BillNumber,IssueDate,SalonId,EmployeeId,StartTime,EndTime,BO,ClientId,BillLines")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                int? employeeId = db1.Users.SingleOrDefault(x => x.Id == userId).EmployeeId;
                if (employeeId.HasValue)
                {
                    bill.ClientId = SetValueToNull(bill.ClientId);
                    bill.EmployeeId = bill.EmployeeId;
                    bill.SalonId = bill.SalonId;
                    for (int i = 0; i < bill.BillLines.Count; i++)
                    {
                        bill.BillLines[i].InsertDateTime = DateTime.Now;
                        bill.BillLines[i].AdminVisaId = employeeId;
                    }
                    db.Bills.Add(bill);
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    if (saveResult[0] == "success")
                    {
                        bill = await db.Bills.Include(b => b.Client).Include(b => b.Employee).Include(b => b.Salon).Include(b => b.BillLines).FirstAsync(b => b.Id == bill.Id);
                        return Json(new { result = "success", data = GetJsonViewModel(bill) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_BillUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "User не связан EmployeeId");
                }
            }

            List<Client> ClientList = db.Clients.ToList();
            ClientList = GetClientSelectName(ClientList);
            ClientList.Insert(0, new Client() { Name = GlobalRes.Empty });

            ViewBag.ClientId = new SelectList(ClientList, "Id", "Name");
            List<Employee> EmployeeList = db.Employees.ToList();
            EmployeeList = GetEmployeeSelectName(EmployeeList);
            ViewBag.EmployeeId = new SelectList(EmployeeList, "Id", "Name");
            List<Salon> SalonList = db.Salons.ToList();

            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");
            List<Product> ProductList = db.Products.Include(x => x.ProductVolume).Include(x => x.Productline).Include(x => x.Trademark).ToList();
            ProductList = GetProductSelectName(ProductList);

            List<Service> ServiceList = db.Services.Include(x => x.ServiceVolume).ToList();
            ServiceList = GetServiceSelectName(ServiceList);

            List<string> servprodNames = new List<string>();

            foreach (Product product in ProductList)
            {
                servprodNames.Add(product.Name);
            }

            foreach (Service service in ServiceList)
            {
                servprodNames.Add(service.Name);
            }
            ViewBag.Services = servprodNames;

            List<Promotion> promotions = db.Promotions.ToList();
            List<string> promoNames = new List<string>();
            foreach (Promotion promotion in promotions)
            {
                promoNames.Add(promotion.Name);
            }

            ViewBag.Promotion = promoNames;

            return PartialView(bill);
        }

        // GET: Bills/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill bill = await db.Bills.FindAsync(id);
            if (bill == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.BillShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
            ViewBag.Concurrency = false;
            List<Client> ClientList = db.Clients.ToList();
            ClientList = GetClientSelectName(ClientList);
            ClientList.Insert(0, new Client() { Name = GlobalRes.Empty });

            ViewBag.ClientId = new SelectList(ClientList, "Id", "Name");
            List<Employee> EmployeeList = db.Employees.ToList();
            EmployeeList = GetEmployeeSelectName(EmployeeList);
            ViewBag.EmployeeId = new SelectList(EmployeeList, "Id", "Name");
            List<Salon> SalonList = db.Salons.ToList();

            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");

            List<Product> ProductList = db.Products.Include(x => x.ProductVolume).Include(x => x.Productline).Include(x => x.Trademark).ToList();
            ProductList = GetProductSelectName(ProductList);

            List<Service> ServiceList = db.Services.Include(x => x.ServiceVolume).ToList();
            ServiceList = GetServiceSelectName(ServiceList);

            List<string> servprodNames = new List<string>();

            foreach (Product product in ProductList)
            {
                servprodNames.Add(product.Name);
            }

            foreach (Service service in ServiceList)
            {
                servprodNames.Add(service.Name);
            }
            ViewBag.Services = servprodNames;

            List<Promotion> promotions = db.Promotions.ToList();
            List<string> promoNames = new List<string>();
            foreach (Promotion promotion in promotions)
            {
                promoNames.Add(promotion.Name);
            }

            ViewBag.Promotion = promoNames;
            ViewBag.StopNumber = bill.BillLines.Count - 1;
            return PartialView(bill);
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,BillNumber,IssueDate,SalonId,EmployeeId,StartTime,EndTime,BO,ClientId,BillLines,RowVersion")] Bill bill)
        {
            ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                int? employeeId = db1.Users.SingleOrDefault(x => x.Id == userId).EmployeeId;
                if (employeeId.HasValue)
                {
                    bill.ClientId = SetValueToNull(bill.ClientId);
                    for (int i = 0; i < bill.BillLines.Count; i++)
                    {
                        bill.BillLines[i].AdminVisaId = employeeId;
                        
                        if (bill.BillLines[i].Id == 0)
                        {
                            bill.BillLines[i].InsertDateTime = DateTime.Now;

                            bill.BillLines[i].BillId = bill.Id;
                            bill.BillLines[i] = db.BillLines.Add(bill.BillLines[i]);
                        }
                        else
                        {
                            if (bill.BillLines[i].Cancel & !bill.BillLines[i].CancelDateTime.HasValue)
                                bill.BillLines[i].CancelDateTime = DateTime.Now;
                            db.Entry(bill.BillLines[i]).State = EntityState.Modified;
                        }
                    }
                    db.Entry(bill).State = EntityState.Modified;
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    if (saveResult[0] == "success")
                    {
                        bill = await db.Bills.Include(b => b.Client).Include(b => b.Employee).Include(b => b.Salon).Include(b => b.BillLines).FirstAsync(b => b.Id == bill.Id);
                        return Json(new { result = "success", data = GetJsonViewModel(bill) }, JsonRequestBehavior.AllowGet);
                    }
                    else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_BillUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError(String.Empty, "User не связан EmployeeId");
                }
            }

            List<Client> ClientList = db.Clients.ToList();
            ClientList = GetClientSelectName(ClientList);
            ClientList.Insert(0, new Client() { Name = GlobalRes.Empty });

            ViewBag.ClientId = new SelectList(ClientList, "Id", "Name");
            List<Employee> EmployeeList = db.Employees.ToList();
            EmployeeList = GetEmployeeSelectName(EmployeeList);
            ViewBag.EmployeeId = new SelectList(EmployeeList, "Id", "Name");
            List<Salon> SalonList = db.Salons.ToList();

            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");

            List<Product> ProductList = db.Products.Include(x => x.ProductVolume).Include(x => x.Productline).Include(x => x.Trademark).ToList();
            ProductList = GetProductSelectName(ProductList);

            List<Service> ServiceList = db.Services.Include(x => x.ServiceVolume).ToList();
            ServiceList = GetServiceSelectName(ServiceList);

            List<string> servprodNames = new List<string>();

            foreach (Product product in ProductList)
            {
                servprodNames.Add(product.Name);
            }

            foreach (Service service in ServiceList)
            {
                servprodNames.Add(service.Name);
            }
            ViewBag.Services = servprodNames;

            List<Promotion> promotions = db.Promotions.ToList();
            List<string> promoNames = new List<string>();
            foreach (Promotion promotion in promotions)
            {
                promoNames.Add(promotion.Name);
            }

            ViewBag.Promotion = promoNames;

            ViewBag.StopNumber = bill.BillLines.Where(x => x.Id != 0).ToList().Count - 1;
            return PartialView(bill);
        }

        
        [HttpPost]
        public async Task<ActionResult> RefreshRow(int id)
        {
            Bill bill = await db.Bills.Include(b => b.Client).Include(b => b.Employee).Include(b => b.Salon).Include(b => b.BillLines).SingleOrDefaultAsync(b => b.Id == id);
            if (bill == null)
                return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(bill) }, JsonRequestBehavior.DenyGet);
        }

        public async Task<JsonResult> Data(string search, string sort, string order, int? offset, int? limit, string datafilter = null)
        {
            var bills = db.Bills.Include(b => b.Client).Include(b => b.Employee).Include(b => b.Salon).Include(b => b.BillLines);


            int TotalNotFiltered = bills.Count();



            int Total = TotalNotFiltered;
            if (datafilter != null)
            {
                bills = BuildFilter(bills, JsonConvert.DeserializeObject<BillFilter>(datafilter));
                Total = bills.Count();
            }
            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

                bills = bills.Where(b => b.Client.Name.Contains(search) || b.Employee.FamilyName.Contains(search) || b.Salon.Name.Contains(search) || b.BillNumber.ToString().Contains(search) || (b.IssueDate.ToString().Substring(8, 2) + "." + b.IssueDate.ToString().Substring(5, 2) + "." + b.IssueDate.ToString().Substring(0, 4)).Contains(search) || b.StartTime.ToString().Contains(search) || b.EndTime.ToString().Contains(search) || b.BO.ToString().Contains(search) || b.RowVersion.ToString().Contains(search));
                Total = bills.Count();
            }


            if (sort != null)
            {
                bills = Function.OrderBy(bills, sort, order);
            }
            else
            {
                bills = bills.OrderBy(e => e.Id);
            }

            if (serverSide)
            {
                bills = bills.Skip(offset.Value);
                BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await bills.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(GetJsonViewModel(await bills.ToListAsync()), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter([Bind(Include = "IssueDateFrom,IssueDateTo,SalonIdSelected,EmployeeIdSelected,StartTimeFrom,StartTimeTo,EndTimeFrom,EndTimeTo,BO")] BillFilter dataFilter)
        {
            if (ModelState.IsValid)
            {
                if (dataFilter.EmployeeIdSelected != null && db.Employees.Count() == dataFilter.EmployeeIdSelected.Length)
                    dataFilter.EmployeeIdSelected = null;
                if (dataFilter.SalonIdSelected != null && db.Salons.Count() == dataFilter.SalonIdSelected.Length)
                    dataFilter.SalonIdSelected = null;


                var bills = db.Bills.Include(b => b.Client).Include(b => b.Employee).Include(b => b.Salon).Include(b => b.BillLines);

                if (dataFilter != null)
                {
                    bills = BuildFilter(bills, dataFilter);
                }

                int Count = bills.Count();

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

        private IQueryable<Bill> BuildFilter(IQueryable<Bill> bills, BillFilter filter)
        {
            if (filter.IssueDateFrom != null)
                bills = bills.Where(b => b.IssueDate >= filter.IssueDateFrom);
            if (filter.IssueDateTo != null)
                bills = bills.Where(b => b.IssueDate <= filter.IssueDateTo);
            if (filter.SalonIdSelected != null)
            {
                bills = bills.WhereFilter("SalonId", filter.SalonIdSelected);
            }
            if (filter.EmployeeIdSelected != null)
            {
                bills = bills.WhereFilter("EmployeeId", filter.EmployeeIdSelected);
            }
            if (filter.StartTimeFrom != null)
                bills = bills.Where(b => b.StartTime >= filter.StartTimeFrom);
            if (filter.StartTimeTo != null)
                bills = bills.Where(b => b.StartTime <= filter.StartTimeTo);
            if (filter.EndTimeFrom != null)
                bills = bills.Where(b => b.EndTime >= filter.EndTimeFrom);
            if (filter.EndTimeTo != null)
                bills = bills.Where(b => b.EndTime <= filter.EndTimeTo);

            return bills;
        }
        private List<BillJsonViewModel> GetJsonViewModel(List<Bill> baseResponse)
        {
            List<BillJsonViewModel> result = new List<BillJsonViewModel>();
            foreach (Bill item in baseResponse)
            {
                result.Add(new BillJsonViewModel(item));
            }
            return result;
        }
        private BillJsonViewModel GetJsonViewModel(Bill baseResponse)
        {
            return new BillJsonViewModel(baseResponse);
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
        private List<Client> GetClientSelectName(List<Client> clients)
        {
            List<Client> result = new List<Client>();
            foreach (Client client in clients)
            {
                client.Name = client.PhoneNumber + (client.FamilyName != null ? " " + client.FamilyName : "") + " " + client.Name;
                result.Add(client);
            }
            return result;
        }

        private List<Employee> GetEmployeeSelectName(List<Employee> employees)
        {
            List<Employee> result = new List<Employee>();
            foreach (Employee employee in employees)
            {
                employee.Name = employee.FamilyName + " " + employee.Name + (employee.StaffNumber.HasValue ? " " + employee.StaffNumber : "");
                result.Add(employee);
            }
            return result;
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

