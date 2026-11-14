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

                int id;
                if (int.TryParse(Request.QueryString["id"], out id))
                {
                    cargarHorario(id);
                }
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
        private void cargarHorario(int id)
        {
            HorarioTrabajoNegocio negocio = new HorarioTrabajoNegocio();

            HorarioTrabajo horario = negocio.listar()
                .FirstOrDefault(h => h.Id == id);

            if (horario == null)
            {
                lblMensaje.Text = "No se encontró el horario.";
                btnGuardar.Enabled = false;
                return;
            }

            ddlMedico.SelectedValue = horario.Medico.Id.ToString();
            ddlDiaSemana.SelectedValue = horario.Dia_Semana;

            txtHoraEntrada.Text = horario.Hora_Entrada.ToString("HH:mm");
            txtHoraSalida.Text = horario.Hora_Salida.ToString("HH:mm");
        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            //Validaciones
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

            //Lógica para guardar el registro
            try
            {
                HorarioTrabajo horario = new HorarioTrabajo();

                horario.Medico = new Medico();
                horario.Medico.Id = int.Parse(ddlMedico.SelectedValue);

                horario.Dia_Semana = ddlDiaSemana.SelectedValue;
                horario.Hora_Entrada = entrada;
                horario.Hora_Salida = salida;

                HorarioTrabajoNegocio negocio = new HorarioTrabajoNegocio();
                int id;

                if (int.TryParse(Request.QueryString["id"], out id))
                {
                    horario.Id = id;
                    negocio.modificar(horario);
                }
                else
                {
                    negocio.agregar(horario);
                }

                Response.Redirect("~/Horarios.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al guardar el horario: " + ex.Message;
            }
        }
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Horarios.aspx", false);
        }
    }
}