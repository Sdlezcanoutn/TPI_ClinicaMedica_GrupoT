<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PresentacionWeb._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- CARRUSEL DE IMAGENES -->
    <div class="full-bleed-carousel mb-5">
        <div id="heroCarousel" class="carousel slide carousel-fade shadow-sm" data-bs-ride="carousel" data-bs-interval="4000">

            <div class="carousel-indicators">
                <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="0" class="active" aria-current="true" aria-label="Slide 1"></button>
                <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="1" aria-label="Slide 2"></button>
                <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="2" aria-label="Slide 3"></button>
                <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="3" aria-label="Slide 4"></button>
            </div>

            <div class="carousel-inner">

                <!-- Slide 1 -->
                <div class="carousel-item active" data-bs-interval="2500">
                    <img src="/Images/slide-1.jpg" class="d-block w-100 carousel-img" alt="Medicina de Vanguardia">
                    <div class="carousel-caption hero-caption-wide">
                        <span class="badge bg-primary mb-2 px-3 py-2">Excelencia Médica</span>
                        <h2 class="fw-bold text-white fs-3">Medicina del futuro, centrada en vos</h2>
                        <p class="text-light mb-0">Innovación, tecnología y calidez humana al servicio de la salud integral.</p>
                    </div>
                </div>

                <!-- Slide 2 -->
                <div class="carousel-item" data-bs-interval="2500">
                    <img src="/Images/slide-2.jpg" class="d-block w-100 carousel-img" alt="Tecnología y Diagnóstico">
                    <div class="carousel-caption hero-caption-wide">
                        <span class="badge bg-info text-dark mb-2 px-3 py-2">Infraestructura</span>
                        <h2 class="fw-bold text-white fs-3">Tecnología de punta para diagnóstico preciso</h2>
                        <p class="text-light mb-0">Consultorios y equipamiento de última generación para tu tranquilidad.</p>
                    </div>
                </div>

                <!-- Slide 3 -->
                <div class="carousel-item" data-bs-interval="2500">
                    <img src="/Images/slide-3.jpg" class="d-block w-100 carousel-img" alt="Atención Profesional">
                    <div class="carousel-caption hero-caption-wide">
                        <span class="badge bg-success mb-2 px-3 py-2">Equipo Profesional</span>
                        <h2 class="fw-bold text-white fs-3">Especialistas comprometidos con tu bienestar</h2>
                        <p class="text-light mb-0">Atención integral y multidisciplinaria todos los días del año.</p>
                    </div>
                </div>

                <!-- Slide 4 -->
                <div class="carousel-item" data-bs-interval="2500">
                    <img src="/Images/slide-4.jpg" class="d-block w-100 carousel-img" alt="Acompañamiento y Cercanía">
                    <div class="carousel-caption hero-caption-wide">
                        <span class="badge badge-violeta text-dark mb-2 px-3 py-2">Cuidado Integral</span>
                        <h2 class="fw-bold text-white fs-3">Acompañamiento cercano en cada consulta</h2>
                        <p class="text-light mb-0">Agenda ágil, seguimiento médico y atención pensada para cada paciente.</p>
                    </div>
                </div>

            </div>

            <button class="carousel-control-prev" type="button" data-bs-target="#heroCarousel" data-bs-slide="prev">
                <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                <span class="visually-hidden">Anterior</span>
            </button>
            <button class="carousel-control-next" type="button" data-bs-target="#heroCarousel" data-bs-slide="next">
                <span class="carousel-control-next-icon" aria-hidden="true"></span>
                <span class="visually-hidden">Siguiente</span>
            </button>
        </div>
    </div>

    <!-- CONTENEDOR PARA EL DASHBOARD -->
    <div class="container-fluid px-4 px-lg-5 mb-5">

        <!-- Banner Bienvenida e Historia -->
        <div class="my-4 py-3">
            <div class="row align-items-center g-4 g-lg-5">
                <div class="col-lg-8">
                    <h2 class="fw-bold text-dark display-6 mb-2">Te damos la bienvenida a la medicina del futuro, centrada en vos y con el sentido humano de siempre
                    </h2>
                    <div class="divider-line mb-4"></div>

                    <div class="row g-4 bienvenida-columna-texto">
                        <div class="col-md-6">
                            <p class="mb-0">
                                Hace más de <strong>170 años</strong> que transformamos la salud con <strong>innovación, excelencia y compromiso</strong>. Ese legado nos impulsa a seguir siendo pioneros en cuidados de vanguardia y atención integral en toda la Argentina.
                            </p>
                        </div>
                        <div class="col-md-6">
                            <p class="mb-0">
                                Combinamos <strong>equipos multidisciplinarios</strong>, consultorios equipados y una gestión organizada para que cada consulta sea ágil y segura, acompañando a cada paciente con calidez y precisión clínica.
                            </p>
                        </div>
                    </div>
                </div>

                <div class="col-lg-4">
                    <div class="rounded-3 overflow-hidden shadow-sm">
                        <img src="/Images/slide-9.jpg" class="welcome-photo" alt="Instalaciones de GSH Clínica Médica">
                    </div>
                </div>
            </div>

            <!-- Contadores Institucionales en Azul -->
            <div class="row text-center g-4 mt-4 pt-3">
                <div class="col-6 col-md-4">
                    <div class="stat-numero-azul">+170</div>
                    <div class="stat-etiqueta-azul">Años de Trayectoria</div>
                </div>
                <div class="col-6 col-md-4">
                    <div class="stat-numero-azul">+120</div>
                    <div class="stat-etiqueta-azul">Profesionales y Especialistas</div>
                </div>
                <div class="col-12 col-md-4">
                    <div class="stat-numero-azul">15 min</div>
                    <div class="stat-etiqueta-azul">Módulos de Atención Ágil</div>
                </div>
            </div>
        </div>

        <hr class="seccion-divisor" />

        <!-- Tarjetas de Acceso Directo -->
        <div class="row g-4">

            <!-- Turnos -->
            <div class="col-md-6 col-xl-3">
                <div class="card dashboard-glass-card border-0 shadow-sm rounded-3 h-100">
                    <div class="card-body p-4 d-flex flex-column justify-content-between">
                        <div>
                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <span class="p-3 bg-primary-subtle text-primary rounded-3">
                                    <i class="fa-solid fa-calendar-check fs-4"></i>
                                </span>
                                <span class="text-muted small">Agendamiento</span>
                            </div>
                            <h5 class="fw-bold text-dark">Turnos</h5>
                            <p class="text-muted small mb-0">Administración de citas, horarios y atención a pacientes.</p>
                        </div>
                        <div class="mt-4 pt-2 border-top">
                            <a href="Turnos.aspx" class="btn btn-sm btn-outline-secondary w-100">Ir a Turnos →</a>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Pacientes -->
            <div class="col-md-6 col-xl-3">
                <div class="card dashboard-glass-card border-0 shadow-sm rounded-3 h-100">
                    <div class="card-body p-4 d-flex flex-column justify-content-between">
                        <div>
                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <span class="p-3 bg-success-subtle text-success rounded-3">
                                    <i class="fa-solid fa-user-group fs-4"></i>
                                </span>
                                <span class="text-muted small">Registros</span>
                            </div>
                            <h5 class="fw-bold text-dark">Pacientes</h5>
                            <p class="text-muted small mb-0">Gestión de datos personales e historias clínicas.</p>
                        </div>
                        <div class="mt-4 pt-2 border-top">
                            <a href="Pacientes.aspx" class="btn btn-sm btn-outline-secondary w-100">Ir a Pacientes →</a>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Médicos -->
            <div class="col-md-6 col-xl-3">
                <div class="card dashboard-glass-card border-0 shadow-sm rounded-3 h-100">
                    <div class="card-body p-4 d-flex flex-column justify-content-between">
                        <div>
                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <span class="p-3 bg-info-subtle text-info-emphasis rounded-3">
                                    <i class="fa-solid fa-user-doctor fs-4"></i>
                                </span>
                                <span class="text-muted small">Personal</span>
                            </div>
                            <h5 class="fw-bold text-dark">Médicos</h5>
                            <p class="text-muted small mb-0">Control del cuerpo médico, matrículas y estados.</p>
                        </div>
                        <div class="mt-4 pt-2 border-top">
                            <a href="Medicos.aspx" class="btn btn-sm btn-outline-secondary w-100">Ir a Médicos →</a>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Especialidades -->
            <div class="col-md-6 col-xl-3">
                <div class="card dashboard-glass-card border-0 shadow-sm rounded-3 h-100">
                    <div class="card-body p-4 d-flex flex-column justify-content-between">
                        <div>
                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <span class="p-3 bg-warning-subtle text-warning-emphasis rounded-3">
                                    <i class="fa-solid fa-stethoscope fs-4"></i>
                                </span>
                                <span class="text-muted small">Catálogo</span>
                            </div>
                            <h5 class="fw-bold text-dark">Especialidades</h5>
                            <p class="text-muted small mb-0">Configuración de áreas médicas de la clínica.</p>
                        </div>
                        <div class="mt-4 pt-2 border-top">
                            <a href="Especialidades.aspx" class="btn btn-sm btn-outline-secondary w-100">Ir a Especialidades →</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <hr class="seccion-divisor" />

        <!-- Franja Visual: Flujo de Atención -->
        <div class="workflow-card border-0 shadow-sm overflow-hidden mt-5">
            <div class="row g-0 align-items-center">
                <div class="col-lg-5">
                    <img src="/Images/slide-5.jpg" class="workflow-img" alt="Equipamiento e infraestructura de GSH Clínica Médica">
                </div>
                <div class="col-lg-7">
                    <div class="p-4 p-lg-5">
                        <span class="badge bg-info-subtle text-info-emphasis mb-2 px-3 py-2">Gestión Ágil</span>
                        <h3 class="fw-bold text-dark mb-2">Cómo organizamos la atención diaria</h3>
                        <p class="text-muted mb-4">
                            Un circuito claro para coordinar profesionales, especialidades y disponibilidad horaria sin superposiciones.
                        </p>

                        <div class="d-flex flex-column gap-3">
                            <div class="d-flex align-items-start gap-3">
                                <span class="step-badge">1</span>
                                <div>
                                    <h6 class="fw-bold text-dark mb-1">Registro y especialidades</h6>
                                    <p class="text-muted small mb-0">Alta de pacientes, gestión de médicos activos y asignación de áreas médicas.</p>
                                </div>
                            </div>

                            <div class="d-flex align-items-start gap-3">
                                <span class="step-badge">2</span>
                                <div>
                                    <h6 class="fw-bold text-dark mb-1">Disponibilidad en bloques de 15 minutos</h6>
                                    <p class="text-muted small mb-0">Cálculo automático de horarios libres según los días y franjas de trabajo de cada profesional.</p>
                                </div>
                            </div>

                            <div class="d-flex align-items-start gap-3">
                                <span class="step-badge">3</span>
                                <div>
                                    <h6 class="fw-bold text-dark mb-1">Reserva, reprogramación y seguimiento</h6>
                                    <p class="text-muted small mb-0">Control centralizado de turnos y estados para agilizar la atención en consultorio.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


    </div>

</asp:Content>
