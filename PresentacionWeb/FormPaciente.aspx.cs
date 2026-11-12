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
                Paciente paciente = new Paciente();
                PacienteNegocio negocio = new PacienteNegocio();

                paciente.Nombre = txtNombre.Text;
                paciente.Apellido = txtApellido.Text;
                paciente.DNI = txtDni.Text;

                if (DateTime.TryParse(txtFechaNacimiento.Text, out DateTime fecha))
                {
                    paciente.Fecha_Nacimiento = fecha;
                }

                paciente.Telefono = txtTelefono.Text;
                paciente.Email = txtEmail.Text;
                paciente.Direccion = txtDireccion.Text;

                // Verificamos si estamos editando o agregando
                if (Request.QueryString["id"] != null)
                {
                    paciente.Id = int.Parse(Request.QueryString["id"]);
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