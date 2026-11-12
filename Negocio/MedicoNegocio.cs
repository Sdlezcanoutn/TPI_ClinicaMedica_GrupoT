using System;
using System.Collections.Generic;
using Dominio;
using Datos;

namespace Negocio
{
    public class MedicoNegocio
    {
        // ============================================================
        // LISTADO DE MÉDICOS CON SUS ESPECIALIDADES
        // ============================================================
        public List<Medico> listar()
        {
            // --- Crear lista de médicos y acceso a datos ---
            List<Medico> lista = new List<Medico>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                // --- Consultar médicos y especialidades asignadas ---
                datos.setearConsulta(
                    "SELECT M.Id_Medico, M.Nombre, M.Apellido, M.DNI, " +
                    "M.Matricula, M.Telefono, M.Email, M.Estado, " +
                    "E.Id_Especialidad, E.Nombre AS NombreEspecialidad " +
                    "FROM MEDICOS M " +
                    "LEFT JOIN MEDICOS_ESPECIALIDADES ME " +
                    "ON M.Id_Medico = ME.Id_Medico " +
                    "LEFT JOIN ESPECIALIDADES E " +
                    "ON ME.Id_Especialidad = E.Id_Especialidad " +
                    "ORDER BY M.Apellido, M.Nombre, E.Nombre"
                );

                datos.ejecutarLectura();

                // --- Evitar duplicar médicos con varias especialidades ---
                Dictionary<int, Medico> medicos = new Dictionary<int, Medico>();

                while (datos.Lector.Read())
                {
                    int idMedico = (int)datos.Lector["Id_Medico"];

                    // --- Crear el médico solo si todavía no fue agregado ---
                    if (!medicos.ContainsKey(idMedico))
                    {
                        Medico aux = new Medico();

                        aux.Id = idMedico;
                        aux.Nombre = (string)datos.Lector["Nombre"];
                        aux.Apellido = (string)datos.Lector["Apellido"];
                        aux.DNI = (string)datos.Lector["DNI"];
                        aux.Matricula = (string)datos.Lector["Matricula"];

                        aux.Telefono = datos.Lector["Telefono"] == DBNull.Value
                            ? ""
                            : (string)datos.Lector["Telefono"];

                        aux.Email = (string)datos.Lector["Email"];
                        aux.Estado = (bool)datos.Lector["Estado"];

                        medicos.Add(idMedico, aux);
                        lista.Add(aux);
                    }

                    // --- Agregar cada especialidad al médico correspondiente ---
                    if (datos.Lector["Id_Especialidad"] != DBNull.Value)
                    {
                        Especialidad especialidad = new Especialidad();

                        especialidad.Id = (int)datos.Lector["Id_Especialidad"];
                        especialidad.Nombre = (string)datos.Lector["NombreEspecialidad"];

                        medicos[idMedico].Especialidades.Add(especialidad);
                    }
                }

                // --- Devolver el listado completo ---
                return lista;
            }
            finally
            {
                // --- Cerrar la conexión a la base de datos ---
                datos.cerrarConexion();
            }
        }

        // ============================================================
        // ALTA DE MÉDICOS CON SUS ESPECIALIDADES
        // ============================================================
        public void agregar(Medico medico)
        {
            // --- Validar que el médico tenga especialidades asignadas ---
            if (medico == null || medico.Especialidades == null ||
                medico.Especialidades.Count == 0)
            {
                throw new ArgumentException(
                    "El médico debe tener al menos una especialidad.");
            }

            AccesoDatos datos = new AccesoDatos();
            bool transaccionIniciada = false;

            try
            {
                // --- Iniciar una transacción para guardar todos los datos juntos ---
                datos.iniciarTransaccion();
                transaccionIniciada = true;

                // ====================================================
                // 1. REGISTRAR LOS DATOS DEL MÉDICO
                // ====================================================

                datos.setearConsulta(
                    "INSERT INTO MEDICOS " +
                    "(Nombre, Apellido, DNI, Matricula, Telefono, Email, Estado) " +
                    "VALUES " +
                    "(@Nombre, @Apellido, @DNI, @Matricula, @Telefono, @Email, 1); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);"
                );

                datos.setearParametro("@Nombre", medico.Nombre);
                datos.setearParametro("@Apellido", medico.Apellido);
                datos.setearParametro("@DNI", medico.DNI);
                datos.setearParametro("@Matricula", medico.Matricula);
                datos.setearParametro("@Telefono",
                    string.IsNullOrWhiteSpace(medico.Telefono)
                        ? (object)DBNull.Value
                        : medico.Telefono);
                datos.setearParametro("@Email", medico.Email);

                // --- Recuperar el ID generado por SQL Server ---
                int idMedico = (int)datos.ejecutarEscalar();

                // ====================================================
                // 2. REGISTRAR LAS ESPECIALIDADES DEL MÉDICO
                // ====================================================

                foreach (Especialidad especialidad in medico.Especialidades)
                {
                    // --- Limpiar los parámetros de la consulta anterior ---
                    datos.limpiarParametros();

                    datos.setearConsulta(
                        "INSERT INTO MEDICOS_ESPECIALIDADES " +
                        "(Id_Medico, Id_Especialidad) " +
                        "VALUES (@IdMedico, @IdEspecialidad)"
                    );

                    // --- Relacionar el médico con cada especialidad ---
                    datos.setearParametro("@IdMedico", idMedico);
                    datos.setearParametro("@IdEspecialidad", especialidad.Id);

                    datos.ejecutarAccionTransaccion();
                }

                // ====================================================
                // 3. CONFIRMAR LA OPERACIÓN
                // ====================================================

                // --- Guardar definitivamente el médico y sus especialidades ---
                datos.confirmarTransaccion();
                transaccionIniciada = false;
            }
            catch
            {
                // --- Deshacer las operaciones si ocurre un error ---
                if (transaccionIniciada)
                    datos.cancelarTransaccion();

                throw;
            }
            finally
            {
                // --- Cerrar la conexión a la base de datos ---
                datos.cerrarConexion();
            }
        }
    }
}


