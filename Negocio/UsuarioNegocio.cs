using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using Datos;

namespace Negocio
{
    public class UsuarioNegocio
    {
        public List<Usuario> listar()
        {
            List<Usuario> lista = new List<Usuario>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "SELECT " +
                    "U.Id_Usuario, " +
                    "U.Nombre_Usuario, " +
                    "U.Email, " +
                    "U.Estado, " +
                    "P.Id_Perfil, " +
                    "P.Nombre AS NombrePerfil " +
                    "FROM USUARIOS U " +
                    "INNER JOIN PERFILES P " +
                    "ON U.Id_Perfil = P.Id_Perfil;"
                );

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Usuario aux = new Usuario();

                    aux.Id = (int)datos.Lector["Id_Usuario"];
                    aux.Nombre_Usuario = (string)datos.Lector["Nombre_Usuario"];
                    aux.Email = (string)datos.Lector["Email"];
                    aux.Estado = (bool)datos.Lector["Estado"];

                    aux.Perfil = new Perfil();
                    aux.Perfil.Id = (int)datos.Lector["Id_Perfil"];
                    aux.Perfil.Nombre = (string)datos.Lector["NombrePerfil"];

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
