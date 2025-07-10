using Proyeto.GestionRepuestos.MVC.Models.Enumerados;
using System;
using System.ComponentModel.DataAnnotations;

namespace Proyeto.GestionRepuestos.MVC.Models
{
    public class SolicitudRepuesto
    {
        public int Id { get; set; }

        [Required]
        public int RepuestoId { get; set; }
        public virtual Repuesto Repuesto { get; set; }

        [Required]
        public string UsuarioSolicitadorId { get; set; }
        public virtual ApplicationUser UsuarioSolicitador { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int CantidadSolicitada { get; set; }

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente; // Pendiente, Aprobado, Entregado
    }
}
