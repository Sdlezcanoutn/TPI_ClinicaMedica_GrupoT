using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using Datos;
using System.Data;
using Negocio.Helpers;

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
        public Usuario autenticar(string nombreUsuario, string contraseña)
        {
            // Validar que se recibió un nombre de usuario
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("No se recibió ningún nombre de usuario.");

            // Validar que se recibió una contraseña
            if (string.IsNullOrWhiteSpace(contraseña))
                throw new ArgumentException("La contraseña es obligatoria.");

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "SELECT " +
                    "U.Id_Usuario, " +
                    "U.Nombre_Usuario, " +
                    "U.Password_Hash, " +
                    "U.Email, " +
                    "U.Estado, " +
                    "P.Id_Perfil, " +
                    "P.Nombre AS NombrePerfil " +
                    "FROM USUARIOS U " +
                    "INNER JOIN PERFILES P " +
                    "ON U.Id_Perfil = P.Id_Perfil " +
                    "WHERE U.Nombre_Usuario = @nombreUsuario " +
                    "AND U.Estado = 1;"
                );

                datos.setearParametro("@nombreUsuario", nombreUsuario.Trim());
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    string hashGuardado = (string)datos.Lector["Password_Hash"];

                    if (!PasswordHelper.Verificar(contraseña, hashGuardado))
                    {
                        return null;
                    }

                    Usuario usuario = new Usuario();

                    usuario.Id = (int)datos.Lector["Id_Usuario"];
                    usuario.Nombre_Usuario = (string)datos.Lector["Nombre_Usuario"];
                    usuario.Email = (string)datos.Lector["Email"];
                    usuario.Estado = (bool)datos.Lector["Estado"];

                    usuario.Perfil = new Perfil();
                    usuario.Perfil.Id = (int)datos.Lector["Id_Perfil"];
                    usuario.Perfil.Nombre = (string)datos.Lector["NombrePerfil"];

                    return usuario;
                }

                return null;
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
