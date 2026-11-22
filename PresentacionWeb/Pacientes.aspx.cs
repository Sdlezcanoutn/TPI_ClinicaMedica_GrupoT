using Dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        private void CargarPacientes(string filtro = "")
        {
            PacienteNegocio negocio = new PacienteNegocio();

            try
            {
                lblErrorListado.Visible = false;
                dgvPacientes.Visible = true;

                List<Paciente> lista = negocio.listar();

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    string texto = filtro.Trim().ToLower();
                    lista = lista.FindAll(x =>
                        (!string.IsNullOrEmpty(x.DNI) && x.DNI.ToLower().Contains(texto)) ||
                        (!string.IsNullOrEmpty(x.Apellido) && x.Apellido.ToLower().Contains(texto))
                    );
                }

                dgvPacientes.DataSource = lista;
                dgvPacientes.DataBind();
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudo cargar el listado de pacientes. Intentá nuevamente.";
                lblErrorListado.Visible = true;
                dgvPacientes.Visible = false;
            }
        }

        protected void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarPacientes(txtBuscar.Text);
            upListado.Update();
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (!int.TryParse(btn.CommandArgument, out int idPaciente))
                return;

            try
            {
                PacienteNegocio negocio = new PacienteNegocio();
                Paciente paciente = negocio.listar().Find(p => p.Id == idPaciente);

                if (paciente == null || !paciente.Estado)
                {
                    lblErrorListado.Text = "El paciente no existe o está inactivo.";
                    lblErrorListado.Visible = true;
                    upListado.Update();
                    return;
                }

                hfIdPaciente.Value = paciente.Id.ToString();
                txtNombrePaciente.Text = paciente.Nombre;
                txtApellidoPaciente.Text = paciente.Apellido;
                txtDniPaciente.Text = paciente.DNI;
                txtFechaNacimientoPaciente.Text = paciente.Fecha_Nacimiento.ToString("yyyy-MM-dd");
                txtTelefonoPaciente.Text = paciente.Telefono;
                txtEmailPaciente.Text = paciente.Email;
                txtDireccionPaciente.Text = paciente.Direccion;

                lblErrorPaciente.Visible = false;
                btnGuardarPaciente.Text = "Guardar Cambios";
                upPaciente.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "abrirEditarPaciente",
                    "prepararEdicionPaciente();",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudieron cargar los datos del paciente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }

        protected void btnGuardarPaciente_Click(object sender, EventArgs e)
        {
            string nombre = txtNombrePaciente.Text.Trim();
            string apellido = txtApellidoPaciente.Text.Trim();
            string dni = txtDniPaciente.Text.Trim();
            string fechaTexto = txtFechaNacimientoPaciente.Text.Trim();
            string telefono = txtTelefonoPaciente.Text.Trim();
            string email = txtEmailPaciente.Text.Trim();
            string direccion = txtDireccionPaciente.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(dni) ||
                string.IsNullOrWhiteSpace(fechaTexto) ||
                string.IsNullOrWhiteSpace(email))
            {
                MostrarErrorPaciente("Completá todos los campos obligatorios.");
                return;
            }

            if (dni.Length < 7 || dni.Length > 8 || !dni.All(char.IsDigit))
            {
                MostrarErrorPaciente("Ingresá un DNI válido de 7 u 8 dígitos numéricos.");
                return;
            }

            if (!DateTime.TryParse(fechaTexto, out DateTime fechaNacimiento) ||
                fechaNacimiento.Date > DateTime.Today ||
                fechaNacimiento.Year < 1900)
            {
                MostrarErrorPaciente("Ingresá una fecha de nacimiento válida.");
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MostrarErrorPaciente("Ingresá un correo electrónico válido.");
                return;
            }

            try
            {
                PacienteNegocio negocio = new PacienteNegocio();
                bool esEdicion = int.TryParse(hfIdPaciente.Value, out int idActual) && idActual > 0;

                // Valida DNI duplicado en alta o en edición (excluyendo el registro actual)
                if (negocio.existeDni(dni, esEdicion ? (int?)idActual : null))
                {
                    MostrarErrorPaciente("Ya existe un paciente registrado con ese DNI.");
                    return;
                }

                Paciente paciente = new Paciente
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    DNI = dni,
                    Fecha_Nacimiento = fechaNacimiento,
                    Telefono = telefono,
                    Email = email,
                    Direccion = direccion,
                    Estado = true
                };

                if (esEdicion)
                {
                    paciente.Id = idActual;
                    negocio.modificar(paciente);
                }
                else
                {
                    negocio.agregar(paciente);
                }

                CargarPacientes(txtBuscar.Text);
                upListado.Update();

                hfIdPaciente.Value = "";
                lblErrorPaciente.Visible = false;
                upPaciente.Update();

                string mensajeExito = esEdicion
                    ? "Paciente modificado correctamente."
                    : "Paciente registrado correctamente.";

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "guardarPacienteOk",
                    $"mostrarExitoPaciente('{mensajeExito}');",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                MostrarErrorPaciente("Ocurrió un error al guardar el paciente. Intentá nuevamente.");
            }
        }

        private void MostrarErrorPaciente(string mensaje)
        {
            lblErrorPaciente.Text = mensaje;
            lblErrorPaciente.Visible = true;
            upPaciente.Update();
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (!int.TryParse(btn.CommandArgument, out int idPaciente))
                return;

            try
            {
                PacienteNegocio negocio = new PacienteNegocio();
                Paciente paciente = negocio.listar().Find(p => p.Id == idPaciente);

                if (paciente == null || !paciente.Estado)
                {
                    lblErrorListado.Text = "El paciente no existe o ya está inactivo.";
                    lblErrorListado.Visible = true;
                    upListado.Update();
                    return;
                }

                hfIdBajaPaciente.Value = paciente.Id.ToString();
                lblNombreBajaPaciente.Text = $"{paciente.Nombre} {paciente.Apellido} (DNI {paciente.DNI})";
                upBajaPaciente.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "abrirModalBajaPaciente",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalBajaPaciente')).show();",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudieron consultar los datos del paciente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }

        protected void btnConfirmarBajaPaciente_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hfIdBajaPaciente.Value, out int idPaciente))
                return;

            try
            {
                PacienteNegocio negocio = new PacienteNegocio();
                negocio.eliminarLogico(idPaciente);

                hfIdBajaPaciente.Value = "";
                CargarPacientes(txtBuscar.Text);
                upListado.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "bajaPacienteOk",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalBajaPaciente')).hide();" +
                    "mostrarAlertaPaciente('Paciente desactivado correctamente.');",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudo desactivar el paciente. Intentá nuevamente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }
    }
}