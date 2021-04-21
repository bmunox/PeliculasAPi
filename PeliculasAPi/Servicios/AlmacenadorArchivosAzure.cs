using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPi.Servicios
{
    public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
    {
        public Task BorrarArchivo(string ruta, string contenedor)
        {
            throw new NotImplementedException();
        }

        public Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string ContenType)
        {
            throw new NotImplementedException();
        }

        public Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string ContenType)
        {
            throw new NotImplementedException();
        }
    }
}
