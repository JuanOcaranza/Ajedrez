using System;
using System.Text.RegularExpressions;

namespace Ajedrez
{
    /// <summary>
    /// Representa la jugada de las blancas y la de las negras
    /// </summary>
    public class JugadaCompleta
    {
        #region Atributos
        /// <summary>
        /// Posicion anterior a la realización de la jugada.
        /// </summary>
        private readonly Posicion posicionAnterior;

        /// <summary>
        /// Posición resultante tras la jugada de las blancas.
        /// </summary>
        private readonly Posicion posicionIntermedia;

        /// <summary>
        /// Posición resultante tras la jugada de las negras.
        /// </summary>
        private Posicion posicionFinal;

        /// <summary>
        /// Comentario sobre la jugada blanca.
        /// </summary>
        private string comentarioJugadaBlanca = string.Empty;

        /// <summary>
        /// Comentario sobre la jugada negra.
        /// </summary>
        private string comentarioJugadaNegra = string.Empty;

        /// <summary>
        /// Expresión regular para las partes de una media jugada en formato PGN.
        /// </summary>
        private const string patronMediaJugadaPGN = @"((?<Coronacion>([a-h]x)?[a-h][18]=[NBRQ])|(?<CapturaPeon>[a-h]x[a-h][1-8])|(?<MovimientoPieza>[NBRQK][a-h]?[1-8]?x?[a-h][1-8])|(?<Enroque>O-O(-O)?)" +
            @"|(?<MovimientoPeon>[a-h][1-8]))[+#]?(?<Simbolo>\?\?|!!|!\?|\?!|\?|!)?(\s\$(?<NAG>\d{1,3}))?(\s*{(?<Comentario>[^}]*)})?";
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece la jugada realizada por las blancas.
        /// </summary>
        public Jugada JugadaBlanca { get; set; }

        /// <summary>
        /// Obtiene o establece la jugada realizada por las negras.
        /// </summary>
        public Jugada JugadaNegra { get; set; }

        /// <summary>
        /// Obtiene o establece el número de jugada.
        /// </summary>
        public int Numero { get; set; }

        /// <summary>
        /// Obtiene la cantidad de caracteres del string que representa la jugada negra.
        /// </summary>
        public int LongitudJugadaNegra { get; private set; } = 0;

        /// <summary>
        /// Obtiene o establece el comentario sobre la jugada blanca.
        /// </summary>
        public string ComentarioJugadaBlanca
        {
            get => comentarioJugadaBlanca;
            set
            {
                comentarioJugadaBlanca = string.IsNullOrWhiteSpace(value) ? value : $"{{ {value} }} "; // Si no está vacio lo pone entre llaves.
            }
        }
        /// <summary>
        /// Obtiene o establece el comentario sobre la jugada negra.
        /// </summary>
        public string ComentarioJugadaNegra
        {
            get => comentarioJugadaNegra;
            set
            {
                comentarioJugadaNegra = string.IsNullOrWhiteSpace(value) ? value : $"{{ {value} }} "; // Si no está vacio lo pone entre llaves.
            }
        }

        /// <summary>
        /// Obtiene el comentario sobre la jugada blanca sin las llaves.
        /// </summary>
        public string ComentarioJugadaBlancaSinLlaves => ComentarioJugadaBlanca.Length > 5 ? ComentarioJugadaBlanca.Substring(2, ComentarioJugadaBlanca.Length - 5) : string.Empty;
        /// <summary>
        /// Obtiene el comentario sobre la jugada negra sin las llaves.
        /// </summary>
        public string ComentarioJugadaNegraSinLlaves => ComentarioJugadaNegra.Length > 5 ? ComentarioJugadaNegra.Substring(2, ComentarioJugadaNegra.Length - 5) : string.Empty;

        /// <summary>
        /// Obtiene o establece el indicador NAG sobre la jugada blanca.
        /// </summary>
        public IndicadorNAG NAGJugadaBlanca { get; set; } = IndicadorNAG.Nulo;

        /// <summary>
        /// Obtiene o establece el indicador NAG sobre la jugada negra.
        /// </summary>
        public IndicadorNAG NAGJugadaNegra { get; set; } = IndicadorNAG.Nulo;
        #endregion

