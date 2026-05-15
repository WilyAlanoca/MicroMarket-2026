using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Models
{
    public class DetalleVenta
    {
            [Key]
            public int DetalleVentaId { get; set; }

            [Required(ErrorMessage = "La venta es obligatoria.")]
            public int VentaId { get; set; }
            public Venta Venta { get; set; }

            [Required(ErrorMessage = "El producto es obligatorio.")]
            public int ProductoId { get; set; }
            public Producto Producto { get; set; }

            [Required(ErrorMessage = "La cantidad es obligatoria.")]
            [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
            public decimal Cantidad { get; set; }

            [Required(ErrorMessage = "El precio unitario es obligatorio.")]
            [Precision(10, 2)]
            [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que 0.")]
            public decimal PrecioUnitario { get; set; }

            [Required(ErrorMessage = "El subtotal es obligatorio.")]
            [Precision(10, 2)]
            [Range(0.01, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor que 0.")]
            public decimal SubTotal { get; set; }
    }
}
