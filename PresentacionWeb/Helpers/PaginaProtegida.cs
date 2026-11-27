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
        protected override void OnInit(EventArgs e)
        {
            if (UsuarioActual == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            base.OnInit(e);
        }
    }
}