using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPi.DTOs
{
    public class ActorPatchDTO
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}
