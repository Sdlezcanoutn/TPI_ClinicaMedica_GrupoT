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

            // Validar que la fecha no sea pasada y que el rango horario sea válido
            DateTime fechaSolo = nuevo.Fecha.Date;
            if (fechaSolo < DateTime.Today)
            {
                throw new ArgumentException("No se pueden asignar turnos en fechas pasadas.");
            }

            TimeSpan horaInicio = nuevo.Hora_Inicio.TimeOfDay;
            TimeSpan horaFin = nuevo.Hora_Fin.TimeOfDay;

            if (horaFin <= horaInicio)
            {
                throw new ArgumentException("La hora de fin debe ser posterior a la hora de inicio.");
            }

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

            bool atiendeEspecialidad = medico.Especialidades != null &&
                medico.Especialidades.Exists(e => e.Id == nuevo.Especialidad.Id);
            if (!atiendeEspecialidad)
            {
                throw new ArgumentException("El médico seleccionado no atiende la especialidad elegida.");
            }

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

            // Evitar superposición de turnos (mismo médico o mismo paciente) en turnos no cancelados
            bool haySuperposicion = listar().Any(t =>
                t.Fecha.Date == fechaSolo &&
                t.Estado != null &&
                t.Estado.Nombre != "Cancelado" &&
                horaInicio < t.Hora_Fin.TimeOfDay &&
                horaFin > t.Hora_Inicio.TimeOfDay &&
                (t.Medico.Id == medico.Id || t.Paciente.Id == paciente.Id)
            );

            if (haySuperposicion)
            {
                throw new ArgumentException("Ya existe un turno superpuesto en ese horario para el médico o el paciente.");
            }

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

        public List<SlotHorario> obtenerHorariosDisponibles(int idMedico, DateTime fecha, int duracionMinutos = 15)
        {
            List<SlotHorario> disponibles = new List<SlotHorario>();

            if (idMedico <= 0 || duracionMinutos <= 0 || fecha.Date < DateTime.Today)
                return disponibles;

            int diaBuscado = (int)fecha.Date.DayOfWeek;
            HorarioTrabajoNegocio horarioNegocio = new HorarioTrabajoNegocio();

            List<HorarioTrabajo> franjas = horarioNegocio.listar()
                .Where(h => h.Estado &&
                            h.Medico != null &&
                            h.Medico.Id == idMedico &&
                            NormalizarDiaSemanaSql(h.Dia_Semana) == diaBuscado)
                .ToList();

            if (franjas.Count == 0)
                return disponibles;

            List<Turno> turnosOcupados = listar()
                .Where(t => t.Medico != null &&
                            t.Medico.Id == idMedico &&
                            t.Fecha.Date == fecha.Date &&
                            t.Estado != null &&
                            !string.Equals(t.Estado.Nombre, "Cancelado", StringComparison.OrdinalIgnoreCase))
                .ToList();

            TimeSpan duracion = TimeSpan.FromMinutes(duracionMinutos);
            TimeSpan ahora = DateTime.Now.TimeOfDay;
            bool esHoy = fecha.Date == DateTime.Today;

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
                        bool yaAgregado = disponibles.Any(s => s.HoraInicio == actual && s.HoraFin == finSlot);
                        if (!yaAgregado)
                        {
                            disponibles.Add(new SlotHorario
                            {
                                HoraInicio = actual,
                                HoraFin = finSlot
                            });
                        }
                    }

                    actual = actual.Add(duracion);
                }
            }

            return disponibles.OrderBy(s => s.HoraInicio).ToList();
        }
    }
}