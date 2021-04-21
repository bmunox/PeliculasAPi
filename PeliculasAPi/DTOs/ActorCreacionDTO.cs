using Microsoft.AspNetCore.Http;
using PeliculasAPi.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPi.DTOs
{
    public class ActorCreacionDTO : ActorPatchDTO
    {
        [PesoArchivoValidacion(PesoMaximoEnMegabytes:4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Foto { get; set; }
    }
}
