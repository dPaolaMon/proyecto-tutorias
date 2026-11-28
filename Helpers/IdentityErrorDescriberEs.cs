using Microsoft.AspNetCore.Identity;

namespace tutorias.Helpers
{
    // Mensajes de ASP.NET Core Identity en español (se conservan los códigos originales)
    public class IdentityErrorDescriberEs : IdentityErrorDescriber
    {
        private static IdentityError Error(string codigo, string descripcion) =>
            new() { Code = codigo, Description = descripcion };

        public override IdentityError DefaultError() =>
            Error(nameof(DefaultError), "Ocurrió un error inesperado. Intenta de nuevo.");

        public override IdentityError ConcurrencyFailure() =>
            Error(nameof(ConcurrencyFailure), "La información cambió mientras la editabas. Vuelve a cargar la página e intenta de nuevo.");

        public override IdentityError PasswordMismatch() =>
            Error(nameof(PasswordMismatch), "La contraseña es incorrecta.");

        public override IdentityError InvalidToken() =>
            Error(nameof(InvalidToken), "El enlace venció, ya se usó o no es válido. Solicita uno nuevo.");

        public override IdentityError RecoveryCodeRedemptionFailed() =>
            Error(nameof(RecoveryCodeRedemptionFailed), "El código de recuperación no es válido.");

        public override IdentityError LoginAlreadyAssociated() =>
            Error(nameof(LoginAlreadyAssociated), "Ese acceso ya está asociado a otra cuenta.");

        public override IdentityError InvalidUserName(string? userName) =>
            Error(nameof(InvalidUserName), $"El correo '{userName}' contiene caracteres no permitidos (por ejemplo, acentos o ñ).");

        public override IdentityError InvalidEmail(string? email) =>
            Error(nameof(InvalidEmail), $"El correo '{email}' no es válido.");

        // Usuario y correo son el mismo dato: comparten mensaje para no mostrarlo dos veces
        public override IdentityError DuplicateUserName(string userName) =>
            Error(nameof(DuplicateUserName), $"El correo '{userName}' ya está registrado.");

        public override IdentityError DuplicateEmail(string email) =>
            Error(nameof(DuplicateEmail), $"El correo '{email}' ya está registrado.");

        public override IdentityError InvalidRoleName(string? role) =>
            Error(nameof(InvalidRoleName), $"El nombre de rol '{role}' no es válido.");

        public override IdentityError DuplicateRoleName(string role) =>
            Error(nameof(DuplicateRoleName), $"El rol '{role}' ya existe.");

        public override IdentityError UserAlreadyHasPassword() =>
            Error(nameof(UserAlreadyHasPassword), "La cuenta ya tiene una contraseña.");

        public override IdentityError UserLockoutNotEnabled() =>
            Error(nameof(UserLockoutNotEnabled), "El bloqueo no está habilitado para esta cuenta.");

        public override IdentityError UserAlreadyInRole(string role) =>
            Error(nameof(UserAlreadyInRole), $"El usuario ya tiene el rol '{role}'.");

        public override IdentityError UserNotInRole(string role) =>
            Error(nameof(UserNotInRole), $"El usuario no tiene el rol '{role}'.");

        public override IdentityError PasswordTooShort(int length) =>
            Error(nameof(PasswordTooShort), $"La contraseña debe tener al menos {length} caracteres.");

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) =>
            Error(nameof(PasswordRequiresUniqueChars), $"La contraseña debe usar al menos {uniqueChars} caracteres distintos.");

        public override IdentityError PasswordRequiresNonAlphanumeric() =>
            Error(nameof(PasswordRequiresNonAlphanumeric), "La contraseña debe incluir al menos un símbolo.");

        public override IdentityError PasswordRequiresDigit() =>
            Error(nameof(PasswordRequiresDigit), "La contraseña debe incluir al menos un número.");

        public override IdentityError PasswordRequiresLower() =>
            Error(nameof(PasswordRequiresLower), "La contraseña debe incluir al menos una letra minúscula.");

        public override IdentityError PasswordRequiresUpper() =>
            Error(nameof(PasswordRequiresUpper), "La contraseña debe incluir al menos una letra mayúscula.");
    }
}
