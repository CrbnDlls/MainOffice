using MainOffice.Functions;
using MainOffice.Models;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MainOffice.Controllers
{
    [Authorize(Roles = "admin,director,secretary,buh")]
    public class CheckBillsController : Controller
    {
        OracleConnection conn = OraDb.SetConnection("VPP", "LONDA", "DWnKp5z1");
        // GET: CheckBills
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
            return View();
        }


        // GET: CheckBills/Create
        [Authorize(Roles = "admin,buh")]
        public ActionResult Create()
        {
            OracleCommand cmd = new OracleCommand("SELECT IDN, SNAME FROM LONDA.SALONS WHERE IDN <> 0", conn);
            List<AnySelectList> SalonList = OraDb.GetSalons(cmd, true);
            
            ViewBag.Salon = new SelectList(SalonList, "Id", "Name");

            cmd = new OracleCommand("SELECT FNAME || ' ' || FSTNAME || ' ' || STAFFID, FNAME || ' ' || FSTNAME || ' ' || STAFFID FROM LONDA.STAFF WHERE STAFFID IS NOT NULL AND HIREDATE IS NOT NULL AND DROPDATE IS NULL ORDER BY FNAME ASC, FSTNAME ASC", conn);
            List<AnySelectList> WorkerList = OraDb.GetWorker(cmd, true);

            ViewBag.Worker = new SelectList(WorkerList, "Id", "Name");

            List<AnySelectList> BOList = new List<AnySelectList> { new AnySelectList() { Id = "0", Name = "Нет" }, new AnySelectList() { Id = "1", Name = "Да" } };
            ViewBag.BO = new SelectList(BOList, "Id", "Name");

            cmd = new OracleCommand("SELECT s.SERV_NAME || ' ' || v.VNAME FROM LONDA.SERV_INFO s, LONDA.SERV_VOL v WHERE s.VOL = v.IDN", conn);
            ViewBag.Services = OraDb.GetServiceNames(cmd, true);

            List<AnySelectList> disc = new List<AnySelectList> { new AnySelectList() { Id = "", Name = "Нет" }, new AnySelectList() { Id = "10%", Name = "10%" }, new AnySelectList() { Id = "с", Name = "Сотрудник" } };
            ViewBag.Discount = new SelectList(disc, "Id", "Name");

            Random number = new Random();

            return PartialView(new TemporaryBillModel() {BillNumber = number.Next(10000, 99999), BillLines = new List<TemporaryBillLine>() { new TemporaryBillLine() } });
        }

        // POST: CheckBills/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,buh")]
        public ActionResult Create(TemporaryBillModel bill)
        {
            if (ModelState.IsValid)
            {
                int idn = OraDb.SaveBill(bill, conn);
                if (idn != -1)
                {
                    OracleCommand cmd2 = new OracleCommand("SELECT b.IDN, b.NUM, b.DAT, s.SNAME, b.WORKER, b.STIME, SUM(d.SELLPRICE * d.QUANTITY), SUM(d.ACTUAL), SUM(d.BO), SUM(d.PRICE * d.QUANTITY) FROM LONDA.BILLSTOCHECK b, LONDA.BILLSDATATOCHECK d, LONDA.SALONS s WHERE b.IDN = d.BILL AND b.SALON = s.IDN AND b.IDN = " + idn + " GROUP BY b.IDN, b.NUM, b.DAT, s.SNAME, b.WORKER, b.STIME", conn);

                    bill = OraDb.GetBill(cmd2, true);

                    cmd2 = new OracleCommand("SELECT SCODE, SNAME, PRICE, QUANTITY, DISC, SELLPRICE, BO FROM LONDA.BILLSDATATOCHECK WHERE BILL = " + bill.id, conn);

                    bill.BillLines = OraDb.GetBillLines(cmd2, true, bill.id);

                    return Json(new { result = "success", data = bill }, JsonRequestBehavior.AllowGet);
                }
                
            }
            OracleCommand cmd = new OracleCommand("SELECT IDN, SNAME FROM LONDA.SALONS WHERE IDN <> 0", conn);
            List<AnySelectList> SalonList = OraDb.GetSalons(cmd, true);

            ViewBag.Salon = new SelectList(SalonList, "Id", "Name");

            cmd = new OracleCommand("SELECT FNAME || ' ' || FSTNAME || ' ' || STAFFID, FNAME || ' ' || FSTNAME || ' ' || STAFFID FROM LONDA.STAFF WHERE STAFFID IS NOT NULL AND HIREDATE IS NOT NULL AND DROPDATE IS NULL ORDER BY FNAME ASC, FSTNAME ASC", conn);
            List<AnySelectList> WorkerList = OraDb.GetWorker(cmd, true);

            ViewBag.Worker = new SelectList(WorkerList, "Id", "Name");

            List<AnySelectList> BOList = new List<AnySelectList> { new AnySelectList() { Id = "0", Name = "Нет" }, new AnySelectList() { Id = "1", Name = "Да" } };
            ViewBag.BO = new SelectList(BOList, "Id", "Name");

            cmd = new OracleCommand("SELECT s.SERV_NAME || ' ' || v.VNAME FROM LONDA.SERV_INFO s, LONDA.SERV_VOL v WHERE s.VOL = v.IDN", conn);
            ViewBag.Services = OraDb.GetServiceNames(cmd, true);

            List<AnySelectList> disc = new List<AnySelectList> { new AnySelectList() { Id = "", Name = "Нет" }, new AnySelectList() { Id = "10%", Name = "10%" }, new AnySelectList() { Id = "c", Name = "Сотрудник" } };
            ViewBag.Discount = new SelectList(disc, "Id", "Name");

            return PartialView(bill);
        }

        // GET: CheckBills/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CheckBills/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Actual(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OracleCommand cmd = new OracleCommand("UPDATE LONDA.BILLSDATATOCHECK SET ACTUAL = 0 WHERE BILL = " + id, conn);
            OraDb.ExecuteNonQuery(cmd, true);

            OracleCommand cmd2 = new OracleCommand("SELECT b.IDN, b.NUM, b.DAT, s.SNAME, b.WORKER, b.STIME, SUM(d.SELLPRICE * d.QUANTITY), SUM(d.ACTUAL), SUM(d.BO), SUM(d.PRICE * d.QUANTITY) FROM LONDA.BILLSTOCHECK b, LONDA.BILLSDATATOCHECK d, LONDA.SALONS s WHERE b.IDN = d.BILL AND b.SALON = s.IDN AND b.IDN = " + id + " GROUP BY b.IDN, b.NUM, b.DAT, s.SNAME, b.WORKER, b.STIME", conn);

            TemporaryBillModel bill = OraDb.GetBill(cmd2, true);

            cmd2 = new OracleCommand("SELECT SCODE, SNAME, PRICE, QUANTITY, DISC, SELLPRICE, BO FROM LONDA.BILLSDATATOCHECK WHERE BILL = " + bill.id, conn);

            bill.BillLines = OraDb.GetBillLines(cmd2, true, bill.id);

            return Json(new { result = "success", data = bill }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Mistake(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OracleCommand cmd = new OracleCommand("UPDATE LONDA.BILLSDATATOCHECK SET ACTUAL = 1 WHERE BILL = " + id, conn);
            OraDb.ExecuteNonQuery(cmd, true);

            OracleCommand cmd2 = new OracleCommand("SELECT b.IDN, b.NUM, b.DAT, s.SNAME, b.WORKER, b.STIME, SUM(d.SELLPRICE * d.QUANTITY), SUM(d.ACTUAL), SUM(d.BO), SUM(d.PRICE * d.QUANTITY) FROM LONDA.BILLSTOCHECK b, LONDA.BILLSDATATOCHECK d, LONDA.SALONS s WHERE b.IDN = d.BILL AND b.SALON = s.IDN AND b.IDN = " + id + " GROUP BY b.IDN, b.NUM, b.DAT, s.SNAME, b.WORKER, b.STIME", conn);

            TemporaryBillModel bill = OraDb.GetBill(cmd2, true);

            cmd2 = new OracleCommand("SELECT SCODE, SNAME, PRICE, QUANTITY, DISC, SELLPRICE, BO FROM LONDA.BILLSDATATOCHECK WHERE BILL = " + bill.id,conn);

            bill.BillLines = OraDb.GetBillLines(cmd2, true, bill.id);

            return Json(new { result = "success", data = bill }, JsonRequestBehavior.AllowGet);
        }

        // GET: CheckBills/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CheckBills/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public JsonResult Data(string search, string sort, string order, int? offset, int? limit, string datafilter = null)
        {
            OracleCommand cmd = new OracleCommand("SELECT b.IDN, b.NUM, b.DAT, s.SNAME, b.WORKER, b.STIME, SUM(d.SELLPRICE * d.QUANTITY), SUM(d.ACTUAL), SUM(d.BO), SUM(d.PRICE * d.QUANTITY) FROM LONDA.BILLSTOCHECK b, LONDA.BILLSDATATOCHECK d, LONDA.SALONS s WHERE b.IDN = d.BILL AND b.SALON = s.IDN AND b.DAT = TO_DATE(\'" + datafilter + "\','YYYY-MM-DD') GROUP BY b.IDN, b.NUM, b.DAT, s.SNAME, b.WORKER, b.STIME ORDER BY s.SNAME DESC, b.STIME ASC", conn);
            
            List<TemporaryBillModel> result = OraDb.GetBills(cmd, true);

            for (int i=0; i<result.Count;i++)
            {
                cmd = new OracleCommand("SELECT SCODE, SNAME, PRICE, QUANTITY, DISC, SELLPRICE, BO FROM LONDA.BILLSDATATOCHECK WHERE BILL = " + result[i].id, conn);
                result[i].BillLines = OraDb.GetBillLines(cmd, true, result[i].id);
            }
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Report(string start, string end)
        {
            OracleCommand cmd = new OracleCommand("SELECT b.WORKER, b.WORKERNUM, d.SNAME FROM LONDA.BILLSTOCHECK b, LONDA.BILLSDATATOCHECK c, LONDA.SALONS d WHERE b.IDN = c.BILL AND d.IDN = b.SALON AND b.DAT >= to_date(\'" + start + "\','YYYY-MM-DD') AND b.DAT <= to_date(\'" + end + "\','YYYY-MM-DD') GROUP BY d.SNAME, b.WORKERNUM, b.WORKER ORDER BY d.SNAME DESC, b.WORKERNUM ASC", conn);
            List<TemporaryBillReport> result = OraDb.GetReport(cmd, true);
            cmd = new OracleCommand("SELECT SUM(c.SELLPRICE*c.QUANTITY), b.WORKER, b.WORKERNUM, d.SNAME FROM LONDA.BILLSTOCHECK b, LONDA.BILLSDATATOCHECK c, LONDA.SALONS d WHERE (c.DISC IS NULL OR (c.DISC <> 'с' AND c.DISC <> '10%')) AND C.BO = 0 AND c.ACTUAL = 0 AND b.IDN = c.BILL AND d.IDN = b.SALON AND b.DAT >= to_date(\'" + start + "\','YYYY-MM-DD') AND b.DAT <= to_date(\'" + end + "\','YYYY-MM-DD') GROUP BY d.SNAME, b.WORKERNUM, b.WORKER ORDER BY d.SNAME DESC", conn);
            result = OraDb.GetReport1(cmd, true,result);
            cmd = new OracleCommand("SELECT SUM(c.SELLPRICE*c.QUANTITY), b.WORKER, b.WORKERNUM, d.SNAME FROM LONDA.BILLSTOCHECK b, LONDA.BILLSDATATOCHECK c, LONDA.SALONS d WHERE c.DISC = '10%' AND C.BO = 0 AND c.ACTUAL = 0 AND b.IDN = c.BILL AND d.IDN = b.SALON AND b.DAT >= to_date(\'" + start + "\','YYYY-MM-DD') AND b.DAT <= to_date(\'" + end + "\','YYYY-MM-DD') GROUP BY d.SNAME, b.WORKERNUM, b.WORKER ORDER BY d.SNAME DESC", conn);
            result = OraDb.GetReport2(cmd, true, result);
            cmd = new OracleCommand("SELECT SUM(c.SELLPRICE*c.QUANTITY), b.WORKER, b.WORKERNUM, d.SNAME FROM LONDA.BILLSTOCHECK b, LONDA.BILLSDATATOCHECK c, LONDA.SALONS d WHERE c.DISC = 'с' AND C.BO = 0 AND c.ACTUAL = 0 AND b.IDN = c.BILL AND d.IDN = b.SALON AND b.DAT >= to_date(\'" + start + "\','YYYY-MM-DD') AND b.DAT <= to_date(\'" + end + "\','YYYY-MM-DD') GROUP BY d.SNAME, b.WORKERNUM, b.WORKER ORDER BY d.SNAME DESC", conn);
            result = OraDb.GetReport3(cmd, true, result);
            cmd = new OracleCommand("SELECT SUM(c.PRICE*c.QUANTITY), b.WORKER, b.WORKERNUM, d.SNAME FROM LONDA.BILLSTOCHECK b, LONDA.BILLSDATATOCHECK c, LONDA.SALONS d WHERE C.BO = 1 AND c.ACTUAL = 0 AND b.IDN = c.BILL AND d.IDN = b.SALON AND b.DAT >= to_date(\'" + start + "\','YYYY-MM-DD') AND b.DAT <= to_date(\'" + end + "\','YYYY-MM-DD') GROUP BY d.SNAME, b.WORKERNUM, b.WORKER ORDER BY d.SNAME DESC", conn);
            result = OraDb.GetReport4(cmd, true, result);
            cmd = new OracleCommand("SELECT COUNT(DISTINCT b.IDN), b.WORKER, b.WORKERNUM, d.SNAME FROM LONDA.BILLSTOCHECK b, LONDA.BILLSDATATOCHECK c, LONDA.SALONS d WHERE b.IDN = c.BILL AND d.IDN = b.SALON AND b.DAT >= to_date(\'" + start + "\','YYYY-MM-DD') AND b.DAT <= to_date(\'" + end + "\','YYYY-MM-DD') GROUP BY d.SNAME, b.WORKERNUM, b.WORKER", conn);
            result = OraDb.GetReport5(cmd, true, result);
            cmd = new OracleCommand("SELECT COUNT(DISTINCT b.IDN), b.WORKER, b.WORKERNUM, d.SNAME FROM LONDA.BILLSTOCHECK b, LONDA.BILLSDATATOCHECK c, LONDA.SALONS d WHERE c.ACTUAL = 1 AND b.IDN = c.BILL AND d.IDN = b.SALON AND b.DAT >= to_date(\'" + start + "\','YYYY-MM-DD') AND b.DAT <= to_date(\'" + end + "\','YYYY-MM-DD') GROUP BY d.SNAME, b.WORKERNUM, b.WORKER", conn);
            result = OraDb.GetReport6(cmd, true, result);
            TemporaryBillReportView report = new TemporaryBillReportView();
            report.start = start;
            report.end = end;
            report.reportsSalon = new List<TemporaryBillReportSalon>();
            IEnumerable<string> salons = result.Select(v => v.Salon).Distinct();
            foreach (string salon in salons)
            {
                TemporaryBillReportSalon reportSalon = new TemporaryBillReportSalon();
                reportSalon.Salon = salon;
                reportSalon.reports = result.Where(v => v.Salon == salon).ToList();
                reportSalon.Summ = reportSalon.reports.Sum(v => v.Summ);
                reportSalon.SummBO = reportSalon.reports.Sum(v => v.SummBO);
                reportSalon.SummDisc10 = reportSalon.reports.Sum(v => v.SummDisc10);
                reportSalon.SummDiscStaff = reportSalon.reports.Sum(v => v.SummDiscStaff);
                reportSalon.BillQuantity = reportSalon.reports.Sum(v => v.BillQuantity);
                reportSalon.BillErrorQuantity = reportSalon.reports.Sum(v => v.BillErrorQuantity);
                report.reportsSalon.Add(reportSalon);
            }
            return View(report); 
        }

    }
}
