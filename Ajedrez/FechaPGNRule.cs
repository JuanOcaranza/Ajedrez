using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Ajedrez
{
    /// <summary>
    /// Regla de validación para la etiqueta Date de un pgn.
    /// </summary>
    public class FechaPGNRule : ValidationRule
    {
        #region Método
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Match match = Regex.Match(value.ToString(), @"^(\?{4}|\d{4})\.(\?{2}|\d{2})\.(\?{2}|\d{2})$");
            if (!match.Success)
            {
                return new ValidationResult(false, "Debe estar en formato aaaa.mm.dd (remplace por '?' los campos que desconozca)");
            }
            bool hayAño = int.TryParse(match.Groups[1].Value, out int año), hayMes = int.TryParse(match.Groups[2].Value, out int mes), hayDia = int.TryParse(match.Groups[3].Value, out int dia);
            if (hayMes)
            {
                if (mes > 12 || mes < 1)
                {
                    return new ValidationResult(false, $"No existe el mes {mes}");
                }
                if (hayDia && (dia < 1 || dia > DateTime.DaysInMonth(hayAño ? año : 2020, mes)))
                {
                    return new ValidationResult(false, $"No existe el día {dia} en el mes {mes}{(hayAño ? $" del {año}" : "")}");
                }
            }
            if (hayDia && (dia > 31 || dia < 1))
            {
                return new ValidationResult(false, $"No existe el día {dia}");
            }
            return ValidationResult.ValidResult;
        }
        #endregion
    }
}
