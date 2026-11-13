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
    public partial class FormHorario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cargarMedicos();
            }
        }
        private void cargarMedicos()
        {
            MedicoNegocio negocio = new MedicoNegocio();

            var medicos = negocio.listar()
                .Select(m => new
                {
                    Id = m.Id,
                    NombreCompleto = m.Nombre + " " + m.Apellido
                })
                .ToList();

            ddlMedico.DataSource = medicos;
            ddlMedico.DataTextField = "NombreCompleto";
            ddlMedico.DataValueField = "Id";
            ddlMedico.DataBind();

            ddlMedico.Items.Insert(0,
                new ListItem("Seleccione un médico", ""));
        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ddlMedico.SelectedValue == "")
            {
                lblMensaje.Text = "Seleccione un médico.";
                return;
            }

            DateTime entrada;
            DateTime salida;

            bool entradaValida = DateTime.TryParseExact(
                txtHoraEntrada.Text,
                "HH:mm",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out entrada);

            bool salidaValida = DateTime.TryParseExact(
                txtHoraSalida.Text,
                "HH:mm",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out salida);

            if (!entradaValida || !salidaValida)
            {
                lblMensaje.Text = "Ingrese horarios válidos.";
                return;
            }

            if (salida <= entrada)
            {
                lblMensaje.Text =
                    "La hora de salida debe ser posterior a la entrada.";
                return;
            }

            lblMensaje.Text = "";

            // Siguiente paso: crear el objeto HorarioTrabajo
            // y guardarlo mediante HorarioTrabajoNegocio.
        }
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("Medicos.aspx", false);
        }
    }
}