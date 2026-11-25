using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using Datos;

namespace Negocio
{
    public class HorarioTrabajoNegocio
    {
        public List<HorarioTrabajo> listar()
        {
            List<HorarioTrabajo> lista = new List<HorarioTrabajo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "SELECT H.Id_Horario_Trabajo, H.Id_Medico, " +
                    "H.Dia_Semana, H.Hora_Entrada, H.Hora_Salida, H.Estado, " +
                    "M.Nombre, M.Apellido " +
                    "FROM HORARIOS_TRABAJO H " +
                    "INNER JOIN MEDICOS M ON H.Id_Medico = M.Id_Medico"
                );

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    HorarioTrabajo aux = new HorarioTrabajo();

                    aux.Id = (int)datos.Lector["Id_Horario_Trabajo"];

                    aux.Medico = new Medico();
                    aux.Medico.Id = (int)datos.Lector["Id_Medico"];
                    aux.Medico.Nombre = (string)datos.Lector["Nombre"];
                    aux.Medico.Apellido = (string)datos.Lector["Apellido"];

                    aux.Dia_Semana = datos.Lector["Dia_Semana"].ToString();

                    TimeSpan entrada = (TimeSpan)datos.Lector["Hora_Entrada"];
                    TimeSpan salida = (TimeSpan)datos.Lector["Hora_Salida"];

                    aux.Hora_Entrada = DateTime.Today.Add(entrada);
                    aux.Hora_Salida = DateTime.Today.Add(salida);

                    aux.Estado = (bool)datos.Lector["Estado"];

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public void agregar(HorarioTrabajo horario)
        {
            // Validar médico activo
            if (!existeMedicoActivo(horario.Medico.Id))
            {
                throw new InvalidOperationException(
                    "El médico no existe o se encuentra inactivo."
                );
            }

            // Validar superposición horaria
            if (existeSuperposicion(horario))
                throw new InvalidOperationException(
                    "El médico ya tiene un horario superpuesto para ese día."
                );

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "INSERT INTO HORARIOS_TRABAJO " +
                    "(Id_Medico, Dia_Semana, Hora_Entrada, Hora_Salida) " +
                    "VALUES (@IdMedico, @DiaSemana, @HoraEntrada, @HoraSalida)"
                );

                datos.setearParametro("@IdMedico", horario.Medico.Id);
                datos.setearParametro("@DiaSemana", byte.Parse(horario.Dia_Semana));
                datos.setearParametro("@HoraEntrada", horario.Hora_Entrada.TimeOfDay);
                datos.setearParametro("@HoraSalida", horario.Hora_Salida.TimeOfDay);

                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public void modificar(HorarioTrabajo horario)
        {
            // Validar si el horario existe en la base de datos y está activo
            if (horario.Id <= 0 || !existeHorarioActivo(horario.Id))
            {
                throw new InvalidOperationException(
                    "El horario no existe o se encuentra inactivo."
                );
            }

            // Validar médico activo
            if (!existeMedicoActivo(horario.Medico.Id)){
                throw new InvalidOperationException(
                    "El médico no existe o se encuentra inactivo."
                );
            }

            // Validar superposición horaria
            if (existeSuperposicion(horario))
                throw new InvalidOperationException(
                    "El médico ya tiene un horario superpuesto para ese día."
                );

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "UPDATE HORARIOS_TRABAJO SET " +
                    "Id_Medico = @IdMedico, " +
                    "Dia_Semana = @DiaSemana, " +
                    "Hora_Entrada = @HoraEntrada, " +
                    "Hora_Salida = @HoraSalida " +
                    "WHERE Id_Horario_Trabajo = @Id " +
                    "AND Estado = 1");

                datos.setearParametro("@IdMedico", horario.Medico.Id);
                datos.setearParametro("@DiaSemana", byte.Parse(horario.Dia_Semana));
                datos.setearParametro("@HoraEntrada", horario.Hora_Entrada.TimeOfDay);
                datos.setearParametro("@HoraSalida", horario.Hora_Salida.TimeOfDay);
                datos.setearParametro("@Id", horario.Id);

                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public void eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "UPDATE HORARIOS_TRABAJO " +
                    "SET Estado = 0 " +
                    "WHERE Id_Horario_Trabajo = @Id");

                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        private bool existeSuperposicion(HorarioTrabajo horario)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "SELECT TOP 1 1 FROM HORARIOS_TRABAJO " +
                    "WHERE Id_Medico = @IdMedico " +
                    "AND Dia_Semana = @DiaSemana " +
                    "AND Estado = 1 " +
                    "AND Id_Horario_Trabajo <> @IdHorario " +
                    "AND Hora_Entrada < @HoraSalida " +
                    "AND Hora_Salida > @HoraEntrada"
                );

                datos.setearParametro("@IdMedico", horario.Medico.Id);
                datos.setearParametro("@DiaSemana", byte.Parse(horario.Dia_Semana));
                datos.setearParametro("@IdHorario", horario.Id);
                datos.setearParametro("@HoraEntrada", horario.Hora_Entrada.TimeOfDay);
                datos.setearParametro("@HoraSalida", horario.Hora_Salida.TimeOfDay);

                datos.ejecutarLectura();

                return datos.Lector.Read();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        private bool existeHorarioActivo(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "SELECT TOP 1 1 " +
                    "FROM HORARIOS_TRABAJO " +
                    "WHERE Id_Horario_Trabajo = @Id " +
                    "AND Estado = 1"
                );

                datos.setearParametro("@Id", id);
                datos.ejecutarLectura();

                return datos.Lector.Read();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        private bool existeMedicoActivo(int idMedico)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "SELECT TOP 1 1 " +
                    "FROM MEDICOS " +
                    "WHERE Id_Medico = @IdMedico " +
                    "AND Estado = 1"
                );

                datos.setearParametro("@IdMedico", idMedico);
                datos.ejecutarLectura();

                return datos.Lector.Read();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
