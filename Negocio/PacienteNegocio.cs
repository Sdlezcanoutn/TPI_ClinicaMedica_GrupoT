using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using Datos;

namespace Negocio
{
    public class PacienteNegocio
    {
        public List<Paciente> listar()
        {
            List<Paciente> lista = new List<Paciente>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "SELECT Id_Paciente, Nombre, Apellido, DNI, Fecha_Nacimiento, Telefono, Email, Direccion, Estado FROM PACIENTES WHERE Estado = 1"
                );
                
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Paciente aux = new Paciente();

                    aux.Id = (int)datos.Lector["Id_Paciente"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Apellido = (string)datos.Lector["Apellido"];
                    aux.DNI = (string)datos.Lector["DNI"];
                    aux.Fecha_Nacimiento = (DateTime)datos.Lector["Fecha_Nacimiento"]; 

                    aux.Telefono = datos.Lector["Telefono"] == DBNull.Value ? "" : (string)datos.Lector["Telefono"];
                    aux.Email = (string)datos.Lector["Email"];
                    aux.Direccion = datos.Lector["Direccion"] == DBNull.Value ? "" : (string)datos.Lector["Direccion"];
                    aux.Estado = (bool)datos.Lector["Estado"];

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void agregar(Paciente nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("INSERT INTO PACIENTES (Nombre, Apellido, DNI, Fecha_Nacimiento, Telefono, Email, Direccion, Estado) VALUES (@Nombre, @Apellido, @DNI, @Fecha_Nacimiento, @Telefono, @Email, @Direccion, 1)");

                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Apellido", nuevo.Apellido);
                datos.setearParametro("@DNI", nuevo.DNI);
                datos.setearParametro("@Fecha_Nacimiento", nuevo.Fecha_Nacimiento);
                datos.setearParametro("@Telefono", string.IsNullOrEmpty(nuevo.Telefono) ? (object)DBNull.Value : nuevo.Telefono);
                datos.setearParametro("@Email", nuevo.Email);
                datos.setearParametro("@Direccion", string.IsNullOrEmpty(nuevo.Direccion) ? (object)DBNull.Value : nuevo.Direccion);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public void modificar(Paciente pac)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE PACIENTES SET Nombre = @nombre, Apellido = @apellido, DNI = @dni, Fecha_Nacimiento = @fecha, Telefono = @telefono, Email = @email, Direccion = @direccion WHERE Id_Paciente = @id");
                datos.setearParametro("@nombre", pac.Nombre);
                datos.setearParametro("@apellido", pac.Apellido);
                datos.setearParametro("@dni", pac.DNI);
                datos.setearParametro("@fecha", pac.Fecha_Nacimiento);
                datos.setearParametro("@telefono", (object)pac.Telefono ?? DBNull.Value);
                datos.setearParametro("@email", pac.Email);
                datos.setearParametro("@direccion", (object)pac.Direccion ?? DBNull.Value);
                datos.setearParametro("@id", pac.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}