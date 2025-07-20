using Microsoft.AspNet.Identity;
using Proyeto.GestionRepuestos.MVC.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Proyeto.GestionRepuestos.MVC.Controllers
{
    [Authorize]
    public class RepuestosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Repuestos
        [Authorize(Roles = "EncargadoBodega")]
        public async Task<ActionResult> Index()
        {
            var repuestos = await db.Repuestos.ToListAsync();
            return View(repuestos);
        }

        // GET: Repuestos/Create
        [Authorize(Roles = "EncargadoBodega")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Repuestos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EncargadoBodega")]
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

        // GET: Repuestos/Edit/5
        [Authorize(Roles = "EncargadoBodega")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var repuesto = await db.Repuestos.FindAsync(id);
            if (repuesto == null)
            {
                return HttpNotFound();
            }

            return View(repuesto);
        }

        // POST: Repuestos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EncargadoBodega")]
        public async Task<ActionResult> Edit(Repuesto repuesto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(repuesto).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Repuesto actualizado exitosamente.";
                return RedirectToAction("Index");
            }
            return View(repuesto);
        }

        // GET: Repuestos
        [Authorize(Roles = "EncargadoBodega")]
        public async Task<ActionResult> AdministrarSolicitudes()
        {
            var solicitudesRepuestos = await db.SolicitudesRepuestos.ToListAsync();
            return View(solicitudesRepuestos);
        }

        // GET: Repuestos/Entregar/5
        [Authorize(Roles = "EncargadoBodega")]
        public async Task<ActionResult> Entregar(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var solicitudRepuesto = await db.SolicitudesRepuestos.FindAsync(id);

            if (solicitudRepuesto == null)
                return HttpNotFound();

            return View(solicitudRepuesto);
        }

        // POST: Repuestos/Entregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EncargadoBodega")]
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
            return RedirectToAction("AdministrarSolicitudes");
        }

        // POST: Repuestos/Rechazar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EncargadoBodega")]
        public async Task<ActionResult> Rechazar(int id)
        {
            var solicitud = await db.SolicitudesRepuestos.FindAsync(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }

            if (solicitud.Estado != Models.Enumerados.EstadoSolicitud.Pendiente)
            {
                TempData["Error"] = "Solo se pueden rechazar solicitudes pendientes.";
                return RedirectToAction("AdministrarSolicitudes");
            }

            solicitud.Estado = Models.Enumerados.EstadoSolicitud.Rechazada;
            await db.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Solicitud de '{solicitud.Repuesto.Nombre}' rechazada correctamente.";
            return RedirectToAction("AdministrarSolicitudes");
        }

        // GET: Repuestos/Solicitar
        [Authorize(Roles = "MecanicoTaller")]
        public ActionResult Solicitar()
        {
            ViewBag.Repuestos = new SelectList(db.Repuestos.Where(r => r.CantidadDisponible > 0).ToList(), "Id", "Nombre");
            var userId = User.Identity.GetUserId();
            var solicitudesUsuario = db.SolicitudesRepuestos
                .Include("Repuesto")
                .Where(s => s.UsuarioSolicitadorId == userId)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToList();
            ViewBag.SolicitudesUsuario = solicitudesUsuario;
            return View();
        }

        // POST: Repuestos/Solicitar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "MecanicoTaller")]
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
            await db.SaveChangesAsync();
            TempData["SolicitudSuccess"] = $"Solicitud registrada correctamente. {cantidadSolicitada} unidad(es) de '{repuesto.Nombre}' solicitadas.";
            return RedirectToAction("Solicitar");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}