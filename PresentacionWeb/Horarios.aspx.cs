using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Negocio;

namespace PresentacionWeb
{
    public partial class Horarios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cargar();
            }
        }

        private void cargar()
        {
            HorarioTrabajoNegocio negocio = new HorarioTrabajoNegocio();

            try
            {
                dgvHorarios.DataSource = negocio.listar();
                dgvHorarios.DataBind();
            }
            catch (Exception ex)
            {
                // Temporalmente, para identificar errores.
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            //Agregar la lógica
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/FormHorario.aspx");
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            LinkButton boton = (LinkButton)sender;

            int id = int.Parse(boton.CommandArgument);

            Response.Redirect("~/FormHorario.aspx?id=" + id);
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            LinkButton boton = (LinkButton)sender;

            int id = int.Parse(boton.CommandArgument);

            try
            {
                HorarioTrabajoNegocio negocio = new HorarioTrabajoNegocio();
                negocio.eliminar(id);

                cargar();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}