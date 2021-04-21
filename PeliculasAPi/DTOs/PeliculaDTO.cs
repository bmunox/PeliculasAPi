using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPi.DTOs
{
    public class PeliculaDTO
    {
        public int id { get; set; }
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        public string Poster { get; set; }
    }
}
