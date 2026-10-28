/* Programación 3 - TP Clínica - Grupo T
   Script unificado de creación de base de datos y tablas.
   Base de datos: CLINICA_DB
*/

IF DB_ID(N'CLINICA_DB') IS NULL
    CREATE DATABASE CLINICA_DB;
GO
USE CLINICA_DB;
GO

CREATE TABLE PERFILES (
    Id_Perfil INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE
);
GO
CREATE TABLE ESTADOS_TURNO (
    Id_Estado_Turno INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE
);
GO
CREATE TABLE PACIENTES (
    Id_Paciente INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    DNI VARCHAR(20) NOT NULL UNIQUE,
    Fecha_Nacimiento DATE NOT NULL,
    Telefono VARCHAR(30) NULL,
    Email NVARCHAR(254) NOT NULL,
    Direccion NVARCHAR(250) NULL,
    Estado BIT NOT NULL CONSTRAINT DF_Pacientes_Estado DEFAULT(1)
);
GO
CREATE TABLE USUARIOS (
    Id_Usuario INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre_Usuario NVARCHAR(80) NOT NULL UNIQUE,
    Password_Hash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(254) NOT NULL,
    Id_Perfil INT NOT NULL,
    Estado BIT NOT NULL CONSTRAINT DF_Usuarios_Estado DEFAULT(1),
    CONSTRAINT FK_Usuarios_Perfiles FOREIGN KEY (Id_Perfil) REFERENCES PERFILES(Id_Perfil)
);
GO
CREATE TABLE MEDICOS (
    Id_Medico INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Id_Usuario INT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    DNI VARCHAR(20) NOT NULL UNIQUE,
    Matricula NVARCHAR(50) NOT NULL UNIQUE,
    Telefono VARCHAR(30) NULL,
    Email NVARCHAR(254) NOT NULL,
    Estado BIT NOT NULL CONSTRAINT DF_Medicos_Estado DEFAULT(1),
    CONSTRAINT FK_Medicos_Usuarios FOREIGN KEY (Id_Usuario) REFERENCES USUARIOS(Id_Usuario),
    CONSTRAINT UQ_Medicos_IdUsuario UNIQUE (Id_Usuario)
);
GO
CREATE TABLE EMPLEADOS (
    Id_Empleado INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Id_Usuario INT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    DNI VARCHAR(20) NOT NULL UNIQUE,
    Estado BIT NOT NULL CONSTRAINT DF_Empleados_Estado DEFAULT(1),
    CONSTRAINT FK_Empleados_Usuarios FOREIGN KEY (Id_Usuario) REFERENCES USUARIOS(Id_Usuario),
    CONSTRAINT UQ_Empleados_IdUsuario UNIQUE (Id_Usuario)
);
GO
CREATE TABLE ESPECIALIDADES (
    Id_Especialidad INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Descripcion NVARCHAR(500) NULL,
    Estado BIT NOT NULL CONSTRAINT DF_Especialidades_Estado DEFAULT(1)
);
GO
CREATE TABLE MEDICOS_ESPECIALIDADES (
    Id_Medico INT NOT NULL,
    Id_Especialidad INT NOT NULL,
    CONSTRAINT PK_MedicosEspecialidades PRIMARY KEY (Id_Medico, Id_Especialidad),
    CONSTRAINT FK_ME_Medicos FOREIGN KEY (Id_Medico) REFERENCES MEDICOS(Id_Medico),
    CONSTRAINT FK_ME_Especialidades FOREIGN KEY (Id_Especialidad) REFERENCES ESPECIALIDADES(Id_Especialidad)
);
GO
CREATE TABLE HORARIOS_TRABAJO (
    Id_Horario_Trabajo INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Id_Medico INT NOT NULL,
    Dia_Semana TINYINT NOT NULL, -- 1=lunes ... 7=domingo (decisión propuesta)
    Hora_Entrada TIME(0) NOT NULL,
    Hora_Salida TIME(0) NOT NULL,
    Estado BIT NOT NULL CONSTRAINT DF_Horarios_Estado DEFAULT(1),
    CONSTRAINT FK_Horarios_Medicos FOREIGN KEY (Id_Medico) REFERENCES MEDICOS(Id_Medico),
    CONSTRAINT CK_Horarios_Dia CHECK (Dia_Semana BETWEEN 1 AND 7),
    CONSTRAINT CK_Horarios_Horas CHECK (Hora_Salida > Hora_Entrada)
);
GO
CREATE TABLE TURNOS (
    Id_Turno INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Id_Paciente INT NOT NULL,
    Id_Medico INT NOT NULL,
    Id_Especialidad INT NOT NULL,
    Fecha_Alta DATETIME2(0) NOT NULL CONSTRAINT DF_Turnos_FechaAlta DEFAULT(SYSDATETIME()),
    Fecha DATE NOT NULL,
    Hora_Inicio TIME(0) NOT NULL,
    Hora_Fin TIME(0) NOT NULL,
    Id_Estado_Turno INT NOT NULL,
    Motivo_Consulta NVARCHAR(1000) NOT NULL,
    Diagnostico NVARCHAR(MAX) NULL,
    Email_Enviado BIT NOT NULL CONSTRAINT DF_Turnos_EmailEnviado DEFAULT(0),
    CONSTRAINT FK_Turnos_Pacientes FOREIGN KEY (Id_Paciente) REFERENCES PACIENTES(Id_Paciente),
    CONSTRAINT FK_Turnos_Estados FOREIGN KEY (Id_Estado_Turno) REFERENCES ESTADOS_TURNO(Id_Estado_Turno),
    CONSTRAINT FK_Turnos_MedicoEspecialidad FOREIGN KEY (Id_Medico, Id_Especialidad)
       REFERENCES MEDICOS_ESPECIALIDADES(Id_Medico, Id_Especialidad),
    CONSTRAINT CK_Turnos_Horas CHECK (Hora_Fin > Hora_Inicio)
);
GO
CREATE TABLE HISTORIAL_TURNOS (
    Id_Historial_Turno INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Id_Turno INT NOT NULL,
    Fecha_Anterior DATE NOT NULL,
    Hora_Inicio_Anterior TIME(0) NOT NULL,
    Hora_Fin_Anterior TIME(0) NOT NULL,
    Id_Estado_Turno_Anterior INT NOT NULL,
    Fecha_Cambio DATETIME2(0) NOT NULL CONSTRAINT DF_Historial_FechaCambio DEFAULT(SYSDATETIME()),
    CONSTRAINT FK_Historial_Turnos FOREIGN KEY (Id_Turno) REFERENCES TURNOS(Id_Turno),
    CONSTRAINT FK_Historial_Estados FOREIGN KEY (Id_Estado_Turno_Anterior) REFERENCES ESTADOS_TURNO(Id_Estado_Turno)
);
GO

