using System;
using System.Data.SqlClient;
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
        // ============================================================
        // ALTA DE USUARIOS
        // ============================================================
        public int agregar(Usuario usuario, string contraseña, int? idMedico)
        {
            // --- Validar datos obligatorios ---
            if (usuario == null || usuario.Perfil == null)
                throw new ArgumentException("Los datos del usuario no son válidos.");

            string nombreUsuario = usuario.Nombre_Usuario == null
                ? ""
                : usuario.Nombre_Usuario.Trim();

            string email = usuario.Email == null
                ? ""
                : usuario.Email.Trim();

            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contraseña))
            {
                throw new ArgumentException("Completá todos los campos obligatorios.");
            }

            if (nombreUsuario.Length > 80)
                throw new ArgumentException("El usuario no puede superar los 80 caracteres.");

            if (email.Length > 254)
                throw new ArgumentException("El correo no puede superar los 254 caracteres.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(
                email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("Ingresá un correo electrónico válido.");
            }

            if (usuario.Perfil.Id <= 0)
                throw new ArgumentException("Seleccioná un perfil válido.");

            // --- Generar el hash de la contraseña ---
            string hash = PasswordHelper.GenerarHash(contraseña);

            // ---Validar el perfil y la asociación-- -
            PerfilNegocio perfilNegocio = new PerfilNegocio();

            Perfil perfilSeleccionado = perfilNegocio.listar().Find(p => p.Id == usuario.Perfil.Id);

            if (perfilSeleccionado == null)
                throw new ArgumentException("El perfil seleccionado no existe.");

            bool esMedico = perfilSeleccionado.Nombre == "Médico";

            if (esMedico && (!idMedico.HasValue || idMedico.Value <= 0))
                throw new ArgumentException("Seleccioná un médico para esta cuenta.");

            if (!esMedico && idMedico.HasValue)
                throw new ArgumentException(
                    "Solo las cuentas con perfil Médico pueden asociarse a un médico.");

            // --- Preparar la transacción ---
            AccesoDatos datos = new AccesoDatos();
            bool transaccionIniciada = false;

            try
            {
                datos.iniciarTransaccion();
                transaccionIniciada = true;

                // 1. Registrar usuario y obtener su ID
                datos.setearConsulta(
                    "INSERT INTO USUARIOS " +
                    "(Nombre_Usuario, Password_Hash, Email, Id_Perfil, Estado) " +
                    "VALUES " +
                    "(@NombreUsuario, @PasswordHash, @Email, @IdPerfil, 1); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);"
                );

                datos.setearParametro("@NombreUsuario", nombreUsuario);
                datos.setearParametro("@PasswordHash", hash);
                datos.setearParametro("@Email", email);
                datos.setearParametro("@IdPerfil", usuario.Perfil.Id);

                int idUsuario = (int)datos.ejecutarEscalar();

                if (esMedico)
                {
                    // 2. Asociar al médico, si corresponde
                    datos.limpiarParametros();

                    datos.setearConsulta(
                    "UPDATE MEDICOS " +
                    "SET Id_Usuario = @IdUsuario " +
                    "WHERE Id_Medico = @IdMedico " +
                    "AND Estado = 1 " +
                    "AND Id_Usuario IS NULL; " +
                    "SELECT @@ROWCOUNT;"
                    );

                    datos.setearParametro("@IdUsuario", idUsuario);
                    datos.setearParametro("@IdMedico", idMedico.Value);

                    int filas = Convert.ToInt32(datos.ejecutarEscalar());

                    if (filas == 0)
                    {
                        throw new ArgumentException("El médico no existe, está inactivo o ya tiene una cuenta asociada.");
                    }
                }

                // 3. Confirmar ambas operaciones
                datos.confirmarTransaccion();
                transaccionIniciada = false;

                return idUsuario;
            }
            catch (SqlException ex)
            {
                if (transaccionIniciada)
                    datos.cancelarTransaccion();

                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    throw new ArgumentException("Ya existe un usuario registrado con ese nombre.", ex);
                }

                if (ex.Number == 547)
                {
                    throw new ArgumentException("El perfil seleccionado no existe.", ex);
                }

                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // ============================================================
        // MODIFICACIÓN DE USUARIOS Y ASOCIACIÓN CON MÉDICOS
        // ============================================================
        public void modificar(Usuario usuario, int? idMedico)
        {
            // --- Validar datos obligatorios ---
            if (usuario == null || usuario.Perfil == null)
                throw new ArgumentException("Los datos del usuario no son válidos.");

            if (usuario.Id <= 0)
                throw new ArgumentException("El usuario seleccionado no es válido.");

            string nombreUsuario = usuario.Nombre_Usuario == null
                ? ""
                : usuario.Nombre_Usuario.Trim();

            string email = usuario.Email == null
                ? ""
                : usuario.Email.Trim();

            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Completá todos los campos obligatorios.");
            }

            if (nombreUsuario.Length > 80)
                throw new ArgumentException("El nombre de usuario no puede superar los 80 caracteres.");

            if (email.Length > 254)
                throw new ArgumentException("El correo no puede superar los 254 caracteres.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(
                email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("Ingresá un correo electrónico válido.");
            }

            if (usuario.Perfil.Id <= 0)
                throw new ArgumentException("Seleccioná un perfil válido.");

            // --- Verificar perfil seleccionado ---
            PerfilNegocio perfilNegocio = new PerfilNegocio();

            Perfil perfilSeleccionado = perfilNegocio.listar()
                .Find(p => p.Id == usuario.Perfil.Id);

            if (perfilSeleccionado == null)
                throw new ArgumentException("El perfil seleccionado no existe.");

            bool esMedico = perfilSeleccionado.Nombre == "Médico";

            if (esMedico && (!idMedico.HasValue || idMedico.Value <= 0))
                throw new ArgumentException("Seleccioná un médico para esta cuenta.");

            if (!esMedico && idMedico.HasValue)
                throw new ArgumentException("Solo las cuentas con perfil Médico pueden asociarse a un médico.");

            AccesoDatos datos = new AccesoDatos();
            bool transaccionIniciada = false;

            try
            {
                datos.iniciarTransaccion();
                transaccionIniciada = true;

                // 1. Proteger al último Administrador activo
                datos.setearConsulta(
                    "SELECT CASE WHEN " +
                    "EXISTS (SELECT 1 FROM USUARIOS U " +
                    "INNER JOIN PERFILES P ON U.Id_Perfil = P.Id_Perfil " +
                    "WHERE U.Id_Usuario = @IdUsuario " +
                    "AND U.Estado = 1 AND P.Nombre = N'Administrador') " +
                    "AND (SELECT COUNT(*) FROM USUARIOS U " +
                    "INNER JOIN PERFILES P ON U.Id_Perfil = P.Id_Perfil " +
                    "WHERE U.Estado = 1 AND P.Nombre = N'Administrador') <= 1 " +
                    "THEN 1 ELSE 0 END"
                );

                datos.setearParametro("@IdUsuario", usuario.Id);

                int esUltimoAdministrador = Convert.ToInt32(datos.ejecutarEscalar());

                if (esUltimoAdministrador == 1 && perfilSeleccionado.Nombre != "Administrador")
                {
                    throw new ArgumentException("No se puede cambiar el perfil del último Administrador activo.");
                }

                datos.limpiarParametros();

                // --- Actualizar únicamente los datos administrativos ---
                // 2. Actualizar los datos del usuario
                datos.setearConsulta(
                    "UPDATE USUARIOS SET " +
                    "Nombre_Usuario = @NombreUsuario, " +
                    "Email = @Email, " +
                    "Id_Perfil = @IdPerfil " +
                    "WHERE Id_Usuario = @IdUsuario; " +
                    "SELECT @@ROWCOUNT;"
                );

                datos.setearParametro("@NombreUsuario", nombreUsuario);
                datos.setearParametro("@Email", email);
                datos.setearParametro("@IdPerfil", usuario.Perfil.Id);
                datos.setearParametro("@IdUsuario", usuario.Id);

                int filas = Convert.ToInt32(datos.ejecutarEscalar());

                if (filas == 0)
                    throw new ArgumentException("El usuario seleccionado no existe.");

                // Desvincular el médico anterior incluso al cambiar de perfil.
                datos.limpiarParametros();
                datos.setearConsulta(
                    "UPDATE MEDICOS " +
                    "SET Id_Usuario = NULL " +
                    "WHERE Id_Usuario = @IdUsuario;"
                );
                datos.setearParametro("@IdUsuario", usuario.Id);
                datos.ejecutarAccionTransaccion();

                if (idMedico.HasValue)
                    AsociarMedico(datos, usuario.Id, idMedico.Value);

                datos.confirmarTransaccion();
                transaccionIniciada = false;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (transaccionIniciada)
                    datos.cancelarTransaccion();

                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    throw new ArgumentException("Ya existe otro usuario con ese nombre.", ex);
                }

                if (ex.Number == 547)
                {
                    throw new ArgumentException("El perfil seleccionado no existe.", ex);
                }

                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // ============================================================
        // BAJA LÓGICA DE USUARIOS
        // ============================================================
        public void desactivar(int idUsuario, int idUsuarioActual)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El usuario seleccionado no es válido.");

            // Evitar que el administrador desactive su propia cuenta
            if (idUsuario == idUsuarioActual)
                throw new ArgumentException("No podés desactivar tu propia cuenta.");

            AccesoDatos datos = new AccesoDatos();
            bool transaccionIniciada = false;
            try
            {
                datos.iniciarTransaccion();
                transaccionIniciada = true;

                // Evitar desactivar el último administrador activo
                datos.setearConsulta(
                    "SELECT CASE WHEN EXISTS (" +
                    "SELECT 1 FROM USUARIOS U WITH (UPDLOCK, HOLDLOCK) " +
                    "INNER JOIN PERFILES P ON P.Id_Perfil=U.Id_Perfil " +
                    "WHERE U.Id_Usuario=@Id AND U.Estado=1 AND P.Nombre=N'Administrador') " +
                    "AND (SELECT COUNT(*) FROM USUARIOS U WITH (UPDLOCK, HOLDLOCK) " +
                    "INNER JOIN PERFILES P ON P.Id_Perfil=U.Id_Perfil " +
                    "WHERE U.Estado=1 AND P.Nombre=N'Administrador')=1 " +
                    "THEN 1 ELSE 0 END");
                datos.setearParametro("@Id", idUsuario);
                if (Convert.ToInt32(datos.ejecutarEscalar()) == 1)
                    throw new ArgumentException("No se puede desactivar el último Administrador activo.");

                datos.limpiarParametros();
                datos.setearConsulta(
                    "UPDATE USUARIOS SET Estado = 0 " +
                    "WHERE Id_Usuario = @IdUsuario AND Estado = 1; " +
                    "SELECT @@ROWCOUNT;"
                );

                datos.setearParametro("@IdUsuario", idUsuario);

                int filas = Convert.ToInt32(datos.ejecutarEscalar());

                if (filas == 0)
                    throw new ArgumentException("El usuario no existe o ya se encuentra inactivo.");

                datos.confirmarTransaccion();
                transaccionIniciada = false;
            }
            catch (Exception ex)
            {
                if (transaccionIniciada)
                    datos.cancelarTransaccion();
                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        // ============================================================
        // REACTIVACIÓN DE USUARIOS
        // ============================================================
        public void reactivar(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El usuario seleccionado no es válido.");

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "UPDATE USUARIOS SET Estado = 1 " +
                    "WHERE Id_Usuario = @IdUsuario AND Estado = 0; " +
                    "SELECT @@ROWCOUNT;"
                );

                datos.setearParametro("@IdUsuario", idUsuario);

                int filas = Convert.ToInt32(datos.ejecutarEscalar());

                if (filas == 0)
                    throw new ArgumentException("El usuario no existe o ya se encuentra activo.");
            }
            catch (Exception ex)
            {
                datos.cancelarTransaccion();
                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        // ============================================================
        // CAMBIO DE CONTRASEÑA DE USUARIOS
        // ============================================================
        public void cambiarContraseña(int idUsuario, string nuevaContraseña)
        {
            // --- Validar usuario seleccionado ---
            if (idUsuario <= 0)
                throw new ArgumentException("El usuario seleccionado no es válido.");

            // --- Validar nueva contraseña ---
            if (string.IsNullOrWhiteSpace(nuevaContraseña))
                throw new ArgumentException("Ingresá una nueva contraseña.");

            if (nuevaContraseña.Length < 8)
                throw new ArgumentException(
                    "La contraseña debe tener al menos 8 caracteres.");

            // --- Generar hash seguro ---
            string nuevoHash = PasswordHelper.GenerarHash(nuevaContraseña);

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.iniciarTransaccion();

                // --- Actualizar solamente la contraseña ---
                datos.setearConsulta(
                    "UPDATE USUARIOS " +
                    "SET Password_Hash = @PasswordHash " +
                    "WHERE Id_Usuario = @IdUsuario AND Estado = 1; " +
                    "SELECT @@ROWCOUNT;"
                );

                datos.setearParametro("@PasswordHash", nuevoHash);
                datos.setearParametro("@IdUsuario", idUsuario);

                int filas = Convert.ToInt32(datos.ejecutarEscalar());

                if (filas == 0)
                {
                    throw new ArgumentException("El usuario no existe o se encuentra inactivo.");
                }
                
                datos.confirmarTransaccion();
            }
            catch (Exception ex)
            {
                datos.cancelarTransaccion();
                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        // Para que el formulario seleccione al profesional ya asociado al editar.
        public int? obtenerIdMedicoAsociado(int idUsuario)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT Id_Medico FROM MEDICOS WHERE Id_Usuario=@Id");
                datos.setearParametro("@Id", idUsuario);
                datos.ejecutarLectura();
                if (datos.Lector.Read()) 
                    return (int)datos.Lector["Id_Medico"];

                return null;
            }
            finally { datos.cerrarConexion(); }
        }
        private void AsociarMedico(AccesoDatos datos, int idUsuario, int idMedico)
        {
            datos.limpiarParametros();
            datos.setearConsulta(
                "UPDATE MEDICOS SET Id_Usuario=@IdUsuario " +
                "WHERE Id_Medico=@IdMedico AND Estado=1 AND Id_Usuario IS NULL; " +
                "SELECT @@ROWCOUNT;");
            datos.setearParametro("@IdUsuario", idUsuario);
            datos.setearParametro("@IdMedico", idMedico);
            if (Convert.ToInt32(datos.ejecutarEscalar()) != 1)
                throw new ArgumentException("El médico no existe, está inactivo o ya tiene otra cuenta.");
        }
    }
}
