using System.Globalization;
using System.Windows.Controls;

namespace Ajedrez
{
    /// <summary>
    /// Regla de validación para ingresar enteros mayores a 0.
    /// </summary>
    public class MayorACeroRule : ValidationRule
    {
        #region Método
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) => int.TryParse(value.ToString(), out int numero) && numero > 0 ? ValidationResult.ValidResult : new
            ValidationResult(false, "Debe ingresar un número entero mayor a 0");
        #endregion
    }
}
