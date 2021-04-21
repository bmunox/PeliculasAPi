using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPi.DTOs
{
    public class PeliculasDetallesDTO : PeliculaDTO
    {
        public List<GeneroDTO> Generos { get; set; }
        public List<ActorPeliculaDetalleDTO> Actores { get; set; }
    }
}
