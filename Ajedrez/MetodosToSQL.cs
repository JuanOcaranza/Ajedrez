namespace Ajedrez
{
    /// <summary>
    /// Proporciona métodos de extensión para ajustar objetos a sentencias SQL.
    /// </summary>
    public static class MetodosToSQL
    {
        #region Métodos
        /// <summary>
        /// Obtiene la cadena entre comillas simples o la cadena "null".
        /// </summary>
        /// <param name="cadena">Cadena que se va a poner entre comillas simples.</param>
        public static string ToSQL(this string cadena) => string.IsNullOrWhiteSpace(cadena) ? "null" : $"'{cadena}'";

        /// <summary>
        /// Obtiene el entero o la cadena "null".
        /// </summary>
        /// <param name="entero">Entero que se va a devolver si no es null.</param>
        public static object ToSQL(this int? entero) => (object)entero ?? "null";

        /// <summary>
        /// Obtiene el decimal o la cadena "null".
        /// </summary>
        /// <param name="real">Decimal que se va a devolver si no es null.</param>
        public static object ToSQL(this decimal? real) => (object)real ?? "null";
        #endregion
    }
}
