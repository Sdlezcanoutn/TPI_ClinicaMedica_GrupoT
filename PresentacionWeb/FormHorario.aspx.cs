using Dominio;
using Negocio;
using PresentacionWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PresentacionWeb
{
    public partial class FormHorario : PaginaProtegida
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cargarMedicos();

                // Validación del parámetro por URL para editar un horario
                string parametroId = Request.QueryString["id"];

                // Si no existe el parámetro, es un horario nuevo.
                if (parametroId == null)
                    return;

                // Si existe, debe ser un entero positivo.
                int id;

                if (!int.TryParse(parametroId, out id) || id <= 0)
                {
                    lblMensaje.Text = "El identificador del horario no es válido.";
                    btnGuardar.Enabled = false;
                    return;
                }

                // Si llegamos acá, el ID tiene un formato válido.
                cargarHorario(id);
                Page.Title = "Editar Horario";
            }
        }

        protected override bool TienePermiso(Usuario usuario)
        {
            return usuario.Perfil != null &&
                (usuario.Perfil.Nombre == "Administrador" ||
                 usuario.Perfil.Nombre == "Recepcionista");
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

            // Validar si el horario existe en la base de datos
            if (horario == null)
            {
                lblMensaje.Text = "No se encontró el horario.";
                btnGuardar.Enabled = false;
                return;
            }

            // Validar si el horario está activo
            if (!horario.Estado)
            {
                lblMensaje.Text = "No se puede editar un horario inactivo.";
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
            // Validaciones de selección
            if (ddlMedico.SelectedValue == "")
            {
                lblMensaje.Text = "Seleccione un médico.";
                return;
            }

            byte dia;

            if (!byte.TryParse(ddlDiaSemana.SelectedValue, out dia) || dia < 1 || dia > 7)
            {
                lblMensaje.Text = "Seleccione un día de la semana válido.";
                return;
            }

            // Validaciones horarias
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

                // Validación del parámetro por URL para editar un horario
                HorarioTrabajoNegocio negocio = new HorarioTrabajoNegocio();
                string parametroId = Request.QueryString["id"];

                if (parametroId == null)
                {
                    // No hay ID: estamos agregando.
                    negocio.agregar(horario);
                }
                else
                {
                    // Hay ID: debe ser válido para modificar.
                    int id;

                    if (!int.TryParse(parametroId, out id) || id <= 0)
                    {
                        lblMensaje.Text = "El identificador del horario no es válido.";
                        return;
                    }

                    horario.Id = id;
                    negocio.modificar(horario);
                }

                Response.Redirect("~/Horarios.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (InvalidOperationException ex)
            {
                lblMensaje.Text = ex.Message;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                lblMensaje.Text = "Ocurrió un error al guardar el horario. Intente nuevamente.";
            }
        }
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Horarios.aspx", false);
        }
    }
}