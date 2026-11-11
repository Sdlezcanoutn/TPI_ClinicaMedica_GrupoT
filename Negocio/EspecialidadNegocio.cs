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

            // --- Validar longitud de los campos ---
            if (nuevo.Nombre.Length > 100)
            {
                throw new ArgumentException("El nombre no puede superar los 100 caracteres.");
            }

            if (!string.IsNullOrEmpty(nuevo.Descripcion) && nuevo.Descripcion.Length > 500)
            {
                throw new ArgumentException("La descripción no puede superar los 500 caracteres.");
            }

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

            // --- Validar longitud de los campos ---
            if (especialidad.Nombre.Length > 100)
            {
                throw new ArgumentException("El nombre no puede superar los 100 caracteres.");
            }

            if (!string.IsNullOrEmpty(especialidad.Descripcion) && especialidad.Descripcion.Length > 500)
            {
                throw new ArgumentException("La descripción no puede superar los 500 caracteres.");
            }

            // --- Verificar que la especialidad exista ---
            List<Especialidad> lista = listar();

            Especialidad actual = lista.FirstOrDefault(e =>
                e.Id == especialidad.Id);

            if (actual == null)
            {
                throw new ArgumentException("La especialidad que intenta modificar no existe.");
            }

            // --- Verificar que la especialidad esté activa ---
            if (!actual.Estado)
            {
                throw new ArgumentException("No se puede modificar una especialidad inactiva.");
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

        // ============================================================
        // CONTEO DE TURNOS PENDIENTES POR ESPECIALIDAD
        // ============================================================
        public int contarTurnosPendientes(int idEspecialidad)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                // --- Consultar turnos futuros pendientes ---
                datos.setearConsulta(
                    "SELECT COUNT(*) AS Cantidad " +
                    "FROM TURNOS T " +
                    "INNER JOIN ESTADOS_TURNO E " +
                    "ON T.Id_Estado_Turno = E.Id_Estado_Turno " +
                    "WHERE T.Id_Especialidad = @IdEspecialidad " +
                    "AND E.Nombre IN (N'Nuevo', N'Reprogramado') " +
                    "AND (T.Fecha > CAST(GETDATE() AS DATE) " +
                    "OR (T.Fecha = CAST(GETDATE() AS DATE) " +
                    "AND T.Hora_Inicio > CAST(GETDATE() AS TIME)))"
                );

                datos.setearParametro("@IdEspecialidad", idEspecialidad);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    return Convert.ToInt32(datos.Lector["Cantidad"]);
                }

                return 0;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // ============================================================
        // BAJA LÓGICA DE ESPECIALIDADES
        // ============================================================
        public void darDeBaja(int id)
        {
            // --- Verificar que la especialidad exista y esté activa ---
            Especialidad especialidad = listar()
                .FirstOrDefault(e => e.Id == id);

            if (especialidad == null)
            {
                throw new ArgumentException("La especialidad seleccionada no existe.");
            }

            if (!especialidad.Estado)
            {
                throw new ArgumentException("La especialidad ya se encuentra dada de baja.");
            }

            // --- Desactivar especialidad ---
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "UPDATE ESPECIALIDADES " +
                    "SET Estado = 0 " +
                    "WHERE Id_Especialidad = @Id AND Estado = 1"
                );

                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // ============================================================
        // REACTIVACIÓN DE ESPECIALIDADES
        // ============================================================
        public void reactivar(int id)
        {
            // --- Verificar que la especialidad exista y esté inactiva ---
            Especialidad especialidad = listar()
                .FirstOrDefault(e => e.Id == id);

            if (especialidad == null)
            {
                throw new ArgumentException("La especialidad seleccionada no existe.");
            }

            if (especialidad.Estado)
            {
                throw new ArgumentException("La especialidad ya se encuentra activa.");
            }

            // --- Reactivar especialidad ---
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "UPDATE ESPECIALIDADES " +
                    "SET Estado = 1 " +
                    "WHERE Id_Especialidad = @Id AND Estado = 0"
                );

                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

    }
}