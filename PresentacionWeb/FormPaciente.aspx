<%@ Page Title="Registrar Paciente" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FormPaciente.aspx.cs" Inherits="PresentacionWeb.FormPaciente" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-4 px-lg-5 my-5">
        <div style="max-width: 800px; margin: 0 auto;">
            
            <div class="card border-0 shadow-sm rounded-4 bg-white p-4 p-md-5">
                <div class="mb-4 pb-3 border-bottom">
                    <h2 class="fw-bold text-dark m-0">Nuevo Paciente</h2>
                    <p class="text-muted small m-0 mt-1">Complete los datos personales para registrar al paciente en el sistema</p>
                </div>

                <!-- Formulario -->
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Nombre</label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control bg-light" placeholder="Ej. Juan"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Apellido</label>
                        <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control bg-light" placeholder="Ej. Pérez"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label fw-semibold">DNI</label>
                        <asp:TextBox ID="txtDni" runat="server" CssClass="form-control bg-light" placeholder="Ej. 35123456"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Fecha de Nacimiento</label>
                        <asp:TextBox ID="txtFechaNacimiento" runat="server" CssClass="form-control bg-light" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Teléfono</label>
                        <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control bg-light" placeholder="Ej. 1123456789"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control bg-light" TextMode="Email" placeholder="correo@ejemplo.com"></asp:TextBox>
                    </div>

                    <div class="col-12">
                        <label class="form-label fw-semibold">Dirección</label>
                        <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control bg-light" placeholder="Av. Corrientes 1234, CABA"></asp:TextBox>
                    </div>

                    <!-- Botones de Acción -->
                    <div class="col-12 mt-4 pt-3 border-top d-flex justify-content-end gap-2">
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary px-4 rounded-3" CausesValidation="false" OnClick="btnCancelar_Click" />
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar Paciente" CssClass="btn btn-clinica-primary px-4 rounded-3 shadow-sm" OnClick="btnGuardar_Click" />
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>