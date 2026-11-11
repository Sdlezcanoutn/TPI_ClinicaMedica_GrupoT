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
    }
}