namespace Ajedrez
{
    /// <summary>
    /// Representa información sobre los resultados obtenidos al realizar una jugada.
    /// </summary>
    public class ResultadoJugada
    {
        #region Propiedades
        /// <summary>
        /// Obtiene o establece la jugada que se realizó.
        /// </summary>
        public string Jugada { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de partidas en las que se realizó la jugada.
        /// </summary>
        public int Partidas { get; set; }

        /// <summary>
        /// Obtiene o establece el porcentaje de partidas en las que ganó el blanco.
        /// </summary>
        public int GanaBlanco  { get; set; }

        /// <summary>
        /// Obtiene o establece el porcentaje de partidas que se empataron.
        /// </summary>
        public int Tablas { get; set; }

        /// <summary>
        /// Obtiene o establece el porcentaje de partidas que ganó el negro.
        /// </summary>
        public int GanaNegro { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ResultadoJugada"/> con la jugada, la cantidad de partidas y los porcentajes de cada resultado.
        /// </summary>
        /// <param name="jugada">Jugada realizada.</param>
        /// <param name="partidas">Cantidad de partidas en las que ser realizó la jugada.</param>
        /// <param name="ganaBlanco">Porcentaje de las partidas en las que ganó el blanco.</param>
        /// <param name="tablas">Porcentaje de partidas que se empataron.</param>
        /// <param name="ganaNegro">Porcentaje de las partidas en las que ganó el negro.</param>
        public ResultadoJugada(string jugada, int partidas, int ganaBlanco, int tablas, int ganaNegro)
        {
            Jugada = jugada;
            Partidas = partidas;
            GanaBlanco = ganaBlanco;
            Tablas = tablas;
            GanaNegro = ganaNegro;
        }
        #endregion
    }
}
