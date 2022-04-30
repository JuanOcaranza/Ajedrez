using Ajedrez.Properties;
using System;

namespace Ajedrez
{
    /// <summary>
    /// Representa una jugada de ajedrez
    /// </summary>
    public class Jugada
    {
        #region Propiedades
        /// <summary>
        /// Obtiene o establece el número de la casilla origen.
        /// </summary>
        /// <remarks>Entre 0 y 63</remarks>
        public int Origen { get; set; }

        /// <summary>
        /// Obtiene o establece el número de la casilla destino.
        /// </summary>
        /// /// <remarks>Entre 0 y 63</remarks>
        public int Destino { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de jugada.
        /// </summary>
        public TipoJugada Tipo { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de pieza que realiza la jugada.
        /// </summary>
        public Pieza Pieza { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de caracteres usados para representar las piezas.
        /// </summary>
        public static CaracterPieza CaracterPieza { get; set; } = (CaracterPieza)Settings.Default.CaracteresPieza;
        #endregion

        #region Constructores
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Jugada"/> con el origen y el destino.
        /// </summary>
        /// <param name="origen">Cadena que representa la casilla de origen.</param>
        /// <param name="destino">Cadena que representa la casilla de destino.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public Jugada(string origen, string destino)
        {
            Origen = CasillaStringToInt(origen);
            Destino = CasillaStringToInt(destino);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Jugada"/> con el origen, el destino, el tipo y la pieza
        /// </summary>
        /// <param name="origen">Bitboard que representa la casilla de origen.</param>
        /// <param name="destino">Bitboard que representa la casilla de destino.</param>
        /// <param name="tipo">Tipo de jugada.</param>
        /// <param name="pieza">Pieza que se mueve en la jugada.</param>
        public Jugada(ulong origen, ulong destino, TipoJugada tipo, Pieza pieza)
        {
            Origen = Bits.IndiceBitMenosSignificativo(origen);
            Destino = Bits.IndiceBitMenosSignificativo(destino);
            Tipo = tipo;
            Pieza = pieza;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Jugada"/> a partir de otra jugada y un tipo.
        /// </summary>
        /// <param name="jugada">Jugada con el orgien y destinos de la nueva jugada.</param>
        /// <param name="tipo">Tipo de la nueva jugada.</param>
        public Jugada(Jugada jugada, TipoJugada tipo)
        {
            Origen = jugada.Origen;
            Destino = jugada.Destino;
            Tipo = tipo;
            Pieza = jugada.Pieza;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Jugada"/> con el origen, el destino, el tipo y la pieza.
        /// </summary>
        /// <param name="origen">Cadena que representa la casilla de origen.</param>
        /// <param name="destino">Cadena que representa la casilla de destino.</param>
        /// <param name="tipo">Tipo de jugada.</param>
        /// <param name="pieza">Pieza que se mueve en la jugada.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public Jugada(string origen, string destino, TipoJugada tipo, Pieza pieza) : this(origen, destino)
        {
            Tipo = tipo;
            Pieza = pieza;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Jugada"/> a partir de dos posiciones.
        /// </summary>
        /// <param name="posicionAnterior">Posición en la que se realizó la jugada.</param>
        /// <param name="posicionPosterior">Posición que quedó luego de realizarse la jugada.</param>
        public Jugada(Posicion posicionAnterior, Posicion posicionPosterior)
        {
            Pieza jugador = posicionAnterior.Turno.ToPieza(), contrario = posicionAnterior.Turno.ToPiezaContraria();
            if (((posicionAnterior[jugador] & posicionAnterior[Pieza.Rey]) != (posicionPosterior[jugador] & posicionPosterior[Pieza.Rey])) && ((posicionAnterior[jugador] & posicionAnterior[Pieza.Torre]) !=
                (posicionPosterior[jugador] & posicionPosterior[Pieza.Torre]))) // Si la jugada fue un enroque.
            {
                Tipo = TipoJugada.Enroque;
                Pieza = Pieza.Rey;
                if (posicionAnterior.Turno == Equipo.Blanco)
                {
                    Origen = 4;
                    Destino = (posicionPosterior[jugador] & posicionPosterior[Pieza.Rey]) == 4 ? 0 : 7; // Según si es enroque largo o corto.
                }
                else
                {
                    Origen = 60;
                    Destino = (posicionPosterior[jugador] & posicionPosterior[Pieza.Rey]) == 0x400_0000_0000_0000 ? 56 : 63; // Según si es enroque largo o corto.
                }
            }
            else
            {
                ulong distinto = posicionAnterior[jugador] ^ posicionPosterior[jugador];
                // Distino tiene en 1 el bit de la casilla de origen y el de la destino.
                int origenODestino = Bits.IndiceBitMasSignificativo(distinto);
                int destinoOOrigen = Bits.IndiceBitMenosSignificativo(distinto);
                Copy(posicionAnterior.JugadasPosibles.Find(j => (j.Origen == origenODestino && j.Destino == destinoOOrigen) || (j.Origen == destinoOOrigen && j.Destino == origenODestino)));
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Obtiene el número que representa una columna.
        /// </summary>
        /// <param name="letra">Letra entre a y b que representa una columna.</param>
        /// <returns>Número entre 0 y 7 que representa la columna representada por el parámetro.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int LetraColumnaToInt(char letra)
        {
            switch (letra)
            {
                case 'a':
                    return 0;
                case 'b':
                    return 1;
                case 'c':
                    return 2;
                case 'd':
                    return 3;
                case 'e':
                    return 4;
                case 'f':
                    return 5;
                case 'g':
                    return 6;
                case 'h':
                    return 7;
                default:
                    throw new ArgumentOutOfRangeException(nameof(letra), "La letra debe estar entre a y h");
            }
        }

        /// <summary>
        /// Obtiene el número que representa una casilla.
        /// </summary>
        /// <param name="casilla">Cadena de una letra que representa la columna y un número que representa la fila.</param>
        /// <returns>Número entre 0 y 63 que representa la casilla.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static int CasillaStringToInt(string casilla) => 8 * (int.Parse(casilla[1].ToString()) - 1) + LetraColumnaToInt(casilla[0]);

        public override bool Equals(object obj) => obj is Jugada jugada && Origen == jugada.Origen && Destino == jugada.Destino;

        public override int GetHashCode() => Origen * 64 + Destino;

        public override string ToString()
        {
            if ((Tipo & TipoJugada.Enroque) != 0) // Si es un enroque.
            {
                return Destino % 8 < 4 ? "O-O-O" : "O-O"; // Según si es enroque largo o corto.
            }
            string caracterPieza = Pieza != Pieza.Peon ? Pieza.ToChar(CaracterPieza).ToString() : string.Empty;
            string captura = (Tipo & TipoJugada.Captura) != 0 ? (Pieza == Pieza.Peon ? ((char)('a' + Origen % 8)).ToString() : string.Empty) + "x" : string.Empty;
            string destino = CasillaNumeroToString(Destino);
            string coronacion = string.Empty;
            if ((Tipo & TipoJugada.PromocionDama) != 0) // Si corona dama.
            {
                coronacion = "=" + Pieza.Dama.ToChar(CaracterPieza);
            }
            else if ((Tipo & TipoJugada.PromocionTorre) != 0) // Si corona torre.
            {
                coronacion = "=" + Pieza.Torre.ToChar(CaracterPieza);
            }
            else if ((Tipo & TipoJugada.PromocionAlfil) != 0) // Si corona alfil.
            {
                coronacion = "=" + Pieza.Alfil.ToChar(CaracterPieza);
            }
            else if ((Tipo & TipoJugada.PromocionCaballo) != 0) // Si corona caballo.
            {
                coronacion = "=" + Pieza.Caballo.ToChar(CaracterPieza);
            }
            return caracterPieza + captura + destino + coronacion;
        }

        /// <summary>
        /// Obtiene la cadena que representa una casilla del tablero de ajedrez.
        /// </summary>
        /// <param name="casilla">Número entre 0 y 63 que representa una casilla.</param>
        /// <returns>Cadena formada por una letra y un número que representa la casilla.</returns>
        public static string CasillaNumeroToString(int casilla) => ((char)('a' + casilla % 8)).ToString() + (casilla / 8 + 1).ToString();

        /// <summary>
        /// Devuelve una cadena que representa la jugada actual considerando que más de una pieza del mismo tipo pueden ir al destino.
        /// </summary>
        /// <param name="mismaColumna">Si las piezas que pueden ir al mismo destino están en la misma columna.</param>
        public string ToString(bool mismaColumna) => Pieza.ToChar(CaracterPieza).ToString() + (mismaColumna ? (Origen / 8 + 1).ToString() : ((char)('a' + Origen % 8)).ToString()) +
            ((Tipo & TipoJugada.Captura) != 0 ? "x" : string.Empty) + CasillaNumeroToString(Destino);

        /// <summary>
        /// Devuelve una cadena que representa la jugada acutal de acuerdo al protocolo UCI.
        /// </summary>
        public string ToUCI()
        {
            string cadenaUCI = string.Empty;
            if ((Tipo & TipoJugada.Enroque) != 0)
            {
                // Si el enroque tuviera como destino dónde estaba la torre debe quedar como si tuviera la casilla donde va el rey.
                switch (Destino)
                {
                    case 0: // a1.
                    case 2: // c1.
                        cadenaUCI = "e1c1";
                        break;
                    case 6: // g1.
                    case 7: // h1.
                        cadenaUCI = "e1g1";
                        break;
                    case 56: // a8.
                    case 58: // c8.
                        cadenaUCI = "e8c8";
                        break;
                    case 62: // g8.
                    case 63: // h8.
                        cadenaUCI = "e8g8";
                        break;
                }
            }
            else
            {
                cadenaUCI = CasillaNumeroToString(Origen) + CasillaNumeroToString(Destino);
                if ((Tipo & TipoJugada.PromocionDama) != 0) // Si corona dama.
                {
                    cadenaUCI += "q";
                }
                else if ((Tipo & TipoJugada.PromocionTorre) != 0) // Si corona torre.
                {
                    cadenaUCI += "r";
                }
                else if ((Tipo & TipoJugada.PromocionAlfil) != 0) // Si corona alfil.
                {
                    cadenaUCI += "b";
                }
                else if ((Tipo & TipoJugada.PromocionCaballo) != 0) // Si corona caballo.
                {
                    cadenaUCI += "n";
                }
            }
            return cadenaUCI;
        }

        /// <summary>
        /// Copia a esta jugada las propiedades de otra.
        /// </summary>
        /// <param name="jugada">Jugada de la que se van a copiar las propiedaes.</param>
        public void Copy(Jugada jugada)
        {
            Origen = jugada.Origen;
            Destino = jugada.Destino;
            Tipo = jugada.Tipo;
            Pieza = jugada.Pieza;
        }

        /// <summary>
        /// Devuelve una cadena que representa al objecto acutal.
        /// </summary>
        /// <returns>Cadena que representa al objecto acutal en notación algebraica en ingles</returns>
        /// <param name="posicion">Posición en la que se realiza la jugada.</param>
        public string ToStringEnIngles(Posicion posicion)
        {
            CaracterPieza = CaracterPieza.Ingles;
            string respuesta = ToString(posicion);
            CaracterPieza = (CaracterPieza)Settings.Default.CaracteresPieza;
            return respuesta;
        }

        /// <summary>
        /// Devuelve una cadena que representa al objecto acutal.
        /// </summary>
        /// <param name="posicion">Posición en la que se realiza la jugada.</param>
        public string ToString(Posicion posicion) => posicion == null || !posicion.PuedeJugarMasDeUnaPieza(this) ? ToString() : ToString(posicion.PuedeJugarMasDeUnaPiezaEnMismaColumna(this));
        #endregion
    }
}
