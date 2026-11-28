using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace tutorias.Helpers
{
    public static class ModelStateExtensions
    {
        // Une los mensajes de validación para decir exactamente qué falló
        public static string ResumenErrores(this ModelStateDictionary modelState) =>
            string.Join(" ", modelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Distinct());
    }
}
