using System.Globalization;
using System.Windows.Controls;

namespace Ajedrez
{
    /// <summary>
    /// Regla de validación para la etiqueta Board de un pgn.
    /// </summary>
    public class TableroPGNRule : ValidationRule
    {
        #region Método
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (int.TryParse(value.ToString(), out int tablero) && tablero >= 0)
            {
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Debe ingresar un número natural");
        }
        #endregion
    }
}
