using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Ajedrez
{
    /// <summary>
    /// Regla de validación para la etiqueta Time de un pgn.
    /// </summary>
    public class HoraPGNRule : ValidationRule
    {
        #region Método
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Match match = Regex.Match(value.ToString(), @"^(\d{2}):(\d{2}):(\d{2})$");
            if (!match.Success)
            {
                return new ValidationResult(false, "Debe estar en formato hh:mm:ss");
            }
            int horas = int.Parse(match.Groups[1].Value), minutos = int.Parse(match.Groups[2].Value), segundos = int.Parse(match.Groups[3].Value);
            if (horas < 0 || horas > 23)
            {
                return new ValidationResult(false, $"{horas} no es una hora válida, debe estar entre 0 y 23");
            }
            if (minutos < 0 || minutos > 59)
            {
                return new ValidationResult(false, $"{minutos} no es un minuto válido, debe estar entre 0 y 59");
            }
            if (segundos < 0 || segundos > 59)
            {
                return new ValidationResult(false, $"{segundos} no es un segundo válido, debe estar entre 0 y 59");
            }
            return ValidationResult.ValidResult;
        }
        #endregion
    }
}
