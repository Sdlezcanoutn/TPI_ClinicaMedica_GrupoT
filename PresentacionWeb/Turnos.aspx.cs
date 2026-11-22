using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dominio;
using Negocio;

namespace PresentacionWeb
{
    public partial class Turnos : System.Web.UI.Page
    {
        private class TurnoFila
        {
            public int Id { get; set; }
            public string Fecha { get; set; }
            public string Hora { get; set; }
            public string Paciente { get; set; }
            public string Medico { get; set; }
            public string Especialidad { get; set; }
            public string Estado { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarCombos();
                CargarTurnos();
            }
        }

        private void CargarCombos()
        {
            PacienteNegocio pacienteNegocio = new PacienteNegocio();
            List<Paciente> pacientesActivos = pacienteNegocio.listar()
                .Where(p => p.Estado)
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToList();

            ddlPaciente.DataSource = pacientesActivos.Select(p => new
            {
                p.Id,
                Texto = $"{p.Apellido}, {p.Nombre} (DNI {p.DNI})"
            }).ToList();
            ddlPaciente.DataValueField = "Id";
            ddlPaciente.DataTextField = "Texto";
            ddlPaciente.DataBind();
            ddlPaciente.Items.Insert(0, new ListItem("-- Seleccione un paciente --", "0"));

            EspecialidadNegocio especialidadNegocio = new EspecialidadNegocio();
            List<Especialidad> especialidadesActivas = especialidadNegocio.listar()
                .Where(esp => esp.Estado)
                .OrderBy(esp => esp.Nombre)
                .ToList();

            ddlEspecialidad.DataSource = especialidadesActivas;
            ddlEspecialidad.DataValueField = "Id";
            ddlEspecialidad.DataTextField = "Nombre";
            ddlEspecialidad.DataBind();
            ddlEspecialidad.Items.Insert(0, new ListItem("-- Seleccione una especialidad --", "0"));

            CargarMedicosPorEspecialidad(0);
        }

        private void CargarMedicosPorEspecialidad(int idEspecialidad)
        {
            MedicoNegocio medicoNegocio = new MedicoNegocio();
            List<Medico> medicosActivos = medicoNegocio.listar()
                .Where(m => m.Estado)
                .ToList();

            if (idEspecialidad > 0)
            {
                medicosActivos = medicosActivos
                    .Where(m => m.Especialidades != null &&
                                m.Especialidades.Any(esp => esp.Id == idEspecialidad))
                    .ToList();
            }

            ddlMedico.DataSource = medicosActivos
                .OrderBy(m => m.Apellido)
                .ThenBy(m => m.Nombre)
                .Select(m => new
                {
                    m.Id,
                    Texto = $"{m.Apellido}, {m.Nombre} (Mat. {m.Matricula})"
                }).ToList();
            ddlMedico.DataValueField = "Id";
            ddlMedico.DataTextField = "Texto";
            ddlMedico.DataBind();
            ddlMedico.Items.Insert(0, new ListItem("-- Seleccione un médico --", "0"));
        }

