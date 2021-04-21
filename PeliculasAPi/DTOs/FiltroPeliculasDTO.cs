using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPi.DTOs
{
    public class FiltroPeliculasDTO
    {
        public int pagina { get; set; } = 1;
        public int CantidadRegistrosPorPagina { get; set; } = 10;
        public PaginacionDTO paginacion 
        {
            get { return new PaginacionDTO() { pagina = pagina, CantidadRegistrosPorPagina = CantidadRegistrosPorPagina };}
        }
        public string Titulo { get; set; }
        public int GeneroId { get; set; }
        public bool EnCines { get; set; }
        public bool ProximosExtrenos { get; set; }
        public string CampoOrdenar { get; set; }
        public bool OrdenarAscendente { get; set; }
    }
}
