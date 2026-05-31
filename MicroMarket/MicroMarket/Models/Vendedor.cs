using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Models
{
    public enum TipoRol
    {
        Administrador = 1,
        Vendedor = 2
    }

    public class Vendedor
    {
        [Key]
        public int VendedorId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; } = string.Empty;

        // ALMACENA: hash de la contraseña, no el texto claro
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número celular es obligatorio.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El número celular debe tener 8 dígitos.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "El teléfono solo puede contener números.")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres.")]
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [Range(1, 2, ErrorMessage = "El rol debe ser 1 (Administrador) o 2 (Vendedor).")]
        public TipoRol Rol { get; set; }

        // Relación uno a muchos con Ventas
        public ICollection<Venta>? Ventas { get; set; }
    }

}
