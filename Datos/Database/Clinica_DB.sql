/* Programación 3 - TP Clínica - Grupo T
   Script unificado de creación de base de datos y tablas.
   Base de datos: CLINICA_DB
*/
-- ============================================================
-- DATOS DE BASE DE DATOS
-- ============================================================
IF DB_ID(N'CLINICA_DB') IS NULL
    CREATE DATABASE CLINICA_DB;
GO
USE CLINICA_DB;
GO
-- ============================================================
-- CREACIÓN DE TABLAS
-- ============================================================
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
    CONSTRAINT FK_Medicos_Usuarios FOREIGN KEY (Id_Usuario) REFERENCES USUARIOS(Id_Usuario)
);
GO
-- Permitir médicos sin usuario, evitando usuarios duplicados
CREATE UNIQUE INDEX UX_Medicos_IdUsuario
ON MEDICOS (Id_Usuario)
WHERE Id_Usuario IS NOT NULL;
GO
CREATE TABLE EMPLEADOS (
    Id_Empleado INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Id_Usuario INT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    DNI VARCHAR(20) NOT NULL UNIQUE,
    Estado BIT NOT NULL CONSTRAINT DF_Empleados_Estado DEFAULT(1),
    CONSTRAINT FK_Empleados_Usuarios FOREIGN KEY (Id_Usuario) REFERENCES USUARIOS(Id_Usuario)
);
GO
-- Permitir empleados sin usuario, evitando usuarios duplicados
CREATE UNIQUE INDEX UX_Empleados_IdUsuario
ON EMPLEADOS (Id_Usuario)
WHERE Id_Usuario IS NOT NULL;
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
-- ============================================================
-- DATOS DE PRUEBA
-- ============================================================
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;

-- 1. PERFILES: tres registros completamente ficticios.
INSERT INTO PERFILES (Nombre)
SELECT v.Nombre
FROM (VALUES
    (N'Administrador'),
    (N'Recepcionista'),
    (N'Médico')
) AS v(Nombre)
WHERE NOT EXISTS (
    SELECT 1 FROM PERFILES p
    WHERE p.Nombre = v.Nombre
);
-- 2. ESTADOS_TURNO: cinco registros completamente ficticios.
INSERT INTO ESTADOS_TURNO (Nombre)
SELECT v.Nombre
FROM (VALUES
    (N'Nuevo'),
    (N'Reprogramado'),
    (N'Cancelado'),
    (N'No asistió'),
    (N'Cerrado')
) AS v(Nombre)
WHERE NOT EXISTS (
    SELECT 1 FROM ESTADOS_TURNO ET
    WHERE ET.Nombre = v.Nombre
);
-- 3. PACIENTES: tres registros completamente ficticios.
INSERT INTO PACIENTES (Nombre,Apellido,DNI,Fecha_Nacimiento,Telefono,Email,Direccion,Estado)
SELECT v.Nombre,v.Apellido,v.DNI,v.FechaNacimiento,v.Telefono,v.Email,v.Direccion,1
FROM (VALUES
    (N'Camila',N'Prueba','88999101',CONVERT(date,'1998-04-12'),'1100000101',N'camila.demo@example.com',N'Calle Ficticia 101'),
    (N'Bruno',N'Prueba','88999102',CONVERT(date,'1987-09-23'),'1100000102',N'bruno.demo@example.com',N'Calle Ficticia 102'),
    (N'Sofía',N'Prueba','88999103',CONVERT(date,'2001-02-05'),'1100000103',N'sofia.demo@example.com',N'Calle Ficticia 103')
) AS v(Nombre,Apellido,DNI,FechaNacimiento,Telefono,Email,Direccion)
WHERE NOT EXISTS (SELECT 1 FROM PACIENTES p WHERE p.DNI=v.DNI);
-- 4. USUARIOS: solo se insertan cuando no existe el nombre de usuario.
INSERT INTO USUARIOS (Nombre_Usuario,Password_Hash,Email,Id_Perfil,Estado)
SELECT v.NombreUsuario,v.PasswordHash,v.Email,p.Id_Perfil,1
FROM (VALUES
    (N'admin.demo',N'PBKDF2-SHA256$600000$GlA/IlCidDH9nKIGZQPFfg==$qCJ6MlpLcOVBbSDjyokG4wiafV3I8x1uL3pDDjiU7E0=',N'admin.demo@example.com',N'Administrador'),
    (N'recepcion.demo',N'PBKDF2-SHA256$600000$JGVkMxj75Ci8xSlqg0Riaw==$rlyUGIHE4yuFfu4a86Fuv+ylmuEnqxxdXHltoEtLX3I=',N'recepcion.demo@example.com',N'Recepcionista'),
    (N'medico.demo',N'PBKDF2-SHA256$600000$nh6q5UDx17jxpygy9nQnJA==$+ZMzpYmeiYcZQ9trvN4SJ7/zDTpkXiEy9xExruMpyHw=',N'medico.demo@example.com',N'Médico')
) AS v(NombreUsuario,PasswordHash,Email,NombrePerfil)
INNER JOIN PERFILES p ON p.Nombre=v.NombrePerfil
WHERE NOT EXISTS (SELECT 1 FROM USUARIOS u WHERE u.Nombre_Usuario=v.NombreUsuario);
-- 5. MEDICOS: cuatro registros completamente ficticios.
INSERT INTO MEDICOS
    (Id_Usuario, Nombre, Apellido, DNI, Matricula, Telefono, Email)
