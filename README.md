# 🏥 Sistema de Gestión de Clínica Médica

**Trabajo Práctico Integrador — Programación III**

Tecnicatura Universitaria en Programación | UTN — Facultad Regional General Pacheco

## 📌 Descripción

Aplicación web para la administración interna de una clínica médica.

El sistema permite gestionar pacientes, empleados, médicos, especialidades, horarios de trabajo y turnos médicos, contemplando la disponibilidad de los profesionales, el seguimiento de las consultas y los distintos perfiles de acceso.

La aplicación está destinada al personal autorizado de la clínica. Los pacientes no cuentan con acceso directo al sistema.

## 👥 Integrantes — Grupo T

- Lanzillotta, Gisela Grisel
- Lezcano, Sol Dánae
- Vázquez Velásquez, Henry José

## 🎯 Objetivo

Desarrollar un sistema web que facilite la organización de los turnos médicos, permita administrar la información de pacientes y profesionales, evite superposiciones de horarios y preserve la integridad y trazabilidad de los datos.

## ⚙️ Funcionalidades

### Gestión de pacientes

- Alta, consulta y modificación de pacientes.
- Baja lógica y reactivación.
- Administración de información personal y de contacto.
- Validación de DNI duplicados.
- Selección de pacientes registrados para la asignación de turnos.

### Gestión de empleados

- Alta, consulta y modificación de empleados.
- Baja lógica y reactivación.
- Administración de información personal.
- Validación de DNI duplicados.

### Gestión de médicos y especialidades

- Alta, consulta y modificación de médicos y especialidades.
- Baja lógica y reactivación.
- Asociación de médicos con una o más especialidades.
- Validación de registros duplicados.
- Control de turnos pendientes antes de realizar bajas o eliminar asociaciones entre médicos y especialidades.

### Gestión de horarios de trabajo

- Registro de horarios laborales de los profesionales.
- Configuración de días y horarios de entrada y salida.
- Consulta, modificación y eliminación de horarios.
- Determinación de la disponibilidad de los médicos para la asignación de turnos.

### Gestión de turnos médicos

- Asignación de turnos seleccionando un paciente y una especialidad.
- Sugerencia de tres alternativas de horarios y médicos disponibles.
- Selección manual de profesionales y consulta de disponibilidad.
- Duración de una hora por turno.
- Validación de superposiciones para médicos y pacientes.
- Validación de fechas y horarios.
- Registro del motivo de consulta.
- Generación de un número identificador de turno.
- Reprogramación de fecha y horario.
- Cancelación mediante cambio de estado, sin eliminación física.
- Registro de inasistencias.
- Consulta, búsqueda y filtrado de turnos.

### Estados de los turnos

| Estado | Descripción |
|---|---|
| Nuevo | Turno registrado. |
| Reprogramado | Turno cuya fecha u horario fue modificado. |
| Cancelado | Turno que dejó de estar vigente. |
| No asistió | Turno al que el paciente no se presentó. |
| Cerrado | Turno atendido con diagnóstico registrado. |

### Atención médica

- Visualización de los datos del turno y motivo de consulta.
- Registro del diagnóstico del paciente.
- Validación del horario y estado del turno para iniciar la atención.
- Cierre del turno mediante el registro de un diagnóstico.
- Consulta de diagnósticos de turnos cerrados en modo de solo lectura.
- Registro de inasistencias una vez finalizado el horario del turno.

### Historial de turnos

- Registro de la fecha, horario y estado anteriores del turno.
- Consulta cronológica de las modificaciones.
- Conservación de la información histórica.
- Uso de transacciones para garantizar la integridad entre el registro del historial y la actualización del turno.

### Notificaciones por correo electrónico

- Confirmación automática de turnos.
- Notificación de reprogramaciones.
- Notificación de cancelaciones.
- Generación de correos mediante plantillas HTML reutilizables.
- Registro del resultado del envío en la base de datos.
- Manejo de errores de envío sin afectar la operación realizada sobre el turno.

El servicio utiliza Mailtrap Sandbox como entorno de pruebas, sin realizar envíos a pacientes reales.

## 🔐 Usuarios y perfiles

El sistema contempla tres perfiles de acceso:

| Perfil | Responsabilidades |
|---|---|
| Administrador | Administración general del sistema. |
| Recepcionista | Gestión de pacientes, médicos y turnos. |
| Médico | Consulta de sus propios turnos y registro de diagnósticos. |

## 🗂️ Modelo de dominio

