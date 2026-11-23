using Dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PresentacionWeb
{
    public partial class Empleados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarEmpleados();
            }
        }

        private void CargarEmpleados(string filtro = "")
        {
            EmpleadoNegocio negocio = new EmpleadoNegocio();

            try
            {
                lblErrorListado.Visible = false;
                dgvEmpleados.Visible = true;

                List<Empleado> lista = negocio.listar();

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    string texto = filtro.Trim().ToLower();
                    lista = lista.FindAll(x =>
                        (!string.IsNullOrEmpty(x.DNI) && x.DNI.ToLower().Contains(texto)) ||
                        (!string.IsNullOrEmpty(x.Apellido) && x.Apellido.ToLower().Contains(texto))
                    );
                }

                dgvEmpleados.DataSource = lista;
                dgvEmpleados.DataBind();
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudo cargar el listado de empleados. Intentá nuevamente.";
                lblErrorListado.Visible = true;
                dgvEmpleados.Visible = false;
            }
        }

        protected void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarEmpleados(txtBuscar.Text);
            upListado.Update();
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (!int.TryParse(btn.CommandArgument, out int idEmpleado))
                return;

            try
            {
                EmpleadoNegocio negocio = new EmpleadoNegocio();
                Empleado empleado = negocio.listar().Find(emp => emp.Id == idEmpleado);

                if (empleado == null || !empleado.Estado)
                {
                    lblErrorListado.Text = "El empleado no existe o está inactivo.";
                    lblErrorListado.Visible = true;
                    upListado.Update();
                    return;
                }

                hfIdEmpleado.Value = empleado.Id.ToString();
                txtNombreEmpleado.Text = empleado.Nombre;
                txtApellidoEmpleado.Text = empleado.Apellido;
                txtDniEmpleado.Text = empleado.DNI;

                lblErrorEmpleado.Visible = false;
                btnGuardarEmpleado.Text = "Guardar Cambios";
                upEmpleado.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "abrirEditarEmpleado",
                    "prepararEdicionEmpleado();",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudieron cargar los datos del empleado.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }

        protected void btnGuardarEmpleado_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreEmpleado.Text.Trim();
            string apellido = txtApellidoEmpleado.Text.Trim();
            string dni = txtDniEmpleado.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(dni))
            {
                MostrarErrorEmpleado("Completá todos los campos obligatorios.");
                return;
            }

            if (dni.Length < 7 || dni.Length > 8 || !dni.All(char.IsDigit))
            {
                MostrarErrorEmpleado("Ingresá un DNI válido de 7 u 8 dígitos numéricos.");
                return;
            }

            try
            {
                EmpleadoNegocio negocio = new EmpleadoNegocio();
                bool esEdicion = int.TryParse(hfIdEmpleado.Value, out int idActual) && idActual > 0;

                if (negocio.existeDni(dni, esEdicion ? (int?)idActual : null))
                {
                    MostrarErrorEmpleado("Ya existe un empleado registrado con ese DNI.");
                    return;
                }

                Empleado empleado = new Empleado
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    DNI = dni,
                    Estado = true
                };

                if (esEdicion)
                {
                    empleado.Id = idActual;
                    negocio.modificar(empleado);
                }
                else
                {
                    negocio.agregar(empleado);
                }

                CargarEmpleados(txtBuscar.Text);
                upListado.Update();

                hfIdEmpleado.Value = "";
                lblErrorEmpleado.Visible = false;
                upEmpleado.Update();

                string mensajeExito = esEdicion
                    ? "Empleado modificado correctamente."
                    : "Empleado registrado correctamente.";

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "guardarEmpleadoOk",
                    $"mostrarExitoEmpleado('{mensajeExito}');",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                MostrarErrorEmpleado("Ocurrió un error al guardar el empleado. Intentá nuevamente.");
            }
        }

        private void MostrarErrorEmpleado(string mensaje)
        {
            lblErrorEmpleado.Text = mensaje;
            lblErrorEmpleado.Visible = true;
            upEmpleado.Update();
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (!int.TryParse(btn.CommandArgument, out int idEmpleado))
                return;

            try
            {
                EmpleadoNegocio negocio = new EmpleadoNegocio();
                Empleado empleado = negocio.listar().Find(emp => emp.Id == idEmpleado);

                if (empleado == null || !empleado.Estado)
                {
                    lblErrorListado.Text = "El empleado no existe o ya está inactivo.";
                    lblErrorListado.Visible = true;
                    upListado.Update();
                    return;
                }

                hfIdBajaEmpleado.Value = empleado.Id.ToString();
                lblNombreBajaEmpleado.Text = $"{empleado.Nombre} {empleado.Apellido} (DNI {empleado.DNI})";
                upBajaEmpleado.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "abrirModalBajaEmpleado",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalBajaEmpleado')).show();",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudieron consultar los datos del empleado.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }

        protected void btnConfirmarBajaEmpleado_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hfIdBajaEmpleado.Value, out int idEmpleado))
                return;

            try
            {
                EmpleadoNegocio negocio = new EmpleadoNegocio();
                negocio.eliminarLogico(idEmpleado);

                hfIdBajaEmpleado.Value = "";
                CargarEmpleados(txtBuscar.Text);
                upListado.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "bajaEmpleadoOk",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalBajaEmpleado')).hide();" +
                    "mostrarAlertaEmpleado('Empleado desactivado correctamente.');",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudo desactivar el empleado. Intentá nuevamente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }
    }
}