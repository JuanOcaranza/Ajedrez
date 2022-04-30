using System.Globalization;
using System.Windows.Controls;

namespace Ajedrez
{
    /// <summary>
    /// Regla para comprobar la validez de un elo-
    /// </summary>
    public class EloPGNRule : ValidationRule
    {
        #region Método
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value.ToString() == "-" || (int.TryParse(value.ToString(), out int elo) && elo >= 0))
            {
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Debe ingresar un número entero mayor a 0 o '-' si el jugador no tiene elo");
        }
        #endregion
    }
}
