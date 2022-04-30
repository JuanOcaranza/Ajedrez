namespace Ajedrez
{
    /// <summary>
    /// Especifica las etiquetas que puede incluir un archivo PGN
    /// </summary>
    public enum EtiquetaPGN
    {
        /// <summary>
        /// Evento en el que se jugó la partida.
        /// </summary>
        Event,

        /// <summary>
        /// Lugar en el que se jugó la partida.
        /// </summary>
        Site,

        /// <summary>
        /// Fecha en la que empezó la partida.
        /// </summary>
        Date,

        /// <summary>
        /// Ronda del evento en la que se jugó la partida.
        /// </summary>
        Round,

        /// <summary>
        /// Nombre de jugador de piezas blancas.
        /// </summary>
        White,

        /// <summary>
        /// Nombre de jugador de piezas negras.
        /// </summary>
        Black,

        /// <summary>
        /// Resultado de la partida.
        /// </summary>
        Result,

        /// <summary>
        /// Título FIDE del jugador de piezas blancas.
        /// </summary>
        WhiteTitle,

        /// <summary>
        /// Título FIDE del jugador de piezas negras.
        /// </summary>
        BlackTitle,

        /// <summary>
        /// Elo FIDE del jugador de piezas blancas.
        /// </summary>
        WhiteElo,

        /// <summary>
        /// Elo FIDE del jugador de piezas negras.
        /// </summary>
        BlackElo,

        /// <summary>
        /// E-mail o dirección de red del jugador de piezas blancas.
        /// </summary>
        WhiteNA,

        /// <summary>
        /// E-mail o dirección de red del jugador de piezas negras.
        /// </summary>
        BlackNA,

        /// <summary>
        /// Tipo(humano o programa) del jugador de piezas blancas.
        /// </summary>
        WhiteType,

        /// <summary>
        /// Tipo(humano o programa) del jugador de piezas negras.
        /// </summary>
        BlackType,

        /// <summary>
        /// Fecha de inicio del evento.
        /// </summary>
        EventDate,

        /// <summary>
        /// Sección de juego del torneo (Ej: Abierto o Reservado).
        /// </summary>
        Section,

        /// <summary>
        /// Etapa del torneo.
        /// </summary>
        Stage,

        /// <summary>
        /// Número de tablero en el que se jugó la partida en un torneo por equipo o en una exhibición simultánea.
        /// </summary>
        Board,

        /// <summary>
        /// Nombre de la apertura.
        /// </summary>
        Opening,

        /// <summary>
        /// Nombre de la variante de la apertura.
        /// </summary>
        Variation,

        /// <summary>
        /// Nombre de la subvariante de la apertura.
        /// </summary>
        SubVariation,

        /// <summary>
        /// Designación de la apertura en _Encyclopedia of Chess Openings_.
        /// </summary>
        ECO,

        /// <summary>
        /// Designación de la apertura en la base de datos _New in Chess_.
        /// </summary>
        NIC,

        /// <summary>
        /// Hora local del comienzo de la partida.
        /// </summary>
        Time,

        /// <summary>
        /// Hora UTC del comienzo de la partida.
        /// </summary>
        UTCTime,

        /// <summary>
        /// Fecha UTC del comienzo de la partida.
        /// </summary>
        UTCDate,

        /// <summary>
        /// Control de tiempo usado en la partida.
        /// </summary>
        TimeControl,

        /// <summary>
        /// Si la partida empezó en la posición habitual o en la dada en <see cref="FEN"/>.
        /// </summary>
        SetUp,

        /// <summary>
        /// Posición inicial de la partida.
        /// </summary>
        FEN,

        /// <summary>
        /// Razón por la que terminó la partida.
        /// </summary>
        Termination,

        /// <summary>
        /// Nombre de la persona que anotó la partida.
        /// </summary>
        Annotator,

        /// <summary>
        /// Modo en el que se jugó la partida (Ej: Sobre el tablero, Correo en papel, Correo electrónico).
        /// </summary>
        Mode,

        /// <summary>
        /// Cantidad de medias jugadas.
        /// </summary>
        PlyCount
    }
}
