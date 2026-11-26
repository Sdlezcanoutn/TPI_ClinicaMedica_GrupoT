using System;
using System.Collections.Generic;
using System.Linq;
using Dominio;
using Datos;

namespace Negocio
{
    public class TurnoNegocio
    {
        // LISTADO DE TURNOS REGISTRADOS 
        public List<Turno> listar()
        {
            List<Turno> lista = new List<Turno>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                // Consultar turnos con datos de paciente, médico, especialidad y estado
                datos.setearConsulta(
                    "SELECT T.Id_Turno, T.Fecha_Alta, T.Fecha, T.Hora_Inicio, T.Hora_Fin, " +
                    "T.Motivo_Consulta, T.Diagnostico, T.Email_Enviado, " +
                    "P.Id_Paciente, P.Nombre AS NombrePaciente, P.Apellido AS ApellidoPaciente, P.DNI AS DniPaciente, P.Estado AS EstadoPaciente, " +
                    "M.Id_Medico, M.Nombre AS NombreMedico, M.Apellido AS ApellidoMedico, M.Matricula, M.Estado AS EstadoMedico, " +
                    "E.Id_Especialidad, E.Nombre AS NombreEspecialidad, E.Estado AS EstadoEspecialidad, " +
                    "ET.Id_Estado_Turno, ET.Nombre AS NombreEstadoTurno " +
                    "FROM TURNOS T " +
                    "INNER JOIN PACIENTES P ON T.Id_Paciente = P.Id_Paciente " +
                    "INNER JOIN MEDICOS M ON T.Id_Medico = M.Id_Medico " +
                    "INNER JOIN ESPECIALIDADES E ON T.Id_Especialidad = E.Id_Especialidad " +
                    "INNER JOIN ESTADOS_TURNO ET ON T.Id_Estado_Turno = ET.Id_Estado_Turno " +
                    "ORDER BY T.Fecha DESC, T.Hora_Inicio DESC"
                );

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    // Combinar Fecha (DATE) y horas (TIME) en DateTime para el modelo de Dominio
                    DateTime fechaTurno = (DateTime)datos.Lector["Fecha"];
                    TimeSpan tsInicio = (TimeSpan)datos.Lector["Hora_Inicio"];
                    TimeSpan tsFin = (TimeSpan)datos.Lector["Hora_Fin"];

                    Turno aux = new Turno
                    {
                        Id = (int)datos.Lector["Id_Turno"],
                        Fecha_Alta = (DateTime)datos.Lector["Fecha_Alta"],
                        Fecha = fechaTurno.Date,
                        Hora_Inicio = fechaTurno.Date.Add(tsInicio),
                        Hora_Fin = fechaTurno.Date.Add(tsFin),
                        Motivo_Consulta = (string)datos.Lector["Motivo_Consulta"],
                        Diagnostico = datos.Lector["Diagnostico"] == DBNull.Value
                            ? ""
                            : (string)datos.Lector["Diagnostico"],
                        Email_Enviado = (bool)datos.Lector["Email_Enviado"],
                        Paciente = new Paciente
                        {
                            Id = (int)datos.Lector["Id_Paciente"],
                            Nombre = (string)datos.Lector["NombrePaciente"],
                            Apellido = (string)datos.Lector["ApellidoPaciente"],
                            DNI = (string)datos.Lector["DniPaciente"],
                            Estado = (bool)datos.Lector["EstadoPaciente"]
                        },
                        Medico = new Medico
                        {
                            Id = (int)datos.Lector["Id_Medico"],
                            Nombre = (string)datos.Lector["NombreMedico"],
                            Apellido = (string)datos.Lector["ApellidoMedico"],
                            Matricula = (string)datos.Lector["Matricula"],
                            Estado = (bool)datos.Lector["EstadoMedico"]
                        },
                        Especialidad = new Especialidad
                        {
                            Id = (int)datos.Lector["Id_Especialidad"],
                            Nombre = (string)datos.Lector["NombreEspecialidad"],
                            Estado = (bool)datos.Lector["EstadoEspecialidad"]
                        },
                        Estado = new EstadoTurno
                        {
                            Id = (int)datos.Lector["Id_Estado_Turno"],
                            Nombre = (string)datos.Lector["NombreEstadoTurno"]
                        }
                    };

                    lista.Add(aux);
                }

                return lista;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // ===== VALIDACIONES DE TURNOS =====

