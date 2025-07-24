using System.ComponentModel.DataAnnotations;

namespace SistemaAgenciaTours.Models
{
    public class Usuario
    {

        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("Clave", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarClave { get; set; }
    }
}
