using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Models
{
    public class Venta
    {
        [Key]
        public int VentaId { get; set; }

        [Required(ErrorMessage = "La fecha de emisión es obligatoria.")]
        [Display(Name = "Fecha de Emisión")]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [Required(ErrorMessage = "El monto total es obligatorio.")]
        [Precision(10, 2)]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto total debe ser mayor que 0.")]
        public decimal MontoTotal { get; set; }

        [Required(ErrorMessage = "El estado de la factura es obligatorio.")]
        [StringLength(20, ErrorMessage = "El estado no puede exceder los 20 caracteres.")]
        public string Estado { get; set; } = "Emitida"; // Ejemplo: "Emitida", "Anulada"

        // Relación uno a muchos con Detalles de la Venta
        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }

}
