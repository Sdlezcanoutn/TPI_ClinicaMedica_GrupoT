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
        // ============================================================
        // LISTADO DE ESPECIALIDADES
        // ============================================================
        public List<Especialidad> listar()
        {
            List<Especialidad> lista = new List<Especialidad>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                // --- Consultar especialidades ---
                datos.setearConsulta(
                    "SELECT Id_Especialidad, Nombre, Descripcion, Estado FROM ESPECIALIDADES"
                );

                datos.ejecutarLectura();

                // --- Cargar listado ---
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

        // ============================================================
        // ALTA DE ESPECIALIDADES
        // ============================================================
        public void agregar(Especialidad nuevo)
        {
            // --- Validar datos ---
            if (nuevo == null || string.IsNullOrWhiteSpace(nuevo.Nombre))
            {
                throw new ArgumentException("El nombre de la especialidad es obligatorio.");
            }

            nuevo.Nombre = nuevo.Nombre.Trim();

            // --- Validar nombres duplicados ---
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

            // --- Registrar especialidad ---
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

        // ============================================================
        // MODIFICACIÓN DE ESPECIALIDADES
        // ============================================================
        public void modificar(Especialidad especialidad)
        {
            // --- Validar datos ---
            if (especialidad == null || string.IsNullOrWhiteSpace(especialidad.Nombre))
            {
                throw new ArgumentException("El nombre de la especialidad es obligatorio.");
            }

            especialidad.Nombre = especialidad.Nombre.Trim();

            // --- Verificar que la especialidad exista ---
            List<Especialidad> lista = listar();

            Especialidad actual = lista.FirstOrDefault(e =>
                e.Id == especialidad.Id);

            if (actual == null)
            {
                throw new ArgumentException("La especialidad que intenta modificar no existe.");
            }

            // --- Validar nombres duplicados ---
            Especialidad existente = lista.FirstOrDefault(e =>
                e.Id != especialidad.Id &&
                System.Globalization.CultureInfo.GetCultureInfo("es-AR")
                    .CompareInfo.Compare(
                        e.Nombre.Trim(),
                        especialidad.Nombre,
                        System.Globalization.CompareOptions.IgnoreCase |
                        System.Globalization.CompareOptions.IgnoreNonSpace
                    ) == 0);

            if (existente != null)
            {
                throw new ArgumentException(
                    existente.Estado
                        ? "Ya existe una especialidad con ese nombre."
                        : "Esta especialidad está dada de baja y debe reactivarse.");
            }

            // --- Actualizar especialidad ---
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "UPDATE ESPECIALIDADES " +
                    "SET Nombre = @Nombre, Descripcion = @Descripcion " +
                    "WHERE Id_Especialidad = @Id"
                );

                datos.setearParametro("@Nombre", especialidad.Nombre);
                datos.setearParametro("@Descripcion", especialidad.Descripcion ?? "");
                datos.setearParametro("@Id", especialidad.Id);

                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}