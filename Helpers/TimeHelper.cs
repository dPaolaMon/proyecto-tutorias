using System;

namespace tutorias.Helpers
{
    public static class TimeHelper
    {
        private static readonly TimeZoneInfo MexicoCityTimeZone = ResolverZonaHorariaMexico();

        private static TimeZoneInfo ResolverZonaHorariaMexico()
        {
            // Intentar identificador IANA (estándar en Linux / contenedores y soportado en .NET 6+)
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City");
            }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }

            // Intentar identificador Windows
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
            }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }

            // Respaldo manual a UTC-6 (Hora del Centro de México)
            return TimeZoneInfo.CreateCustomTimeZone(
                "Mexico_Central",
                TimeSpan.FromHours(-6),
                "Hora del Centro de Mexico",
                "Hora del Centro de Mexico"
            );
        }

        public static DateTime NowMexico
        {
            get
            {
                var simuladaStr = Environment.GetEnvironmentVariable("FECHA_SIMULADA_DEV");
                if (!string.IsNullOrWhiteSpace(simuladaStr) && DateTime.TryParse(simuladaStr, out var fechaSimulada))
                {
                    return fechaSimulada;
                }

                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, MexicoCityTimeZone);
            }
        }

        public static DateTime UtcNow
        {
            get
            {
                var simuladaStr = Environment.GetEnvironmentVariable("FECHA_SIMULADA_DEV");
                if (!string.IsNullOrWhiteSpace(simuladaStr) && DateTime.TryParse(simuladaStr, out var fechaSimulada))
                {
                    return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(fechaSimulada, DateTimeKind.Unspecified), MexicoCityTimeZone);
                }

                return DateTime.UtcNow;
            }
        }

        public static DateTime TodayMexico => NowMexico.Date;

        public static bool EstaSimulandoHora => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FECHA_SIMULADA_DEV"));

        public static DateTime ConvertToMexico(DateTime utcDateTime)
        {
            var utc = utcDateTime.Kind == DateTimeKind.Utc
                ? utcDateTime
                : DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

            return TimeZoneInfo.ConvertTimeFromUtc(utc, MexicoCityTimeZone);
        }

        public static (DateTime InicioUtc, DateTime FinUtc) GetRangoUtcDia(DateTime fechaLocalMexico)
        {
            var inicioLocal = fechaLocalMexico.Date;
            var finLocal = inicioLocal.AddDays(1);

            var inicioUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(inicioLocal, DateTimeKind.Unspecified), MexicoCityTimeZone);
            var finUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(finLocal, DateTimeKind.Unspecified), MexicoCityTimeZone);

            return (inicioUtc, finUtc);
        }

        public static (DateTime InicioUtc, DateTime FinUtc) GetRangoUtcMes(int anio, int mes)
        {
            var inicioLocal = new DateTime(anio, mes, 1);
            var finLocal = inicioLocal.AddMonths(1);

            var inicioUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(inicioLocal, DateTimeKind.Unspecified), MexicoCityTimeZone);
            var finUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(finLocal, DateTimeKind.Unspecified), MexicoCityTimeZone);

            return (inicioUtc, finUtc);
        }
    }
}
