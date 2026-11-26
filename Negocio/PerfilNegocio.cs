using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using Datos;

namespace Negocio
{
    public class PerfilNegocio
    {
        public List<Perfil> listar()
        {
            List<Perfil> lista = new List<Perfil>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "SELECT Id_Perfil, Nombre " +
                    "FROM PERFILES"
                );

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Perfil aux = new Perfil();

                    aux.Id = (int)datos.Lector["Id_Perfil"];
                    aux.Nombre = (string)datos.Lector["Nombre"];

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
    }
}
