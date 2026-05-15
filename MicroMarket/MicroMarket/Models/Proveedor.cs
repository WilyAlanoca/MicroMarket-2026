using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Models
{
    public class Proveedor
    {
        [Key]
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El CI es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El CI debe ser un número positivo.")]
        public int? CI { get; set; }

        [Required]
        [StringLength(2, ErrorMessage = "El complemento no cumple formato. Ej: 1A")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80, ErrorMessage = "El nombre no puede exceder los 80 caracteres.")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "El número celular es obligatorio.")]
        [Phone(ErrorMessage = "El formato del número de celular no es válido.")]
        [StringLength(8, ErrorMessage = "El número de celular no puede exceder los 8 dígitos.")]
        public string? Celular { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres.")]
        public string? Direccion { get; set; }

        // Atributo computado
        [NotMapped]
        public string? Info => $"{CI} - {Nombre}";

        // Relación uno a muchos con Productos
        [Required(ErrorMessage = "Debe especificar al menos un producto que provee.")]
        public ICollection<Producto>? Productos { get; set; } = new List<Producto>();
    }
}
