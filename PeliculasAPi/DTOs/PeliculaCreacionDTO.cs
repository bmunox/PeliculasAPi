using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PeliculasAPi.Helpers;
using PeliculasAPi.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPi.DTOs
{
    public class PeliculaCreacionDTO : PeliculaPatchDTO
    {
        [PesoArchivoValidacion(PesoMaximoEnMegabytes: 4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Poster { get; set; }
        
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIDs { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculasCreacionDTO>>))]
        public List<ActorPeliculasCreacionDTO> Actores { get; set; }
    }
}