SELECT
    NULL,
    v.Nombre,
    v.Apellido,
    v.DNI,
    v.Matricula,
    v.Telefono,
    v.Email
FROM (
    VALUES
        (N'Juan', N'Perez', '99999991', 'TEST-001',
         '1112345678', 'medico.prueba@example.com'),

        (N'Nicolás', N'Muñoz', '99999992', 'TEST-002',
         '1112345679', 'nicolas.test@example.com'),

        (N'María', N'Peña', '99999993', 'TEST-003',
         '1112345680', 'maria.test@example.com'),

        (N'Juan', N'González', '99999994', 'TEST-004',
         '1112345681', 'juan.test@example.com')
) AS v(Nombre, Apellido, DNI, Matricula, Telefono, Email)
WHERE NOT EXISTS (
    SELECT 1
    FROM MEDICOS m
    WHERE m.Matricula = v.Matricula
);
-- 6. EMPLEADOS: dos vinculados a sus cuentas y un tercero sin cuenta.
-- Se evita asociar dos empleados al mismo usuario.
INSERT INTO EMPLEADOS (Id_Usuario,Nombre,Apellido,DNI,Estado)
SELECT u.Id_Usuario,v.Nombre,v.Apellido,v.DNI,1
FROM (VALUES
    (N'admin.demo',N'Ana',N'Administradora','88999001'),
    (N'recepcion.demo',N'Lucía',N'Recepción','88999002')
) AS v(NombreUsuario,Nombre,Apellido,DNI)
INNER JOIN USUARIOS u ON u.Nombre_Usuario=v.NombreUsuario
WHERE NOT EXISTS (SELECT 1 FROM EMPLEADOS e WHERE e.DNI=v.DNI)
  AND NOT EXISTS (SELECT 1 FROM EMPLEADOS e WHERE e.Id_Usuario=u.Id_Usuario);

INSERT INTO EMPLEADOS (Id_Usuario,Nombre,Apellido,DNI,Estado)
SELECT NULL,N'Martín',N'Apoyo','88999003',1
WHERE NOT EXISTS (SELECT 1 FROM EMPLEADOS WHERE DNI='88999003');

-- 6B. Asociar la cuenta medica al medico de prueba TEST-001.
-- No se reemplazan vinculaciones existentes.
UPDATE m
   SET m.Id_Usuario=u.Id_Usuario
FROM MEDICOS m
INNER JOIN USUARIOS u ON u.Nombre_Usuario=N'medico.demo'
WHERE m.Matricula=N'TEST-001' AND m.Id_Usuario IS NULL
  AND NOT EXISTS (SELECT 1 FROM MEDICOS otro WHERE otro.Id_Usuario=u.Id_Usuario);
