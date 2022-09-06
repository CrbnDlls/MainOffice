using MainOffice.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MainOffice.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                if (User.IsInRole("pin"))
                {
                    return RedirectToAction("Index", "PinLock");
                }
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        

        public ActionResult GetResolution(string act, string width, string height, string returncontroller, string returnaction)
        {
            if (!String.IsNullOrEmpty(act))
            {
                Session["ScreenResolution"] = width + "x" + height;
                return RedirectToAction(returnaction, returncontroller);
            }
            else
            {
                return PartialView("GetResolution", new string[] { returncontroller, returnaction });
            }
        }

        public ActionResult GetGeoLocation(string act, string longitude, string latitude, string returncontroller, string returnaction, string error)
        {
            if (!String.IsNullOrEmpty(act))
            {
                if (String.IsNullOrEmpty(error))
                { 
                    Session["Longitude"] = longitude;
                    Session["Latitude"] = latitude;
                }
                else
                {
                    Session["GeoError"] = error;
                }
                return RedirectToAction(returnaction, returncontroller);
            }
            else
            {
                return PartialView("GetGeoLocation", new string[] { returncontroller, returnaction });
            }
        }

        public ActionResult NotFound(NotFoundViewModel model)
        { return PartialView(model); }

    }
}