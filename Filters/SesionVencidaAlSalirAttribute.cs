using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;

namespace tutorias.Filters
{
    // Si la sesión ya venció, el token antifalsificación de "Cerrar sesión" deja de ser válido:
    // en lugar de un error 400 se envía al inicio de sesión.
    public class SesionVencidaAlSalirAttribute : Attribute, IAlwaysRunResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is IAntiforgeryValidationFailedResult
                && context.HttpContext.User.Identity?.IsAuthenticated != true)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
