using Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PresentacionWeb
{
    public partial class Pacientes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarPacientes();
            }
        }

        private void CargarPacientes()
        {
            PacienteNegocio negocio = new PacienteNegocio();
            try
            {
                dgvPacientes.DataSource = negocio.listar();
                dgvPacientes.DataBind();
            }
            catch (Exception ex)
            {
                // Manejo de errores o mostrar mensaje
                Session.Add("error", ex.ToString());
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            Response.Redirect("FormPaciente.aspx");
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string id = btn.CommandArgument;
            Response.Redirect("FormPaciente.aspx?id=" + id);
        }

        protected void btnEliminar_Command(object sender, CommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Eliminar")
                {
                    int idPaciente = int.Parse(e.CommandArgument.ToString());
                    PacienteNegocio negocio = new PacienteNegocio();

                    // Llamamos al método de baja lógica
                    negocio.eliminarLogico(idPaciente);

                    // Recargamos la grilla para que desaparezca el paciente dado de baja
                    CargarPacientes();
                }
            }
            catch (Exception ex)
            {
                Session.Add("error", ex.ToString());
            }
        }
    }
}