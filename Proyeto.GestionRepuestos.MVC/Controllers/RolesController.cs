using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Proyeto.GestionRepuestos.MVC.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RolesController : Controller
    {
        private ApplicationRoleManager _roleManager;
        private ApplicationUserManager _userManager;

        public RolesController()
        {
        }

        public RolesController(ApplicationRoleManager roleManager, ApplicationUserManager userManager)
        {
            RoleManager = roleManager;
            UserManager = userManager;
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

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Roles
        public ActionResult Index()
        {
            var roles = RoleManager.Roles.ToList();
            return View(roles);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var result = await RoleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                    return RedirectToAction("Index");
                ModelState.AddModelError("", result.Errors.FirstOrDefault());
            }
            return View();
        }

        // GET: Roles/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
                return HttpNotFound();

            return View(role);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, string name)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
                return HttpNotFound();

            role.Name = name;
            var result = await RoleManager.UpdateAsync(role);
            if (result.Succeeded)
                return RedirectToAction("Index");

            ModelState.AddModelError("", result.Errors.FirstOrDefault());
            return View(role);
        }

        // GET: Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
                return HttpNotFound();

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
                return HttpNotFound();

            var result = await RoleManager.DeleteAsync(role);
            if (result.Succeeded)
                return RedirectToAction("Index");

            ModelState.AddModelError("", result.Errors.FirstOrDefault());
            return View(role);
        }

        // GET: Roles/Assign
        public ActionResult Assign()
        {
            ViewBag.Users = new SelectList(UserManager.Users.ToList(), "Id", "Email");
            ViewBag.Roles = new SelectList(RoleManager.Roles.ToList(), "Name", "Name");
            return View();
        }

        // POST: Roles/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Assign(string userId, string roleName)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(roleName))
            {
                var result = await UserManager.AddToRoleAsync(userId, roleName);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                ModelState.AddModelError("", result.Errors.FirstOrDefault());
            }
            ViewBag.Users = new SelectList(UserManager.Users.ToList(), "Id", "Email");
            ViewBag.Roles = new SelectList(RoleManager.Roles.ToList(), "Name", "Name");
            return View();
        }
    }
}