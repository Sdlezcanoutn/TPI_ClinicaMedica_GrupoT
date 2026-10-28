<%@ Page Title="Acerca de" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="PresentacionWeb.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card border-0 shadow-sm rounded-3 bg-white p-4 my-4">
        <h3 class="fw-bold text-dark m-0">Sobre Clínica Médica</h3>
        <p class="text-muted small m-0">Sistema integral de gestión de turnos y atención médica</p>
    </div>

    <div class="row g-4 mb-4">
        <div class="col-md-8">
            <div class="card border-0 shadow-sm rounded-3 bg-white p-4 h-100">
                <h5 class="fw-bold text-dark mb-3">Nuestra Institución</h5>
                <p class="text-secondary">
                    Este sistema está diseñado para optimizar el flujo de trabajo administrativo y médico, 
                    brindando una gestión ágil de pacientes, médicos, especialidades y agendamiento de turnos.
                </p>
                <p class="text-secondary mb-0">
                    Desarrollado con arquitectura sólida en ASP.NET Web Forms y SQL Server para garantizar velocidad, seguridad y precisión en los datos.
                </p>
            </div>
        </div>

        <div class="col-md-4">
            <div class="card border-0 shadow-sm rounded-3 bg-white p-4 h-100">
                <h5 class="fw-bold text-dark mb-3">Información del Proyecto</h5>
                <ul class="list-unstyled text-muted small m-0 d-flex flex-column gap-2">
                    <li><strong class="text-dark">Desarrollo:</strong> Grupo T</li>
                    <li><strong class="text-dark">Materia:</strong> Programación / BD</li>
                    <li><strong class="text-dark">Versión:</strong> 1.0.0</li>
                    <li><strong class="text-dark">Estado:</strong> En Desarrollo</li>
                </ul>
            </div>
        </div>
    </div>

</asp:Content>