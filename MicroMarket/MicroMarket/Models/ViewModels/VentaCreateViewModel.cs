using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Models.ViewModels
{
    public class VentaCreateViewModel
    {
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public int ClienteId { get; set; }

        public List<DetalleVentaInput> Detalles { get; set; } = new();
    }

    public class DetalleVentaInput
    {
        [Required(ErrorMessage = "El producto es obligatorio.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
        public int Cantidad { get; set; }
    }
}