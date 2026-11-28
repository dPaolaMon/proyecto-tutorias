// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Los mensajes de validación nativa salen en el idioma del navegador: se reemplazan por español
(function () {
    function mensaje(campo) {
        const v = campo.validity;
        if (v.valueMissing) {
            if (campo.type === 'checkbox') return 'Marca esta casilla para continuar.';
            return campo.tagName === 'SELECT' ? 'Selecciona una opción.' : 'Completa este campo.';
        }
        if (v.typeMismatch) return campo.type === 'email' ? 'Escribe un correo válido.' : 'El formato no es válido.';
        if (v.rangeUnderflow) return `El valor debe ser ${campo.min} o mayor.`;
        if (v.rangeOverflow) return `El valor debe ser ${campo.max} o menor.`;
        if (v.badInput || v.stepMismatch || v.patternMismatch) return 'El valor no es válido.';
        if (v.tooShort) return `Escribe al menos ${campo.minLength} caracteres.`;
        if (v.tooLong) return `Escribe como máximo ${campo.maxLength} caracteres.`;
        return '';
    }

    function limpiar(formulario) {
        Array.from(formulario.elements).forEach(c => c.setCustomValidity && c.setCustomValidity(''));
    }

    document.addEventListener('invalid', e => {
        e.target.setCustomValidity('');
        e.target.setCustomValidity(mensaje(e.target));
    }, true);

    // Se limpia al editar y antes de cada envío para volver a validar desde cero
    ['input', 'change'].forEach(tipo => document.addEventListener(tipo, e => {
        if (e.target.setCustomValidity) e.target.setCustomValidity('');
    }, true));

    document.addEventListener('click', e => {
        const boton = e.target.closest('button, input[type=submit]');
        if (boton && boton.type === 'submit' && boton.form) limpiar(boton.form);
    }, true);
})();