        // --- Validar fecha y horario ---
        private void validarFechaHorario(
            DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin)
        {
            if (fecha.Date.Add(horaInicio) <= DateTime.Now)
            {
                throw new ArgumentException(
                    "No se permiten turnos en fechas u horarios pasados.");
            }

            if (horaFin <= horaInicio)
            {
                throw new ArgumentException(
                    "La hora de fin debe ser posterior a la hora de inicio.");
            }

            if (horaFin - horaInicio != TimeSpan.FromMinutes(60))
            {
                throw new ArgumentException(
                    "El turno debe tener una duración de 60 minutos.");
            }
        }

        // --- Validar relación médico-especialidad ---
        private void validarEspecialidadMedico(Medico medico, int idEspecialidad)
        {
            bool atiendeEspecialidad = medico.Especialidades != null &&
                medico.Especialidades.Exists(e => e.Id == idEspecialidad);

            if (!atiendeEspecialidad)
            {
                throw new ArgumentException(
                    "El médico seleccionado no atiende la especialidad elegida.");
            }
        }

        // --- Verificar superposición de turnos ---
        private void validarSuperposicion(
            int idMedico,
            int idPaciente,
            DateTime fecha,
            TimeSpan horaInicio,
            TimeSpan horaFin,
            int idTurnoExcluir = 0)
        {
            bool haySuperposicion = listar().Any(t =>
                t.Id != idTurnoExcluir &&
                t.Fecha.Date == fecha.Date &&
                t.Estado != null &&
                t.Estado.Nombre != "Cancelado" &&
                horaInicio < t.Hora_Fin.TimeOfDay &&
                horaFin > t.Hora_Inicio.TimeOfDay &&
                (t.Medico.Id == idMedico || t.Paciente.Id == idPaciente)
            );

            if (haySuperposicion)
            {
                throw new ArgumentException(
                    "Ya existe un turno superpuesto en ese horario para el médico o el paciente.");
            }
        }

        // ===== HISTORIAL DE TURNOS =====

        // --- Registrar los datos anteriores del turno ---
        private void registrarHistorial(AccesoDatos datos, Turno original)
        {
            datos.setearConsulta(
                "INSERT INTO HISTORIAL_TURNOS " +
                "(Id_Turno, Fecha_Anterior, Hora_Inicio_Anterior, " +
                "Hora_Fin_Anterior, Id_Estado_Turno_Anterior) " +
                "SELECT Id_Turno, Fecha, Hora_Inicio, Hora_Fin, Id_Estado_Turno " +
                "FROM TURNOS WITH (UPDLOCK, HOLDLOCK) " +
                "WHERE Id_Turno = @IdTurno " +
                "AND Fecha = @FechaAnterior " +
                "AND Hora_Inicio = @HoraInicioAnterior " +
                "AND Hora_Fin = @HoraFinAnterior " +
                "AND Id_Estado_Turno = @IdEstadoAnterior; " +
                "IF @@ROWCOUNT = 0 " +
                "THROW 50001, 'El turno fue modificado. Actualice el listado.', 1;"
            );

            datos.setearParametro("@IdTurno", original.Id);
            datos.setearParametro("@FechaAnterior", original.Fecha.Date);
            datos.setearParametro("@HoraInicioAnterior", original.Hora_Inicio.TimeOfDay);
            datos.setearParametro("@HoraFinAnterior", original.Hora_Fin.TimeOfDay);
            datos.setearParametro("@IdEstadoAnterior", original.Estado.Id);

            datos.ejecutarAccionTransaccion();
        }

        // --- Consultar el historial de un turno ---
        public List<HistorialTurno> listarHistorial(int idTurno)
        {
            List<HistorialTurno> lista = new List<HistorialTurno>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "SELECT H.Id_Historial_Turno, H.Id_Turno, " +
                    "H.Fecha_Anterior, H.Hora_Inicio_Anterior, " +
                    "H.Hora_Fin_Anterior, H.Fecha_Cambio, " +
                    "H.Id_Estado_Turno_Anterior, E.Nombre AS NombreEstadoAnterior " +
                    "FROM HISTORIAL_TURNOS H " +
                    "INNER JOIN ESTADOS_TURNO E " +
                    "ON H.Id_Estado_Turno_Anterior = E.Id_Estado_Turno " +
                    "WHERE H.Id_Turno = @IdTurno " +
                    "ORDER BY H.Fecha_Cambio DESC, H.Id_Historial_Turno DESC"
                );

