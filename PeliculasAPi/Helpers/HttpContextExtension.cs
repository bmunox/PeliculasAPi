using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPi.Helpers
{
    public static class HttpContextExtension
    {
        public async static Task InsertarParametrosPaginacion<T>(this HttpContext httpContext, IQueryable<T> queryable, int cantidadRegistrosPorPagina)
        {
            double cantidad = await queryable.CountAsync();
            double cantidadPaginas = Math.Ceiling(cantidad / cantidadRegistrosPorPagina);
            httpContext.Response.Headers.Add("CantidadPaginas",cantidadPaginas.ToString());
        }
    }
}
