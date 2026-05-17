using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Models
{
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Precision(10, 2)]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El stock minimo es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public int StockMinimo { get; set; }

        [Required(ErrorMessage = "El stock maximo es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock máximo no puede ser negativo.")]
        public int StockMaximo { get; set; }

        [Required(ErrorMessage = "El tipo de producto es obligatorio.")]
        [StringLength(50, ErrorMessage = "El tipo de producto no puede exceder los 50 caracteres.")]
        public string TipoProducto { get; set; } = string.Empty;

        [DataType(DataType.Date, ErrorMessage = "La fecha de vencimiento debe ser una fecha válida.")]
        [FutureDate(ErrorMessage = "La fecha de vencimiento debe ser una fecha futura.")]
        public DateOnly? FechaVencimiento { get; set; }

        public string? UrlFoto { get; set; }

        // Para subir archivos
        [NotMapped]
        [Display(Name = "Cargar Foto")]
        public IFormFile? FotoFile { get; set; }

        [NotMapped]
        public string? Info => $"{Descripcion}";

        // Relación uno a muchos con Ventas
        // Relación uno a muchos con Detalles de Venta
        public ICollection<DetalleVenta> DetallesVentas { get; set; } = new List<DetalleVenta>();
    }

    // Validación personalizada para fechas futuras
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly date && date < DateOnly.FromDateTime(DateTime.Now))
            {
                return new ValidationResult(ErrorMessage ?? "La fecha debe ser futura.");
            }
            return ValidationResult.Success;
        }
    }
}
