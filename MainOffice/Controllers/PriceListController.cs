using MainOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using MainOffice.App_LocalResources;

namespace MainOffice.Controllers
{
    [Authorize(Roles = "admin,director,worksheets")]
    public class PriceListController : Controller
    {
        private AppDbContext db = new AppDbContext();
        // GET: PriceList
        public async Task<ActionResult> Index(int? identity)
        {
            PriceListSelectViewModel viewModel = new PriceListSelectViewModel();
            viewModel.Identity = identity.HasValue ? identity.Value : 0;
            if (viewModel.Identity == 0)
            {
                var identifier = (ClaimsIdentity)User.Identity;
                Claim claim = identifier.Claims.ToList().FirstOrDefault(c => c.Type.Equals(ClaimTypes.SerialNumber));
                if (claim != null)
                {

                    if (int.TryParse(claim.Value, out int x))
                    {
                        viewModel.Identity = x;
                    }
                }
            }
            if (viewModel.Identity != 0)
            {
                
                viewModel.PriceListUnits = await db.PriceListUnits.Include(b => b.Employees).Where(x => x.Employees.Any(e => e.Id == viewModel.Identity)).OrderBy(x => x.Name).ToListAsync();
                viewModel.PriceListUnits.Add(new PriceListUnit() { Id = 0, Name = GlobalRes.All });
            }
            else //if (identity.HasValue && identity.Value == 0)
            {
                viewModel.PriceListUnits = await db.PriceListUnits.Include(x => x.CashRegCodes).Where(x => x.CashRegCodes.Count > 0).OrderBy(x => x.Name).ToListAsync();
            }

            return View(viewModel);
        }
        public async Task<ActionResult> PriceListUnits(int identity, int? fullPrice)
        {
            PriceListSelectViewModel viewModel = new PriceListSelectViewModel();
            viewModel.Identity = identity;
            if (identity != 0 && ((fullPrice.HasValue && fullPrice.Value != 0) | !fullPrice.HasValue))
            {
                viewModel.PriceListUnits = await db.PriceListUnits.Include(b => b.Employees).Where(x => x.Employees.Any(e => e.Id == identity)).OrderBy(x => x.Name).ToListAsync();
                viewModel.PriceListUnits.Add(new PriceListUnit() { Id = 0, Name = GlobalRes.All });
            }
            else
            {
                viewModel.PriceListUnits = await db.PriceListUnits.Include(x => x.CashRegCodes).Where(x => x.CashRegCodes.Count > 0).OrderBy(x => x.Name).ToListAsync();
                if (identity != 0)
                {
                    viewModel.ShowFull = true;
                }
            }
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            return PartialView(viewModel);
        }

        public async Task<ActionResult> CodesList(int identity, int fullPrice, int PriceList)
        {
            PriceListSelectViewModel viewModel = new PriceListSelectViewModel();
            viewModel.Identity = identity;
            viewModel.ShowFull = fullPrice == 0;
            viewModel.CashRegCodes = await db.CashRegCodes.Include(c => c.Product).Include(c => c.Product.ProductVolume).Include(c => c.Product.Trademark).Include(c => c.Product.Productline).Include(c => c.Service).Include(c => c.Service.ServiceVolume).Where(x => x.PriceListUnitId == PriceList).OrderBy(x=> x.Code).ToListAsync();
            return PartialView(viewModel);
        }
    }
}