        protected void ddlEspecialidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idEspecialidad = int.TryParse(ddlEspecialidad.SelectedValue, out int id) ? id : 0;
            CargarMedicosPorEspecialidad(idEspecialidad);
            lblErrorTurno.Visible = false;
            upTurno.Update();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "reabrirTurnoEspecialidad",
                "mostrarModalTurno();",
                true
            );
        }

        private void CargarTurnos(string filtro = "")
        {
            TurnoNegocio negocio = new TurnoNegocio();

            try
            {
                lblErrorListadoTurno.Visible = false;
                dgvTurnos.Visible = true;

                List<Turno> lista = negocio.listar();

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    string texto = filtro.Trim().ToLower();
                    lista = lista.FindAll(t =>
                        (t.Paciente != null &&
                         ((!string.IsNullOrEmpty(t.Paciente.Nombre) && t.Paciente.Nombre.ToLower().Contains(texto)) ||
                          (!string.IsNullOrEmpty(t.Paciente.Apellido) && t.Paciente.Apellido.ToLower().Contains(texto)) ||
                          (!string.IsNullOrEmpty(t.Paciente.DNI) && t.Paciente.DNI.ToLower().Contains(texto)))) ||
                        (t.Medico != null &&
                         ((!string.IsNullOrEmpty(t.Medico.Nombre) && t.Medico.Nombre.ToLower().Contains(texto)) ||
                          (!string.IsNullOrEmpty(t.Medico.Apellido) && t.Medico.Apellido.ToLower().Contains(texto)))) ||
                        (t.Especialidad != null &&
                         !string.IsNullOrEmpty(t.Especialidad.Nombre) && t.Especialidad.Nombre.ToLower().Contains(texto))
                    );
                }

                List<TurnoFila> filas = lista.Select(t => new TurnoFila
                {
                    Id = t.Id,
                    Fecha = t.Fecha.ToString("dd/MM/yyyy"),
                    Hora = $"{t.Hora_Inicio:HH:mm} - {t.Hora_Fin:HH:mm}",
                    Paciente = t.Paciente != null ? $"{t.Paciente.Apellido}, {t.Paciente.Nombre}" : "",
                    Medico = t.Medico != null ? $"{t.Medico.Apellido}, {t.Medico.Nombre}" : "",
                    Especialidad = t.Especialidad != null ? t.Especialidad.Nombre : "",
                    Estado = t.Estado != null ? t.Estado.Nombre : "Nuevo"
                }).ToList();

                dgvTurnos.DataSource = filas;
                dgvTurnos.DataBind();
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListadoTurno.Text = "No se pudo cargar el listado de turnos. Intentá nuevamente.";
                lblErrorListadoTurno.Visible = true;
                dgvTurnos.Visible = false;
            }
        }

        protected void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarTurnos(txtBuscar.Text);
            upListadoTurnos.Update();
        }

        protected void btnGuardarTurno_Click(object sender, EventArgs e)
        {
            int idPaciente = int.TryParse(ddlPaciente.SelectedValue, out int pId) ? pId : 0;
            int idEspecialidad = int.TryParse(ddlEspecialidad.SelectedValue, out int eId) ? eId : 0;
            int idMedico = int.TryParse(ddlMedico.SelectedValue, out int mId) ? mId : 0;
            string motivo = txtMotivoConsulta.Text.Trim();

            if (idPaciente <= 0 || idEspecialidad <= 0 || idMedico <= 0)
            {
                MostrarErrorTurno("Seleccioná paciente, especialidad y médico.");
                return;
            }

            if (!DateTime.TryParseExact(
                    txtFechaTurno.Text.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fechaSeleccionada))
            {
                MostrarErrorTurno("Ingresá una fecha válida.");
                return;
            }

            if (!TimeSpan.TryParse(txtHoraInicio.Text.Trim(), out TimeSpan tsInicio) ||
                !TimeSpan.TryParse(txtHoraFin.Text.Trim(), out TimeSpan tsFin))
            {
                MostrarErrorTurno("Ingresá hora de inicio y hora de fin válidas.");
                return;
            }

            try
            {
                Turno nuevo = new Turno
                {
                    Paciente = new Paciente { Id = idPaciente },
                    Especialidad = new Especialidad { Id = idEspecialidad },
                    Medico = new Medico { Id = idMedico },
                    Fecha = fechaSeleccionada.Date,
                    Hora_Inicio = fechaSeleccionada.Date.Add(tsInicio),
                    Hora_Fin = fechaSeleccionada.Date.Add(tsFin),
                    Motivo_Consulta = motivo
                };

                TurnoNegocio negocio = new TurnoNegocio();
                negocio.agregar(nuevo);

                CargarTurnos(txtBuscar.Text);
                upListadoTurnos.Update();

                lblErrorTurno.Visible = false;
                upTurno.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "guardarTurnoOk",
                    "mostrarExitoTurno('Turno registrado correctamente.');",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                MostrarErrorTurno(ex.Message);
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                MostrarErrorTurno("Ocurrió un error al guardar el turno. Intentá nuevamente.");
            }
        }

        private void MostrarErrorTurno(string mensaje)
        {
            lblErrorTurno.Text = mensaje;
            lblErrorTurno.Visible = true;
            upTurno.Update();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "errorTurnoModal",
                "mostrarModalTurno();",
                true
            );
        }
        protected string ObtenerClaseBadgeEstado(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
                return "badge bg-success";

            switch (estado.Trim().ToLower())
            {
                case "nuevo":
                    return "badge bg-success";

                case "reprogramado":
                    return "badge bg-warning text-dark";

                case "cancelado":
                case "no asistió":
                case "no asistio":
                case "cerrado":
                    return "badge bg-danger";

                default:
                    return "badge bg-secondary";
            }
        }
    }
}