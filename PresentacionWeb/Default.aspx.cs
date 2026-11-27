using Dominio;
using PresentacionWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PresentacionWeb
{
    public partial class _Default : PaginaProtegida
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override bool TienePermiso(Usuario usuario)
        {
            return usuario.Perfil != null && 
                (usuario.Perfil.Nombre == "Administrador" || 
                 usuario.Perfil.Nombre == "Recepcionista" || 
                 usuario.Perfil.Nombre == "Médico");
        }
    }
}