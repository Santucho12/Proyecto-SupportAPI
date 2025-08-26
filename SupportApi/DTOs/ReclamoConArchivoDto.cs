using System.ComponentModel.DataAnnotations;

namespace SupportApi.DTOs
{
    public class ReclamoConArchivoDto
    {
        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(2000, ErrorMessage = "La descripción no puede superar los 2000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La prioridad es requerida")]
        public string Prioridad { get; set; } = string.Empty;

        public int? UsuarioAsignadoId { get; set; }

        public IFormFile? Archivo { get; set; }
    }
}