-- Datos de catálogo mínimos para probar la lectura desde la aplicación
INSERT INTO PERFILES (Nombre) VALUES (N'Administrador'), (N'Recepcionista'), (N'Médico');
INSERT INTO ESTADOS_TURNO (Nombre) VALUES
  (N'Nuevo'), (N'Reprogramado'), (N'Cancelado'), (N'No asistió'), (N'Cerrado');
INSERT INTO ESPECIALIDADES (Nombre, Descripcion) VALUES
  (N'Clínica médica', N'Atención clínica general'),
  (N'Cardiología', N'Atención cardiovascular'),
  (N'Dermatología', N'Atención dermatológica');
GO
-- Consulta sugerida para el checkpoint:
SELECT Id_Especialidad, Nombre, Descripcion, Estado FROM ESPECIALIDADES ORDER BY Nombre;
GO
/* PENDIENTE EN NEGOCIO / IMPLEMENTACIÓN:
 - Validar turnos no vencidos, duración de 1 hora y horarios de atención vigentes.
 - Evitar superposición de turnos del médico y del paciente, con transacción y concurrencia.
 - No asignar turnos a pacientes/médicos inactivos.
 - Controlar que un usuario no quede asociado a la vez a MEDICOS y EMPLEADOS.
 - Controlar perfiles acordes a la persona asociada y validaciones de correo.
 - Reprogramación: registrar valores anteriores en HISTORIAL_TURNOS y resetear EmailEnviado=0.
 - Las tablas se crean una sola vez; no re-ejecutar este script completo sobre la misma BD.
*/
