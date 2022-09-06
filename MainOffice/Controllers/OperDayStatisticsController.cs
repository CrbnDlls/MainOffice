using MainOffice.Functions;
using MainOffice.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MainOffice.Controllers
{
    [Authorize(Roles = "admin,director,owner")]
    public class OperDayStatisticsController : Controller
    {
        private AppDbContext db = new AppDbContext();
        // GET: OperDayStatistics
        public async Task<ActionResult> Index()
        {
            List<OperDayStatisticsViewModel> viewModel = new List<OperDayStatisticsViewModel>();
            List<OperationDay> operationDays = await db.OperationDays.Include(i => i.Salon).Include(i => i.OpenEmployee).ToListAsync();
            foreach (OperationDay day in operationDays)
            {
                viewModel.Add(new OperDayStatisticsViewModel(day,false, true));
            }

            return View(viewModel);
        }
        public async Task<ActionResult> Details(int Id)
        {
            OperDayStatisticsViewModel viewModel = null;
            try
            { 
                OperationDay operationDay = await db.OperationDays.Include(i => i.Salon).Include(i => i.OpenEmployee).Include(i => i.OperationDayEmployees.Select(s=> s.CloseEmployee)).Include(i => i.OperationDayEmployees.Select(s => s.StartEmployee)).Include(i => i.OperationDayEmployees.Select(s => s.Employee.BarberLevel)).Include(i => i.OperationDayEmployees.Select(s => s.Employee.Profession)).SingleAsync(x => x.Id == Id);
                viewModel = new OperDayStatisticsViewModel(operationDay,true,true);
            }
            catch (Exception e)
            {
                ViewBag.errorMessage = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
                return View("Error");
            }
            return View(viewModel);
        }
        public async Task<ActionResult> DisableAlarm(int Id)
        {
            try
            {
                OperationDay operationDay = await db.OperationDays.SingleAsync(x => x.Id == Id);
                operationDay.Alarm = false;
                db.Entry(operationDay).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
                if (saveResult[0] == "success")
                {
                    return Json(new { result = "success", data = Id }, JsonRequestBehavior.DenyGet);
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
    }
}
