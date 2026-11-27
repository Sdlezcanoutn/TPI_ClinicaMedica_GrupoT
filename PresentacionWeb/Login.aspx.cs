using Dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PresentacionWeb
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioAutenticado"] != null)
            {
                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }
        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            string nombreUsuario = txtUsuario.Text.Trim();
            string contraseña = txtContraseña.Text;

            if (string.IsNullOrWhiteSpace(nombreUsuario) ||
                string.IsNullOrWhiteSpace(contraseña))
            {
                lblError.Text = "Ingresá tu usuario y contraseña.";
                lblError.Visible = true;
                return;
            }

            Usuario usuario;

            try
            {
                UsuarioNegocio negocio = new UsuarioNegocio();
                usuario = negocio.autenticar(nombreUsuario, contraseña);

            }
            catch (Exception)
            {
                lblError.Text = "Ocurrió un error al iniciar sesión. Intentá nuevamente.";
                lblError.Visible = true;
                return;
            }

            if (usuario == null)
            {
                lblError.Text = "Usuario o contraseña incorrectos.";
                lblError.Visible = true;
                return;
            }

            // Autenticación correcta
            Session["UsuarioAutenticado"] = usuario;

            // Redirigir a la página principal
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}