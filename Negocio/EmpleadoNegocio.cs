using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using Datos;

namespace Negocio
{
    public class EmpleadoNegocio
    {
        // Devuelve todos los empleados registrados (activos e inactivos)
        public List<Empleado> listar()
        {
            List<Empleado> lista = new List<Empleado>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "SELECT Id_Empleado, Nombre, Apellido, DNI, Estado FROM EMPLEADOS"
                );
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Empleado aux = new Empleado
                    {
                        Id = (int)datos.Lector["Id_Empleado"],
                        Nombre = (string)datos.Lector["Nombre"],
                        Apellido = (string)datos.Lector["Apellido"],
                        DNI = (string)datos.Lector["DNI"],
                        Estado = (bool)datos.Lector["Estado"]
                    };

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

        public void agregar(Empleado nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "INSERT INTO EMPLEADOS (Nombre, Apellido, DNI, Estado) VALUES (@Nombre, @Apellido, @DNI, 1)"
                );
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Apellido", nuevo.Apellido);
                datos.setearParametro("@DNI", nuevo.DNI);

                datos.ejecutarAccion();
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

        public void modificar(Empleado emp)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "UPDATE EMPLEADOS SET Nombre = @Nombre, Apellido = @Apellido, DNI = @DNI WHERE Id_Empleado = @Id"
                );
                datos.setearParametro("@Nombre", emp.Nombre);
                datos.setearParametro("@Apellido", emp.Apellido);
                datos.setearParametro("@DNI", emp.DNI);
                datos.setearParametro("@Id", emp.Id);

                datos.ejecutarAccion();
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

        public void eliminarLogico(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("UPDATE EMPLEADOS SET Estado = 0 WHERE Id_Empleado = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
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

        // Reactiva un empleado dado de baja lógica
        public void reactivar(int id)
        {
            Empleado empleado = listar().FirstOrDefault(emp => emp.Id == id);

            if (empleado == null)
            {
                throw new ArgumentException("El empleado seleccionado no existe.");
            }

            if (empleado.Estado)
            {
                throw new ArgumentException("El empleado ya se encuentra activo.");
            }

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "UPDATE EMPLEADOS SET Estado = 1 WHERE Id_Empleado = @Id AND Estado = 0"
                );
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
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
        public bool existeDni(string dni, int? idActual = null)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = "SELECT COUNT(*) AS Cantidad FROM EMPLEADOS WHERE DNI = @DNI";
                if (idActual.HasValue)
                {
                    consulta += " AND Id_Empleado != @Id";
                }

                datos.setearConsulta(consulta);
                datos.setearParametro("@DNI", dni);

                if (idActual.HasValue)
                {
                    datos.setearParametro("@Id", idActual.Value);
                }

                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    int cantidad = Convert.ToInt32(datos.Lector["Cantidad"]);
                    return cantidad > 0;
                }

                return false;
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
    }
}