using Ajedrez.Properties;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ajedrez
{
    /// <summary>
    /// Representa una posición de ajedrez
    /// </summary>
    public class Posicion
    {
        #region Atributos
        /// <summary>
        /// Bitboards que representan la ubicación de las piezas de un tipo o color.
        /// </summary>
        /// <remarks>Ordenado con a1 = 0 y  h1 = 7.</remarks>
        private readonly ulong[] piezas = new ulong[8];

        /// <summary>
        /// Bitboard que representa las posible capturas al paso.
        /// </summary>
        /// <remarks>Ordenado con a1 = 0 y  h1 = 7.</remarks>
        private ulong capturasAlPaso;

        /// <summary>
        /// Bitboard que representa las casillas vacias.
        /// </summary>
        /// <remarks>Ordenado con a1 = 0 y  h1 = 7.</remarks>
        private ulong casillaVacias;

        /// <summary>
        /// Bitboard que representa las casillas ocupadas.
        /// </summary>
        /// <remarks>Ordenado con a1 = 0 y  h1 = 7.</remarks>
        private ulong casillasOcupadas;

        /// <summary>
        /// Biboards de ataques que realiza cada equipo, separados por pieza.
        /// </summary>
        /// <remarks>Ordenados con a1 = 0 y  h1 = 7.</remarks>
        private Dictionary<Equipo, Dictionary<Pieza, ulong>> ataques;

        /// <summary>
        /// Bitboard que representa las columnas a, b, c y d.
        /// </summary>
        /// <remarks>Ordenado con a1 = 0 y h1 = 7.</ remarks >
        private const ulong flancoDama = 0xF0F_0F0F_0F0F_0F0F;

        /// <summary>
        /// Bitboard que representa las columnas e, f, g y h.
        /// </summary>
        /// <remarks>Ordenado con a1 = 0 y  h1 = 7.</remarks>
        private const ulong flancoRey = 0xF0F0_F0F0_F0F0_F0F0;

        /// <summary>
        /// Bitboards que representan cada fila.
        /// </summary>
        /// <remarks>Ordenados desde 1 hasta 8.</remarks>
        private static readonly ulong[] fila = { 255, 0xFF00, 0xFF_0000, 0xFF00_0000, 0xFF_0000_0000, 0xFF00_0000_0000, 0xFF_0000_0000_0000, 0xFF00_0000_0000_0000 };

        /// <summary>
        /// Bitboards que representan cada columna.
        /// </summary>
        /// <remarks>Ordenados desde a hasta h.</remarks>
        private static readonly ulong[] columna = { 0x101_0101_0101_0101, 0x202_0202_0202_0202, 0x404_0404_0404_0404, 0x808_0808_0808_0808, 0x1010_1010_1010_1010, 0x2020_2020_2020_2020,
            0x4040_4040_4040_4040, 0x8080_8080_8080_8080};

        /// <summary>
        /// Bitboards que representan las casillas que atacan el peón, el caballo y el rey desde cada posición.
        /// </summary>
        /// <remarks>Peón blanco 0, peón negro 1, caballo 2, rey 3.</remarks>
        private static readonly ulong[,] destinos = new ulong[4, 64];

        /// <summary>
        /// Bitboards que representan las casillas que se encuentran en cada dirección de cada casilla.
        /// </summary>
        /// <remarks>Norte 0, Nordeste 1, Este 2, Sudeste 3, Sur 4, Sudoeste 5, Oeste 6, Noroeste 7.</remarks>
        private static readonly ulong[,] casillasEnDireccion = new ulong[8, 64];

        /// <summary>
        /// Expresión regular para validar un FEN.
        /// </summary>
        private const string patronFEN = @"^(?<Fila8>[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila7>[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila6>[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila5>[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila4>" +
                @"[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila3>[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila2>[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila1>[pnbrqkPNBRQK1-8]{1,8})\s(?<Turno>b|w)\s(?<Enroque>-|K?Q?k?q|K?Q?k|K?Q|K)\s" +
                @"(?<AlPaso>-|[a-h]3|[a-h]6)\s(?<MediaJugada>\d{1,2})\s(?<Jugada>\d+)\s*$";

        /// <summary>
        /// Expresión regular para validar la descripción de la posición.
        /// </summary>
        private const string patronDescripcionPosicion = @"^(?<Fila8>[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila7>[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila6>[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila5>[pnbrqkPNBRQK1-8]{1,8})\/" +
            @"(?<Fila4>[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila3>[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila2>[pnbrqkPNBRQK1-8]{1,8})\/(?<Fila1>[pnbrqkPNBRQK1-8]{1,8})\s(?<Turno>b|w)\s(?<Enroque>-|K?Q?k?q|K?Q?k|K?Q|K)\s" +
            @"(?<AlPaso>-|[a-h]3|[a-h]6)$";
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece el equipo al que le toca mover
        /// </summary>
        public Equipo Turno { get; set; }

        /// <summary>
        /// Obtiene o establece el listado de todas las jugadas posibles en la posición.
        /// </summary>
        public List<Jugada> JugadasPosibles { get; set; }

        /// <summary>
        /// Obtiene o establece el número de jugada completa.
        /// </summary>
        public int NumeroJugada { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de medias jugadas sin mover peón ni capturar.
        /// </summary>
        public int JugadasSinMoverPeonNiCapturar { get; set; }

        /// <summary>
        /// Obtiene o establece los enroques posibles.
        /// </summary>
        /// <remarks>No tiene en cuenta limitaciones temporales.</remarks>
        public Enroque EnroquesPosibles { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la posición.
        /// </summary>
        /// <remarks>similar al FEN pero sin las cantidades de jugadas.</remarks>
        public string Descripcion { get; private set; }
        #endregion

        #region Indexer
        /// <summary>
        /// Obtiene o establece el bitboard que representa la posicion de una pieza o color de piezas.
        /// </summary>
        /// <param name="pieza">Tipo de pieza o color.</param>
        /// <remarks>Ordenado con a1 = 0 y h1 = 7.</remarks>
        public ulong this[Pieza pieza]
        {
            get => piezas[(int)pieza];
            set => piezas[(int)pieza] = value;
        }
        #endregion

        #region Constructores
        /// <summary>
        /// Inicializa los atributos estáticos.
        /// </summary>
        static Posicion()
        {
            ulong destinosCaballoC3 = 0xA_1100_110A; // Bitboard que representa las casillas a las que se puede mover un caballo en c3.
            ulong destinosReyC3 = 0xE0A_0E00; // Bitboard que representa las casillas a las que se puede mover un rey en c3.
            for (int i = 0; i < 64; i++) // Para cada casilla
            {
                if (i < 18) // la casilla c3 es el bit 18 (contando desde 0), según el valor de i los bits de destino se desplazan a un lado o al otro.
                {
                    destinos[2, i] = destinosCaballoC3 >> (18 - i);
                    destinos[3, i] = destinosReyC3 >> (18 - i);
                }
                else
                {
                    destinos[2, i] = destinosCaballoC3 << (i - 18);
                    destinos[3, i] = destinosReyC3 << (i - 18);
                }
                // Si se está en las primeras o últimas columnas al desplazar los bits aparecen valores en las columnas del otro lado, estos valores no son jugadas posibles, el rey y el caballo tienen
                // menos jugadas posibles si están en los bordes, entoces se aplica un AND con el lado en el que está la pieza para que no se consideren los valores del otro lado.
                if (i % 8 < 2)
                {
                    destinos[2, i] &= flancoDama;
                    destinos[3, i] &= flancoDama;
                }
                else if (i % 8 > 5)
                {
                    destinos[2, i] &= flancoRey;
                    destinos[3, i] &= flancoRey;
                }
                if (i % 8 != 0) // Si no es la columna a.
                {
                    destinos[0, i] |= i / 8 != 7 ? 1UL << (i + 7) : 0; // Los peones blancos pueden capturar si no están en la última fila.
                    destinos[1, i] |= i / 8 != 0 ? 1UL << (i - 9) : 0; // Los peones negros pueden capturar si no están en la primera fila.
                }
                if (i % 8 != 7) // Si no es la columna h.
                {
                    destinos[0, i] |= i / 8 != 7 ? 1UL << (i + 9) : 0; // Captura hacia el otro lado del peón blanco.
                    destinos[1, i] |= i / 8 != 0 ? 1UL << (i - 7) : 0; // Captura hacia el otro lado del peón negro.
                }
                casillasEnDireccion[0, i] = 0x101_0101_0101_0100UL << i; // Norte.
                casillasEnDireccion[1, i] = MoverEste(0x8040_2010_0804_0200, i % 8) << (i / 8 * 8); // Nordeste.
                casillasEnDireccion[2, i] = 2 * ((1UL << (i | 7)) - (1UL << i)); // Este.
                casillasEnDireccion[3, i] = MoverEste(0x2_0408_1020_4080, i % 8) >> ((7 - i / 8) * 8); // Sudeste.
                casillasEnDireccion[4, i] = 0x80_8080_8080_8080UL >> (63 - i); // Sur.
                casillasEnDireccion[5, i] = MoverOeste(0x40_2010_0804_0201, 7 - i % 8) >> ((7 - i / 8) * 8); // Sudoeste.
                casillasEnDireccion[6, i] = (1UL << i) - (1UL << (i & 56)); // Oeste.
                casillasEnDireccion[7, i] = MoverOeste(0x102_0408_1020_4000, 7 - i % 8) << (i / 8 * 8); // Noroeste.
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Posicion"/>.
        /// </summary>
        public Posicion() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Posicion"/> a partir de un FEN.
        /// </summary>
        /// <param name="FEN">FEN que describe la posición.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public Posicion(string FEN)
        {
            Match match = Regex.Match(FEN, patronFEN);
            if (match.Success)
            {
                Descripcion = $"{match.Groups["Fila8"]}/{match.Groups["Fila7"]}/{match.Groups["Fila6"]}/{match.Groups["Fila5"]}/{match.Groups["Fila4"]}/{match.Groups["Fila3"]}/{match.Groups["Fila2"]}/" +
                    $"{match.Groups["Fila1"]} {match.Groups["Turno"]} {match.Groups["Enroque"]} {match.Groups["AlPaso"]}";
                string[] FENFila = new string[8];
                for (int i = 0; i < 8; i++) // Comprueba la suma en cada fila y las agrega a un arreglo.
                {
                    int suma = 0;
                    foreach (char item in match.Groups["Fila" + (i + 1)].Value)
                    {
                        // Cada caracter puede ser una letra representando una pieza o un número representando una cantidad de casillas vacias, la suma de la catidad de letras y el valor de cada número
                        // debe ser 8 porque cada fila tiene 8 columnas.
                        if (int.TryParse(item.ToString(), out int numero))
                        {
                            suma += numero;
                        }
                        else
                        {
                            suma++;
                        }
                    }
                    if (suma == 8)
                    {
                        FENFila[i] = match.Groups["Fila" + (i + 1)].Value;
                    }
                    else
                    {
                        throw new ArgumentException("El FEN no es valido", nameof(FEN));
                    }
                }
                string FENEnroque = match.Groups["Enroque"].Value;
                JugadasSinMoverPeonNiCapturar = int.Parse(match.Groups["MediaJugada"].Value);
                NumeroJugada = int.Parse(match.Groups["Jugada"].Value);
                for (int i = 0; i < 8; i++)
                {
                    int k = 0; // Aumenta los desplazamientos para no poner piezas en las casillas que deben estar vacias y evita que se acceda a posiciones de FENFila que no existen cuando uno de los
                               // string tiene longitud menor a 8.
                    for (int j = 0; j + k < 8; j++)
                    {
                        switch (FENFila[i][j])
                        {
                            case 'P':
                                this[Pieza.Peon] |= 1UL << (i * 8 + j + k);
                                this[Pieza.Blanca] |= 1UL << (i * 8 + j + k);
                                break;
                            case 'N':
                                this[Pieza.Caballo] |= 1UL << (i * 8 + j + k);
                                this[Pieza.Blanca] |= 1UL << (i * 8 + j + k);
                                break;
                            case 'B':
                                this[Pieza.Alfil] |= 1UL << (i * 8 + j + k);
                                this[Pieza.Blanca] |= 1UL << (i * 8 + j + k);
                                break;
                            case 'R':
                                this[Pieza.Torre] |= 1UL << (i * 8 + j + k);
                                this[Pieza.Blanca] |= 1UL << (i * 8 + j + k);
                                break;
                            case 'Q':
                                this[Pieza.Dama] |= 1UL << (i * 8 + j + k);
                                this[Pieza.Blanca] |= 1UL << (i * 8 + j + k);
                                break;
                            case 'K':
                                this[Pieza.Rey] |= 1UL << (i * 8 + j + k);
                                this[Pieza.Blanca] |= 1UL << (i * 8 + j + k);
                                break;
                            case 'p':
                                this[Pieza.Peon] |= 1UL << (i * 8 + j + k);
                                this[Pieza.Negra] |= 1UL << (i * 8 + j + k);
                                break;
                            case 'n':
                                this[Pieza.Caballo] |= 1UL << (i * 8 + j + k);
                                this[Pieza.Negra] |= 1UL << (i * 8 + j + k);
                                break;
                            case 'b':
                                this[Pieza.Alfil] |= 1UL << (i * 8 + j + k);
                                this[Pieza.Negra] |= 1UL << (i * 8 + j + k);
                                break;
                            case 'r':
                                this[Pieza.Torre] |= 1UL << (i * 8 + j + k);
                                this[Pieza.Negra] |= 1UL << (i * 8 + j + k);
                                break;
                            case 'q':
                                this[Pieza.Dama] |= 1UL << (i * 8 + j + k);
                                this[Pieza.Negra] |= 1UL << (i * 8 + j + k);
                                break;
                            case 'k':
                                this[Pieza.Rey] |= 1UL << (i * 8 + j + k);
                                this[Pieza.Negra] |= 1UL << (i * 8 + j + k);
                                break;
                            default:
                                k += int.Parse(FENFila[i][j].ToString()) - 1;
                                break;
                        }
                    }
                }
                Turno = (match.Groups["Turno"].Value == "w") ? Equipo.Blanco : Equipo.Negro;
                EnroquesPosibles = 0;
                EnroquesPosibles |= (FENEnroque.Contains("K")) ? Enroque.CortoBlancas : 0;
                EnroquesPosibles |= (FENEnroque.Contains("Q")) ? Enroque.LargoBlancas : 0;
                EnroquesPosibles |= (FENEnroque.Contains("k")) ? Enroque.CortoNegras : 0;
                EnroquesPosibles |= (FENEnroque.Contains("q")) ? Enroque.LargoNegras : 0;
                if (match.Groups["AlPaso"].Value == "-")
                {
                    capturasAlPaso = 0;
                }
                else
                {
                    capturasAlPaso = 1UL << Jugada.CasillaStringToInt(match.Groups["AlPaso"].Value);
                }
            }
            else
            {
                throw new ArgumentException("El FEN no es valido", nameof(FEN));
            }
            casillasOcupadas = this[Pieza.Blanca] | this[Pieza.Negra];
            casillaVacias = ~casillasOcupadas;
            CalcularAtaques();
            CalcularJugadasPosibles();
        }

        /// <summary>
        /// Incializa una nueva instancia de la clase <see cref="Posicion"/> a partir de otra posición y una jugada.
        /// </summary>
        /// <param name="posicion">Posición en la que se realiza la jugada que lleva a la nueva posición.</param>
        /// <param name="jugada">Jugada que se realiza en la nueva posición.</param>
        public Posicion(Posicion posicion, Jugada jugada)
        {
            EnroquesPosibles = posicion.EnroquesPosibles;
            JugadasSinMoverPeonNiCapturar = posicion.JugadasSinMoverPeonNiCapturar;
            NumeroJugada = posicion.NumeroJugada;
            piezas = (ulong[])posicion.piezas.Clone();
            Turno = posicion.Turno;
            RealizarJugada(jugada);
            InicializarDescripcion();
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Actualiza la posición según una jugada.
        /// </summary>
        /// <param name="jugada">Jugada a realizar.</param>
        public void RealizarJugada(Jugada jugada)
        {
            if ((jugada.Tipo & TipoJugada.Captura) == 0 && jugada.Pieza != Pieza.Peon) // Si la jugada no es ni movimiento de peón ni captura se incrementa el contador para la regla de los 50 movimientos.
            {
                JugadasSinMoverPeonNiCapturar++;
            }
            else
            {
                JugadasSinMoverPeonNiCapturar = 0;
            }
            if (Turno == Equipo.Negro)
            {
                NumeroJugada++; // Número jugada empieza en 1 y luego se incrementa después de cada jugada de las negras.
            }
            if ((jugada.Tipo & TipoJugada.Enroque) != 0)
            {
                if (Turno == Equipo.Blanco)
                {
                    if (jugada.Destino % 8 > 3) // En un enroque el destino es la posicón de la torre por lo que así se puede saber si el enroque es largo o corto.
                    {
                        this[Pieza.Rey] ^= 80; // 80 es e1 y g1
                        this[Pieza.Torre] ^= 160; // 160 es f1 y h1.
                        this[Pieza.Blanca] ^= 240; // 240 es e1, f1, g1 y h1.
                    }
                    else
                    {
                        this[Pieza.Rey] ^= 20; // 20 es e1 y c1.
                        this[Pieza.Torre] ^= 9; // 9 es a1 y d1.
                        this[Pieza.Blanca] ^= 29; // 29 es a1, c1, d1 y e1.
                    }
                    EnroquesPosibles &= (Enroque)12; // Las blancas ya no pueden enrocar.
                }
                else
                {
                    if (jugada.Destino % 8 > 3)
                    {
                        this[Pieza.Rey] ^= 0x5000_0000_0000_0000; // Ese número hexadecimal es e8 y g8.
                        this[Pieza.Torre] ^= 0xA000_0000_0000_0000; // Ese número hexadecimal es f8 y h8.
                        this[Pieza.Negra] ^= 0xF000_0000_0000_0000; // Ese número hexadecimal es e8, f8, g8 y h8.
                    }
                    else
                    {
                        this[Pieza.Rey] ^= 0x1400_0000_0000_0000; // Ese número hexadecimal es e8 y c8.
                        this[Pieza.Torre] ^= 0x900_0000_0000_0000; // Ese número hexadecimal es a8 y d8.
                        this[Pieza.Negra] ^= 0x1D00_0000_0000_0000; // Ese número hexadecimal es a8, c8, d8 y e8.
                    }
                    EnroquesPosibles &= (Enroque)3; // Las negras ya no pueden enrocar.
                }
            }
            else
            {
                ulong origen = 1UL << jugada.Origen; // Bitboard que representa el origen.
                ulong destino = 1UL << jugada.Destino; // Birboard que representa el destino.
                ulong origenDestino = origen | destino; // Bitboard que representa el origen y el destino.
                this[Turno.ToPieza()] ^= origenDestino; // Pone en 0 el bit que representa donde estaba antes la pieza y en 1 el que representa donde está ahora.
                if (jugada.Pieza == Pieza.Rey)
                {
                    EnroquesPosibles &= (Enroque)((int)Turno.ToPieza() * -9 + 12); // El bando que movió el rey ya no podrá enrocar.
                }
                else if (jugada.Pieza == Pieza.Torre) // Si una torre se mueve ya no se podrá utilizar para enrocar.
                {
                    if (jugada.Origen % 8 == 0) // Si la torre estaba en la columna a ya no se puede hacer el enroque largo.
                    {
                        EnroquesPosibles &= (Enroque)((int)Turno * -3 + 10);
                    }
                    else if (jugada.Origen % 8 == 7) // Si la torre estaba en la columna h ya no se puede hacer el enroque corto.
                    {
                        EnroquesPosibles &= (Enroque)((int)Turno.ToPieza() * -3 + 14);
                    }
                }
                else if ((jugada.Tipo & TipoJugada.CapturaAlPaso) != 0)
                {
                    destino = (Turno == Equipo.Blanco) ? destino >> 8 : destino << 8; // Si la captura fue al paso acomoda el destino para borrar la pieza capturada en la posición dónde estaba.
                }
                if((jugada.Tipo & TipoJugada.Captura) != 0) // Si fue una captura
                {
                    this[((Equipo)((int)Turno * -1)).ToPieza()] ^= destino; // Pone en 0 el bit que representa donde estaba la pieza capturada en el bitboard del color que jugó el turno anterior.
                    // Elimina el destino en todos los bitboards de piezas (excepto el rey porque no puede ser capturado), luego se agregará en el que corresponda
                    this[Pieza.Peon] &= ~destino;
                    this[Pieza.Caballo] &= ~destino;
                    this[Pieza.Alfil] &= ~destino;
                    this[Pieza.Torre] &= ~destino;
                    this[Pieza.Dama] &= ~destino;

                }
                if ((jugada.Tipo & (TipoJugada)60) != 0) // Si es alguna coronación.
                {
                    this[Pieza.Peon] ^= origen; // Pone en 0 el bit que representa donde estaba antes la pieza.
                    if ((jugada.Tipo & TipoJugada.PromocionDama) != 0) // Si corona dama.
                    {
                        this[Pieza.Dama] ^= destino; // Pone en 1 el bit que representa la casilla en la que se coronó.
                    }
                    else if ((jugada.Tipo & TipoJugada.PromocionTorre) != 0) // Si corona torre.
                    {
                        this[Pieza.Torre] ^= destino;
                    }
                    else if ((jugada.Tipo & TipoJugada.PromocionAlfil) != 0) // Si corona alfil.
                    {
                        this[Pieza.Alfil] ^= destino;
                    }
                    else // Si corona caballo.
                    {
                        this[Pieza.Caballo] ^= destino;
                    }
                }
                else
                {
                    this[jugada.Pieza] ^= origenDestino; // Pone en 0 el bit que representa donde estaba antes la pieza y en 1 el que representa donde está ahora.
                    if ((jugada.Tipo & TipoJugada.AvanceDoble) != 0) // Si fue un avance doble de peón.
                    {
                        capturasAlPaso = Turno == Equipo.Blanco ? destino >> 8 : destino << 8; // Marca la posible captura al paso.
                    }
                }
            }
            Turno = (Equipo)((int)Turno * -1); // Cambia el turno.
            casillasOcupadas = this[Pieza.Blanca] | this[Pieza.Negra];
            casillaVacias = ~casillasOcupadas;
            CalcularAtaques();
            if ((jugada.Tipo & TipoJugada.Prueba) == 0) // Para evitar llamarse infinitamente, ya que CalcularJugadasPosibles() llama a RealizarJugada().
            {
                CalcularJugadasPosibles();
            }
        }

        /// <summary>
        /// Calcula todas las jugadas posibles.
        /// </summary>
        private void CalcularJugadasPosibles()
        {
            ReiniciarJugadasPosibles();
            // Obtiene las casillas a las que pueden avanzar los peones, primero se considera que los peones son blancos y luego si son negros se retrocede dos casillas.
            ulong DestinosAvancePeon = (((this[Pieza.Peon] & this[Turno.ToPieza()]) << 8) >> ((int)Turno.ToPieza() << 4)) & casillaVacias;
            AgregarJugadas(Pieza.Peon, DestinosAvancePeon, TipoJugada.Normal);
            AgregarJugadas(Pieza.Peon, ((DestinosAvancePeon << 8) >> ((int)Turno.ToPieza() << 4)) & casillaVacias & fila[3 + (int)Turno.ToPieza()], TipoJugada.AvanceDoble);
            AgregarJugadas(Pieza.Peon, ataques[Turno][Pieza.Peon], TipoJugada.Captura);
            AgregarJugadas(Pieza.Peon, capturasAlPaso, TipoJugada.CapturaAlPaso);
            AgregarJugadas(Pieza.Caballo, ataques[Turno][Pieza.Caballo]);
            AgregarJugadas(Pieza.Alfil, ataques[Turno][Pieza.Alfil]);
            AgregarJugadas(Pieza.Torre, ataques[Turno][Pieza.Torre]);
            AgregarJugadas(Pieza.Dama, ataques[Turno][Pieza.Dama]);
            AgregarJugadas(Pieza.Rey, ataques[Turno][Pieza.Rey]);
            AgregarEnroques(EnroquesSeguros());
            AgregarCoronaciones();
            DeterminarImposibles();
            JugadasPosibles.RemoveAll(jugada => jugada.Tipo == TipoJugada.Imposible);
        }

        /// <summary>
        /// Determina las jugadas que dejan al rey propio en jaque.
        /// </summary>
        private void DeterminarImposibles()
        {
            Posicion prueba; // Posición dónde se realzarán las jugadas para ver que no dejen una situación ìmposible.
            foreach (Jugada jugada in JugadasPosibles)
            {
                Jugada pruebaJugada = new Jugada(jugada, jugada.Tipo | TipoJugada.Prueba); // Le agrega el tipo prueba para evitar llamarse infinitamente.
                prueba = new Posicion(this, pruebaJugada);
                if ((((prueba.TodosLosAtaques(Equipo.Blanco) & prueba[Pieza.Rey] & prueba[Pieza.Negra]) != 0) && prueba.Turno == Equipo.Blanco) ||
                    (((prueba.TodosLosAtaques(Equipo.Negro) & prueba[Pieza.Rey] & prueba[Pieza.Blanca]) != 0) && prueba.Turno == Equipo.Negro))
                {
                    jugada.Tipo = TipoJugada.Imposible;
                }
            }
        }

        /// <summary>
        /// Modifica <see cref="JugadasPosibles"/> para agregar las coronaciones.
        /// </summary>
        private void AgregarCoronaciones()
        {
            List<Jugada> coronaciones = new List<Jugada>();
            foreach (Jugada jugada in JugadasPosibles)
            {
                if (jugada.Pieza == Pieza.Peon && jugada.Destino / 8 == (int)Turno.ToPieza() * -7 + 7)
                {
                    // Agrega las distintas promociones posibles.
                    coronaciones.Add(new Jugada(jugada, TipoJugada.PromocionCaballo | jugada.Tipo));
                    coronaciones.Add(new Jugada(jugada, TipoJugada.PromocionAlfil | jugada.Tipo));
                    coronaciones.Add(new Jugada(jugada, TipoJugada.PromocionTorre | jugada.Tipo));
                    coronaciones.Add(new Jugada(jugada, TipoJugada.PromocionDama | jugada.Tipo));
                    jugada.Tipo = TipoJugada.Imposible; // Quita la jugada que no es valida porque lleva un peón a última fila sin coronarlo.
                }
            }
            JugadasPosibles.AddRange(coronaciones);
            
        }

        /// <summary>
        /// Agrega a <see cref="JugadasPosibles"/> los enroques.
        /// </summary>
        /// <param name="enroque">Enroques que se pueden realizar.</param>
        private void AgregarEnroques(Enroque enroque)
        {
            if (Turno == Equipo.Blanco)
            {
                if ((enroque & Enroque.CortoBlancas) != 0)
                {
                    JugadasPosibles.Add(new Jugada(16, 128, TipoJugada.Enroque, Pieza.Rey)); // Agrega el enroque.
                    JugadasPosibles.Add(new Jugada(16, 64, TipoJugada.Enroque, Pieza.Rey)); // Agrega un versión con destino en g1.
                }
                if ((enroque & Enroque.LargoBlancas) != 0)
                {
                    JugadasPosibles.Add(new Jugada(16, 1, TipoJugada.Enroque, Pieza.Rey)); // Agrega el enroque.
                    JugadasPosibles.Add(new Jugada(16, 4, TipoJugada.Enroque, Pieza.Rey)); // Agrega un versión con destino en c1.
                }
            }
            else
            {
                if ((enroque & Enroque.CortoNegras) != 0)
                {
                    JugadasPosibles.Add(new Jugada(0x1000_0000_0000_0000, 0x8000_0000_0000_0000, TipoJugada.Enroque, Pieza.Rey)); // Agrega el enroque.
                    JugadasPosibles.Add(new Jugada(0x1000_0000_0000_0000, 0x4000_0000_0000_0000, TipoJugada.Enroque, Pieza.Rey)); // Agrega un versión con destino en g8.
                }
                if ((enroque & Enroque.LargoNegras) != 0)
                {
                    JugadasPosibles.Add(new Jugada(0x1000_0000_0000_0000, 0x100_0000_0000_0000, TipoJugada.Enroque, Pieza.Rey)); // Agrega el enroque.
                    JugadasPosibles.Add(new Jugada(0x1000_0000_0000_0000, 0x400_0000_0000_0000, TipoJugada.Enroque, Pieza.Rey)); // Agrega un versión con destino en c8.
                }
            }
        }

        /// <summary>
        /// Calcular los ataque que hace cada bando.
        /// </summary>
        private void CalcularAtaques()
        {
            ReiniciarAtaques();
            AgregarAtaques(Equipo.Blanco, Pieza.Peon);
            AgregarAtaques(Equipo.Negro, Pieza.Peon);
            AgregarAtaques(Equipo.Blanco, Pieza.Caballo);
            AgregarAtaques(Equipo.Negro, Pieza.Caballo);
            AgregarAtaques(Equipo.Blanco, Pieza.Alfil);
            AgregarAtaques(Equipo.Negro, Pieza.Alfil);
            AgregarAtaques(Equipo.Blanco, Pieza.Torre);
            AgregarAtaques(Equipo.Negro, Pieza.Torre);
            AgregarAtaques(Equipo.Blanco, Pieza.Dama);
            AgregarAtaques(Equipo.Negro, Pieza.Dama);
            AgregarAtaques(Equipo.Blanco, Pieza.Rey);
            AgregarAtaques(Equipo.Negro, Pieza.Rey);
        }

        /// <summary>
        /// Agrega a <see cref="ataques"/> los ataques realizados por un tipo de pieza de un equipo.
        /// </summary>
        /// <param name="equipo">Equipo del que se van a agrear los ataques.</param>
        /// <param name="pieza">Pieza de la que se van a agregar los ataques.</param>
        /// <exception cref="ArgumentException"></exception>
        private void AgregarAtaques(Equipo equipo, Pieza pieza)
        {
            ulong piezasAtacantes = this[pieza] & this[equipo.ToPieza()];
            ulong piezaAtacante = piezasAtacantes & ~(piezasAtacantes - 1); // El primer bit que representa una pieza.
            while (piezaAtacante != 0) // Mientras haya alguna pieza.
            {
                int posicionPieza = Bits.IndiceBitMenosSignificativo(piezaAtacante);
                if (pieza == Pieza.Alfil)
                {
                    ataques[equipo][Pieza.Alfil] |= CalcularAtaquesAlfil(posicionPieza);
                }
                else if (pieza == Pieza.Torre)
                {
                    ataques[equipo][Pieza.Torre] |= CalcularAtaquesTorre(posicionPieza);
                }
                else if (pieza == Pieza.Dama)
                {
                    ataques[equipo][Pieza.Dama] |= CalcularAtaquesAlfil(posicionPieza) | CalcularAtaquesTorre(posicionPieza);
                }
                else
                {
                    ataques[equipo][pieza] |= destinos[IndiceEnDestinos(equipo, pieza), posicionPieza]; // Agrega a los ataques los que puede hacer la pieza desde la posición actual.
                }
                piezasAtacantes ^= piezaAtacante; // Pone en 0 el bit de la pieza que ya se utilizó.
                piezaAtacante = piezasAtacantes & ~(piezasAtacantes - 1); // El próximo bit que representa una pieza, 0 si no queta ninguna.
            }
        }

        /// <summary>
        /// Obtiene el índice de la pieza en <see cref="destinos"/>.
        /// </summary>
        /// <param name="equipo">Equipo de la pieza.</param>
        /// <param name="pieza">Tipo de la pieza.</param>
        /// <exception cref="ArgumentException"></exception>
        private int IndiceEnDestinos(Equipo equipo, Pieza pieza)
        {
            switch (pieza)
            {
                case Pieza.Peon:
                    return equipo == Equipo.Blanco ? 0 : 1;
                case Pieza.Caballo:
                    return 2;
                case Pieza.Rey:
                    return 3;
                default:
                    throw new ArgumentException("Los destinos de esta pieza no aperecen en destino", nameof(pieza));
            }
        }

        /// <summary>
        /// Calcula los ataques que puede hacer una torre habiendo unos bloqueadores.
        /// </summary>
        /// <param name="casilla">La casilla donde está la torre.</param>
        /// <returns>Bitboard con 1 en las casillas que ataca la torre desde <paramref name="casilla"/></returns>
        private ulong CalcularAtaquesTorre(int casilla)
        {
            ulong ataques = 0;
            for (int i = 0; i < 8; i += 2) // Itera por cada dirección en la que puede ir la torre.
            {
                if (casillasEnDireccion[i, casilla] != 0) // Si hay casillas en esa dirección
                {
                    ulong ataque = casillasEnDireccion[i, casilla];
                    ulong bloqueadores = casillasOcupadas & ataque;
                    if (bloqueadores != 0)
                    {
                        // Quita las casillas que la pieza no alcanza por haber otra en medio.
                        if (i < 3)
                        {
                            ataque ^= casillasEnDireccion[i, Bits.IndiceBitMenosSignificativo(bloqueadores)];
                        }
                        else
                        {
                            ataque ^= casillasEnDireccion[i, Bits.IndiceBitMasSignificativo(bloqueadores)];
                        }
                    }
                    ataques |= ataque;
                }
            }
            return ataques;
        }

        /// <summary>
        /// Calcula los ataques que puede hacer un alfil desde una casilla habiendo unos bloqueadores.
        /// </summary>
        /// <param name="casilla">La casilla donde está el alfil.</param>
        /// <returns>Bitboard con 1 en las casillas que ataca el alfil desde <paramref name="casilla"/></returns>
        private ulong CalcularAtaquesAlfil(int casilla)
        {
            ulong ataques = 0;
            for (int i = 1; i < 8; i += 2) // Itera por cada dirección en la que puede ir el alfil.
            {
                if (casillasEnDireccion[i, casilla] != 0) // Si hay casillas en esa dirección
                {
                    ulong ataque = casillasEnDireccion[i, casilla];
                    ulong bloqueadores = casillasOcupadas & ataque;
                    if (bloqueadores != 0)
                    {
                        // Quita las casilla que la pieza no puede alcanzar por haber otra en medio.
                        if (i == 1 || i == 7)
                        {
                            ataque ^= casillasEnDireccion[i, Bits.IndiceBitMenosSignificativo(bloqueadores)];
                        }
                        else
                        {
                            ataque ^= casillasEnDireccion[i, Bits.IndiceBitMasSignificativo(bloqueadores)];
                        }
                    }
                    ataques |= ataque;
                }
            }
            return ataques;
        }

        /// <summary>
        /// Pone en 0 todos los ataques.
        /// </summary>
        private void ReiniciarAtaques()
        {
            ataques = new Dictionary<Equipo, Dictionary<Pieza, ulong>>
            {
                {
                    Equipo.Blanco, new Dictionary<Pieza, ulong>
                    {
                        {Pieza.Peon, 0 },
                        {Pieza.Caballo, 0 },
                        {Pieza.Alfil, 0 },
                        {Pieza.Torre, 0 },
                        {Pieza.Dama, 0 },
                        {Pieza.Rey, 0 }
                    }
                },
                {
                    Equipo.Negro, new Dictionary<Pieza, ulong>
                    {
                        {Pieza.Peon, 0 },
                        {Pieza.Caballo, 0 },
                        {Pieza.Alfil, 0 },
                        {Pieza.Torre, 0 },
                        {Pieza.Dama, 0 },
                        {Pieza.Rey, 0 }
                    }
                }
            };
        }
        /// <summary>
        /// Agrega a <see cref="JugadasPosibles"/> las jugadas que puede realizar una pieza.
        /// </summary>
        /// <param name="pieza">Pieza cuyos movimientos se van a agregar.</param>
        /// <param name="destinos">Casillas a las que se podría mover la pieza.</param>
        /// <param name="tipo">Tipo de jugada (solo necesario para el peón).</param>
        /// <exception cref="ArgumentException"></exception>
        private void AgregarJugadas(Pieza pieza, ulong destinos, TipoJugada tipo = 0)
        {
            ulong piezasAMover = this[pieza] & this[Turno.ToPieza()];
            ulong piezaActual = piezasAMover & ~(piezasAMover - 1); // La primera pieza a mover.
            while (piezaActual != 0) // Mientras queden piezas piezas para mover.
            {
                ulong destinosPiezaActual = destinos;
                ulong destinoActual = destinosPiezaActual & ~(destinosPiezaActual - 1); // El primer destino.
                while (destinoActual != 0) // Mientras queden destionos.
                {
                    TipoJugada tipoPosibleJugada = pieza == Pieza.Peon ? PuedeIrA(Pieza.Peon, piezaActual, destinoActual, tipo) : PuedeIrA(pieza, piezaActual, destinoActual);
                    if ((tipoPosibleJugada & TipoJugada.Imposible) == 0) // Si es posible.
                    {
                        JugadasPosibles.Add(new Jugada(piezaActual, destinoActual, tipoPosibleJugada, pieza));
                    }
                    destinosPiezaActual &= ~destinoActual; // Quita el destino que ya se utilizó.
                    destinoActual = destinosPiezaActual & ~(destinosPiezaActual - 1); // El próximo destino, 0 si no quedan.
                }
                piezasAMover &= ~piezaActual; // Quita la pieza cuyos movimientos ya se calcularon.
                piezaActual = piezasAMover & ~(piezasAMover - 1); // La próxima pieza a mover, 0 si no quedan.
            }
        }

        /// <summary>
        /// Determina si una pieza puede ir a una casilla.
        /// </summary>
        /// <param name="pieza">Pieza que se tendría que mover.</param>
        /// <param name="origen">Casilla en la que se encuentra la pieza.</param>
        /// <param name="destino">Casilla a la que tendría que ir la pieza..</param>
        /// <param name="tipo">Tipo de jugada que le permitiría ir (solo necesario para el peón).</param>
        /// <returns>Tipo de jugada del movimiento de <paramref name="pieza"/> desde <paramref name="origen"/> a <paramref name="destino"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        private TipoJugada PuedeIrA(Pieza pieza, ulong origen, ulong destino, TipoJugada tipo = 0)
        {
            if ((destino & this[Turno.ToPieza()]) != 0 || (destino & this[Pieza.Rey]) != 0)
            {
                return TipoJugada.Imposible; // No se puede ir a una casilla ocupada por otra pieza del mismo color o por el rey enemigo.
            }
            TipoJugada tipoJugada = (destino & this[((Equipo)((int)Turno * -1)).ToPieza()]) != 0 ? TipoJugada.Captura : TipoJugada.Normal; // Si hay una pieza enemiga es una captura.
            switch (pieza)
            {
                case Pieza.Peon:
                    switch (tipo)
                    {
                        case TipoJugada.Normal:
                            return (((origen << 8) >> ((int)Turno.ToPieza() << 4) & destino) != 0) ? TipoJugada.Normal : TipoJugada.Imposible;
                        case TipoJugada.Captura:
                            return ((destinos[(int)Turno.ToPieza(), Bits.IndiceBitMenosSignificativo(origen)] & destino) != 0 && tipoJugada == TipoJugada.Captura) ? TipoJugada.Captura :
                                TipoJugada.Imposible;
                        case TipoJugada.CapturaAlPaso:
                            // (TipoJugada)129 significa Captura y CapturaAlPaso.
                            return (destinos[(int)Turno.ToPieza(), Bits.IndiceBitMenosSignificativo(origen)] & destino) != 0 ? (TipoJugada)129 : TipoJugada.Imposible;
                        case TipoJugada.AvanceDoble:
                            return ((Bits.IndiceBitMenosSignificativo(origen) % 8 == Bits.IndiceBitMenosSignificativo(destino) % 8) && ((origen & fila[(int)Turno.ToPieza() * 5 + 1]) != 0)) ?
                                TipoJugada.AvanceDoble : TipoJugada.Imposible;
                        default:
                            throw new ArgumentException("El tipo debe ser Normal, Captura, CapturaAlPaso o AvanceDoble", nameof(tipo));
                    }
                case Pieza.Caballo:
                    return ((destinos[2, Bits.IndiceBitMenosSignificativo(origen)] & destino) != 0) ? tipoJugada : TipoJugada.Imposible;
                case Pieza.Alfil:
                    return (CalcularAtaquesAlfil(Bits.IndiceBitMenosSignificativo(origen)) & destino) != 0 ? tipoJugada : TipoJugada.Imposible;
                case Pieza.Torre:
                    return (CalcularAtaquesTorre(Bits.IndiceBitMenosSignificativo(origen)) & destino) != 0 ? tipoJugada : TipoJugada.Imposible;
                case Pieza.Dama:
                    return ((CalcularAtaquesTorre(Bits.IndiceBitMenosSignificativo(origen)) | CalcularAtaquesAlfil(Bits.IndiceBitMenosSignificativo(origen)))
                        & destino) != 0 ? tipoJugada : TipoJugada.Imposible;
                case Pieza.Rey:
                    return ((destinos[3, Bits.IndiceBitMenosSignificativo(origen)] & destino) != 0) ? tipoJugada : TipoJugada.Imposible;
                default:
                    throw new ArgumentException("La pieza debe ser peón, caballo, alfil, torre, dama o rey", nameof(pieza));
            }
        }

        /// <summary>
        /// Reinicia <see cref="JugadasPosibles"/>.
        /// </summary>
        private void ReiniciarJugadasPosibles() => JugadasPosibles = new List<Jugada>();

        /// <summary>
        /// Mueve los bits de un bitboard hacia el este borrando los que aparecen en el oeste.
        /// </summary>
        /// <param name="bitboad">Bitboard en el que se quieren mover los bits.</param>
        /// <param name="veces">Cantidad de veces que se quieren mover los bits.</param>
        /// <returns>Nuevo bitboard con los bits corridos al este.</returns>
        private static ulong MoverEste(ulong bitboad, int veces)
        {
            for (int i = 0; i < veces; i++)
            {
                bitboad = (bitboad << 1) & ~columna[0];
            }
            return bitboad;
        }

        /// <summary>
        /// Mueve los bits de un bitboard hacia el oeste borrando los que aparecen en el este.
        /// </summary>
        /// <param name="bitboad">Bitboard en el que se quieren mover los bits.</param>
        /// <param name="veces">Cantidad de veces que se quieren mover los bits.</param>
        /// <returns>Nuevo bitboard con los bits corridos al oeste</returns>
        private static ulong MoverOeste(ulong bitboad, int veces)
        {
            for (int i = 0; i < veces; i++)
            {
                bitboad = (bitboad >> 1) & ~columna[7];
            }
            return bitboad;
        }

        /// <summary>
        /// Determina que enroques de los <see cref="EnroquesPosibles"/> no están actualmente impedidos.
        /// </summary>
        /// <returns>Enroques que se pueden hacer en esta jugada.</returns>
        private Enroque EnroquesSeguros()
        {
            Enroque enroque = EnroquesPosibles;
            // Si hay piezas en el medio o se ataca una casilla por la que pasaría el rey, no se puede enrocar.
            if (Turno == Equipo.Blanco)
            {
                if ((EnroquesPosibles & Enroque.CortoBlancas) != 0)
                {
                    // 112 es e1, f1 y g1, las casillas que no pueden estar atacadas para que el blanco enroque corto.
                    // 96 es f1 y g1, las casillas que deben estar libre para que el blanco enroque corto
                    if (((TodosLosAtaques(Equipo.Negro) & 112) != 0) || ((casillasOcupadas & 96) != 0))
                    {
                        enroque &= ~Enroque.CortoBlancas;
                    }
                }
                if ((EnroquesPosibles & Enroque.LargoBlancas) != 0)
                {
                    // 28 es c1, d1 y e1, las casillas que no pueden estar atacas para que el blanco enroque largo.
                    // 14 es b1, c1 y d1, las casilla que deben estar libres para que el blanco enroque largo.
                    if (((TodosLosAtaques(Equipo.Negro) & 28) != 0) || ((casillasOcupadas & 14) != 0))
                    {
                        enroque &= ~Enroque.LargoBlancas;
                    }
                }
            }
            else
            {
                if ((EnroquesPosibles & Enroque.CortoNegras) != 0)
                {
                    // 0x7000_0000_0000_0000 es e8, f8 y g8, las casillas que no pueden estar atacadas para que el negro enroque corto.
                    // 0x6000_0000_0000_0000 es f8 y g8, las casillas que deben estar libres para que el negro enroque corto.
                    if (((TodosLosAtaques(Equipo.Blanco) & 0x7000_0000_0000_0000) != 0) || ((casillasOcupadas & 0x6000_0000_0000_0000) != 0))
                    {
                        enroque &= ~Enroque.CortoNegras;
                    }
                }
                if ((EnroquesPosibles & Enroque.LargoNegras) != 0)
                {
                    // 0x1C00_0000_0000_0000 es c8, d8 y e8, las casillas que no pueden estar atacadas para que el negro enroque largo.
                    // 0xE00_0000_0000_0000 es b8, c8 y d8, las casillas que deben estar libres para que el negro enroque largo.
                    if (((TodosLosAtaques(Equipo.Blanco) & 0x1C00_0000_0000_0000) != 0) || ((casillasOcupadas & 0xE00_0000_0000_0000) != 0))
                    {
                        enroque &= ~Enroque.LargoNegras;
                    }
                }
            }
            return enroque;
        }

        /// <summary>
        /// Determina todos los ataques que realiza un bando.
        /// </summary>
        /// <param name="equipo">Equipo que realiza los ataques.</param>
        /// <returns>Bitboard con 1 en todas las casillas atacadas por <paramref name="equipo"/> (Ordenado con a1 = 0 y h1 = 7.)</returns>
        private ulong TodosLosAtaques(Equipo equipo) => ataques[equipo][Pieza.Peon] | ataques[equipo][Pieza.Caballo] | ataques[equipo][Pieza.Alfil] | ataques[equipo][Pieza.Torre] |
            ataques[equipo][Pieza.Dama] | ataques[equipo][Pieza.Rey];

        /// <summary>
        /// Determina si el bando al que le toca mover está en jaque.
        /// </summary>
        public bool HayJaque() => (TodosLosAtaques((Equipo)((int)Turno * -1)) & this[Turno.ToPieza()] & this[Pieza.Rey]) != 0;

        /// <summary>
        /// Determina si el bando al que le toca mover está en jaque mate.
        /// </summary>
        public bool HayJaqueMate() => JugadasPosibles.Count == 0 && HayJaque();

        /// <summary>
        /// Determina si el bando al que le toca mover está ahogado.
        /// </summary>
        public bool HayAhogado() => JugadasPosibles.Count == 0 && !HayJaque();

        /// <summary>
        /// Determina si más de una pieza del mismo tipo pueden ir a un destino.
        /// </summary>
        /// <param name="jugada">Jugada que tiene el destino y el tipo de pieza.</param>
        public bool PuedeJugarMasDeUnaPieza(Jugada jugada) => JugadasPosibles.Exists(j => !j.Equals(jugada) && j.Pieza == jugada.Pieza && j.Destino == jugada.Destino);

        /// <summary>
        /// Determina si más de una pieza del mismo tipo que están en la misma columna pueden ir a un destino.
        /// </summary>
        /// <param name="jugada">Jugade que tiene el destino, el tipo de pieza y el origen.</param>
        public bool PuedeJugarMasDeUnaPiezaEnMismaColumna(Jugada jugada) => JugadasPosibles.Exists(j => !j.Equals(jugada) && j.Pieza == jugada.Pieza && j.Destino == jugada.Destino && j.Origen % 8 ==
            jugada.Origen % 8);

        public override bool Equals(object obj) => obj is Posicion posicion && posicion[Pieza.Blanca] == this[Pieza.Blanca] && posicion[Pieza.Negra] == this[Pieza.Negra] && posicion[Pieza.Peon] ==
            this[Pieza.Peon] && posicion[Pieza.Caballo] == this[Pieza.Caballo] && posicion[Pieza.Alfil] == this[Pieza.Alfil] && posicion[Pieza.Torre] == this[Pieza.Torre] && posicion[Pieza.Dama] ==
            this[Pieza.Dama] && posicion[Pieza.Rey] == this[Pieza.Rey] && posicion.EnroquesPosibles == EnroquesPosibles && posicion.capturasAlPaso == capturasAlPaso && posicion.Turno == Turno;

        public override int GetHashCode()
        {
            int hashCode = 1972040182;
            hashCode = hashCode * -1521134295 + EqualityComparer<ulong[]>.Default.GetHashCode(piezas);
            hashCode = hashCode * -1521134295 + capturasAlPaso.GetHashCode();
            hashCode = hashCode * -1521134295 + EnroquesPosibles.GetHashCode();
            hashCode = hashCode * -1521134295 + Turno.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Determina si un equipo tiene material insuficiente para dar jaque mate.
        /// </summary>
        /// <param name="equipo">Equipo del que se quiere saber si no puede dar mate.</param>
        public bool NoPuedeDarMate(Equipo equipo)
        {
            // Las piezas del color que se está comprobando.
            ulong rey = this[equipo.ToPieza()] & this[Pieza.Rey], caballo = this[equipo.ToPieza()] & this[Pieza.Caballo], alfil = this[equipo.ToPieza()] & this[Pieza.Alfil];
            caballo &= ~(caballo - 1); // El primer caballo.
            alfil &= ~(alfil - 1); // El primer alfil.
            // Comprueba si queda solo rey o solo rey y un alfil o solo rey y un caballo.
            return rey == this[equipo.ToPieza()] || (rey | caballo) == this[equipo.ToPieza()] || (rey | alfil) == this[equipo.ToPieza()];
        }

        public override string ToString() => $"{Descripcion} {JugadasSinMoverPeonNiCapturar} {NumeroJugada}";

        /// <summary>
        /// Inizializa <see cref="Descripcion"/>.
        /// </summary>
        private void InicializarDescripcion()
        {
            char[,] filas = { { '1', '1', '1', '1', '1', '1', '1', '1' }, { '1', '1', '1', '1', '1', '1', '1', '1' }, { '1', '1', '1', '1', '1', '1', '1', '1' }, { '1', '1', '1', '1', '1', '1', '1', '1' },
                { '1', '1', '1', '1', '1', '1', '1', '1' }, { '1', '1', '1', '1', '1', '1', '1', '1' }, { '1', '1', '1', '1', '1', '1', '1', '1' }, { '1', '1', '1', '1', '1', '1', '1', '1' } };
            for (int i = 0; i < 2; i++)
            {
                for (int j = 2; j < 8; j++)
                {
                    ulong piezasAAnotar = piezas[i] & piezas[j]; // Bitboard con las piezs de un tipo y color.
                    ulong piezaActual = piezasAAnotar & ~(piezasAAnotar - 1); // La primera pieza.
                    while (piezaActual != 0) // Mienstras queden piezas
                    {
                        int posicion = Bits.IndiceBitMenosSignificativo(piezaActual);
                        filas[posicion / 8, posicion % 8] = ((Pieza)j).ToChar((CaracterPieza)(i * 2 + 2));
                        piezasAAnotar &= ~piezaActual; // Quita la pieza anotada.
                        piezaActual = piezasAAnotar & ~(piezasAAnotar - 1); // La proxima pieza, 0 si no quedan.
                    }
                }
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 7; i >= 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (filas[i, j] == '1' && builder.Length > 0 && char.IsDigit(builder[builder.Length - 1]))
                    {
                        builder[builder.Length - 1]++;
                    }
                    else
                    {
                        builder.Append(filas[i, j]);
                    }
                }
                if (i > 0)
                {
                    builder.Append('/');
                }
            }
            builder.AppendFormat(" {0} {1} {2}", Turno == Equipo.Blanco ? "w" : "b", EnroquesPosibles.ToFEN(), capturasAlPaso > 0 ?
                Jugada.CasillaNumeroToString(Bits.IndiceBitMenosSignificativo(capturasAlPaso)) : "-");
            Descripcion = builder.ToString();
        }

        /// <summary>
        /// Inserta la posición en la base de datos, actualiza la apertura si la posición ya está en la base de datos.
        /// </summary>
        /// <param name="apertura">Apertura a la que pertenece la posición.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public void InsertarEnBaseDeDatos(Apertura apertura = null)
        {
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT Descripcion FROM Posicion WHERE Descripcion = '{Descripcion}'", conexion))
                {
                    if (select.ExecuteScalar() == null)
                    {
                        using (SqlCommand insert = new SqlCommand($"INSERT INTO Posicion VALUES ('{Descripcion}', {apertura?.Id.ToString() ?? "null"})", conexion))
                        {
                            insert.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (SqlCommand update = new SqlCommand($"UPDATE Posicion SET Id_Apertura = {apertura?.Id.ToString() ?? "null"} WHERE Descripcion = '{Descripcion}'", conexion))
                        {
                            update.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determina si un string es una descripción válida de una posición.
        /// </summary>
        /// <param name="descripcion">String que describe una posición (similar al FEN pero sin las cantidades de jugadas).</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public static bool EsDescripcionValida(string descripcion)
        {
            Match match = Regex.Match(descripcion, patronDescripcionPosicion);
            if (match.Success)
            {
                for (int i = 0; i < 8; i++) // Comprueba la suma en cada fila
                {
                    int suma = 0;
                    foreach (char item in match.Groups["Fila" + (i + 1)].Value)
                    {
                        // Cada caracter puede ser una letra representando una pieza o un número representando una cantidad de casillas vacias, la suma de la catidad de letras y el valor de
                        // cada número debe ser 8 porque cada fila tiene 8 columnas
                        if (int.TryParse(item.ToString(), out int numero))
                        {
                            suma += numero;
                        }
                        else
                        {
                            suma++;
                        }
                    }
                    if (suma != 8)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determina si se han intercambiado las damas.
        /// </summary>
        public bool IntercambioDeDamas() => this[Pieza.Dama] == 0;

        /// <summary>
        /// Determina si uno de los equipo tiene pareja de alfiles y el otro no.
        /// </summary>
        public bool ParejaDeAlfiles() => ParejaDeAlfiles(Pieza.Blanca) != ParejaDeAlfiles(Pieza.Negra);

        /// <summary>
        /// Determina si un equipo tiene más de un alfil.
        /// </summary>
        /// <param name="equipo">Pieza que representa el color que se quiere saber si tiene más de un alfil.</param>
        private bool ParejaDeAlfiles(Pieza equipo) => Bits.IndiceBitMasSignificativo(this[Pieza.Alfil] & this[equipo]) != Bits.IndiceBitMenosSignificativo(this[Pieza.Alfil] & this[equipo]);

        /// <summary>
        /// Determina si ningún bando tiene peones en las columnas centrales (d y e).
        /// </summary>
        public bool ColumnasCentralesAbiertas() => (this[Pieza.Peon] & (columna[3] | columna[4])) == 0;

        /// <summary>
        /// Determina si uno de los equipos tiene el peón de la columna d aislado y el otro no.
        /// </summary>
        public bool PeonDamaAislado() => PeonDamaAislado(Pieza.Blanca) != PeonDamaAislado(Pieza.Negra);

        /// <summary>
        /// Determina si un equipo tiene el peón de la columna d aislado.
        /// </summary>
        /// <param name="equipo">Pieza que representa el color que se quiere saber si tiene el peón de la columna de aislado.</param>
        private bool PeonDamaAislado(Pieza equipo) => ((this[Pieza.Peon] & this[equipo] & columna[3]) != 0) && ((this[Pieza.Peon] & this[equipo] & (columna[2] | columna[4])) == 0);

        /// <summary>
        /// Determina si los reyes están en flancos opuestos.
        /// </summary>
        public bool ReyesEnFlancosOpuestos() => !(((this[Pieza.Rey] & flancoDama) == 0) || ((this[Pieza.Rey] & flancoRey) == 0));

        /// <summary>
        /// Determina si un peón blanco está en sexta fila o uno negro está en tercera.
        /// </summary>
        public bool PeonLlegaASexta() => ((this[Pieza.Peon] & this[Pieza.Blanca] & fila[5]) != 0) || ((this[Pieza.Peon] & this[Pieza.Negra] & fila[2]) != 0);

        /// <summary>
        /// Determina si un equipo tiene solo un tipo de pieza además del rey y los peones.
        /// </summary>
        /// <param name="equipo">Pieza que representa el equipo que se quiere saber si tiene solo un tipo de piezas.</param>
        /// <param name="pieza">Tipo de pieza que tiene el equipo además del rey.</param>
        private bool SoloUnaPieza(Pieza equipo, Pieza pieza) => this[equipo] == ((this[pieza] | this[Pieza.Rey] | this[Pieza.Peon]) & this[equipo]);

        /// <summary>
        /// Determina si es un final con los tipos de piezas indicadps.
        /// </summary>
        /// <param name="pieza1">Primer tipo de pieza.</param>
        /// <param name="pieza2">Segundo tipo de pieza.</param>
        public bool FinalDePiezas(Pieza pieza1, Pieza pieza2) => (SoloUnaPieza(Pieza.Blanca, pieza1) && SoloUnaPieza(Pieza.Negra, pieza2)) || (SoloUnaPieza(Pieza.Blanca, pieza2) &&
            SoloUnaPieza(Pieza.Negra, pieza1));
        #endregion
    }
}