        #region Indexer
        /// <summary>
        /// Obtiene la posición intermedia o final.
        /// </summary>
        /// <param name="indice">"0" para posición intermedia, "1" para posición final.</param>
        public Posicion this[int indice] => indice == 0 ? posicionIntermedia : posicionFinal;
        #endregion

        #region Constructores
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="JugadaCompleta"/> con la jugada de las blancas.
        /// </summary>
        /// <param name="posicionIncial">Posición anterior a la realización de la jugada.</param>
        /// <param name="jugada">Jugada que se realiza en la posición.</param>
        public JugadaCompleta(Posicion posicionIncial, Jugada jugada)
        {
            posicionAnterior = posicionIncial;
            JugadaBlanca = jugada;
            posicionIntermedia = new Posicion(posicionAnterior, JugadaBlanca);
            Numero = posicionIntermedia.NumeroJugada;
        }

        /// <summary>
        /// Incializa una nueva instancia de la clase <see cref="JugadaCompleta"/> con la jugada de las blancas y la de las negras.
        /// </summary>
        /// <param name="posicionIncial">Posición anterior a la jugada.</param>
        /// <param name="numero">Número de la jugada.</param>
        /// <param name="jugadaBlanca">Cadena con la jugada y blanca, sus indicadores y sus comentarios.</param>
        /// <param name="jugadaNegra">Cadena con la jugada y blanca, sus indicadores y sus comentarios.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public JugadaCompleta(Posicion posicionIncial, int numero, string jugadaBlanca, string jugadaNegra)
        {
            posicionAnterior = posicionIncial;
            Numero = numero;
            Match match = Regex.Match(jugadaBlanca ?? "", patronMediaJugadaPGN); // Obtiene las distintas partes de la jugada.
            if (match.Success) // Si hay jugada blanca.
            {
                if (match.Groups["Coronacion"].Success) // Si la jugada es una coronación.
                {
                    bool esCaptura = match.Groups["Coronacion"].Value.Contains("x"); // Determina si la coronación es una captura para saber en que posición del string están los datos necesarios.
                    JugadaBlanca = new Jugada(match.Groups["Coronacion"].Value[0] + "7", esCaptura ? match.Groups["Coronacion"].Value.Substring(2, 2) : match.Groups["Coronacion"].Value.Substring(0, 2),
                        esCaptura ? (MetodosPieza.CharToPieza(CaracterPieza.Ingles, match.Groups["Coronacion"].Value[5]).ToTipoPromocion() | TipoJugada.Captura) :
                        MetodosPieza.CharToPieza(CaracterPieza.Ingles, match.Groups["Coronacion"].Value[3]).ToTipoPromocion(), Pieza.Peon);
                }
                else if (match.Groups["CapturaPeon"].Success) // Si es una captura realizada por un peón que no coronó.
                {
                    JugadaBlanca = new Jugada(match.Groups["CapturaPeon"].Value[0] + (Convert.ToInt32(match.Groups["CapturaPeon"].Value[3].ToString()) - 1).ToString(),
                        match.Groups["CapturaPeon"].Value.Substring(2, 2), TipoJugada.Captura, Pieza.Peon);
                    if ((posicionAnterior[Pieza.Negra] & (1UL << JugadaBlanca.Destino)) == 0) // Si no hay pieza en el destino, es una captura al paso.
                    {
                        JugadaBlanca.Tipo |= TipoJugada.CapturaAlPaso;
                    }
                }
                else if (match.Groups["MovimientoPieza"].Success) // Si la jugada es un movimiento de pieza.
                {
                    bool esCaptura = match.Groups["MovimientoPieza"].Value.Contains("x"); // Determina si es una captura.
                    bool tieneColumnaOFilaDeOrigen = match.Groups["MovimientoPieza"].Value.Length > (esCaptura ? 4 : 3);
                    int destino = Jugada.CasillaStringToInt(match.Groups["MovimientoPieza"].Value.Substring(1 + (esCaptura ? 1 : 0) + (tieneColumnaOFilaDeOrigen ? 1 : 0), 2));
                    Pieza pieza = MetodosPieza.CharToPieza(CaracterPieza.Ingles, match.Groups["MovimientoPieza"].Value[0]); // Pieza que realiza la jugada.
                    // Asigna a origen el origen de la jugada posible con igual destino y pieza, en caso de haber ambigüedad en el origen compara la fila o columna para obtener el origen correcto.
                    int origen = tieneColumnaOFilaDeOrigen ? posicionAnterior.JugadasPosibles.Find(j => j.Destino == destino && j.Pieza == pieza &&
                        (int.TryParse(match.Groups["MovimientoPieza"].Value.Substring(1, 1), out int fila) ? j.Origen / 8 == fila : j.Origen % 8 ==
                        Jugada.LetraColumnaToInt(match.Groups["MovimientoPieza"].Value[1]))).Origen : posicionAnterior.JugadasPosibles.Find(j => j.Destino == destino && j.Pieza == pieza).Origen;
                    JugadaBlanca = new Jugada(1UL << origen, 1UL << destino, esCaptura ? TipoJugada.Captura : TipoJugada.Normal, pieza);
                }
                else if (match.Groups["Enroque"].Success) // Si la jugada es un enroque.
                {
                    // Agrega el enroque, decide el destino teniendo en cuenta si es enroque largo o corto.
                    JugadaBlanca = new Jugada(16, match.Groups["Enroque"].Value.Length > 3 ? 4UL : 64UL, TipoJugada.Enroque, Pieza.Rey);
                }
                else if (match.Groups["MovimientoPeon"].Success) // Si es un movimiento de peón.
                {
                    // Determina si es un avance doble fijandose si no hay peón en la casilla anterior al destino.
                    bool esAvanceDoble = (posicionAnterior[Pieza.Peon] & (1UL << Jugada.CasillaStringToInt(match.Groups["MovimientoPeon"].Value[0] +
                        (Convert.ToInt32(match.Groups["MovimientoPeon"].Value[1].ToString()) - 1).ToString()))) == 0;
                    JugadaBlanca = new Jugada(match.Groups["MovimientoPeon"].Value[0] + (Convert.ToInt32(match.Groups["MovimientoPeon"].Value[1].ToString()) - (esAvanceDoble ? 2 : 1)).ToString(),
                        match.Groups["MovimientoPeon"].Value, esAvanceDoble ? TipoJugada.AvanceDoble : TipoJugada.Normal, Pieza.Peon); // Agrega la jugada.
                }
                else
                {
                    throw new ArgumentException("La jugada no es válida", nameof(jugadaBlanca));
                }
                // Asigna el NAG a partir de el símbolo o el NAG.
                NAGJugadaBlanca = match.Groups["Simbolo"].Success ? MetodosIndicadorNAG.SimboloToNAG(match.Groups["Simbolo"].Value) :
                    (Enum.TryParse(match.Groups["NAG"]?.Value, out IndicadorNAG indicadorNAG) ? indicadorNAG : IndicadorNAG.Nulo);
                ComentarioJugadaBlanca = match.Groups["Comentario"].Success ? (match.Groups["Comentario"].Value) : string.Empty; // Agrega el comentario si hay alguno.
                posicionIntermedia = new Posicion(posicionAnterior, JugadaBlanca);
            }
            else
            {
                posicionIntermedia = posicionAnterior;
            }
            match = Regex.Match(jugadaNegra ?? "", patronMediaJugadaPGN); // Obtiene las distintas partes de la jguada de las negras.
            if (match.Success) // Si hay jugada negra.
            {
                if (match.Groups["Coronacion"].Success) // Si la jugada es una coronación.
                {
                    bool esCaptura = match.Groups["Coronacion"].Value.Contains("x"); // Determina si la coronación es una captura para saber en que posición del string están los datos necesarios.
                    JugadaNegra = new Jugada(match.Groups["Coronacion"].Value[0] + "2", esCaptura ? match.Groups["Coronacion"].Value.Substring(2, 2) : match.Groups["Coronacion"].Value.Substring(0, 2),
                        esCaptura ? (MetodosPieza.CharToPieza(CaracterPieza.Ingles, match.Groups["Coronacion"].Value[5]).ToTipoPromocion() | TipoJugada.Captura) :
                        MetodosPieza.CharToPieza(CaracterPieza.Ingles, match.Groups["Coronacion"].Value[3]).ToTipoPromocion(), Pieza.Peon);
                }
                else if (match.Groups["CapturaPeon"].Success) // Si es una captura realizada por un peón que no coronó.
                {
                    JugadaNegra = new Jugada(match.Groups["CapturaPeon"].Value[0] + (Convert.ToInt32(match.Groups["CapturaPeon"].Value[3].ToString()) + 1).ToString(),
                        match.Groups["CapturaPeon"].Value.Substring(2, 2), TipoJugada.Captura, Pieza.Peon); // Crea la jugada.
                    if ((posicionIntermedia[Pieza.Blanca] & (1UL << JugadaNegra.Destino)) == 0) // Si no hay pìeza en el destino, es una captura al paso.
                    {
                        JugadaNegra.Tipo |= TipoJugada.CapturaAlPaso;
                    }
                }
                else if (match.Groups["MovimientoPieza"].Success) // Si la jugada es un movimiento de pieza.
                {
                    bool esCaputra = match.Groups["MovimientoPieza"].Value.Contains("x"); // Determina si es una captura.
                    bool tieneColumnaOFilaDeOrigen = match.Groups["MovimientoPieza"].Value.Length > (esCaputra ? 4 : 3);
                    int destino = Jugada.CasillaStringToInt(match.Groups["MovimientoPieza"].Value.Substring(1 + (esCaputra ? 1 : 0) + (tieneColumnaOFilaDeOrigen ? 1 : 0), 2));
                    Pieza pieza = MetodosPieza.CharToPieza(CaracterPieza.Ingles, match.Groups["MovimientoPieza"].Value[0]); // Pieza que realiza la jugada.
                    // Asigna a origen el origen de la jugada posible con igual destino y pieza, en caso de haber ambigüedad en el origen compara la fila o columna para obtener el origen correcto.
                    int origen = tieneColumnaOFilaDeOrigen ? posicionIntermedia.JugadasPosibles.Find(j => j.Destino == destino && j.Pieza == pieza &&
                        (int.TryParse(match.Groups["MovimientoPieza"].Value.Substring(1, 1), out int fila) ? j.Origen / 8 == fila : j.Origen % 8 ==
                        Jugada.LetraColumnaToInt(match.Groups["MovimientoPieza"].Value[1]))).Origen : posicionIntermedia.JugadasPosibles.Find(j => j.Destino == destino && j.Pieza == pieza).Origen;
                    JugadaNegra = new Jugada(1UL << origen, 1UL << destino, esCaputra ? TipoJugada.Captura : TipoJugada.Normal, pieza);
                }
                else if (match.Groups["Enroque"].Success) // Si la jugada es un enroque.
                {
                    // Agrega el enroque, decide el destino teniendo en cuenta si es enroque largo o corto.
                    JugadaNegra = new Jugada(0x1000_0000_0000_0000, match.Groups["Enroque"].Value.Length > 3 ? 0x400_0000_0000_0000UL : 0x4000_0000_0000_0000UL, TipoJugada.Enroque, Pieza.Rey);
                }
                else if (match.Groups["MovimientoPeon"].Success) // Si es un movimiento de peón.
                {
                    // Determina si es un avance doble fijandose si no hay peón en la casilla posterior al destino.
                    bool esAvanceDoble = (posicionIntermedia[Pieza.Peon] & (1UL << Jugada.CasillaStringToInt(match.Groups["MovimientoPeon"].Value[0] +
                        (Convert.ToInt32(match.Groups["MovimientoPeon"].Value[1].ToString()) + 1).ToString()))) == 0;
                    JugadaNegra = new Jugada(match.Groups["MovimientoPeon"].Value[0] + (Convert.ToInt32(match.Groups["MovimientoPeon"].Value[1].ToString()) + (esAvanceDoble ? 2 : 1)).ToString(),
                        match.Groups["MovimientoPeon"].Value, esAvanceDoble ? TipoJugada.AvanceDoble : TipoJugada.Normal, Pieza.Peon); // Agrega la jugada.
                }
                else
                {
                    throw new ArgumentException("La jugada no es válida", nameof(jugadaNegra));
                }
                // Asigna el NAG a partir del símbolo o el NAG.
                NAGJugadaNegra = match.Groups["Simbolo"].Success ? MetodosIndicadorNAG.SimboloToNAG(match.Groups["Simbolo"].Value) :
                    (Enum.TryParse(match.Groups["NAG"]?.Value, out IndicadorNAG indicadorNAG) ? indicadorNAG : IndicadorNAG.Nulo);
                ComentarioJugadaNegra = match.Groups["Comentario"].Success ? (match.Groups["Comentario"].Value) : string.Empty; // Agrega el comentario si hay alguno.
                posicionFinal = new Posicion(posicionIntermedia, JugadaNegra);
            }
        }
        #endregion

