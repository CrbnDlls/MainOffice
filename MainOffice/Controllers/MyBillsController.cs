using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using MainOffice.Functions;
using MainOffice.Models;
using System.Net.Http;
using System.Text;

namespace MainOffice.Controllers
{
    [Authorize(Roles = "worksheets")]
    public class MyBillsController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private ApplicationDbContext db1 = new ApplicationDbContext();
        // GET: MyBills
        public ActionResult Index()
        {
            if (Session["OperEmployee"] != null)
            {
                int Id = (int)Session["OperEmployee"];
                OperationDayEmployee opEmployee = db.OperationDayEmployees.Include(i => i.Employee).Single(x => x.Id == Id);
                ViewBag.Name = "№"+ opEmployee.Employee.StaffNumber + " " + opEmployee.Employee.FamilyName + " " + opEmployee.Employee.Name;
            }
            return View(new List<MyBillsViewModel>());
        }
        public async Task<ActionResult> Data()
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            if (opEmployee != null)
            {
                List<MyBillsViewModel> myBills = new List<MyBillsViewModel>();
                foreach (OperDayBill bill in opEmployee.OperDayBills)
                {
                    if (opEmployee.OperationDay.Alarm & bill.IsHidden())
                        continue;
                    MyBillsViewModel myBill = new MyBillsViewModel(bill);
                    myBills.Add(myBill);
                }
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { result = "success", myBills }
                };
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result = "error", message = "You are not registered" }
            };
        }

        public async Task<ActionResult> Info(int? Id)
        {
            if (Id != null & Id != 0)
            { 
                OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
                if (opEmployee != null)
                {
                        return PartialView(opEmployee.OperDayBills.Single(x => x.Id == Id));    
                }
            
            }
            return null;
        }

        public async Task<ActionResult> Create()
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            Random random = new Random();

            OperDayBill bill = new OperDayBill() { BillNumber = random.Next(10000, 99999) };
            bill.OperationDayEmployee = opEmployee;
            return PartialView(bill);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int BillNumber, CodeQuantity[] list, string discount)
        {
            
            
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            if (opEmployee != null)
            {
                if (discount == "staff")
                    discount = "Сотрудник";
                OperDayBill bill = new OperDayBill();
                bill.BillNumber = BillNumber;
                bill.BillLines = new List<OperDayBillLine>();
                foreach (CodeQuantity code in list)
                {
                    CashRegCode regCode = await db.CashRegCodes.Include(c => c.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.Service.ServiceVolume).SingleAsync(x => x.Code == code.Code);
                    OperDayBillLine line = new OperDayBillLine() { CashRegCode = regCode.Code };
                    line.ProductOrServiceName = regCode.GetCodeName();
                    line.MaxPrice = regCode.Price;
                    line.SellPrice = regCode.GetPriceAccordingToDiscount(discount);
                    line.InsertDateTime = DateTime.Now;
                    line.Promotion = discount;
                    line.Quantity = code.Quantity;
                    bill.BillLines.Add(line);
                }
                bill.OperationDayEmployeeId = opEmployee.Id;
                bill.StartTime = DateTime.Now;
                db.OperDayBills.Add(bill);
                string[] saveResult = await Function.SaveChangesToDb(db);
                if (saveResult[0] == "success")
                {

                    return Json(new { result = "success", bill = new MyBillsViewModel(bill) }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                        Data = new { result = "error", message = saveResult[1] }
                    };
                }
            }
            
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error", message = "" }
            };
        }
        [HttpPost]
        public async Task<ActionResult> GetLinePrice(int codeId, string discount)
        {
            if (discount == "staff")
                discount = "Сотрудник";
            try
            {
                CashRegCode regCode = await db.CashRegCodes.Include(c => c.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.Service.ServiceVolume).SingleAsync(x => x.Id == codeId);
                OperDayBillLine line = new OperDayBillLine() { CashRegCode = regCode.Code };
                line.ProductOrServiceName = regCode.GetCodeName();
                line.SellPrice = regCode.GetPriceAccordingToDiscount(discount);
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = "success", data = line }
                };
            }
            catch { }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error", message = "" }
            };
        }
        [HttpPost]
        public async Task<ActionResult> GetBillPrices(OperDayBill bill, string discount)
        {
            if (discount == "staff")
                discount = "Сотрудник";
            if (bill.BillLines != null)
            {
                for (int i = 0; i < bill.BillLines.Count; i++)
                {
                    try
                    {
                        int code = bill.BillLines[i].CashRegCode;
                        CashRegCode regCode = await db.CashRegCodes.SingleAsync(x => x.Code == code);
                        bill.BillLines[i].SellPrice = regCode.GetPriceAccordingToDiscount(discount);

                    }
                    catch (Exception e)
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = e.Message }
                        };
                    }
                }
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "success", bill }
            };

        }

        public async Task<ActionResult> Edit(int? id)
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            if (opEmployee != null & id.HasValue)
            {
                try
                {
                    OperDayBill bill = await db.OperDayBills.SingleAsync(x => x.Id == id.Value & x.OperationDayEmployeeId == opEmployee.Id);
                    if (bill.Locked)
                    {
                        if (bill.WhoLockedId != opEmployee.EmployeeId)
                            return PartialView("_UnlockEdit", new UnlockEditViewModel() { WhoLocked = bill.WhoLocked.FamilyName + " " + bill.WhoLocked.Name.Substring(0, 1) + "." + bill.WhoLocked.FathersName.Substring(0, 1) + ".", MayUnlock = false, Id = bill.Id, Version = Convert.ToBase64String(bill.RowVersion) });
                    }
                    bill.Locked = true;
                    bill.WhoLockedId = opEmployee.EmployeeId;
                    db.Entry(bill).State = EntityState.Modified;
                    string[] result = await Function.SaveChangesToDb(db);
                    if (result[0] == "success")
                        return PartialView(bill);
                }
                catch
                { }
            }
            return null;
        }
        public async Task<ActionResult> EditData(int? id)
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            if (opEmployee != null & id.HasValue)
            {
                try
                {
                    MyBillsViewModel bill = new MyBillsViewModel(await db.OperDayBills.Include(i => i.WhoLocked).SingleAsync(x => x.Id == id.Value & x.OperationDayEmployeeId == opEmployee.Id));
                    
                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new { result = "success", bill }
                    };
                }
                catch
                { }
                
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result = "error", message = "You are not registered" }
            };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int Id, List<CodeQuantity> list, string discount, byte[] version)
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            if (opEmployee != null)
            {
                if (discount == "staff")
                    discount = "Сотрудник";
                try
                {
                    OperDayBill bill = opEmployee.OperDayBills.Single(x => x.Id == Id & !x.EndTime.HasValue & x.RowVersion.SequenceEqual(version));
                    bill.Locked = false;
                    bill.WhoLockedId = null;
                    db.Entry(bill).State = EntityState.Modified;
                    foreach (OperDayBillLine line in bill.BillLines.ToList())
                    {
                        CodeQuantity editData;
                        try
                        {
                            editData = list.Single(x => x.Id == line.Id); 
                        }
                        catch
                        {
                            line.Cancel = true;
                            line.CancelDateTime = DateTime.Now;
                            if (opEmployee.Employee.ProfessionId == 1)
                                line.AdminVisaId = opEmployee.EmployeeId;
                            db.Entry(line).State = EntityState.Modified;
                            continue;
                        }

                        if (editData.Quantity != line.Quantity | line.Promotion != discount )
                        {
                            line.Cancel = true;
                            line.CancelDateTime = DateTime.Now;
                            db.Entry(line).State = EntityState.Modified;

                            CashRegCode regCode = await db.CashRegCodes.Include(c => c.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.Service.ServiceVolume).SingleAsync(x => x.Code == line.CashRegCode);
                            OperDayBillLine line1 = new OperDayBillLine() { CashRegCode = regCode.Code };
                            line1.ProductOrServiceName = regCode.GetCodeName();
                            line1.MaxPrice = regCode.Price;
                            line1.SellPrice = regCode.GetPriceAccordingToDiscount(discount);
                            line1.InsertDateTime = DateTime.Now;
                            line1.Promotion = discount;
                            line1.Quantity = editData.Quantity;
                            line1.OperDayBillId = line.OperDayBillId;
                            db.OperDayBillLines.Add(line1);
                        }
                    }
                    foreach (CodeQuantity code in list)
                    {
                        if (code.Id == 0)
                        {
                            CashRegCode regCode = await db.CashRegCodes.Include(c => c.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.Service.ServiceVolume).SingleAsync(x => x.Code == code.Code);
                            OperDayBillLine line = new OperDayBillLine() { CashRegCode = regCode.Code };
                            line.ProductOrServiceName = regCode.GetCodeName();
                            line.MaxPrice = regCode.Price;
                            line.SellPrice = regCode.GetPriceAccordingToDiscount(discount);
                            line.InsertDateTime = DateTime.Now;
                            line.Promotion = discount;
                            line.Quantity = code.Quantity;
                            line.OperDayBillId = bill.Id;
                            db.OperDayBillLines.Add(line);
                        }
                    }

                    string[] saveResult = await Function.SaveChangesToDb(db);
                    bill = await db.OperDayBills.SingleAsync(x => x.Id == bill.Id);
                    if (saveResult[0] == "success")
                    {

                        return Json(new { result = "success", bill = new MyBillsViewModel(bill) }, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = saveResult[1] }
                        };
                    }
                }
                catch (Exception e)
                {
                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                        Data = new { result = "error", message = e.Message }
                    };
                }
                
            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error", message = "" }
            };

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CloseBill(int Id)
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            if (opEmployee != null)
            {
                try
                {
                    OperDayBill bill = opEmployee.OperDayBills.Single(x => x.Id == Id & x.EndTime == null);
                    if (bill.Locked && bill.WhoLockedId != opEmployee.EmployeeId)
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = "Счет №" + bill.BillNumber + " сейчас редактирует " + bill.WhoLocked.FamilyName + " " + bill.WhoLocked.Name.Substring(0, 1) + "." + bill.WhoLocked.FathersName.Substring(0, 1) + "." }
                        };
                    }
                    if (bill.RequireAdminVisa())
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = "Счет №" + bill.BillNumber + " требует подписи администратора." }
                        };
                    }
                    bill.Locked = false;
                    bill.WhoLockedId = null;
                    bill.EndTime = DateTime.Now;
                    db.Entry(bill).State = EntityState.Modified;
                    //print bill
                    OperDayBillPrint print = new OperDayBillPrint() { Id = bill.Id, Count = 0 };
                    db.OperDayBillPrints.Add(print);
                    string[] saveResult = await Function.SaveChangesToDb(db);
                    if (saveResult[0] == "success")
                    {
                        if (bill.BillLines != null && bill.BillLines.Count > 0 & bill.BillLines.Any(x => x.Cancel == false))
                            await PrintBill(bill);
                        bill = await db.OperDayBills.SingleAsync(x => x.Id == bill.Id);
                        return Json(new { result = "success", bill = new MyBillsViewModel(bill) }, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "error", message = saveResult[1] }
                        };
                    }
                }
                catch (Exception e)
                { }

            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error", message = "You are not registered" }
            };

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FinishEdit(int Id, byte[] version)
        {
            OperationDayEmployee opEmployee = await GetOPEmployeeFromUser();
            if (opEmployee != null)
            {
                try
                {
                    OperDayBill bill = opEmployee.OperDayBills.Single(x => x.Id == Id & x.RowVersion.SequenceEqual(version));
                    if (bill.Locked && bill.WhoLockedId == opEmployee.EmployeeId)
                    {
                        bill.Locked = false;
                        bill.WhoLockedId = null;
                        db.Entry(bill).State = EntityState.Modified;
                        await Function.SaveChangesToDb(db);
                    }
                }
                catch (Exception e)
                {

                }
            }
            return null;
        }
        
        private async Task<OperationDayEmployee> GetOPEmployeeFromUser()
        {
            if (Session["OperEmployee"] != null)
            {
                int Id = (int)Session["OperEmployee"];
                return await db.OperationDayEmployees.Include(v => v.OperationDay.Salon).Include(i => i.OperDayBills.Select(s => s.WhoLocked)).Include(i => i.Employee.BarberLevel).Include(i => i.Employee.Profession).SingleOrDefaultAsync(y => (y.Id == Id) & (y.EndPoint == null));
            }
            string userId = User.Identity.GetUserId();
            try { 
                int? employeeId = db1.Users.Single(x => x.Id == userId).EmployeeId;

                if (employeeId.HasValue)
                {
                    return await db.OperationDayEmployees.Include(v => v.OperationDay.Salon).Include(i => i.OperDayBills.Select(s => s.WhoLocked)).Include(i => i.Employee.BarberLevel).Include(i => i.Employee.Profession).SingleOrDefaultAsync(y => (y.EmployeeId == employeeId.Value) & (y.EndPoint == null));
                }
            }
            catch (Exception e)
            { }


            return null;
            
        }

        private async Task<string> PrintBill(OperDayBill bill)
        {
            string responseString = null;
            string test = Convert.ToBase64String((byte[])Drawing.DrawBill(bill));
            using (HttpClient httpClient = new HttpClient() { })
            {
                httpClient.Timeout = new TimeSpan(0, 0, 10);

                PrintBillContent printBill = new PrintBillContent() { action = "print", bill = Convert.ToBase64String((byte[])Drawing.DrawBill(bill)) };
                try
                {
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(printBill), Encoding.UTF8, "application/json");
                    try
                    {
                        var response = await httpClient.PostAsync("http://" + bill.OperationDayEmployee.OperationDay.Salon.IP + ":1522", content);
                        //var response = await httpClient.PostAsync("http://" + "192.168.1.49" + ":1521", content);

                        responseString = await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception e)
                    { }
                }
                catch (Exception e)
                { }
            }
            if (responseString == "OK")
            {
                OperDayBillPrint operDayBillPrint = db.OperDayBillPrints.Single(x => x.Id == bill.Id);
                operDayBillPrint.Count = operDayBillPrint.Count + 1;
                db.Entry(operDayBillPrint).State = EntityState.Modified;
                await Function.SaveChangesToDb(db);
            }
            return responseString;
        }
       
        public class CodeQuantity
        {
            public int Id { get; set; }
            public int Code { get; set; }
            public int Quantity { get; set; }
        }
    }
}