using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPi.DTOs;
using PeliculasAPi.Entidades;
using PeliculasAPi.Helpers;
using PeliculasAPi.Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;

namespace PeliculasAPi.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ILogger<PeliculasController> logger;
        private readonly string contenedor = "peliculas";
        public DateTime hoy = DateTime.Today;

        public PeliculasController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos, ILogger<PeliculasController> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.logger = logger;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<PeliculaDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Peliculas.AsQueryable();

            await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadRegistrosPorPagina);

            var peliculas = await queryable.Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpGet("GetIndex")]
        public async Task<ActionResult<PeliculasIndexDTO>> GetIndex()
        {
            var top = 5;
            var proximosExtrenos = await context.Peliculas
                .Where(x => x.FechaEstreno > hoy)
                .OrderBy(x => x.FechaEstreno)
                .Take(top)
                .ToListAsync();

            var enCines = await context.Peliculas
                .Where(x => x.EnCines)
                .Take(top)
                .ToListAsync();

            var resultado = new PeliculasIndexDTO();
            resultado.FuturosEstrenos = mapper.Map<List<PeliculaDTO>>(proximosExtrenos);
            resultado.EnCines = mapper.Map<List<PeliculaDTO>>(enCines);

            return resultado;
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] FiltroPeliculasDTO   filtroPeliculasDTO)
        {
            var PeliculasQueryable = context.Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(filtroPeliculasDTO.Titulo))
            {
                PeliculasQueryable = PeliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculasDTO.Titulo));
            }
            if (filtroPeliculasDTO.EnCines)
            {
                PeliculasQueryable = PeliculasQueryable.Where(x => x.EnCines);
            }
            if (filtroPeliculasDTO.ProximosExtrenos)
            {
                PeliculasQueryable = PeliculasQueryable.Where(x => x.FechaEstreno > hoy);
            }
            if (filtroPeliculasDTO.GeneroId != 0)
            {
                PeliculasQueryable = PeliculasQueryable
                    .Where(x => x.PeliculasGeneros.Select(y => y.GeneroId)
                    .Contains(filtroPeliculasDTO.GeneroId));
            }
            if (!string.IsNullOrEmpty(filtroPeliculasDTO.CampoOrdenar))
            {
                var tipoOrden = filtroPeliculasDTO.OrdenarAscendente ? "ascending" : "descending";
                try
                {
                    PeliculasQueryable = PeliculasQueryable.OrderBy($"{filtroPeliculasDTO.CampoOrdenar} {tipoOrden}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                }
            }

            await HttpContext.InsertarParametrosPaginacion(PeliculasQueryable, filtroPeliculasDTO.CantidadRegistrosPorPagina);

            var peliculas = await PeliculasQueryable.Paginar(filtroPeliculasDTO.paginacion).ToListAsync();

            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpGet("{id:int}", Name ="obtenerPelicula")]
        public async Task<ActionResult<PeliculasDetallesDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas
                .Include(x => x.PeliculasActores).ThenInclude(x => x.Actor)
                .Include(x => x.PeliculasGeneros).ThenInclude(x => x.Genero)
                .FirstOrDefaultAsync(x => x.id == id);
            if (pelicula == null)
            {
                return NotFound();
            }
            pelicula.PeliculasActores = pelicula.PeliculasActores.OrderBy(x => x.Orden).ToList();
            return mapper.Map<PeliculasDetallesDTO>(pelicula);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);
                    pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, peliculaCreacionDTO.Poster.ContentType);
                }
            }
            AsignarOrdenActores(pelicula);

            context.Add(pelicula);
            await context.SaveChangesAsync();
            var dto = mapper.Map<PeliculaDTO>(pelicula);
            return new CreatedAtRouteResult("obtenerPelicula", new { id = pelicula.id }, dto);
        }

        private void AsignarOrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculasActores != null)
            {
                for (int i = 0; i < pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PeliculaDTO>> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var peliculaDB = await context.Peliculas
                .Include(x => x.PeliculasActores)
                .Include(x => x.PeliculasGeneros)
                .FirstOrDefaultAsync(x => x.id == id);

            if (peliculaDB == null)
            {
                return NotFound();
            }
            peliculaDB = mapper.Map(peliculaCreacionDTO, peliculaDB);

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);
                    peliculaDB.Poster = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, peliculaDB.Poster, peliculaCreacionDTO.Poster.ContentType);
                }
            }
            AsignarOrdenActores(peliculaDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }
            var entidadDB = await context.Peliculas.FirstOrDefaultAsync(x => x.id == id);

            if (entidadDB == null)
            {
                return NotFound();
            }

            var entidadDTO = mapper.Map<PeliculaPatchDTO>(entidadDB);

            patchDocument.ApplyTo(entidadDTO, ModelState);

            var esValido = TryValidateModel(entidadDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(entidadDTO, entidadDB);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Peliculas.AnyAsync(x => x.id == id);
            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new Pelicula() { id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}
