using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Proyeto.GestionRepuestos.MVC.Models;

namespace Proyeto.GestionRepuestos.MVC.Controllers
{
    [Authorize]
    public class RepuestosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Repuestos
        public async Task<ActionResult> Index()
        {
            var repuestos = await db.Repuestos.ToListAsync();
            return View(repuestos);
        }

        // GET: Repuestos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Repuestos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Repuesto repuesto)
        {
            if (ModelState.IsValid)
            {
                db.Repuestos.Add(repuesto);
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Repuesto ingresado exitosamente.";
                return RedirectToAction("Create");
            }
            return View(repuesto);
        }

        // GET: Repuestos
        public async Task<ActionResult> AdministrarSolicitudes()
        {
            var solicitudesRepuestos = await db.SolicitudesRepuestos.ToListAsync();
            return View(solicitudesRepuestos);
        }

        // GET: Repuestos/Entregar/5

        //[AllowAnonymous]
        public async Task<ActionResult> Entregar(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var solicitudRepuesto = await db.SolicitudesRepuestos.FindAsync(id);

            if (solicitudRepuesto == null)
                return HttpNotFound();

            return View(solicitudRepuesto);
        }

        // POST: Repuestos/Entregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Entregar(int id, int cantidadEntregada)
        {
            var solicitud = await db.SolicitudesRepuestos.FindAsync(id);
            if (solicitud == null)
            {
                ModelState.AddModelError("", "Repuesto no encontrado.");
                return View();
            }

            if (cantidadEntregada < 1 || cantidadEntregada > solicitud.Repuesto.CantidadDisponible)
            {
                ModelState.AddModelError("", "Cantidad inválida. No hay suficiente inventario.");
                ViewBag.RepuestoNombre = solicitud.Repuesto.Nombre;
                ViewBag.RepuestoId = solicitud.Id;
                return View();
            }

            // Registrar la entrega
            var entrega = new EntregaRepuesto
            {
                SolicitudRepuestoId = solicitud.Id,
                UsuarioEntregadorId = User.Identity.GetUserId(),
                CantidadEntregada = cantidadEntregada
            };
            db.EntregasRepuestos.Add(entrega);

            // Actualizar inventario
            solicitud.Repuesto.CantidadDisponible -= cantidadEntregada;
            solicitud.Estado = Models.Enumerados.EstadoSolicitud.Entregada;
            await db.SaveChangesAsync();

            TempData["EntregaSuccess"] = $"Entrega registrada correctamente. {cantidadEntregada} unidad(es) de '{solicitud.Repuesto.Nombre}' entregadas a {solicitud.UsuarioSolicitador.Nombre}.";
            return RedirectToAction("Index");
        }

        // GET: Repuestos/Solicitar
        public ActionResult Solicitar()
        {
            //var mecanicoRoleId = db.Roles.Where(r => r.Name == "Mecanico").Select(r => r.Id).FirstOrDefault();
            //ViewBag.Mecanicos = new SelectList(db.Users.Where(u => u.Roles.Any(ur => ur.RoleId == mecanicoRoleId)).ToList(), "IdUsuario", "NombreUsuario");

            ViewBag.Repuestos = new SelectList(db.Repuestos.Where(r => r.CantidadDisponible > 0).ToList(), "Id", "Nombre");

            return View();
        }

        // POST: Repuestos/Solicitar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Solicitar(int repuestoId, int cantidadSolicitada)
        {
            var repuesto = await db.Repuestos.FindAsync(repuestoId);
            if (repuesto == null)
            {
                ModelState.AddModelError("", "Repuesto no encontrado.");
            }
            else if (cantidadSolicitada < 1 || cantidadSolicitada > repuesto.CantidadDisponible)
            {
                ModelState.AddModelError("", "Cantidad inválida o insuficiente en inventario.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Repuestos = new SelectList(db.Repuestos.Where(r => r.CantidadDisponible > 0).ToList(), "Id", "Nombre");
                return View();
            }

            var solicitud = new SolicitudRepuesto
            {
                RepuestoId = repuesto.Id,
                UsuarioSolicitadorId = User.Identity.GetUserId(),
                CantidadSolicitada = cantidadSolicitada
            };
            db.SolicitudesRepuestos.Add(solicitud);
            // Opcional: puedes descontar del inventario aquí si la política lo requiere
            await db.SaveChangesAsync();
            TempData["SolicitudSuccess"] = $"Solicitud registrada correctamente. {cantidadSolicitada} unidad(es) de '{repuesto.Nombre}' solicitadas.";
            return RedirectToAction("Solicitar");
        }
    }
}