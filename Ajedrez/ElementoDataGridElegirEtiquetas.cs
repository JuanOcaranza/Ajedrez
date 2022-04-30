namespace Ajedrez
{
    /// <summary>
    /// Representa un elemento para <see cref="ElegirEtiquetas.dataGrid"/>.
    /// </summary>
    public class ElementoDataGridElegirEtiquetas
    {
        #region Propiedades
        /// <summary>
        /// Obtiene el nombre de la etiqueta pgn.
        /// </summary>
        public string Etiqueta { get; }

        /// <summary>
        /// Obtiene la descripción de la etiqueta pgn.
        /// </summary>
        public string Descripcion { get; }

        /// <summary>
        /// Obtiene o estable un valor que indica si se debe incluir la etiqueta en el archivo pgn.
        /// </summary>
        public bool Incluir { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ElementoDataGridElegirEtiquetas"/> con el nombre y la descripción.
        /// </summary>
        /// <param name="etiqueta">Nombre de la etiqueta pgn.</param>
        /// <param name="descripcion">Descripción de la etiqueta.</param>
        public ElementoDataGridElegirEtiquetas(string etiqueta, string descripcion)
        {
            Etiqueta = etiqueta;
            Descripcion = descripcion;
            Incluir = false; // No se incluirá la etiqueta a menos que el usuario seleccione lo contrario.
        }
        #endregion
    }
}
