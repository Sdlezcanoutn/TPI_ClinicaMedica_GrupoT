using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PresentacionWeb
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Eliminar los datos del usuario autenticado
            Session.Clear();

            // Finalizar la sesión actual
            Session.Abandon();

            // Volver al Login
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
    }
}