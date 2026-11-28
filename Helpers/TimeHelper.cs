using System;
using System.Globalization;

namespace tutorias.Helpers
{
    public enum ResultadoSimulacionHora
    {
        SinConfigurar,
        Activa,
        IgnoradaFueraDeDesarrollo,
        FormatoInvalido
    }

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

        private const string FormatoFechaSimulada = "yyyy-MM-ddTHH:mm:ss";

        // Diferencia entre el reloj simulado y el real; null cuando no hay simulación.
        private static TimeSpan? _desfaseSimulacion;

        /// <summary>
        /// Activa el reloj simulado, que avanza a partir de la fecha indicada (hora de México).
        /// Solo tiene efecto en el entorno Development.
        /// </summary>
        public static ResultadoSimulacionHora ConfigurarSimulacion(string? valor, bool esDesarrollo)
        {
            _desfaseSimulacion = null;

            var texto = valor?.Trim().Trim('"', '\'');
            if (string.IsNullOrWhiteSpace(texto))
            {
                return ResultadoSimulacionHora.SinConfigurar;
            }

            if (!esDesarrollo)
            {
                return ResultadoSimulacionHora.IgnoradaFueraDeDesarrollo;
            }

            if (!DateTime.TryParseExact(texto, FormatoFechaSimulada, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaSimulada)
                || MexicoCityTimeZone.IsInvalidTime(fechaSimulada))
            {
                return ResultadoSimulacionHora.FormatoInvalido;
            }

            _desfaseSimulacion = TimeZoneInfo.ConvertTimeToUtc(fechaSimulada, MexicoCityTimeZone) - DateTime.UtcNow;
            return ResultadoSimulacionHora.Activa;
        }

        public static DateTime UtcNow => DateTime.UtcNow + (_desfaseSimulacion ?? TimeSpan.Zero);

        public static DateTime NowMexico => ConvertToMexico(UtcNow);

        public static DateTime TodayMexico => NowMexico.Date;

        public static bool EstaSimulandoHora => _desfaseSimulacion.HasValue;

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
