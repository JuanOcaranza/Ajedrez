namespace Ajedrez
{
    /// <summary>
    /// Especifica los color que representa cada equipo de una partida de ajedrez.
    /// </summary>
    public enum Equipo
    {
        Blanco = -1,
        Negro = 1
    }

    /// <summary>
    /// Proporciona métodos de extensión para <see cref="Equipo"/>.
    /// </summary>
    public static class MetodosEquipo
    {
        /// <summary>
        /// Obtiene el color de pieza que corresponde al equipo.
        /// </summary>
        /// <param name="equipo">Equipo del que se va a obtener la pieza.</param>
        public static Pieza ToPieza(this Equipo equipo) => equipo == Equipo.Blanco ? Pieza.Blanca : Pieza.Negra;

        /// <summary>
        /// Obtiene el color de pieza que corresponde al equipo contrario.
        /// </summary>
        /// <param name="equipo">Equipo contrario al que se va a obtener la pieza.</param>
        public static Pieza ToPiezaContraria(this Equipo equipo) => equipo == Equipo.Blanco ? Pieza.Negra : Pieza.Blanca;
    }
}
