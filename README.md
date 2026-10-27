# 🏥 Sistema de Gestión de Clínica Médica

**Trabajo Práctico Integrador — Programación III**  
**Tecnicatura Universitaria en Programación | UTN — Facultad Regional General Pacheco**

## 📌 Descripción

Desarrollo de una aplicación web para la administración interna de una clínica médica.

El sistema permitirá gestionar pacientes, médicos, especialidades, horarios de trabajo y asignación de turnos, contemplando la disponibilidad de los profesionales, el seguimiento de las consultas y los distintos perfiles de acceso.

La aplicación está destinada al personal autorizado de la clínica. Los pacientes no contarán con acceso directo al sistema.

## 👥 Integrantes — Grupo T

- **Lanzillotta, Gisela Grisel**
- **Lezcano, Sol Dánae**
- **Vázquez Velásquez, Henry José**

## 🎯 Objetivo

Desarrollar un sistema web que facilite la organización de los turnos médicos, permita administrar la información de pacientes y profesionales y evite superposiciones en las reservas.

## ⚙️ Funcionalidades previstas

### Gestión de pacientes
- Registrar y administrar la información personal de los pacientes.
- Consultar y actualizar sus datos.
- Seleccionar pacientes previamente registrados para asignar turnos.

### Gestión de médicos y especialidades
- Registrar y administrar profesionales médicos.
- Asociar cada médico con una o más especialidades.
- Configurar turnos de trabajo con horarios de entrada y salida.
- Administrar la disponibilidad de los profesionales.

### Gestión de turnos médicos
- Asignar turnos seleccionando un paciente y una especialidad.
- Proponer alternativas de horarios y médicos disponibles.
- Permitir seleccionar manualmente un profesional y consultar sus horarios.
- Establecer una duración de una hora por turno.
- Evitar superposiciones de turnos para un mismo médico o paciente.
- Impedir la asignación de turnos en fechas u horarios pasados.
- Registrar el motivo de la consulta.
- Generar un número de turno y enviar una confirmación por correo electrónico.
- Permitir reprogramar y cancelar turnos sin eliminarlos.
- Administrar los estados de los turnos.

### Registro de consultas
- Permitir que cada médico consulte sus turnos asignados.
- Registrar información relacionada con el diagnóstico del paciente.
- Conservar la información de las consultas.

### Usuarios y perfiles

El sistema contará con tres perfiles:

| Perfil | Responsabilidades |
|---|---|
| Administrador | Administración general del sistema |
| Recepcionista | Gestión de pacientes, médicos y asignación de turnos |
| Médico | Consulta de sus propios turnos y registro de diagnósticos |

## 🗂️ Modelo de dominio

El diseño inicial contempla las siguientes entidades:

- `Paciente`
- `Medico`
- `Especialidad`
- `MedicoEspecialidad`
- `HorarioTrabajo`
- `Turno`
- `EstadoTurno`
- `HistorialTurno`
- `Usuario`
- `Perfil`
- `Empleado`

El modelo se encuentra sujeto a ajustes durante el desarrollo.

## 🛠️ Tecnologías utilizadas y previstas

| Tecnología | Aplicación |
|---|---|
| C# | Desarrollo de la lógica del sistema |
| ASP.NET Web Forms | Aplicación web |
| .NET Framework | Plataforma de desarrollo |
| SQL Server | Base de datos relacional |
| ADO.NET | Acceso a datos |
| HTML, CSS y JavaScript | Interfaz de usuario |
| Bootstrap | Diseño responsive |
| Visual Studio | Entorno de desarrollo |
| Git y GitHub | Control de versiones y trabajo colaborativo |

## 🏗️ Arquitectura del proyecto

La solución se organiza en capas para separar responsabilidades y facilitar el desarrollo.

```text
TPI_ClinicaMedica_GrupoT/
├── PresentacionWeb/
├── Dominio/
├── Negocio/
└── Datos/
```

**PresentacionWeb:** páginas ASP.NET Web Forms e interfaz de usuario.

**Dominio:** entidades y estructuras que representan la información del sistema.

**Negocio:** operaciones, reglas de negocio y validaciones.

**Datos:** comunicación con SQL Server mediante ADO.NET.

## 📋 Etapas de desarrollo

**Etapa 1 — Estructura inicial**

- Diseño del modelo de clases y de datos.
- Creación de la solución y arquitectura en capas.
- Configuración de la base de datos y su conexión.
- Implementación de una primera lectura de datos.
- Desarrollo de pantallas iniciales con navegación.

**Etapa 2 — Administración de entidades**

- Gestión de pacientes.
- Gestión de médicos y especialidades.
- Configuración de horarios de trabajo.

**Etapa 3 — Gestión de turnos**

- Asignación y consulta de turnos.
- Validación de disponibilidad.
- Sugerencia de alternativas.
- Reprogramaciones, cancelaciones y estados.

**Etapa 4 — Integración y finalización**

- Autenticación y permisos.
- Registro de diagnósticos.
- Confirmaciones por correo electrónico.
- Pruebas y correcciones finales.

## 🚧 Estado del proyecto

**En desarrollo — Primera etapa**

Actualmente se trabaja en la estructura inicial de la aplicación, el diseño de las entidades, la arquitectura en capas y la configuración del entorno de desarrollo.

Las funcionalidades descriptas corresponden al alcance previsto y se incorporarán progresivamente.

---

**UTN — Facultad Regional General Pacheco**  
**Tecnicatura Universitaria en Programación | Programación III | Grupo T | 2026**
