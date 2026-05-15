using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Models
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El Numero de Documento es Incorrecto.")]
        public int? NumeroDocumento { get; set; }

        [Required]
        [StringLength(2, ErrorMessage = "El complemento no cumple formato. Ej: 1A")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80, ErrorMessage = "El nombre no puede exceder los 80 caracteres.")]
        public string? NombreRazonSocial { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string? Email { get; set; }

        [NotMapped]
        public string? Info { get { return $"{NumeroDocumento} - {NombreRazonSocial}"; } }

        public ICollection<Venta>? Ventas { get; set; }
    }
}
