using MainOffice.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MainOffice.Controllers
{
    [Authorize(Roles = "pin")]
    public class PinLockController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private ApplicationDbContext db1 = new ApplicationDbContext();
        // GET: PinLock
        public ActionResult Index()
        {
            Session["OperEmployee"] = null;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Check(string pin)
        {
            if (int.TryParse(pin, out int code))
            {
                string userId = User.Identity.GetUserId();
                int? salonId = db1.Users.Single(x => x.Id == userId).SalonId;
                if (db.Salons.Any(x => Request.UserHostAddress == "192.168.1.1" ? x.IP == "195.177.73.220" & x.Id == (salonId.HasValue ? salonId.Value : 0) : x.IP == Request.UserHostAddress & x.Id == (salonId.HasValue ? salonId.Value : 0)))
                {
                    try
                    {
                        OperationDayEmployee employee = await db.OperationDayEmployees.SingleAsync(x => x.pin == code & x.OperationDay.SalonId == salonId & x.EndPoint == null);
                        Session["OperEmployee"] = employee.Id;
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                            Data = new { result = "success" }
                        };
                    }
                    catch
                    { }
                }
                //try
                //{
                //    OperationDayEmployee employee = await db.OperationDayEmployees.SingleAsync(x => x.pin == code & x.OperationDay.SalonId == salonId & x.EndPoint == null);
                //    Session["OperEmployee"] = employee.Id;
                //    return new JsonResult()
                //    {
                //        JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                //        Data = new { result = "success" }
                //    };
                //}
                //catch
                //{ }
                //Delete Above and uncomment after debug 
            }
        
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = "error" }
            };
        }
    }
}