        #region Métodos
        public override string ToString() => ToCadena(false);

        /// <summary>
        /// Obtiene la cadena que representa la jugada actual en formato PGN
        /// </summary>
        public string ToPGN() => ToCadena(true);

        /// <summary>
        /// Obtiene una cadena que representa la jugada completa
        /// </summary>
        /// <param name="paraPGN">True si la cadena es para un PGN</param>
        private string ToCadena(bool paraPGN)
        {
            string jugada = Numero.ToString() + ". ";
            if (JugadaBlanca != null)
            {
                jugada += JugadaBlanca.Pieza != Pieza.Peon && posicionAnterior.PuedeJugarMasDeUnaPieza(JugadaBlanca) ?
                    JugadaBlanca.ToString(posicionAnterior.PuedeJugarMasDeUnaPiezaEnMismaColumna(JugadaBlanca)) : JugadaBlanca.ToString();
                if (posicionIntermedia.HayJaqueMate())
                {
                    jugada += "#";
                }
                else if (posicionIntermedia.HayJaque())
                {
                    jugada += "+";
                }
                jugada += (paraPGN ? $" ${(int)NAGJugadaBlanca} " : NAGJugadaBlanca.ToStringSimbolo()) + ComentarioJugadaBlanca;
            }
            else
            {
                jugada += " ... ";
            }
            if (JugadaNegra != null)
            {
                LongitudJugadaNegra = jugada.Length;
                jugada += JugadaNegra.Pieza != Pieza.Peon && posicionIntermedia.PuedeJugarMasDeUnaPieza(JugadaNegra) ?
                    JugadaNegra.ToString(posicionIntermedia.PuedeJugarMasDeUnaPiezaEnMismaColumna(JugadaNegra)) : JugadaNegra.ToString();
                LongitudJugadaNegra = jugada.Length - LongitudJugadaNegra;
                if (posicionFinal.HayJaqueMate())
                {
                    jugada += "#";
                    LongitudJugadaNegra++;
                }
                else if (posicionFinal.HayJaque())
                {
                    jugada += "+";
                    LongitudJugadaNegra++;
                }
                string nagPGN = $" ${(int)NAGJugadaNegra} ";
                jugada += paraPGN ? nagPGN : NAGJugadaNegra.ToStringSimbolo() + ComentarioJugadaNegra;
                LongitudJugadaNegra += (paraPGN ? nagPGN.Length : NAGJugadaNegra.ToStringSimbolo().Length) + ComentarioJugadaNegra.Length;
            }
            return jugada;
        }