-- 7. ESPECIALIDADES: veintidos registros completamente ficticios.
INSERT INTO ESPECIALIDADES (Nombre, Descripcion, Estado)
SELECT v.Nombre, v.Descripcion, 1
FROM (VALUES
    (N'Clínica médica', N'Atención integral de adultos, grupos familiares y derivación inicial.'),
    (N'Cardiología', N'Atención cardiovascular'),
    (N'Dermatología', N'Atención dermatológica'),
    (N'Ginecología', N'Atención ginecológica'),
    (N'Endocrinología', N'Atención del sistema hormonal y metabolismo'),
    (N'Gastroenterología', N'Atención del aparato digestivo, hígado y vías biliares.'),
    (N'Infectología', N'Atención de enfermedades infecciosas y vacunas.'),
    (N'Nefrología', N'Atención de riñones y vías urinarias superiores.'),
    (N'Neumonología', N'Atención del aparato respiratorio y pulmones'),
    (N'Neurología', N'Atención del sistema nervioso central y periférico.'),
    (N'Oncología', N'Atención de tumores y cáncer.'),
    (N'Reumatología', N'Atención de enfermedades autoinmunes y del tejido conectivo / musculoesquelético.'),
    (N'Cirugía general', N'Atención de intervenciones abdominales, partes blandas y digestivas frecuentes.'),
    (N'Traumatología y ortopedia', N'Atención de huesos, articulaciones, músculos y lesiones físicas.'),
    (N'Urología', N'Atención de tracto urinario y aparato reproductor masculino.'),
    (N'Oftalmología', N'Atención de salud visual y cirugía ocular.'),
    (N'Otorrinolaringología', N'Atención del oído, nariz, senos paranasales y garganta.'),
    (N'Neurocirugía', N'Atención de intervenciones quirúrgicas en cerebro, columna y nervios periféricos.'),
    (N'Cirugía cardiovascular, torácica y plástico/reparadora', N'Atención de procedimientos especializados por región o reconstrucción.'),
    (N'Pediatría y neonatología', N'Atención de recién nacidos, niños y adolescentes.'),
    (N'Psiquiatría', N'Atención, diagnóstico y tratamiento farmacológico/clínico de trastornos de salud mental.'),
    (N'Diagnóstico por imágenes y laboratorio', N'Atención de estudios radiológicos, ecografías, análisis clínicos y biopsias.')
) AS v(Nombre, Descripcion)
WHERE NOT EXISTS (
    SELECT 1 FROM ESPECIALIDADES e WHERE e.Nombre = v.Nombre
);

-- 8. MEDICOS_ESPECIALIDADES: relaciones necesarias para turnos.
INSERT INTO MEDICOS_ESPECIALIDADES (Id_Medico,Id_Especialidad)
SELECT m.Id_Medico,e.Id_Especialidad
FROM (VALUES
    (N'TEST-001',N'Clínica médica'),
    (N'TEST-002',N'Cardiología'),
    (N'TEST-003',N'Dermatología'),
    (N'TEST-004',N'Clínica médica')
) AS v(Matricula,NombreEspecialidad)
INNER JOIN MEDICOS m ON m.Matricula=v.Matricula
INNER JOIN ESPECIALIDADES e ON e.Nombre=v.NombreEspecialidad
WHERE NOT EXISTS (
    SELECT 1 FROM MEDICOS_ESPECIALIDADES me
    WHERE me.Id_Medico=m.Id_Medico AND me.Id_Especialidad=e.Id_Especialidad
);
-- 9. HORARIOS_TRABAJO: cuatro registros completamente ficticios.
INSERT INTO HORARIOS_TRABAJO
    (Id_Medico, Dia_Semana, Hora_Entrada, Hora_Salida)
SELECT
    m.Id_Medico,
    v.Dia_Semana,
    v.Hora_Entrada,
    v.Hora_Salida
FROM (
    VALUES
        ('TEST-001', 1, CAST('09:00' AS TIME), CAST('13:00' AS TIME)),
        ('TEST-002', 1, CAST('14:00' AS TIME), CAST('18:00' AS TIME)),
        ('TEST-003', 3, CAST('08:00' AS TIME), CAST('12:00' AS TIME)),
        ('TEST-004', 5, CAST('10:00' AS TIME), CAST('16:00' AS TIME))
) AS v(Matricula, Dia_Semana, Hora_Entrada, Hora_Salida)
INNER JOIN MEDICOS m
    ON m.Matricula = v.Matricula
WHERE NOT EXISTS (
    SELECT 1
    FROM HORARIOS_TRABAJO h
    WHERE h.Id_Medico = m.Id_Medico
      AND h.Dia_Semana = v.Dia_Semana
      AND h.Hora_Entrada = v.Hora_Entrada
      AND h.Hora_Salida = v.Hora_Salida
);

-- Precondiciones para no generar conjuntos parciales silenciosamente.
IF (SELECT COUNT(*) FROM PERFILES WHERE Nombre IN (N'Administrador',N'Recepcionista',N'Médico')) <> 3
    THROW 51001, 'Faltan perfiles iniciales. Ejecutar primero el script base.', 1;
IF (SELECT COUNT(*) FROM MEDICOS WHERE Matricula IN (N'TEST-001',N'TEST-002',N'TEST-003')) <> 3
    THROW 51002, 'Faltan medicos TEST-001/002/003. Ejecutar primero la carga base.', 1;
