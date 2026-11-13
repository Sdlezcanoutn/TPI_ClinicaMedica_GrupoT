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
    public partial class FormPaciente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Validamos si nos pasan un ID por la URL
                if (Request.QueryString["id"] != null)
                {
                    int id = int.Parse(Request.QueryString["id"]);
                    PacienteNegocio negocio = new PacienteNegocio();

                    // Buscamos al paciente 
                    // Como alternativa rápida, filtramos de listar() o traemos el objeto:
                    List<Paciente> lista = negocio.listar();
                    Paciente seleccionado = lista.Find(x => x.Id == id);

                    if (seleccionado != null)
                    {
                        txtNombre.Text = seleccionado.Nombre;
                        txtApellido.Text = seleccionado.Apellido;
                        txtDni.Text = seleccionado.DNI;
                        txtFechaNacimiento.Text = seleccionado.Fecha_Nacimiento.ToString("yyyy-MM-dd");
                        txtTelefono.Text = seleccionado.Telefono;
                        txtEmail.Text = seleccionado.Email;
                        txtDireccion.Text = seleccionado.Direccion;
                    }
                }
            }
        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Limpiamos el mensaje de error al empezar a validar
                lblMensaje.Text = "";

                // Validaciones básicas de campos vacíos
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtDni.Text) ||
                    string.IsNullOrWhiteSpace(txtFechaNacimiento.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    lblMensaje.Text = "Por favor, complete todos los campos obligatorios (Nombre, Apellido, DNI, Fecha de Nacimiento y Email).";
                    return;
                }

                // Validación de formato de DNI (solo números)
                if (!long.TryParse(txtDni.Text.Trim(), out _))
                {
                    lblMensaje.Text = "El DNI ingresado no es válido. Debe contener únicamente números.";
                    return;
                }

                PacienteNegocio negocio = new PacienteNegocio();

                // Control de registros duplicados por DNI
                int? idActual = Request.QueryString["id"] != null ? int.Parse(Request.QueryString["id"]) : (int?)null;

                if (negocio.existeDni(txtDni.Text.Trim(), idActual))
                {
                    lblMensaje.Text = "Ya existe un paciente registrado con el número de DNI ingresado.";
                    return;
                }

                // Carga de datos y guardado
                Paciente paciente = new Paciente();
                paciente.Nombre = txtNombre.Text.Trim();
                paciente.Apellido = txtApellido.Text.Trim();
                paciente.DNI = txtDni.Text.Trim();

                if (DateTime.TryParse(txtFechaNacimiento.Text, out DateTime fecha))
                {
                    paciente.Fecha_Nacimiento = fecha;
                }

                paciente.Telefono = txtTelefono.Text.Trim();
                paciente.Email = txtEmail.Text.Trim();
                paciente.Direccion = txtDireccion.Text.Trim();

                if (idActual.HasValue)
                {
                    paciente.Id = idActual.Value;
                    negocio.modificar(paciente);
                }
                else
                {
                    negocio.agregar(paciente);
                }

                Response.Redirect("Pacientes.aspx", false);
            }
            catch (Exception ex)
            {
                Session.Add("error", ex.ToString());
            }
        }
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("Pacientes.aspx", false);
        }
    }
}