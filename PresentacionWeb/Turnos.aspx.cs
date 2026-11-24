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
                CargarHorariosDisponiblesModal();
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

            ddlFiltroEspecialidad.DataSource = especialidadesActivas;
            ddlFiltroEspecialidad.DataValueField = "Id";
            ddlFiltroEspecialidad.DataTextField = "Nombre";
            ddlFiltroEspecialidad.DataBind();
            ddlFiltroEspecialidad.Items.Insert(0, new ListItem("Todas las especialidades", "0"));

            MedicoNegocio medicoNegocio = new MedicoNegocio();
            List<Medico> todosMedicosActivos = medicoNegocio.listar()
                .Where(m => m.Estado)
                .OrderBy(m => m.Apellido)
                .ThenBy(m => m.Nombre)
                .ToList();

            ddlFiltroMedico.DataSource = todosMedicosActivos.Select(m => new
            {
                m.Id,
                Texto = $"{m.Apellido}, {m.Nombre}"
            }).ToList();
            ddlFiltroMedico.DataValueField = "Id";
            ddlFiltroMedico.DataTextField = "Texto";
            ddlFiltroMedico.DataBind();
            ddlFiltroMedico.Items.Insert(0, new ListItem("Todos los médicos", "0"));

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
            txtHoraInicio.Text = string.Empty;
            txtHoraFin.Text = string.Empty;
            CargarHorariosDisponiblesModal();
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

        private void CargarHorariosDisponiblesModal()
        {
            ddlHorariosDisponibles.Items.Clear();
            lblInfoDisponibilidad.Text = string.Empty;

            int idMedico = int.TryParse(ddlMedico.SelectedValue, out int mId) ? mId : 0;
            bool fechaValida = DateTime.TryParseExact(
                txtFechaTurno.Text.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime fechaSeleccionada);

            if (idMedico <= 0 || !fechaValida)
            {
                ddlHorariosDisponibles.Items.Insert(0, new ListItem("-- Seleccione médico y fecha --", ""));
                lblInfoDisponibilidad.Text = "Elegí un médico y una fecha para ver los horarios libres cada 15 minutos.";
                return;
            }

            TurnoNegocio turnoNegocio = new TurnoNegocio();
            List<TurnoNegocio.SlotHorario> slots = turnoNegocio.obtenerHorariosDisponibles(idMedico, fechaSeleccionada.Date, 15);

            if (slots.Count == 0)
            {
                ddlHorariosDisponibles.Items.Insert(0, new ListItem("-- Sin horarios disponibles --", ""));
                lblInfoDisponibilidad.Text = "No hay franjas horarias libres para el médico en esa fecha.";
                txtHoraInicio.Text = string.Empty;
                txtHoraFin.Text = string.Empty;
                return;
            }

            ddlHorariosDisponibles.Items.Insert(0, new ListItem("-- Seleccione un horario disponible --", ""));
            foreach (TurnoNegocio.SlotHorario slot in slots)
            {
                string valor = $"{slot.HoraInicio:hh\\:mm}|{slot.HoraFin:hh\\:mm}";
                ddlHorariosDisponibles.Items.Add(new ListItem(slot.Etiqueta, valor));
            }

            lblInfoDisponibilidad.Text = $"{slots.Count} bloque(s) de 15 min disponible(s).";
        }

        protected void DisponibilidadTurno_Changed(object sender, EventArgs e)
        {
            txtHoraInicio.Text = string.Empty;
            txtHoraFin.Text = string.Empty;
            CargarHorariosDisponiblesModal();
            lblErrorTurno.Visible = false;
            upTurno.Update();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "reabrirTurnoDisponibilidad",
                "mostrarModalTurno();",
                true
            );
        }

        protected void ddlHorariosDisponibles_SelectedIndexChanged(object sender, EventArgs e)
        {
            string seleccionado = ddlHorariosDisponibles.SelectedValue;
            if (!string.IsNullOrWhiteSpace(seleccionado) && seleccionado.Contains("|"))
            {
                string[] partes = seleccionado.Split('|');
                txtHoraInicio.Text = partes[0];
                txtHoraFin.Text = partes[1];
            }

            lblErrorTurno.Visible = false;
            upTurno.Update();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "reabrirTurnoSlot",
                "mostrarModalTurno();",
                true
            );
        }

        private void CargarTurnos(string filtro = "", int idEspecialidad = 0, int idMedico = 0, DateTime? fechaFiltro = null)
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

                if (idEspecialidad > 0)
                {
                    lista = lista.FindAll(t => t.Especialidad != null && t.Especialidad.Id == idEspecialidad);
                }

                if (idMedico > 0)
                {
                    lista = lista.FindAll(t => t.Medico != null && t.Medico.Id == idMedico);
                }

                if (fechaFiltro.HasValue)
                {
                    DateTime fechaBuscada = fechaFiltro.Value.Date;
                    lista = lista.FindAll(t => t.Fecha.Date == fechaBuscada);
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

        private void AplicarFiltrosListado()
        {
            int idEspecialidad = int.TryParse(ddlFiltroEspecialidad.SelectedValue, out int espId) ? espId : 0;
            int idMedico = int.TryParse(ddlFiltroMedico.SelectedValue, out int medId) ? medId : 0;

            DateTime? fechaFiltro = null;
            if (!string.IsNullOrWhiteSpace(txtFiltroFecha.Text) &&
                DateTime.TryParseExact(
                    txtFiltroFecha.Text.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fechaParseada))
            {
                fechaFiltro = fechaParseada.Date;
            }

            CargarTurnos(txtBuscar.Text, idEspecialidad, idMedico, fechaFiltro);
        }

        protected void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltrosListado();
            upListadoTurnos.Update();
        }

        protected void FiltroListado_Changed(object sender, EventArgs e)
        {
            AplicarFiltrosListado();
            upListadoTurnos.Update();
        }

        protected void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = string.Empty;
            ddlFiltroEspecialidad.SelectedIndex = 0;
            ddlFiltroMedico.SelectedIndex = 0;
            txtFiltroFecha.Text = string.Empty;

            CargarTurnos();
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

                AplicarFiltrosListado();
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