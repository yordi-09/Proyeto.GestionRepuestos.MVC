using System;
using System.ComponentModel.DataAnnotations;

namespace Proyeto.GestionRepuestos.MVC.Models
{
    public class EntregaRepuesto
    {
        public int Id { get; set; }

        [Required]
        public int SolicitudRepuestoId { get; set; }

        public virtual SolicitudRepuesto SolicitudRepuesto { get; set; }

        [Required]
        public string UsuarioEntregadorId { get; set; }
        public virtual ApplicationUser UsuarioEntregador { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int CantidadEntregada { get; set; }

        public DateTime FechaEntrega { get; set; } = DateTime.Now;
    }
}