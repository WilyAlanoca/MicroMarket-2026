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

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Almacena el hash de la contraseña
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, Phone]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        [Range(1, 2)]
        public TipoRol Rol { get; set; }

        public ICollection<Venta>? Ventas { get; set; }
    }
}