El modelo está compuesto por las siguientes clases:

- Paciente
- Empleado
- Medico
- Especialidad
- HorarioTrabajo
- Turno
- EstadoTurno
- HistorialTurno
- Usuario
- Perfil

La relación de muchos a muchos entre médicos y especialidades se representa mediante una colección de especialidades en la clase Medico y una tabla intermedia en la base de datos.

## 🛠️ Tecnologías utilizadas

| Tecnología | Aplicación |
|---|---|
| C# | Desarrollo de la lógica del sistema. |
| ASP.NET Web Forms | Desarrollo de la aplicación web. |
| .NET Framework | Plataforma de desarrollo. |
| SQL Server | Base de datos relacional. |
| ADO.NET | Acceso a datos. |
| LINQ | Consultas y filtrado de colecciones. |
| HTML, CSS y JavaScript | Interfaz y comportamiento de usuario. |
| Bootstrap | Diseño responsive y componentes visuales. |
| SMTP | Envío de correos electrónicos. |
| Mailtrap Sandbox | Entorno de pruebas de correo electrónico. |
| Visual Studio | Entorno de desarrollo. |
| Git y GitHub | Control de versiones y trabajo colaborativo. |

## 🏗️ Arquitectura del proyecto

La solución se organiza en capas para separar responsabilidades, facilitar el mantenimiento y favorecer la reutilización de código.

```
TPI_ClinicaMedica_GrupoT/
├── PresentacionWeb/
├── Dominio/
├── Negocio/
└── Datos/
```

**PresentacionWeb:** páginas ASP.NET Web Forms e interacción con el usuario.

**Dominio:** clases que representan las entidades del sistema.

**Negocio:** operaciones, reglas de negocio, validaciones y servicios.

**Datos:** comunicación con SQL Server mediante ADO.NET, centralizada a través de la clase AccesoDatos.

## 🛡️ Validaciones e integridad de los datos

- Validaciones de campos obligatorios y registros duplicados.
- Consultas SQL parametrizadas.
- Bajas lógicas para conservar información y relaciones.
- Control de turnos pendientes.
- Validación de disponibilidad y superposición de horarios.
- Control de cambios de estado.
- Transacciones mediante Commit y Rollback.
- Registro histórico de modificaciones.

## 🎨 Interfaz de usuario

La aplicación utiliza Bootstrap y estilos CSS compartidos para mantener una interfaz consistente entre los distintos módulos.

Se incorporaron búsquedas dinámicas, filtros, ventanas modales, mensajes de validación, indicadores de procesamiento y actualizaciones parciales mediante UpdatePanel.

## 📋 Etapas de desarrollo

### Etapa 1 — Estructura inicial

- Diseño del modelo de dominio y de datos.
- Creación de la solución y arquitectura en capas.
- Configuración de la base de datos y su conexión.
- Implementación de las primeras lecturas de datos.
- Desarrollo de pantallas iniciales y navegación.

### Etapa 2 — Administración de entidades

- Gestión de pacientes y empleados.
- Gestión de médicos y especialidades.
- Configuración de horarios de trabajo.
- Validaciones, bajas lógicas y reactivaciones.

### Etapa 3 — Gestión de turnos

- Asignación y consulta de turnos.
- Validación de disponibilidad.
- Sugerencia de alternativas.
- Reprogramación y cancelación.

### Etapa 4 — Integración y finalización

- Autenticación y permisos.
- Historial de modificaciones.
- Registro de inasistencias.
- Atención médica y diagnósticos.
- Notificaciones por correo electrónico.
- Unificación visual y funcional.
- Pruebas y correcciones finales.

## 💾 Base de datos

El script de creación de la base de datos se encuentra en:

```
Datos/Database/Clinica_DB.sql
```

## 📧 Configuración del servicio de correo

Para utilizar las notificaciones por correo electrónico en el entorno de desarrollo, se deben configurar las siguientes variables de entorno de usuario:

- `GSH_EMAIL_USER`
- `GSH_EMAIL_PASSWORD`

Estas variables corresponden a las credenciales SMTP de Mailtrap Sandbox.

Las credenciales no se incluyen en el repositorio por motivos de seguridad.

## 🚧 Estado del proyecto

**Etapa final — Integración y pruebas**

El sistema cuenta con sus principales módulos desarrollados y se encuentra en la etapa de integración, pruebas funcionales y revisión final.

---

**UTN — Facultad Regional General Pacheco**

Tecnicatura Universitaria en Programación | Programación III | Grupo T | 2026