IF NOT EXISTS (SELECT 1 FROM ESPECIALIDADES WHERE Nombre = N'Clínica médica')
   OR NOT EXISTS (SELECT 1 FROM ESPECIALIDADES WHERE Nombre = N'Cardiología')
   OR NOT EXISTS (SELECT 1 FROM ESPECIALIDADES WHERE Nombre = N'Dermatología')
    THROW 51003, 'Faltan especialidades de prueba.', 1;

-- 10. TURNOS: fechas futuras en relacion al momento de preparacion del demo.
-- Dos reprogramados con historial y uno nuevo; horarios validos para los medicos de prueba (lunes y miercoles).
INSERT INTO TURNOS
    (Id_Paciente,Id_Medico,Id_Especialidad,Fecha,Hora_Inicio,Hora_Fin,
     Id_Estado_Turno,Motivo_Consulta,Diagnostico,Email_Enviado)
SELECT p.Id_Paciente,m.Id_Medico,e.Id_Especialidad,v.Fecha,
       v.HoraInicio,v.HoraFin,et.Id_Estado_Turno,v.Motivo,NULL,0
FROM (VALUES
    ('88999101',N'TEST-001',N'Clínica médica',CONVERT(date,'2027-01-04'),CONVERT(time(0),'10:00'),CONVERT(time(0),'11:00'),N'Reprogramado',N'PRUEBA-SEED-001 - Control general'),
    ('88999102',N'TEST-002',N'Cardiología',CONVERT(date,'2027-01-11'),CONVERT(time(0),'15:00'),CONVERT(time(0),'16:00'),N'Reprogramado',N'PRUEBA-SEED-002 - Consulta cardiovascular'),
    ('88999103',N'TEST-003',N'Dermatología',CONVERT(date,'2027-01-06'),CONVERT(time(0),'09:00'),CONVERT(time(0),'10:00'),N'Nuevo',N'PRUEBA-SEED-003 - Consulta dermatológica')
) AS v(DNI,Matricula,Especialidad,Fecha,HoraInicio,HoraFin,EstadoTurno,Motivo)
INNER JOIN PACIENTES p ON p.DNI=v.DNI
INNER JOIN MEDICOS m ON m.Matricula=v.Matricula
INNER JOIN ESPECIALIDADES e ON e.Nombre=v.Especialidad
INNER JOIN MEDICOS_ESPECIALIDADES me ON me.Id_Medico=m.Id_Medico AND me.Id_Especialidad=e.Id_Especialidad
INNER JOIN ESTADOS_TURNO et ON et.Nombre=v.EstadoTurno
WHERE NOT EXISTS (SELECT 1 FROM TURNOS t WHERE t.Motivo_Consulta=v.Motivo);

-- 11. HISTORIAL_TURNOS: estado anterior de dos turnos reprogramados.
INSERT INTO HISTORIAL_TURNOS
    (Id_Turno,Fecha_Anterior,Hora_Inicio_Anterior,Hora_Fin_Anterior,Id_Estado_Turno_Anterior)
SELECT t.Id_Turno,v.FechaAnterior,v.HoraInicioAnterior,v.HoraFinAnterior,et.Id_Estado_Turno
FROM (VALUES
    (N'PRUEBA-SEED-001 - Control general',CONVERT(date,'2026-12-28'),CONVERT(time(0),'10:00'),CONVERT(time(0),'11:00'),N'Nuevo'),
    (N'PRUEBA-SEED-002 - Consulta cardiovascular',CONVERT(date,'2027-01-04'),CONVERT(time(0),'15:00'),CONVERT(time(0),'16:00'),N'Nuevo')
) AS v(Motivo,FechaAnterior,HoraInicioAnterior,HoraFinAnterior,EstadoAnterior)
INNER JOIN TURNOS t ON t.Motivo_Consulta=v.Motivo
INNER JOIN ESTADOS_TURNO et ON et.Nombre=v.EstadoAnterior
WHERE NOT EXISTS (
    SELECT 1 FROM HISTORIAL_TURNOS h
    WHERE h.Id_Turno=t.Id_Turno
      AND h.Fecha_Anterior=v.FechaAnterior
      AND h.Hora_Inicio_Anterior=v.HoraInicioAnterior
      AND h.Hora_Fin_Anterior=v.HoraFinAnterior
      AND h.Id_Estado_Turno_Anterior=et.Id_Estado_Turno
);
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO