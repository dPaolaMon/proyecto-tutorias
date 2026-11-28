using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace tutorias.Helpers
{
    public static class ValidacionEnEspanol
    {
        public static void UsarMensajesEnEspanol(this MvcOptions options)
        {
            var m = options.ModelBindingMessageProvider;
            m.SetMissingBindRequiredValueAccessor(campo => $"Falta el valor de {campo}.");
            m.SetMissingKeyOrValueAccessor(() => "Se requiere un valor.");
            m.SetMissingRequestBodyRequiredValueAccessor(() => "La solicitud llegó vacía.");
            m.SetValueMustNotBeNullAccessor(_ => "Este campo es obligatorio.");
            m.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor '{valor}' no es válido para {campo}.");
            m.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
            m.SetUnknownValueIsInvalidAccessor(campo => $"El valor de {campo} no es válido.");
            m.SetNonPropertyUnknownValueIsInvalidAccessor(() => "El valor no es válido.");
            m.SetValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
            m.SetValueMustBeANumberAccessor(campo => $"{campo} debe ser un número.");
            m.SetNonPropertyValueMustBeANumberAccessor(() => "El valor debe ser un número.");

            options.ModelMetadataDetailsProviders.Add(new RequeridoEnEspanol());
        }

        // Los "obligatorio" que el framework agrega solo (sin ErrorMessage) saldrían en inglés
        private sealed class RequeridoEnEspanol : IValidationMetadataProvider
        {
            private const string Mensaje = "El campo {0} es obligatorio.";

            public void CreateValidationMetadata(ValidationMetadataProviderContext context)
            {
                var validadores = context.ValidationMetadata.ValidatorMetadata;
                var requeridos = validadores.OfType<RequiredAttribute>().ToList();

                foreach (var requerido in requeridos.Where(r => r.ErrorMessage == null && r.ErrorMessageResourceName == null))
                {
                    requerido.ErrorMessage = Mensaje;
                }

                var tipo = context.Key.ModelType;
                if (requeridos.Count == 0
                    && context.Key.MetadataKind == ModelMetadataKind.Property
                    && tipo.IsValueType && Nullable.GetUnderlyingType(tipo) == null)
                {
                    validadores.Add(new RequiredAttribute { ErrorMessage = Mensaje });
                }
            }
        }
    }
}
