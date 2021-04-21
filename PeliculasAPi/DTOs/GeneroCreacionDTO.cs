using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPi.DTOs
{
    public class GeneroCreacionDTO
    {
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; }
    }
}
