using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MainOffice
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            string culture = "uk-UA";
            var identity = (ClaimsIdentity)User.Identity;
            Claim claim = identity.Claims.ToList().FirstOrDefault(c => c.Type.Equals(ClaimTypes.Locality));
            if (claim != null)
            {
                switch (claim.Value)
                {
                    case "ru-RU":
                        culture = "ru-RU";
                        break;
                    case "uk-UA":
                        culture = "uk-UA";
                        break;
                    default:
                        culture = "uk-UA";
                        break;
                }
            }
            else
            {
                if (Request.UserLanguages != null)
                {
                    if (Request.UserLanguages[0].Substring(0,2) == "ru")
                    {
                        culture = "ru-RU";
                    }
                }
            }
            
            
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        }
    }
}
