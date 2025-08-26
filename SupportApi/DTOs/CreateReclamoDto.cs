using System;
using System.ComponentModel.DataAnnotations;

namespace SupportApi.DTOs
{
    public class CreateReclamoDto
    {
        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public string Prioridad { get; set; } = "Baja";
    }
}
