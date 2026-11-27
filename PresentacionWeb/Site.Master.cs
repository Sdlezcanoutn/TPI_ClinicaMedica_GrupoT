using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PresentacionWeb
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ConfigurarNavegacion();
        }
        private void ConfigurarNavegacion()
        {
            Usuario usuario = Session["UsuarioAutenticado"] as Usuario;

            bool autenticado = usuario != null && usuario.Perfil != null;

            // Opciones que requieren iniciar sesión
            navInicio.Visible = autenticado;
            navUsuario.Visible = autenticado;
            navLogout.Visible = autenticado;

            // Opción pública
            navLogin.Visible = !autenticado;

            // Ocultar inicialmente los módulos
            navPacientes.Visible = false;
            navMedicos.Visible = false;
            navEspecialidades.Visible = false;
            navHorarios.Visible = false;
            navEmpleados.Visible = false;
            navTurnos.Visible = false;
            liUsuarios.Visible = false;

            if (!autenticado)
                return;

            // Mostrar información del usuario
            litUsuario.Text = usuario.Nombre_Usuario;

            string perfil = usuario.Perfil.Nombre;

            bool esAdministrador = perfil == "Administrador";
            bool esRecepcionista = perfil == "Recepcionista";
            bool esMedico = perfil == "Médico";

            navPacientes.Visible = esAdministrador || esRecepcionista;
            navMedicos.Visible = esAdministrador || esRecepcionista;
            navEspecialidades.Visible = esAdministrador || esRecepcionista;
            navHorarios.Visible = esAdministrador || esRecepcionista;

            navEmpleados.Visible = esAdministrador;
            liUsuarios.Visible = esAdministrador;

            navTurnos.Visible = esAdministrador || esRecepcionista || esMedico;
        }
    }
}