        /// <summary>
        /// Agrega la jugada de las negras.
        /// </summary>
        /// <param name="jugada">Jugada realizada por las negras.</param>
        public void CompletarJugada(Jugada jugada)
        {
            JugadaNegra = jugada;
            posicionFinal = new Posicion(posicionIntermedia, JugadaNegra);
        }

        /// <summary>
        /// Agrega la jugada de las negras.
        /// </summary>
        /// <param name="jugada">Cadena que representa la jugada de las negras (notacion algebraica en inglés).</param>
        public void CompletarJugada(string jugada)
        {
            JugadaNegra = posicionIntermedia.JugadasPosibles.Find(j => j.ToStringEnIngles(posicionIntermedia) == jugada);
            posicionFinal = new Posicion(posicionIntermedia, JugadaNegra);
        }

        /// <summary>
        /// Deshace la jugada de las negras.
        /// </summary>
        public void DeshacerJugada()
        {
            JugadaNegra = null;
            posicionFinal = null;
        }

        /// <summary>
        /// Determina si se cumple una condicion en <see cref="posicionIntermedia"/> o en <see cref="posicionFinal"/>.
        /// </summary>
        /// <param name="condicion">Condición que se tiene que cumplir.</param>
        public bool Cumple(Func<Posicion, bool> condicion) => (posicionIntermedia != null && condicion.Invoke(posicionIntermedia)) || (posicionFinal != null && condicion.Invoke(posicionFinal));
        #endregion
    }
}
