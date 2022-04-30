using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Ajedrez
{
    /// <summary>
    /// Regla de validación para la etiqueta TimeControl de un pgn
    /// </summary>
    public class TiempoPGNRule : ValidationRule
    {
        #region Atributos
        /// <summary>
        /// Expresión regular para validar la etiqueta TimeControl de un pgn.
        /// </summary>
        private const string patronTimeControlPGN = @"(?<desconocido>^\?$)|(?<ilimitado>^-$)|(?<muerteSubita>^(?<parcialAntesDeMuerteSubita>\d{1,}\/\d{1,}:)?\d{1,}$)|(?<incremental>^" +
            @"(?<parcialAntesDeIncremental>\d{1,}\/\d{1,}:)?\d{1,}\+\d{1,}$)|(?<arena>^(?<parcialAntesDeArena>\d{1,}\/\d{1,}:)?\*\d{1,}$)";
        #endregion

        #region Propiedad
        /// <summary>
        /// Obtiene la expresión regular para validar la etiqueta TimeControl de un pgn.
        /// </summary>
        public static string PatronTimeControlPGN => patronTimeControlPGN;
        #endregion

        #region Método
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Match match = Regex.Match(value.ToString(), patronTimeControlPGN);
            if (!match.Success)
            {
                return new ValidationResult(false, "Valor inválido consulte el manual de usuario para más información");
            }
            return ValidationResult.ValidResult;
        }
        #endregion
    }
}
