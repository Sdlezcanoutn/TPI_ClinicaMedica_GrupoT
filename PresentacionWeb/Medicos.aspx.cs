using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dominio;
using Negocio;

namespace PresentacionWeb
{
    public partial class Medicos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // ============================================================
            // CARGA INICIAL DE LA PÁGINA
            // ============================================================

            if (!IsPostBack)
            {
                // Cargar el listado de médicos registrados.
                cargarMedicos();

                // Cargar las especialidades disponibles para asignar.
                cargarEspecialidades();
            }
        }

        // ============================================================
        // LISTADO DE MÉDICOS
        // ============================================================
        private void cargarMedicos()
        {
            // --- Crear instancia de la capa de negocio ---
            MedicoNegocio negocio = new MedicoNegocio();

            try
            {
                // --- Ocultar errores anteriores y habilitar la tabla ---
                lblErrorListado.Visible = false;
                dgvMedicos.Visible = true;

                // --- Obtener el listado de médicos desde la base de datos ---
                List<Medico> lista = negocio.listar();

                // --- Asignar los datos y actualizar la tabla ---
                dgvMedicos.DataSource = lista;
                dgvMedicos.DataBind();
            }
            catch (Exception)
            {
                // --- Mostrar un mensaje general sin exponer errores técnicos ---
                lblErrorListado.Text =
                    "No se pudo cargar el listado de médicos. Intente nuevamente.";

                lblErrorListado.Visible = true;

                // --- Ocultar la tabla si falla la consulta ---
                dgvMedicos.Visible = false;
            }
        }
        // ============================================================
        // CARGA DE ESPECIALIDADES
        // ============================================================
        private void cargarEspecialidades()
        {
            EspecialidadNegocio negocio = new EspecialidadNegocio();

            try
            {
                // Obtener las especialidades registradas en la base de datos.
                List<Especialidad> lista = negocio.listar();

                // Mostrar únicamente las especialidades activas.
                lista = lista.Where(e => e.Estado).ToList();

                // Configurar el listado de especialidades.
                cblEspecialidades.DataSource = lista;

                // Texto visible para el usuario.
                cblEspecialidades.DataTextField = "Nombre";

                // Identificador que utilizaremos al guardar el médico.
                cblEspecialidades.DataValueField = "Id";

                // Cargar los datos en el control.
                cblEspecialidades.DataBind();
            }
            catch (Exception)
            {
                // Evitar mostrar especialidades incompletas si ocurre un error.
                cblEspecialidades.Items.Clear();
            }
        }
        // ============================================================
        // ALTA DE MÉDICOS
        // ============================================================
        protected void btnGuardarMedico_Click(object sender, EventArgs e)
        {
            // --- Obtener los datos ingresados en el formulario ---
            string nombre = txtNombreMedico.Text.Trim();
            string apellido = txtApellidoMedico.Text.Trim();
            string dni = txtDniMedico.Text.Trim();
            string matricula = txtMatriculaMedico.Text.Trim();
            string telefono = txtTelefonoMedico.Text.Trim();
            string email = txtEmailMedico.Text.Trim();

            // ========================================================
            // 1. VALIDAR LOS CAMPOS OBLIGATORIOS
            // ========================================================

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(dni) ||
                string.IsNullOrWhiteSpace(matricula) ||
                string.IsNullOrWhiteSpace(email))
            {
                mostrarErrorMedico("Completá todos los campos obligatorios.");
                return;
            }

            // --- Validar que el DNI contenga únicamente números ---
            if (dni.Length < 7 || dni.Length > 8 || !dni.All(char.IsDigit))
            {
                mostrarErrorMedico("Ingresá un DNI válido de 7 u 8 dígitos.");
                return;
            }

            // --- Validar el formato del correo electrónico ---
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                mostrarErrorMedico("Ingresá un correo electrónico válido.");
                return;
            }

            // ========================================================
            // 2. OBTENER LAS ESPECIALIDADES SELECCIONADAS
            // ========================================================

            List<Especialidad> especialidades = new List<Especialidad>();

            foreach (ListItem item in cblEspecialidades.Items)
            {
                if (item.Selected)
                {
                    Especialidad especialidad = new Especialidad();

                    especialidad.Id = int.Parse(item.Value);
                    especialidad.Nombre = item.Text;

                    especialidades.Add(especialidad);
                }
            }

            // --- Exigir al menos una especialidad ---
            if (especialidades.Count == 0)
            {
                mostrarErrorMedico("Seleccioná al menos una especialidad.");
                return;
            }

            // ========================================================
            // 3. PREPARAR EL OBJETO MÉDICO
            // ========================================================

            Medico medico = new Medico();

            medico.Nombre = nombre;
            medico.Apellido = apellido;
            medico.DNI = dni;
            medico.Matricula = matricula;
            medico.Telefono = telefono;
            medico.Email = email;
            medico.Estado = true;
            medico.Especialidades = especialidades;

            // ========================================================
            // 4. REGISTRAR EL MÉDICO EN LA BASE DE DATOS
            // ========================================================

            try
            {
                // --- Crear instancia de la capa de negocio ---
                MedicoNegocio negocio = new MedicoNegocio();

                // --- Registrar el médico junto con sus especialidades ---
                negocio.agregar(medico);

                // --- Actualizar el listado de médicos ---
                cargarMedicos();
                upListado.Update();

                // --- Limpiar el formulario después de guardar ---
                txtNombreMedico.Text = "";
                txtApellidoMedico.Text = "";
                txtDniMedico.Text = "";
                txtMatriculaMedico.Text = "";
                txtTelefonoMedico.Text = "";
                txtEmailMedico.Text = "";
                cblEspecialidades.ClearSelection();
                lblErrorMedico.Visible = false;

                // --- Cerrar el modal y mostrar confirmación ---
                ScriptManager.RegisterStartupScript(
                    upMedico,
                    upMedico.GetType(),
                    "guardarExito",
                    "mostrarExitoMedico();",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                // --- Mostrar errores de validación específicos ---
                mostrarErrorMedico(ex.Message);
            }
            catch (Exception ex)
            {
                // --- Mostrar el error real para identificar el problema ---
                mostrarErrorMedico(ex.Message);
            }
        }

        // ============================================================
        // MENSAJES DE VALIDACIÓN DEL FORMULARIO
        // ============================================================
        private void mostrarErrorMedico(string mensaje)
        {
            // --- Mostrar el error dentro del formulario ---
            lblErrorMedico.Text = mensaje;
            lblErrorMedico.Visible = true;
        }

    }
}
