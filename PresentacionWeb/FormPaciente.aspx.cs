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

        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Paciente nuevo = new Paciente();
                PacienteNegocio negocio = new PacienteNegocio();

                nuevo.Nombre = txtNombre.Text;
                nuevo.Apellido = txtApellido.Text;
                nuevo.DNI = txtDni.Text;

                // Conversión segura de la fecha
                if (DateTime.TryParse(txtFechaNacimiento.Text, out DateTime fecha))
                {
                    nuevo.Fecha_Nacimiento = fecha;
                }

                nuevo.Telefono = txtTelefono.Text;
                nuevo.Email = txtEmail.Text;
                nuevo.Direccion = txtDireccion.Text;

                // Llamamos al método agregar que creamos en el negocio
                negocio.agregar(nuevo);

                // Redirigimos de vuelta a la lista de pacientes
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