                datos.setearParametro("@IdTurno", idTurno);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    DateTime fechaAnterior =
                        (DateTime)datos.Lector["Fecha_Anterior"];

                    HistorialTurno aux = new HistorialTurno
                    {
                        Id = (int)datos.Lector["Id_Historial_Turno"],

                        Turno = new Turno
                        {
                            Id = (int)datos.Lector["Id_Turno"]
                        },

                        Fecha_Anterior = fechaAnterior.Date,

                        Hora_Inicio_Anterior = fechaAnterior.Date.Add(
                            (TimeSpan)datos.Lector["Hora_Inicio_Anterior"]),

                        Hora_Fin_Anterior = fechaAnterior.Date.Add(
                            (TimeSpan)datos.Lector["Hora_Fin_Anterior"]),

                        Estado_Anterior = new EstadoTurno
                        {
                            Id = (int)datos.Lector["Id_Estado_Turno_Anterior"],
                            Nombre = (string)datos.Lector["NombreEstadoAnterior"]
                        },

                        Fecha_Cambio = (DateTime)datos.Lector["Fecha_Cambio"]
                    };

                    lista.Add(aux);
                }

                return lista;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // ALTA Y ASIGNACIÓN DE TURNOS (ETAPA 1)
        public void agregar(Turno nuevo)
        {
            // Validar selección de entidades y campos obligatorios
            if (nuevo == null ||
                nuevo.Paciente == null || nuevo.Paciente.Id <= 0 ||
                nuevo.Medico == null || nuevo.Medico.Id <= 0 ||
                nuevo.Especialidad == null || nuevo.Especialidad.Id <= 0)
            {
                throw new ArgumentException("Debe seleccionar paciente, especialidad y médico.");
            }

            if (string.IsNullOrWhiteSpace(nuevo.Motivo_Consulta))
            {
                throw new ArgumentException("El motivo de consulta es obligatorio.");
            }

            if (nuevo.Motivo_Consulta.Trim().Length > 1000)
            {
                throw new ArgumentException("El motivo de consulta no puede superar los 1000 caracteres.");
            }

            // --- Validar fecha y horario ---
            DateTime fechaSolo = nuevo.Fecha.Date;
            TimeSpan horaInicio = nuevo.Hora_Inicio.TimeOfDay;
            TimeSpan horaFin = nuevo.Hora_Fin.TimeOfDay;

            validarFechaHorario(fechaSolo, horaInicio, horaFin);

            // Verificar existencia y estado activo de paciente, médico y especialidad
            PacienteNegocio pacienteNegocio = new PacienteNegocio();
            Paciente paciente = pacienteNegocio.listar().Find(p => p.Id == nuevo.Paciente.Id);
            if (paciente == null || !paciente.Estado)
            {
                throw new ArgumentException("El paciente seleccionado no existe o está inactivo.");
            }

            MedicoNegocio medicoNegocio = new MedicoNegocio();
            Medico medico = medicoNegocio.listar().Find(m => m.Id == nuevo.Medico.Id);
            if (medico == null || !medico.Estado)
            {
                throw new ArgumentException("El médico seleccionado no existe o está inactivo.");
            }

            // --- Validar relación médico-especialidad ---
            validarEspecialidadMedico(medico, nuevo.Especialidad.Id);

            EspecialidadNegocio especialidadNegocio = new EspecialidadNegocio();
            Especialidad especialidad = especialidadNegocio.listar()
                .Find(e => e.Id == nuevo.Especialidad.Id);
            if (especialidad == null || !especialidad.Estado)
            {
                throw new ArgumentException("La especialidad seleccionada no existe o está inactiva.");
            }

            if (!EstaDentroDeHorarioLaboral(medico.Id, fechaSolo, horaInicio, horaFin))
            {
                throw new ArgumentException("El horario seleccionado está fuera del horario laboral del médico para ese día.");
            }

            // --- Validar superposición de turnos ---
            validarSuperposicion(
                medico.Id,
                paciente.Id,
                fechaSolo,
                horaInicio,
                horaFin
            );

            AccesoDatos datos = new AccesoDatos();

            try
            {
                // Insertar turno con estado inicial 'Nuevo' y Email_Enviado en 0 
                datos.setearConsulta(
                    "INSERT INTO TURNOS " +
                    "(Id_Paciente, Id_Medico, Id_Especialidad, Fecha_Alta, Fecha, Hora_Inicio, Hora_Fin, " +
                    "Id_Estado_Turno, Motivo_Consulta, Diagnostico, Email_Enviado) " +
                    "VALUES " +
                    "(@IdPaciente, @IdMedico, @IdEspecialidad, GETDATE(), @Fecha, @HoraInicio, @HoraFin, " +
                    "(SELECT TOP 1 Id_Estado_Turno FROM ESTADOS_TURNO WHERE Nombre = N'Nuevo'), " +
                    "@Motivo, NULL, 0)"
                );

                datos.setearParametro("@IdPaciente", paciente.Id);
                datos.setearParametro("@IdMedico", medico.Id);
                datos.setearParametro("@IdEspecialidad", especialidad.Id);
                datos.setearParametro("@Fecha", fechaSolo);
                datos.setearParametro("@HoraInicio", horaInicio);
                datos.setearParametro("@HoraFin", horaFin);
                datos.setearParametro("@Motivo", nuevo.Motivo_Consulta.Trim());

                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // ===== REPROGRAMACIÓN DE TURNOS =====
        public void reprogramar(Turno turno)
        {
            // --- Validar identificador ---
            if (turno == null || turno.Id <= 0)
            {
                throw new ArgumentException("El turno seleccionado no es válido.");
            }

            // --- Recuperar turno original ---
            Turno original = listar()
                .FirstOrDefault(t => t.Id == turno.Id);

            if (original == null)
            {
                throw new ArgumentException("El turno seleccionado no existe.");
            }

            // --- Validar estado del turno ---
            if (original.Estado.Nombre != "Nuevo" &&
                original.Estado.Nombre != "Reprogramado")
            {
                throw new ArgumentException(
                    "Solo se pueden reprogramar turnos nuevos o reprogramados.");
            }

            // --- Validar fecha y horario ---
            DateTime fecha = turno.Fecha.Date;
            TimeSpan horaInicio = turno.Hora_Inicio.TimeOfDay;
            TimeSpan horaFin = turno.Hora_Fin.TimeOfDay;

            validarFechaHorario(fecha, horaInicio, horaFin);

            // --- Verificar que exista una modificación ---
            if (original.Fecha.Date == fecha &&
                original.Hora_Inicio.TimeOfDay == horaInicio &&
                original.Hora_Fin.TimeOfDay == horaFin)
            {
                throw new ArgumentException(
                    "Debe seleccionar una fecha o un horario diferente al actual.");
            }

            // --- Validar entidades activas ---
            if (!original.Paciente.Estado ||
                !original.Medico.Estado ||
                !original.Especialidad.Estado)
            {
                throw new ArgumentException(
                    "El paciente, el médico y la especialidad deben estar activos.");
            }

            // --- Validar relación médico-especialidad ---
            MedicoNegocio medicoNegocio = new MedicoNegocio();

            Medico medico = medicoNegocio.listar()
                .FirstOrDefault(m => m.Id == original.Medico.Id);

            if (medico == null || !medico.Estado)
            {
                throw new ArgumentException(
                    "El médico seleccionado no existe o está inactivo.");
            }

            validarEspecialidadMedico(medico, original.Especialidad.Id);

            // --- Validar horario laboral ---
            if (!EstaDentroDeHorarioLaboral(
                original.Medico.Id, fecha, horaInicio, horaFin))
            {
                throw new ArgumentException(
                    "El horario seleccionado está fuera del horario laboral del médico.");
            }

            // --- Validar superposición de turnos ---
            validarSuperposicion(
                original.Medico.Id,
                original.Paciente.Id,
                fecha,
                horaInicio,
                horaFin,
                original.Id
            );
            // --- Preparar la conexión ---
            AccesoDatos datos = new AccesoDatos();
            bool transaccionIniciada = false;

            try
            {
                datos.iniciarTransaccion();
                transaccionIniciada = true;

                // --- Registrar los datos anteriores en el historial ---
                registrarHistorial(datos, original);

                // --- Actualizar fecha, horario y estado del turno ---
                datos.limpiarParametros();

                datos.setearConsulta(
                    "UPDATE TURNOS SET " +
                    "Fecha = @Fecha, " +
                    "Hora_Inicio = @HoraInicio, " +
                    "Hora_Fin = @HoraFin, " +
                    "Id_Estado_Turno = (SELECT Id_Estado_Turno " +
                    "FROM ESTADOS_TURNO WHERE Nombre = N'Reprogramado'), " +
                    "Email_Enviado = 0 " +
                    "WHERE Id_Turno = @IdTurno"
                );

                datos.setearParametro("@Fecha", fecha);
                datos.setearParametro("@HoraInicio", horaInicio);
                datos.setearParametro("@HoraFin", horaFin);
                datos.setearParametro("@IdTurno", original.Id);

                datos.ejecutarAccionTransaccion();

                // --- Confirmar las operaciones ---
                datos.confirmarTransaccion();
                transaccionIniciada = false;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (transaccionIniciada)
                    datos.cancelarTransaccion();

                if (ex.Number == 50001)
                    throw new ArgumentException(ex.Message);

                throw;
            }
            catch
            {
                if (transaccionIniciada)
                    datos.cancelarTransaccion();

                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // ===== CAMBIOS DE ESTADO DE TURNOS =====

        // --- Cancelar turno ---
        public void cancelar(int id)
        {
            cambiarEstado(id, "Cancelado");
        }

        // --- Registrar inasistencia ---
        public void registrarInasistencia(int id)
        {
            cambiarEstado(id, "No asistió");
        }

        // --- Cerrar turno con diagnóstico ---
        public void cerrar(int id, string diagnostico)
        {
            if (string.IsNullOrWhiteSpace(diagnostico))
            {
                throw new ArgumentException(
                    "Debe ingresar el diagnóstico del paciente.");
            }

            cambiarEstado(id, "Cerrado", diagnostico.Trim());
        }

        // --- Centralizar cambios de estado ---
        private void cambiarEstado(int id, string nuevoEstado,
            string diagnostico = null)
        {
            // --- Validar identificador ---
            if (id <= 0)
            {
                throw new ArgumentException(
                    "El turno seleccionado no es válido.");
            }

            // --- Recuperar turno original ---
            Turno original = listar()
                .FirstOrDefault(t => t.Id == id);

            if (original == null)
            {
                throw new ArgumentException(
                    "El turno seleccionado no existe.");
            }

            // --- Validar estado del turno ---
            if (original.Estado.Nombre != "Nuevo" &&
                original.Estado.Nombre != "Reprogramado")
            {
                throw new ArgumentException(
                    "Solo se pueden modificar turnos nuevos o reprogramados.");
            }

            // --- Validar horario para registrar inasistencia ---
            if (nuevoEstado == "No asistió" &&
                original.Fecha.Date.Add(original.Hora_Fin.TimeOfDay) > DateTime.Now)
            {
                throw new ArgumentException(
                    "No se puede registrar una inasistencia antes de que finalice el turno.");
            }

            // --- Validar horario para cerrar turno ---
            if (nuevoEstado == "Cerrado" &&
                original.Fecha.Date.Add(original.Hora_Inicio.TimeOfDay) > DateTime.Now)
            {
                throw new ArgumentException(
                    "No se puede cerrar un turno antes de que comience.");
            }

            // --- Preparar la conexión ---
            AccesoDatos datos = new AccesoDatos();
            bool transaccionIniciada = false;

            try
            {
                datos.iniciarTransaccion();
                transaccionIniciada = true;

                // --- Registrar los datos anteriores en el historial ---
                registrarHistorial(datos, original);

                // --- Actualizar estado y diagnóstico del turno ---
                datos.limpiarParametros();

                datos.setearConsulta(
                    "UPDATE TURNOS SET " +
                    "Id_Estado_Turno = (SELECT Id_Estado_Turno " +
                    "FROM ESTADOS_TURNO WHERE Nombre = @NuevoEstado), " +
                    "Diagnostico = CASE WHEN @NuevoEstado = 'Cerrado' " +
                    "THEN @Diagnostico ELSE Diagnostico END " +
                    "WHERE Id_Turno = @IdTurno"
                );

                datos.setearParametro("@NuevoEstado", nuevoEstado);
                datos.setearParametro("@IdTurno", original.Id);
                datos.setearParametro("@Diagnostico", (object)diagnostico ?? DBNull.Value);

                datos.ejecutarAccionTransaccion();

                // --- Confirmar las operaciones ---
                datos.confirmarTransaccion();
                transaccionIniciada = false;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (transaccionIniciada)
                    datos.cancelarTransaccion();

                if (ex.Number == 50001)
                    throw new ArgumentException(ex.Message);

                throw;
            }
            catch
            {
                if (transaccionIniciada)
                    datos.cancelarTransaccion();

                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public class SlotHorario
        {
            public TimeSpan HoraInicio { get; set; }
            public TimeSpan HoraFin { get; set; }
            public string Etiqueta => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";
        }

        private int NormalizarDiaSemanaSql(string diaSemanaTexto)
        {
            if (!int.TryParse(diaSemanaTexto, out int valor))
                return -1;

            // Si está en 1..7 (ej. 1=Lunes..7=Domingo en ISO/SQL), convertir 7 -> 0 (Domingo)
            // y 1..6 -> 1..6 (Lunes..Sábado) para alinear con DayOfWeek (Domingo=0..Sábado=6).
            if (valor >= 1 && valor <= 7)
                return valor == 7 ? 0 : valor;

            // Si ya viene en 0..6 (Domingo=0..Sábado=6)
            if (valor >= 0 && valor <= 6)
                return valor;

            return -1;
        }

        private bool EstaDentroDeHorarioLaboral(int idMedico, DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin)
        {
            int diaBuscado = (int)fecha.Date.DayOfWeek;
            HorarioTrabajoNegocio horarioNegocio = new HorarioTrabajoNegocio();

            List<HorarioTrabajo> horariosMedico = horarioNegocio.listar()
                .Where(h => h.Estado &&
                            h.Medico != null &&
                            h.Medico.Id == idMedico &&
                            NormalizarDiaSemanaSql(h.Dia_Semana) == diaBuscado)
                .ToList();

            if (horariosMedico.Count == 0)
                return false;

            return horariosMedico.Any(h =>
            {
                TimeSpan entrada = h.Hora_Entrada.TimeOfDay;
                TimeSpan salida = h.Hora_Salida.TimeOfDay;
                return horaInicio >= entrada && horaFin <= salida;
            });
        }

        // ===== DISPONIBILIDAD DE TURNOS =====

        // --- Obtener horarios disponibles ---
        public List<SlotHorario> obtenerHorariosDisponibles(
            int idMedico,
            DateTime fecha,
            int idPaciente = 0,
            int idTurnoExcluir = 0)
        {
            List<SlotHorario> disponibles = new List<SlotHorario>();

            if (idMedico <= 0 || fecha.Date < DateTime.Today)
                return disponibles;

            int diaBuscado = (int)fecha.Date.DayOfWeek;

            HorarioTrabajoNegocio horarioNegocio = new HorarioTrabajoNegocio();

            // --- Obtener horarios laborales del médico ---
            List<HorarioTrabajo> franjas = horarioNegocio.listar()
                .Where(h => h.Estado &&
                            h.Medico != null &&
                            h.Medico.Id == idMedico &&
                            NormalizarDiaSemanaSql(h.Dia_Semana) == diaBuscado)
                .OrderBy(h => h.Hora_Entrada.TimeOfDay)
                .ToList();

            if (franjas.Count == 0)
                return disponibles;

            // --- Obtener turnos ocupados del médico o paciente ---
            List<Turno> turnosOcupados = listar()
                .Where(t => t.Id != idTurnoExcluir &&
                            t.Fecha.Date == fecha.Date &&
                            t.Estado != null &&
                            t.Estado.Nombre != "Cancelado" &&
                            (t.Medico.Id == idMedico ||
                            (idPaciente > 0 && t.Paciente.Id == idPaciente)))
                .ToList();

            TimeSpan duracion = TimeSpan.FromMinutes(60);
            TimeSpan ahora = DateTime.Now.TimeOfDay;
            bool esHoy = fecha.Date == DateTime.Today;

            // --- Calcular franjas disponibles ---
            foreach (HorarioTrabajo franja in franjas)
            {
                TimeSpan actual = franja.Hora_Entrada.TimeOfDay;
                TimeSpan finFranja = franja.Hora_Salida.TimeOfDay;

                while (actual + duracion <= finFranja)
                {
                    TimeSpan finSlot = actual + duracion;

                    bool yaPaso = esHoy && actual <= ahora;

                    bool ocupado = turnosOcupados.Any(t =>
                        actual < t.Hora_Fin.TimeOfDay &&
                        finSlot > t.Hora_Inicio.TimeOfDay);

                    if (!yaPaso && !ocupado)
                    {
                        bool yaAgregado = disponibles.Any(s =>
                            s.HoraInicio == actual &&
                            s.HoraFin == finSlot);

                        if (!yaAgregado)
                        {
                            disponibles.Add(new SlotHorario
                            {
                                HoraInicio = actual,
                                HoraFin = finSlot
                            });
                        }
                    }

                    actual = finSlot;
                }
            }

            return disponibles.OrderBy(s => s.HoraInicio).ToList();
        }
    }
}