using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace PresentacionWeb.Helpers
{
    public class PaginaProtegida : Page
    {
        // Propiedades
        protected Usuario UsuarioActual
        {
            get
            {
                return Session["UsuarioAutenticado"] as Usuario;
            }
        }
        // Por defecto, solo el Administrador puede acceder.
        protected virtual bool TienePermiso(Usuario usuario)
        {
            return usuario.Perfil != null && usuario.Perfil.Nombre == "Administrador";
        }
        protected override void OnInit(EventArgs e)
        {
            // 1. Verificar autenticación.
            if (UsuarioActual == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            // 2. Verificar autorización.
            if (!TienePermiso(UsuarioActual))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            base.OnInit(e);
        }
    }
}