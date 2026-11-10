using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using Datos;
using System.Security.Cryptography.X509Certificates;

namespace Negocio
{
    public class EspecialidadNegocio
    {
        public List<Especialidad> listar()
        {
            List<Especialidad> lista = new List<Especialidad>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "SELECT Id_Especialidad, Nombre, Descripcion, Estado FROM ESPECIALIDADES"
                );

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Especialidad aux = new Especialidad();

                    aux.Id = (int)datos.Lector["Id_Especialidad"];
                    aux.Nombre = (string)datos.Lector["Nombre"];

                    aux.Descripcion =
                        datos.Lector["Descripcion"] == DBNull.Value
                        ? ""
                        : (string)datos.Lector["Descripcion"];

                    aux.Estado = (bool)datos.Lector["Estado"];

                    lista.Add(aux);
                }
                return lista;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void agregar(Especialidad nuevo)
        {
            if (nuevo == null || string.IsNullOrWhiteSpace(nuevo.Nombre))
            {
                throw new ArgumentException("El nombre de la especialidad es obligatorio.");
            }

            nuevo.Nombre = nuevo.Nombre.Trim();

            Especialidad existente = listar().FirstOrDefault(e =>
             System.Globalization.CultureInfo.GetCultureInfo("es-AR")
                 .CompareInfo.Compare(
                     e.Nombre.Trim(),
                     nuevo.Nombre,
                     System.Globalization.CompareOptions.IgnoreCase |
                     System.Globalization.CompareOptions.IgnoreNonSpace
                 ) == 0);

            if (existente != null)
            {
                if (existente.Estado)
                {
                    throw new ArgumentException("Ya existe una especialidad con ese nombre.");
                }
                else
                {
                    throw new ArgumentException("Esta especialidad está dada de baja y debe reactivarse.");
                }
            }

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "INSERT INTO ESPECIALIDADES (Nombre, Descripcion, Estado) " +
                    "VALUES (@Nombre, @Descripcion, @Estado)"
                );
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@Estado", nuevo.Estado);

                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}