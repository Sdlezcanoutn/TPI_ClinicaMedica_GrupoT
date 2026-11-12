using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace Datos
{
    public class AccesoDatos
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;

        // ============================================================
        // TRANSACCIONES
        // ============================================================
        private SqlTransaction transaccion;
        public SqlDataReader Lector
        {
            get { return lector; }
        }

        // ============================================================
        // CONSTRUCTOR
        // ============================================================
        public AccesoDatos()
        {
            conexion = new SqlConnection("server=.\\SQLEXPRESS; database=CLINICA_DB; integrated security=true;");
            comando = new SqlCommand();
        }

        // ============================================================
        // CONFIGURAR CONSULTA
        // Establece la consulta SQL que se va a ejecutar.
        // ============================================================
        public void setearConsulta(string consulta)
        {
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = consulta;
        }

        // ============================================================
        // EJECUTAR LECTURA
        // Ejecuta una consulta y permite recorrer los registros obtenidos.
        // ============================================================
        public void ejecutarLectura()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // ============================================================
        // EJECUTAR ACCIÓN
        // Ejecuta operaciones SQL que no devuelven registros,como INSERT, UPDATE o DELETE.
        // ============================================================
        public void ejecutarAccion()
        {
            comando.Connection = conexion;

            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // ============================================================
        // CONFIGURAR PARÁMETROS
        // Agrega los parámetros necesarios para ejecutar una consulta SQL.
        // ============================================================
        public void setearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor);
        }

        // ============================================================
        // LIMPIAR PARÁMETROS
        // Elimina los parámetros de la consulta anterior para poder reutilizar el comando en otra operación.
        // ============================================================
        public void limpiarParametros()
        {
            comando.Parameters.Clear();
        }

        // ============================================================
        // INICIAR TRANSACCIÓN
        // Abre la conexión e inicia una transacción para ejecutar varias operaciones como una sola unidad de trabajo.
        // ============================================================
        public void iniciarTransaccion()
        {
            conexion.Open();
            transaccion = conexion.BeginTransaction();

            comando.Connection = conexion;
            comando.Transaction = transaccion;
        }

        // ============================================================
        // EJECUTAR CONSULTA Y OBTENER UN VALOR
        // ============================================================
        public object ejecutarEscalar()
        {
            return comando.ExecuteScalar();
        }

        // ============================================================
        // EJECUTAR ACCIÓN DENTRO DE UNA TRANSACCIÓN
        // ============================================================
        public void ejecutarAccionTransaccion()
        {
            comando.ExecuteNonQuery();
        }

        // ============================================================
        // CONFIRMAR TRANSACCIÓN
        // ============================================================
        public void confirmarTransaccion()
        {
            transaccion.Commit();
            transaccion = null;
        }

        // ============================================================
        // REVERTIR TRANSACCIÓN
        // Deshace las operaciones realizadas si ocurre un error.
        // ============================================================
        public void cancelarTransaccion()
        {
            if (transaccion != null)
            {
                transaccion.Rollback();
                transaccion = null;
            }
        }

        // ============================================================
        // CERRAR CONEXIÓN
        // Cierra el lector de datos, si existe, y la conexión a SQL Server.
        // ============================================================
        public void cerrarConexion()
        {
            //Lector de datos: si existe, cerrarlo para liberar recursos.
            if (lector != null)
                lector.Close();
            conexion.Close();
        }
    }
}

