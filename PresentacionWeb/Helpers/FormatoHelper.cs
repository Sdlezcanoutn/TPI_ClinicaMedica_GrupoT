using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace PresentacionWeb.Helpers
{
    public static class FormatoHelper
    {
        public static string ObtenerNombreDia(int dia, string idioma = "es-AR")
        {
            if(dia < 1 || dia > 7)
                return "Día inválido";

            //Indicamos que idioma está usando el sistema
            CultureInfo cultura = CultureInfo.GetCultureInfo(idioma);

            //Convertimos el número de día a un texto reconocible en el idioma seleccionado
            DayOfWeek diaSemana = (DayOfWeek)(dia % 7);

            return cultura.DateTimeFormat.GetDayName(diaSemana);
        }

        public static string ObtenerEstado(bool activo)
        {
            return activo ? "Activo" : "Inactivo";
        }

    }
}