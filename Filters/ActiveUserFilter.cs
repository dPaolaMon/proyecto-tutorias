using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using tutorias.Models;

namespace tutorias.Filters
{
    public class ActiveUserFilter : IAsyncActionFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public ActiveUserFilter(
            UserManager<ApplicationUser> userManager,
            ITempDataDictionaryFactory tempDataFactory)
        {
            _userManager = userManager;
            _tempDataFactory = tempDataFactory;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var controller = context.RouteData.Values["controller"]?.ToString()?.ToLowerInvariant() ?? "";
                var action = context.RouteData.Values["action"]?.ToString()?.ToLowerInvariant() ?? "";

                // Acciones permitidas para un usuario con cuenta en baja
                bool isAllowed = controller == "account" &&
                    (action == "profile" || action == "reactivateprofile" || action == "logout" || action == "accessdenied");

                if (!isAllowed)
                {
                    var user = await _userManager.GetUserAsync(context.HttpContext.User);
                    if (user != null && !user.Activo)
                    {
                        var tempData = _tempDataFactory.GetTempData(context.HttpContext);
                        tempData["WarningMessage"] = "Tu cuenta se encuentra dada de baja. Solo puedes visualizar tu perfil o reactivar tu cuenta para acceder a las funciones del sistema.";

                        context.Result = new RedirectToActionResult("Profile", "Account", null);
                        return;
                    }
                }
            }

            await next();
        }
    }
}
