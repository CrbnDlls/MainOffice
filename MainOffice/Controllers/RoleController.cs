using MainOffice.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MainOffice.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private ApplicationRoleManager _roleManager;

        public RoleController()
        {
        }

        public RoleController(ApplicationRoleManager roleManager)
        {
            RoleManager = roleManager;
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            List<RoleModel> list = new List<RoleModel>();
            foreach (var role in RoleManager.Roles)
            {
                list.Add(new RoleModel(role));
            }
            return View(list);
        }
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(RoleModel model)
        {
            if (ModelState.IsValid)
            { 
                var role = new ApplicationRole() { Name = model.Name };
                var result = await RoleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Edit(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            return View(new RoleModel(role));
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> Edit(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole() { Id = model.Id, Name = model.Name };
                var result = await RoleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Details(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            return View(new RoleModel(role));
        }
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            return View(new RoleModel(role));
        }
        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role.Name == "visitor")
            {
                AddErrors(new IdentityResult(new string[] { "Роль visitor не может быть удалена" }));
                return View(new RoleModel(role));
            }
            await RoleManager.DeleteAsync(role);
            return RedirectToAction("Index");
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}