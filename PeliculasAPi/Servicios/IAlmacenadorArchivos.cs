using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPi.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task<String> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string ContenType);
        Task BorrarArchivo(string ruta, string contenedor);
        Task<String> GuardarArchivo(byte[] contenido, string extension, string contenedor, string ContenType);
    }
}
