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
    public class ProductsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Products

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
			var products = db.Products.Include(p => p.Productline).Include(p => p.ProductVolume).Include(p => p.Trademark);
			ViewBag.ServerSide = false;
			
				if (products.Count() > 400)
                ViewBag.ServerSide = true;
			 
            ProductFilter filter = new ProductFilter();
			            
					List<Productline> ProductlineList = db.Productlines.ToList();
						ProductlineList.Insert(0, new Productline() { Name = GlobalRes.Empty });
						ViewBag.ProductlinesSelectList = new MultiSelectList(ProductlineList, "Id", "Name");
            
						List<ProductVolume> ProductVolumeList = db.ProductVolumes.ToList();
						ViewBag.ProductVolumesSelectList = new MultiSelectList(ProductVolumeList, "Id", "Name");
            
						List<Trademark> TrademarkList = db.Trademarks.ToList();
						TrademarkList.Insert(0, new Trademark() { Name = GlobalRes.Empty });
						ViewBag.TrademarksSelectList = new MultiSelectList(TrademarkList, "Id", "Name");
            
						ViewBag.Filter = filter;
            			ViewBag.Edit = Edit.HasValue ? Edit.Value : 0;
			return View(new List<Product>() { });
		}

        // GET: Products/Create
        public ActionResult Create()
        {
						List<Productline> ProductlineList = db.Productlines.ToList();
							ProductlineList.Insert(0, new Productline() { Name = GlobalRes.Empty });
			
            ViewBag.ProductlineId = new SelectList(ProductlineList, "Id", "Name");
			List<ProductVolume> ProductVolumeList = db.ProductVolumes.ToList();
			
            ViewBag.ProductVolumeId = new SelectList(ProductVolumeList, "Id", "Name");
			List<Trademark> TrademarkList = db.Trademarks.ToList();
							TrademarkList.Insert(0, new Trademark() { Name = GlobalRes.Empty });
			
            ViewBag.TrademarkId = new SelectList(TrademarkList, "Id", "Name");
            return PartialView();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,TrademarkId,ProductlineId,ProductVolumeId,RowVersion")] Product product)
        {
            if (ModelState.IsValid)
            {
							product.ProductlineId = SetValueToNull(product.ProductlineId);
								product.ProductVolumeId = product.ProductVolumeId;
				product.TrademarkId = SetValueToNull(product.TrademarkId);
				                db.Products.Add(product);
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				product = await db.Products.Include(p => p.Productline).Include(p => p.ProductVolume).Include(p => p.Trademark).FirstAsync(p => p.Id == product.Id);
					return Json(new { result = "success", data = GetJsonViewModel(product) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_ProductUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            									ModelState.AddModelError("Name", GlobalRes.Duplicate);
																	ModelState.AddModelError("TrademarkId", GlobalRes.Duplicate);
																	ModelState.AddModelError("ProductlineId", GlobalRes.Duplicate);
																	ModelState.AddModelError("ProductVolumeId", GlobalRes.Duplicate);
														}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            }

			List<Productline> ProductlineList = db.Productlines.ToList();
							ProductlineList.Insert(0, new Productline() { Name = GlobalRes.Empty });
			
            ViewBag.ProductlineId = new SelectList(ProductlineList, "Id", "Name");
			List<ProductVolume> ProductVolumeList = db.ProductVolumes.ToList();
			
            ViewBag.ProductVolumeId = new SelectList(ProductVolumeList, "Id", "Name");
			List<Trademark> TrademarkList = db.Trademarks.ToList();
							TrademarkList.Insert(0, new Trademark() { Name = GlobalRes.Empty });
			
            ViewBag.TrademarkId = new SelectList(TrademarkList, "Id", "Name");
            return PartialView(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return RedirectToAction("NotFound", "Home", new NotFoundViewModel() { Title = GlobalRes.Edit, OriginalViewAction = GlobalRes.ProductShortName + " " + GlobalRes.Edit, Message = "404. Данная запись отсутствует." });
            }
			ViewBag.Concurrency = false;
			List<Productline> ProductlineList = db.Productlines.ToList();
							ProductlineList.Insert(0, new Productline() { Name = GlobalRes.Empty });
			
            ViewBag.ProductlineId = new SelectList(ProductlineList, "Id", "Name");
			List<ProductVolume> ProductVolumeList = db.ProductVolumes.ToList();
			
            ViewBag.ProductVolumeId = new SelectList(ProductVolumeList, "Id", "Name");
			List<Trademark> TrademarkList = db.Trademarks.ToList();
							TrademarkList.Insert(0, new Trademark() { Name = GlobalRes.Empty });
			
            ViewBag.TrademarkId = new SelectList(TrademarkList, "Id", "Name");
            return PartialView(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,TrademarkId,ProductlineId,ProductVolumeId,RowVersion")] Product product)
        {
			ViewBag.Concurrency = false;
            if (ModelState.IsValid)
            {
										product.ProductlineId = SetValueToNull(product.ProductlineId);
								product.ProductVolumeId = product.ProductVolumeId;
				product.TrademarkId = SetValueToNull(product.TrademarkId);
				                db.Entry(product).State = EntityState.Modified;
                string[] saveResult = await Function.SaveChangesToDb(db);
				if (saveResult[0] == "success")
                    {
				product = await db.Products.Include(p => p.Productline).Include(p => p.ProductVolume).Include(p => p.Trademark).FirstAsync(p => p.Id == product.Id);
					return Json(new { result = "success", data = GetJsonViewModel(product) }, JsonRequestBehavior.AllowGet);
                    }
					else if (saveResult[0] == "concurrencyError")
                    {
                        ModelState.AddModelError(String.Empty, saveResult[1]);
                        ViewBag.Concurrency = true;
                    }
                    else
                    {
                        if (saveResult[1].Contains("IX_ProductUnique"))
                        {
                            ModelState.AddModelError(String.Empty, GlobalRes.DuplicateDataError);
                            									ModelState.AddModelError("Name", GlobalRes.Duplicate);
																	ModelState.AddModelError("TrademarkId", GlobalRes.Duplicate);
																	ModelState.AddModelError("ProductlineId", GlobalRes.Duplicate);
																	ModelState.AddModelError("ProductVolumeId", GlobalRes.Duplicate);
														}
                        else
                        {
                            ModelState.AddModelError(String.Empty, saveResult[1]);
                        }
                    }
                
				            
			}

			List<Productline> ProductlineList = db.Productlines.ToList();
							ProductlineList.Insert(0, new Productline() { Name = GlobalRes.Empty });
			
            ViewBag.ProductlineId = new SelectList(ProductlineList, "Id", "Name");
			List<ProductVolume> ProductVolumeList = db.ProductVolumes.ToList();
			
            ViewBag.ProductVolumeId = new SelectList(ProductVolumeList, "Id", "Name");
			List<Trademark> TrademarkList = db.Trademarks.ToList();
							TrademarkList.Insert(0, new Trademark() { Name = GlobalRes.Empty });
			
            ViewBag.TrademarkId = new SelectList(TrademarkList, "Id", "Name");
            return PartialView(product);
        }

        // GET: Products/Delete/5
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
            
            Product product = await db.Products.Include(p => p.Productline).Include(p => p.ProductVolume).Include(p => p.Trademark).SingleOrDefaultAsync(p => p.Id == id.Value);
            if (product == null)
            {
                return RedirectToAction("NotFound","Home",new NotFoundViewModel() { Title = GlobalRes.Delete, OriginalViewAction = GlobalRes.DeleteConfirm, Message = message });
            }
            return PartialView(product);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Product product)
        {
            
			            db.Entry(product).State = EntityState.Deleted;
            string[] saveResult = await Function.SaveChangesToDb(db);
                
			if (saveResult[0] == "concurrencyError")
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = product.Id, message = saveResult[1] });
                }
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new { result = saveResult[0], message = saveResult[1], value = product.Id }
                };
						
        }

		[HttpPost]
		        public async Task<ActionResult> RefreshRow(int id)
		        {
		            Product product = await db.Products.Include(p => p.Productline).Include(p => p.ProductVolume).Include(p => p.Trademark).SingleOrDefaultAsync(p => p.Id == id);
			            if (product == null)
            return Json(new { result = "delete" }, JsonRequestBehavior.DenyGet);
            return Json(new { result = "edit", data = GetJsonViewModel(product) }, JsonRequestBehavior.DenyGet);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		        public async Task<JsonResult> DeleteList(int[] ids)
        
        {
            List<Product> products;
            
                for (int i = 0; i <= ids.Length/500; i++)
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
                    products = await db.Products.Where(e => x.Contains(e.Id)).ToListAsync();
                    db.Products.RemoveRange(products);

                }
				            string[] saveResult = await Function.SaveChangesToDb(db);
									return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { result = saveResult[0], message = saveResult[1] }
            };
        }
		public async Task<JsonResult> Data(string search, string sort, string order, int? offset, int? limit, string datafilter = null)
        {
            var products = db.Products.Include(p => p.Productline).Include(p => p.ProductVolume).Include(p => p.Trademark);
            
			
			int TotalNotFiltered = products.Count();
			        
            

            int Total = TotalNotFiltered;
			            if (datafilter != null)
            { 
                products = BuildFilter(products, JsonConvert.DeserializeObject<ProductFilter>(datafilter));
                Total = products.Count();
			}
			            bool serverSide = Total > 400 ? true : false;
            if (!String.IsNullOrEmpty(search) & serverSide)
            {
                string dateSearch = search;
                // Работает только в культуре с форматом даты дд.мм.гггг

												products = products.Where(p => p.Productline.Name.Contains(search) 								|| p.ProductVolume.Name.Contains(search)								|| p.Trademark.Name.Contains(search)							|| p.Name.ToString().Contains(search)							|| p.RowVersion.ToString().Contains(search));
                Total = products.Count();
			}

             
            if (sort != null)
            {
                products = Function.OrderBy(products, sort, order);
            }
            else
            {
                products = products.OrderBy(e => e.Id);
            }
            
            if (serverSide)
            {                
                products = products.Skip(offset.Value);
								BootsrapTableServerDataFormat data = new BootsrapTableServerDataFormat(GetJsonViewModel(await products.Take(limit.Value).ToListAsync()), Total, TotalNotFiltered);
				                return Json(data, JsonRequestBehavior.AllowGet);
            }
			else
			{
            			return Json(GetJsonViewModel(await products.ToListAsync()), JsonRequestBehavior.AllowGet);
			            }
}

		[HttpPost]
        [ValidateAntiForgeryToken]
		public ActionResult Filter([Bind(Include = "TrademarkIdSelected,ProductlineIdSelected,ProductVolumeIdSelected")] ProductFilter dataFilter)
		{
			if (ModelState.IsValid)
            {
									if (dataFilter.ProductlineIdSelected != null && (db.Productlines.Count() + 1) == dataFilter.ProductlineIdSelected.Length)
                        dataFilter.ProductlineIdSelected = null;
               					if (dataFilter.ProductVolumeIdSelected != null && db.ProductVolumes.Count() == dataFilter.ProductVolumeIdSelected.Length)
                        dataFilter.ProductVolumeIdSelected = null;
									if (dataFilter.TrademarkIdSelected != null && (db.Trademarks.Count() + 1) == dataFilter.TrademarkIdSelected.Length)
                        dataFilter.TrademarkIdSelected = null;
               	
                                
                var products = db.Products.Include(p => p.Productline).Include(p => p.ProductVolume).Include(p => p.Trademark);

                if (dataFilter != null)
                {
                    products = BuildFilter(products, dataFilter);
                }
								
				int Count = products.Count();
				
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

		private IQueryable<Product> BuildFilter(IQueryable<Product> products, ProductFilter filter)
        {
								if (filter.TrademarkIdSelected != null)
					{
						products = products.WhereFilter("TrademarkId", filter.TrademarkIdSelected);
					}
								if (filter.ProductlineIdSelected != null)
					{
						products = products.WhereFilter("ProductlineId", filter.ProductlineIdSelected);
					}
								if (filter.ProductVolumeIdSelected != null)
					{
						products = products.WhereFilter("ProductVolumeId", filter.ProductVolumeIdSelected);
					}
			            
            return products;
        }
		private List<ProductJsonViewModel> GetJsonViewModel(List<Product> baseResponse)
        {
            List<ProductJsonViewModel> result = new List<ProductJsonViewModel>();
            foreach (Product item in baseResponse)
            {
                result.Add(new ProductJsonViewModel(item));
            }
            return result;
        }
        private ProductJsonViewModel GetJsonViewModel(Product baseResponse)
        {
            return new ProductJsonViewModel(baseResponse);
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
    }
	
}
 
