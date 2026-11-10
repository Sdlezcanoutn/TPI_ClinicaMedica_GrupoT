using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;

namespace PresentacionWeb
{
    public partial class Especialidades : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EspecialidadNegocio negocio = new EspecialidadNegocio();

                dgvEspecialidades.DataSource = negocio.listar();
                dgvEspecialidades.DataBind();
            }
        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Especialidad nueva = new Especialidad();

                nueva.Nombre = txtNombre.Text.Trim();
                nueva.Descripcion = txtDescripcion.Text.Trim();
                nueva.Estado = true;

                EspecialidadNegocio negocio = new EspecialidadNegocio();
                negocio.agregar(nueva);

                dgvEspecialidades.DataSource = negocio.listar();
                dgvEspecialidades.DataBind();

                upListado.Update();

                txtNombre.Text = "";
                txtDescripcion.Text = "";
                lblMensaje.Text = "";
                lblMensaje.Visible = false;

                ScriptManager.RegisterStartupScript(
                    upEspecialidad,
                    upEspecialidad.GetType(),
                    "guardarExito",
                    "mostrarExito();",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                lblMensaje.Text = ex.Message;
                lblMensaje.Visible = true;
            }
        }
    }
}