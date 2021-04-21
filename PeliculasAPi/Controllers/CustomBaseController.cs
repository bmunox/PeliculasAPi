using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPi.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPi.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CustomBaseController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class 
        {
            var entidades = await context.Set<TEntidad>().AsNoTracking().ToListAsync();
            var dtos = mapper.Map<List<TDTO>>(entidades);
            return dtos;
        }
        protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id) where TEntidad : class, IID
        {
            var entidad = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id == id);
            if (entidad == null)
            {
                return NotFound();
            }
            var dtos = mapper.Map<TDTO>(entidad);
            return dtos;
        }

        protected async Task<ActionResult> Post<Tcreacion, TEntidad, Tlectura>(Tcreacion creacionDTO, string nombreRuta) where TEntidad : class, IID
        {
            var entidad = mapper.Map<TEntidad>(creacionDTO);
            context.Add(entidad);
            await context.SaveChangesAsync();

            var lecturaDTO = mapper.Map<Tlectura>(entidad);
            return new CreatedAtRouteResult(nombreRuta, new { id = entidad.Id }, lecturaDTO);
        }
        protected async Task<ActionResult> Put<Tcreacion, TEntidad>(Tcreacion creacionDTO, int id) where TEntidad : class, IID
        {
            var entidad = mapper.Map<TEntidad>(creacionDTO);
            entidad.Id = id;
            context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }
        protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad : class, IID, new()
        {
            var existe = await context.Set<TEntidad>().AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new TEntidad() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
