using Ajedrez.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Ajedrez
{
    /// <summary>
    /// Representa una partida de ajedrez
    /// </summary>
    public class Partida : INotifyPropertyChanged
    {
        #region Atributos
        /// <summary>
        /// Índice de <see cref="Posicion"/>.
        /// </summary>
        private int indicePosicion = -1;

        /// <summary>
        /// Texto que representa las jugadas realizadas.
        /// </summary>
        private string textoJugadas = string.Empty;

        /// <summary>
        /// Reloj que indica el tiempo de los jugadores mientras se juega la partida.
        /// </summary>
        private Reloj reloj;

        /// <summary>
        /// FEN que representa la posición inicial de una partida normal.
        /// </summary>
        private const string posicionInicialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        /// <summary>
        /// Expresión regular para etiquetas PGN.
        /// </summary>
        private const string patronEtiquetaPGN = @"\[\s*(?<Nombre>[A-Z][a-zA-Z]*)\s*""(?<Valor>[^""]+)""\s*\]";

        /// <summary>
        /// Expresión regular para jugadas PGN.
        /// </summary>
        private const string patronJugadaPGN = @"(?<Numero>\d*)\s*\.?\s*(?<JugadaBlanca>(([a-h]x)?[a-h]8=[NBRQ]|[a-h]x[a-h][1-8]|[NBRQK][a-h]?[1-8]?x?[a-h][1-8]|O-O(-O)?|[a-h][1-8])[+#]?(\?\?|!!|!\?|\?!" +
            @"|\?|!)?(\s\$\d{1,3})?(\s*{[^}]*})?|\.{3})(\s+(?<NumeroRepetido>\d*)\s*\.*\s*(?<JugadaNegra>(([a-h]x)?[a-h]1=[NBRQ]|[a-h]x[a-h][1-8]|[NBRQK][a-h]?[1-8]?x?[a-h][1-8]|O-O(-O)?|[a-h][1-8])[+#]" +
            @"?(\?\?|!!|!\?|\?!|\?|!)?(\s\$\d{1,3})?(\s*{[^}]*})?))?";

        /// <summary>
        /// Etiquetas que deben incluirse obligatoriamente en un PGN.
        /// </summary>
        private static readonly List<EtiquetaPGN> etiquetasObligatorias = new List<EtiquetaPGN> { EtiquetaPGN.Event, EtiquetaPGN.Site, EtiquetaPGN.Date, EtiquetaPGN.Round, EtiquetaPGN.White,
            EtiquetaPGN.Black,EtiquetaPGN.Result };
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Tiene lugar cuando termina la partida.
        /// </summary>
        public event EventHandler PartidaTerminada;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece el índice de la posición actual.
        /// </summary>
        public int IndicePosicion
        {
            get => indicePosicion;
            set
            {
                if (value == -1 || (value >= 0 && value < CantidadPosicionesSinContarInicial()))
                {
                    indicePosicion = value;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece las jugadas realizadas en la partida.
        /// </summary>
        public List<JugadaCompleta> Jugadas { get; set; } = new List<JugadaCompleta>();

        /// <summary>
        /// Obtiene la posición actual.
        /// </summary>
        public Posicion Posicion => indicePosicion < 0 ? PosicionInicial : Jugadas[IndicePosicion / 2][indicePosicion % 2];

        /// <summary>
        /// Obtiene o establece la anotación de la partida.
        /// </summary>
        public string TextoJugadas
        {
            get => textoJugadas;
            set
            {
                textoJugadas = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextoJugadas)));
            }
        }

        /// <summary>
        /// Obtiene o establece las etiquetas PGN de la partida.
        /// </summary>
        public ObservableConcurrentDictionary<EtiquetaPGN, string> Etiquetas { get; set; } = new ObservableConcurrentDictionary<EtiquetaPGN, string>
        {
            { EtiquetaPGN.Event, "?" }, { EtiquetaPGN.Site, "?"}, {EtiquetaPGN.Date, "????.??.??"}, {EtiquetaPGN.Round, "?"}, {EtiquetaPGN.White, "?"}, {EtiquetaPGN.Black, "?"}, {EtiquetaPGN.Result,"*"},
            { EtiquetaPGN.WhiteTitle, null }, { EtiquetaPGN.BlackTitle, null }, { EtiquetaPGN.WhiteElo, null }, { EtiquetaPGN.BlackElo, null }, { EtiquetaPGN.WhiteNA, null }, { EtiquetaPGN.BlackNA, null },
            { EtiquetaPGN.WhiteType, null }, { EtiquetaPGN.BlackType, null }, {EtiquetaPGN.EventDate, null}, {EtiquetaPGN.Section, null}, {EtiquetaPGN.Stage, null}, {EtiquetaPGN.Board, null},
            { EtiquetaPGN.Opening, null}, {EtiquetaPGN.Variation, null}, {EtiquetaPGN.SubVariation, null}, {EtiquetaPGN.ECO, null}, {EtiquetaPGN.NIC, null}, {EtiquetaPGN.Time, null},
            { EtiquetaPGN.UTCTime, null}, {EtiquetaPGN.UTCDate, null}, { EtiquetaPGN.TimeControl, null}, { EtiquetaPGN.SetUp, "0" }, {EtiquetaPGN.FEN, null }, {EtiquetaPGN.Termination, null},
            { EtiquetaPGN.Annotator, null}, {EtiquetaPGN.Mode, null}
        };

        /// <summary>
        /// Obtiene o establece el reloj que indica el tiempo de los jugadores mientras se juega la partida..
        /// </summary>
        public Reloj Reloj
        {
            get => reloj;
            set
            {
                reloj = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Reloj)));
            }
        }

        /// <summary>
        /// Obtiene o establece el valor que indica si la partida se está jugando ahora mismo.
        /// </summary>
        public bool EnJuego { get; private set; } = false;

        /// <summary>
        /// Obtiene la última posición de la partida.
        /// </summary>
        public Posicion UltimaPosicion => Jugadas.Count == 0 ? PosicionInicial : (Jugadas[Jugadas.Count - 1][1] ?? Jugadas[Jugadas.Count - 1][0]);

        /// <summary>
        /// OBtiene o establece las medias jugadas restantes para volver a poder ofrecer tablas.
        /// </summary>
        public int ProximaPropuestaDeTablas { get; set; } = 0;

        /// <summary>
        /// Obtiene la partida configurada para jugarse como revancha de la actual.
        /// </summary>
        public Partida Revancha { get; private set; }

        /// <summary>
        /// Obtiene o establece las jugadas realizadas en formato para UCI.
        /// </summary>
        public string TextoJugadasUCI { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece el identificador de la partida en la base de datos, null si la partida no está en la base de datos.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Obtiene o establece el número de la última posición en la base de datos.
        /// </summary>
        public int NumeroUltimaPosicionEnBaseDeDatos { get; set; }

        /// <summary>
        /// Obtiene la posición en la que se comenzó la partida.
        /// </summary>
        public Posicion PosicionInicial { get; }

        /// <summary>
        /// Obtiene o establece comentario de la jugada que llevó a la posición actual.
        /// </summary>
        /// <remarks>Sin llaves ni espacios extras</remarks>
        public string ComentarioPosicionActual
        {
            get => Posicion.Turno == Equipo.Blanco ? Jugadas[IndicePosicion / 2].ComentarioJugadaNegraSinLlaves : Jugadas[IndicePosicion / 2].ComentarioJugadaBlancaSinLlaves;
            set
            {
                int longitudTextoJugadasAntesDePosicionActual = QuitarTextoJugadaActual();
                if (Posicion.Turno == Equipo.Blanco)
                {
                    Jugadas[IndicePosicion / 2].ComentarioJugadaNegra = value;
                }
                else
                {
                    Jugadas[IndicePosicion / 2].ComentarioJugadaBlanca = value;
                }
                TextoJugadas = TextoJugadas.Insert(longitudTextoJugadasAntesDePosicionActual, Jugadas[indicePosicion / 2].ToString()); // Agrega la jugada con el comentario nuevo.
            }
        }

        /// <summary>
        /// Establece el NAG de la jugada que llevó a la posición actual.
        /// </summary>
        public int NAGPosicionActual
        {
            set
            {
                int longitudTextoJugadasAntesDePosicionActual = QuitarTextoJugadaActual();
                if (Posicion.Turno == Equipo.Blanco)
                {
                    Jugadas[IndicePosicion / 2].NAGJugadaNegra = (IndicadorNAG)value;
                }
                else
                {
                    Jugadas[IndicePosicion / 2].NAGJugadaBlanca = (IndicadorNAG)value;
                }
                TextoJugadas = TextoJugadas.Insert(longitudTextoJugadasAntesDePosicionActual, Jugadas[indicePosicion / 2].ToString()); // Agrega la jugada con el NAG nuevo.
            }
        }
        #endregion

        #region Constructores
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Partida"/> a partir de un PGN.
        /// </summary>
        /// <param name="PGN">PGN que describe la partida.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        public Partida(string PGN)
        {
            Etiquetas.PropertyChanged += Etiquetas_PropertyChanged;
            int carateresEtiquetas = 0;
            foreach (Match match in Regex.Matches(PGN, patronEtiquetaPGN)) // Obtiene todas las etiquetas del PGN.
            {
                carateresEtiquetas += match.Length;
                if (Enum.TryParse(match.Groups["Nombre"].Value, out EtiquetaPGN etiqueta))
                {
                    Etiquetas[etiqueta] = match.Groups["Valor"].Value;
                }
            }
            PosicionInicial = new Posicion(Etiquetas[EtiquetaPGN.SetUp] == "1" ? Etiquetas[EtiquetaPGN.FEN] : posicionInicialFEN); // Estable la posición inicial predeterminada o la recibida.
            foreach (Match match in Regex.Matches(PGN.Remove(0, carateresEtiquetas), patronJugadaPGN)) // Obtiene todas las jugadas del PGN.
            {
                Jugadas.Add(new JugadaCompleta(Posicion, int.Parse(match.Groups["Numero"].Value), match.Groups["JugadaBlanca"].Value, match.Groups["JugadaNegra"].Value));
                TextoJugadas += Jugadas[Jugadas.Count - 1].ToString();
                TextoJugadasUCI += Jugadas[Jugadas.Count - 1].JugadaBlanca.ToUCI() + " ";
                IndicePosicion++;
                if (Jugadas[Jugadas.Count - 1].JugadaNegra != null)
                {
                    TextoJugadasUCI += Jugadas[Jugadas.Count - 1].JugadaNegra.ToUCI() + " ";
                    indicePosicion++;
                }
                else
                {
                    break; // Si una jugada solo tiene la jugada de las blancas, no puede haber más jugadas detrás, y si hay deben ingnorarse.
                }
            }
            DeterminarResultado(false);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Partida"/> con la posicion inicial.
        /// </summary>
        /// <param name="posicion">Posición inicial.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public Partida(Posicion posicion = null)
        {
            PartidaTerminada += Application.Current.Windows.OfType<MainWindow>().First().Partida_PartidaTerminada;
            Etiquetas.PropertyChanged += Etiquetas_PropertyChanged;
            if (posicion == null)
            {
                PosicionInicial = new Posicion(posicionInicialFEN);
            }
            else
            {
                PosicionInicial = posicion;
                Etiquetas[EtiquetaPGN.SetUp] = "1";
                Etiquetas[EtiquetaPGN.FEN] = PosicionInicial.ToString();
            }
            if (PosicionInicial.Turno == Equipo.Negro)
            {
                Jugadas.Add(new JugadaCompleta(PosicionInicial, 1, null, null));
                IndicePosicion++;
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Partida"/> con los jugadores, el evento, el tiempo y la ronda.
        /// </summary>
        /// <param name="blanco">Jugador que juega con las piezas blancas.</param>
        /// <param name="negro">Jugador que jeuga con las piezas negras.</param>
        /// <param name="evento">Evento en el que se juega la partida.</param>
        /// <param name="tiempo">Tiempo con el que se juega la partida.</param>
        /// <param name="ronda">Ronda del torneo en la que se juega la partida.</param>
        public Partida(Jugador blanco, Jugador negro, Evento evento, string tiempo, int ronda) : this()
        {
            EstablecerJugador(blanco, Equipo.Blanco);
            EstablecerJugador(negro, Equipo.Negro);
            Etiquetas[EtiquetaPGN.Event] = evento.Nombre;
            Etiquetas[EtiquetaPGN.Site] = evento.Sitio;
            Etiquetas[EtiquetaPGN.Section] = evento.Seccion;
            Etiquetas[EtiquetaPGN.EventDate] = evento.Fecha;
            Etiquetas[EtiquetaPGN.TimeControl] = tiempo;
            Etiquetas[EtiquetaPGN.Round] = ronda.ToString();
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Partida"/> con la información en la base de datos.
        /// </summary>
        /// <param name="id">Id de la partida en la base de datos.</param>
        /// <exception cref="InvalidCastException" ></ exception >
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public Partida(int id)
        {
            PartidaTerminada += Application.Current.Windows.OfType<MainWindow>().First().Partida_PartidaTerminada;
            Etiquetas.PropertyChanged += Etiquetas_PropertyChanged;
            Id = id;
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT j.Titulo, j.Elo, j.Nombre_Jugador, u.Na, u.Tipo FROM Partida AS p JOIN Juega AS j ON p.Id = j.Id_Partida JOIN Jugador AS u ON " +
                    $"j.Nombre_Jugador = u.Nombre WHERE p.Id = {Id} ORDER BY j.Color", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        reader.Read();
                        Etiquetas[EtiquetaPGN.WhiteTitle] = reader.IsDBNull(0) ? null : reader.GetString(0);
                        Etiquetas[EtiquetaPGN.WhiteElo] = reader.IsDBNull(1) ? null : reader.GetInt32(1).ToString();
                        Etiquetas[EtiquetaPGN.White] = reader.GetString(2);
                        Etiquetas[EtiquetaPGN.WhiteNA] = reader.IsDBNull(3) ? null : reader.GetString(3);
                        Etiquetas[EtiquetaPGN.WhiteType] = reader.IsDBNull(4) ? null : reader.GetString(4);
                        reader.Read();
                        Etiquetas[EtiquetaPGN.BlackTitle] = reader.IsDBNull(0) ? null : reader.GetString(0);
                        Etiquetas[EtiquetaPGN.BlackElo] = reader.IsDBNull(1) ? null : reader.GetInt32(1).ToString();
                        Etiquetas[EtiquetaPGN.Black] = reader.GetString(2);
                        Etiquetas[EtiquetaPGN.BlackNA] = reader.IsDBNull(3) ? null : reader.GetString(3);
                        Etiquetas[EtiquetaPGN.BlackType] = reader.IsDBNull(4) ? null : reader.GetString(4);
                    }
                }
                using (SqlCommand select = new SqlCommand($"SELECT p.Fecha, p.Resultado, p.Hora, p.Hora_UTC, p.Anotador, p.Fecha_UTC, p.Tiempo, p.Modo, p.Terminacion, p.Nombre_Evento, e.Fecha AS " +
                    $"Fecha_Evento, e.Seccion, e.Sitio, p.Ronda, p.Tablero, p.Etapa FROM Partida AS p JOIN Evento AS e ON p.Nombre_Evento = e.Nombre WHERE p.Id = {id}", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        reader.Read();
                        Etiquetas[EtiquetaPGN.Date] = reader.GetString(0);
                        Etiquetas[EtiquetaPGN.Result] = reader.GetString(1);
                        Etiquetas[EtiquetaPGN.Time] = reader.IsDBNull(2) ? null : reader.GetString(2);
                        Etiquetas[EtiquetaPGN.UTCTime] = reader.IsDBNull(3) ? null : reader.GetString(3);
                        Etiquetas[EtiquetaPGN.Annotator] = reader.IsDBNull(4) ? null : reader.GetString(4);
                        Etiquetas[EtiquetaPGN.UTCDate] = reader.IsDBNull(5) ? null : reader.GetString(5);
                        Etiquetas[EtiquetaPGN.TimeControl] = reader.IsDBNull(6) ? null : reader.GetString(6);
                        Etiquetas[EtiquetaPGN.Mode] = reader.IsDBNull(7) ? null : reader.GetString(7);
                        Etiquetas[EtiquetaPGN.Termination] = reader.IsDBNull(8) ? null : reader.GetString(8);
                        Etiquetas[EtiquetaPGN.Event] = reader.GetString(9);
                        Etiquetas[EtiquetaPGN.EventDate] = reader.IsDBNull(10) ? null : reader.GetString(10);
                        Etiquetas[EtiquetaPGN.Section] = reader.IsDBNull(11) ? null : reader.GetString(11);
                        Etiquetas[EtiquetaPGN.Site] = reader.GetString(12);
                        Etiquetas[EtiquetaPGN.Round] = reader.IsDBNull(13) ? null : reader.GetString(13);
                        Etiquetas[EtiquetaPGN.Board] = reader.IsDBNull(14) ? null : reader.GetInt32(14).ToString();
                        Etiquetas[EtiquetaPGN.Stage] = reader.IsDBNull(15) ? null : reader.GetString(15);
                    }
                }
                using (SqlCommand select = new SqlCommand($"SELECT t.Numero, t.Jugada, t.NAG, t.Comentario, t.Descripcion_Posicion FROM Partida AS p JOIN Tiene AS t ON p.Id = t.Id_Partida WHERE p.Id = " +
                    $"{Id} ORDER BY t.Numero", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PosicionInicial = new Posicion($"{reader.GetString(4)} 0 1");
                            NumeroUltimaPosicionEnBaseDeDatos = -1;
                            while (!reader.IsDBNull(1))
                            {
                                if (reader.GetInt32(0) % 2 != 0)
                                {
                                    Jugadas.Add(new JugadaCompleta(Posicion, Posicion.JugadasPosibles.Find(j => j.ToStringEnIngles(Posicion) == reader.GetString(1))));
                                    Jugadas[Jugadas.Count - 1].NAGJugadaBlanca = reader.IsDBNull(2) ? IndicadorNAG.Nulo : (IndicadorNAG)reader.GetInt32(2);
                                    Jugadas[Jugadas.Count - 1].ComentarioJugadaBlanca = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                    TextoJugadas += Jugadas[Jugadas.Count - 1].ToString(); // Agrega la jugada al string.
                                    TextoJugadasUCI += Jugadas[Jugadas.Count - 1].JugadaBlanca.ToUCI() + " ";
                                }
                                else
                                {
                                    TextoJugadas = TextoJugadas.Substring(0, TextoJugadas.Length - Jugadas[Jugadas.Count - 1].ToString().Length); // Quita la jugada empezada.
                                    Jugadas[Jugadas.Count - 1].CompletarJugada(reader.GetString(1));
                                    Jugadas[Jugadas.Count - 1].NAGJugadaNegra = reader.IsDBNull(2) ? IndicadorNAG.Nulo : (IndicadorNAG)reader.GetInt32(2);
                                    Jugadas[Jugadas.Count - 1].ComentarioJugadaNegra = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                    TextoJugadas += Jugadas[Jugadas.Count - 1].ToString(); // Agrega la jugada completa al string.
                                    TextoJugadasUCI += Jugadas[Jugadas.Count - 1].JugadaNegra.ToUCI() + " ";
                                }
                                indicePosicion++;
                                NumeroUltimaPosicionEnBaseDeDatos = reader.GetInt32(0);
                                if (!reader.Read())
                                {
                                    // Nunca se debería llegar aquí porque si t.Jugada no es null, debería haber otro tiene con la posición luego de esa jugada.
                                    // Pero si no hubiera no se debería lanzar una exepción.
                                    break;
                                }
                            }
                        }
                    }
                }
                DeterminarResultado(false);
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Reliza la jugada indicada.
        /// </summary>
        /// <param name="jugada">Jugada a realizar.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public void RealizarJugada(Jugada jugada)
        {
            if (Posicion.Turno == Equipo.Blanco)
            {
                Jugadas.Add(new JugadaCompleta(Posicion, jugada));
                TextoJugadas += Jugadas[Jugadas.Count - 1].ToString();
                TextoJugadasUCI += Jugadas[Jugadas.Count - 1].JugadaBlanca.ToUCI() + " ";
            }
            else
            {
                if (PosicionInicial.Turno == Equipo.Blanco)
                {
                    TextoJugadas = TextoJugadas.Substring(0, TextoJugadas.Length - Jugadas[Jugadas.Count - 1].ToString().Length); // Quita la jugada empezada.
                }
                Jugadas[Jugadas.Count - 1].CompletarJugada(jugada);
                TextoJugadas += Jugadas[Jugadas.Count - 1].ToString(); // Agrega la jugada completa al string.
                TextoJugadasUCI += Jugadas[Jugadas.Count - 1].JugadaNegra.ToUCI() + " ";
            }
            IndicePosicion++;
            DeterminarResultado(false);
            ActualizarReloj();
            ProximaPropuestaDeTablas--;
            ActualizarPosicionesEnBaseDeDatos();
        }

        /// <summary>
        /// Si la partida está en juego cambia el turno de juego y detiene el reloj si terminó la partida.
        /// </summary>
        private void ActualizarReloj()
        {
            if (EnJuego)
            {
                Reloj?.CambiarTurno();
                if (Etiquetas[EtiquetaPGN.Result] != "*")
                {
                    Terminar();
                }
            }
        }

        /// <summary>
        /// Determina si <see cref="Posicion"/> es la última posición de la partida.
        /// </summary>
        public bool EsUltimaPosicion() => (Jugadas.Count == 0 && IndicePosicion == -1) || (IndicePosicion + 1 == CantidadPosicionesSinContarInicial());

        /// <summary>
        /// Determina si <see cref="Posicion"/> es la primera posición de la partida.
        /// </summary>
        public bool EsPrimeraPosicion() => PosicionInicial.Turno == Equipo.Blanco ? IndicePosicion == -1 : IndicePosicion == 0;

        /// <summary>
        /// Deshace la última jugada.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public void DeshacerJugada()
        {
            if (UltimaPosicion.Turno == Equipo.Blanco)
            {
                //Quita la jugada de las negras del string. Borra todo en caso de que en la posición inicial les toque a las negras porque la partida solo tendría una jugada.
                TextoJugadas = PosicionInicial.Turno == Equipo.Blanco ? TextoJugadas.Substring(0, TextoJugadas.Length - Jugadas[Jugadas.Count - 1].LongitudJugadaNegra) : string.Empty;
                // Quita la jugada de las negras y el espacio al final.
                TextoJugadasUCI = TextoJugadasUCI.Substring(0, TextoJugadasUCI.Length - ((Jugadas[Jugadas.Count - 1].JugadaNegra.Tipo & (TipoJugada)60) != 0 ? 6 : 5));
                Jugadas[Jugadas.Count - 1].DeshacerJugada(); // Quita la jugada de las negras de la lista.
            }
            else
            {
                TextoJugadas = TextoJugadas.Substring(0, TextoJugadas.Length - Jugadas[Jugadas.Count - 1].ToString().Length); // Quita la jugada del string.
                // Quita la jugada de las blancas y el espacio al final.
                TextoJugadasUCI = TextoJugadasUCI.Substring(0, TextoJugadasUCI.Length - ((Jugadas[Jugadas.Count - 1].JugadaBlanca.Tipo & (TipoJugada)60) != 0 ? 6 : 5));
                Jugadas.RemoveAt(Jugadas.Count - 1); // Quita la jugada de la lista.
            }
            indicePosicion--;
            DeterminarResultado();
            ActualizarReloj();
            ActualizarPosicionesEnBaseDeDatos();
        }

        /// <summary>
        /// Asigna a <see cref="IndicePosicion"/> el valor de la posición inicial.
        /// </summary>
        public void IrAlInicio() => IndicePosicion = PosicionInicial.Turno == Equipo.Blanco ? -1 : 0;

        /// <summary>
        /// Obtiene la representación en PGN de la partida.
        /// </summary>
        /// <param name="etiquetasOpcionales">Lista de etiquetas opcionales que se desea incluir.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public string ToPGN(List<EtiquetaPGN> etiquetasOpcionales)
        {
            etiquetasOpcionales.Sort((x, y) => string.CompareOrdinal(Enum.GetName(typeof(EtiquetaPGN), x), Enum.GetName(typeof(EtiquetaPGN), y))); // Ordena las etiquetas PGN opcionales según su nombre.
            StringBuilder builderPartida = new StringBuilder();
            if (etiquetasOpcionales.Exists(e => e == EtiquetaPGN.ECO || e == EtiquetaPGN.NIC || e == EtiquetaPGN.Opening || e == EtiquetaPGN.Variation || e == EtiquetaPGN.SubVariation))
            {
                try
                {
                    DetectarApertura();
                }
                catch
                {
                    MessageBox.Show("No se completará la información etiquetas realacionadsa con la apertura", "Error al detectar la apertura", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            foreach (EtiquetaPGN etiqueta in etiquetasObligatorias.Concat(etiquetasOpcionales))
            {
                builderPartida.AppendFormat("[{0} \"{1}\"]\n", Enum.GetName(typeof(EtiquetaPGN), etiqueta), Etiquetas[etiqueta]); // Agrega la etiqueta.
            }
            builderPartida.AppendLine(); // Separa las etiquetas de las jugadas.
            Jugada.CaracterPieza = CaracterPieza.Ingles;
            int caracteresEnLaLinea = 0;
            foreach (JugadaCompleta jugada in Jugadas)
            {
                string textoJugada = jugada.ToPGN();
                caracteresEnLaLinea += textoJugada.Length;
                if (caracteresEnLaLinea > 80)
                {
                    builderPartida.AppendLine();
                    caracteresEnLaLinea = textoJugada.Length;
                }
                builderPartida.Append(textoJugada);
            }
            Jugada.CaracterPieza = (CaracterPieza)Settings.Default.CaracteresPieza;
            builderPartida.AppendFormat($" {Etiquetas[EtiquetaPGN.Result]}"); // Agrega el resultado.
            return builderPartida.ToString();
        }

        /// <summary>
        /// Comprueba si la partida ha terminado y actualiza <see cref="Etiquetas"/> y la base de datos.
        /// </summary>
        /// <param name="puedeEstablecerEnJuego">Indica si se pondrá "*" en caso de que no haya jaque mate ni tablas.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        private void DeterminarResultado(bool puedeEstablecerEnJuego = true)
        {
            if (UltimaPosicion.HayJaqueMate())
            {
                Etiquetas[EtiquetaPGN.Result] = UltimaPosicion.Turno == Equipo.Blanco ? "0-1" : "1-0";
            }
            else if (UltimaPosicion.HayAhogado() || UltimaPosicion.JugadasSinMoverPeonNiCapturar > 99 || SeRepiteLaUltimaPosicionTresVeces() ||
                (UltimaPosicion.NoPuedeDarMate(Equipo.Blanco) && UltimaPosicion.NoPuedeDarMate(Equipo.Negro)))
            {
                Etiquetas[EtiquetaPGN.Result] = "1/2-1/2";
            }
            else if (puedeEstablecerEnJuego)
            {
                Etiquetas[EtiquetaPGN.Result] = "*";
            }
            if (Id != null)
            {
                using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                {
                    conexion.Open();
                    using (SqlCommand update = new SqlCommand($"UPDATE Partida SET Resultado = '{Etiquetas[EtiquetaPGN.Result]}' WHERE ID = {Id}", conexion))
                    {
                        update.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Determina si se ha repetido la última posición 3 veces.
        /// </summary>
        private bool SeRepiteLaUltimaPosicionTresVeces()
        {
            int repeticiones = 1;
            if (Jugadas.Count > 0)
            {
                // Empieza en la posición anterior a la última y termina en la inicial a menos que encuentre 2 repeticiones antes.
                for (int i = Jugadas.Count * 2 - (Jugadas[Jugadas.Count - 1].JugadaNegra == null ? 3 : 2); i > -2 && repeticiones < 3; i--)
                {
                    if (UltimaPosicion.Equals(i < 0 ? PosicionInicial : Jugadas[i / 2][i % 2]))
                    {
                        repeticiones++;
                    }
                }
                return repeticiones > 2;
            }
            return false;
        }

        /// <summary>
        /// Juega la partida.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public void Jugar()
        {
            InicializarReloj();
            EnJuego = true;
            Reloj?.Iniciar();
        }

        /// <summary>
        /// Inicializa <see cref="Reloj"/> teniendo en cuenta el valor de la etiqueta TimeControl.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        private void InicializarReloj()
        {
            Match match = Regex.Match(Etiquetas[EtiquetaPGN.TimeControl] ?? "", TiempoPGNRule.PatronTimeControlPGN);
            if (!match.Success || match.Groups["desconocido"].Success || match.Groups["ilimitado"].Success)
            {
                Reloj = null;
            }
            else if (match.Groups["muerteSubita"].Success)
            {
                if (match.Groups["parcialAntesDeMuerteSubita"].Success)
                {
                    string[] valoresRelojParcial = match.Groups["parcialAntesDeMuerteSubita"].Value.Split('/', ':');
                    string[] valoresReloj = match.Value.Split(':');
                    Reloj = new RelojParcial(TimeSpan.FromSeconds(int.Parse(valoresRelojParcial[1])), PosicionInicial.Turno == Equipo.Blanco, int.Parse(valoresRelojParcial[0]), (s, e) => Reloj =
                        new Reloj(TimeSpan.FromSeconds(int.Parse(valoresReloj[1])), true, Reloj_TiempoAcabado, true), Reloj_TiempoAcabado);
                }
                else
                {
                    Reloj = new Reloj(TimeSpan.FromSeconds(int.Parse(match.Value)), PosicionInicial.Turno == Equipo.Blanco, Reloj_TiempoAcabado);
                }
            }
            else if (match.Groups["incremental"].Success)
            {
                if (match.Groups["parcialAntesDeIncremental"].Success)
                {
                    string[] valoresRelojParcial = match.Groups["parcialAntesDeIncremental"].Value.Split('/', ':');
                    string[] valoresReloj = match.Value.Split(':');
                    string[] valoresRelojIncremental = valoresReloj[1].Split('+');
                    Reloj = new RelojParcial(TimeSpan.FromSeconds(int.Parse(valoresRelojParcial[1])), PosicionInicial.Turno == Equipo.Blanco, int.Parse(valoresRelojParcial[0]), (s, e) => Reloj =
                        new RelojIncremental(TimeSpan.FromSeconds(int.Parse(valoresRelojIncremental[0])), true, TimeSpan.FromSeconds(int.Parse(valoresRelojIncremental[1])), Reloj_TiempoAcabado, true),
                        Reloj_TiempoAcabado);
                }
                else
                {
                    string[] valoresRelojIncremental = match.Value.Split('+');
                    Reloj = new RelojIncremental(TimeSpan.FromSeconds(int.Parse(valoresRelojIncremental[0])), PosicionInicial.Turno == Equipo.Blanco,
                        TimeSpan.FromSeconds(int.Parse(valoresRelojIncremental[1])), Reloj_TiempoAcabado);
                }
            }
            else
            {
                if (match.Groups["parcialAntesDeArena"].Success)
                {
                    string[] valoresRelojParcial = match.Groups["parcialAntesDeArena"].Value.Split('/', ':');
                    string[] valoresReloj = match.Value.Split(':');
                    Reloj = new RelojParcial(TimeSpan.FromSeconds(int.Parse(valoresRelojParcial[1])), PosicionInicial.Turno == Equipo.Blanco, int.Parse(valoresRelojParcial[0]), (s, e) => Reloj =
                        new RelojArena(TimeSpan.FromSeconds(int.Parse(valoresReloj[1].TrimStart('*'))), true, Reloj_TiempoAcabado, true), Reloj_TiempoAcabado);
                }
                else
                {
                    Reloj = new RelojArena(TimeSpan.FromSeconds(int.Parse(match.Value.TrimStart('*'))), PosicionInicial.Turno == Equipo.Blanco, Reloj_TiempoAcabado);
                }
            }
        }

        /// <summary>
        /// Abandona la partida.
        /// </summary>
        /// <param name="perdedor">Equipo que se rinde.</param>
        public void Abandonar(Equipo perdedor)
        {
            Etiquetas[EtiquetaPGN.Result] = perdedor == Equipo.Blanco ? "0-1" : "1-0";
            Terminar();
        }

        /// <summary>
        /// Empata la partida.
        /// </summary>
        public void AcordarTablas()
        {
            Etiquetas[EtiquetaPGN.Result] = "1/2-1/2";
            Terminar();
        }

        /// <summary>
        /// Inicializa <see cref="Revancha"/> teniendo en cuenta los valores esta partida terminada.
        /// </summary>
        private void InicializarRevancha()
        {
            Revancha = new Partida();
            Revancha.Etiquetas[EtiquetaPGN.White] = Etiquetas[EtiquetaPGN.Black];
            Revancha.Etiquetas[EtiquetaPGN.WhiteTitle] = Etiquetas[EtiquetaPGN.BlackTitle];
            Revancha.Etiquetas[EtiquetaPGN.WhiteElo] = Etiquetas[EtiquetaPGN.BlackElo];
            Revancha.Etiquetas[EtiquetaPGN.WhiteType] = Etiquetas[EtiquetaPGN.BlackType];
            Revancha.Etiquetas[EtiquetaPGN.WhiteNA] = Etiquetas[EtiquetaPGN.BlackNA];
            Revancha.Etiquetas[EtiquetaPGN.Black] = Etiquetas[EtiquetaPGN.White];
            Revancha.Etiquetas[EtiquetaPGN.BlackTitle] = Etiquetas[EtiquetaPGN.WhiteTitle];
            Revancha.Etiquetas[EtiquetaPGN.BlackElo] = Etiquetas[EtiquetaPGN.WhiteElo];
            Revancha.Etiquetas[EtiquetaPGN.BlackType] = Etiquetas[EtiquetaPGN.WhiteType];
            Revancha.Etiquetas[EtiquetaPGN.BlackNA] = Etiquetas[EtiquetaPGN.WhiteNA];
            Revancha.Etiquetas[EtiquetaPGN.SetUp] = Etiquetas[EtiquetaPGN.SetUp];
            Revancha.Etiquetas[EtiquetaPGN.FEN] = Etiquetas[EtiquetaPGN.FEN];
            Revancha.Etiquetas[EtiquetaPGN.TimeControl] = Etiquetas[EtiquetaPGN.TimeControl];
        }

        /// <summary>
        /// Termina la partida, detiene el reloj, prepara la revancha e invoca el evento <see cref="PartidaTerminada"/>.
        /// </summary>
        private void Terminar()
        {
            Reloj?.Detener();
            EnJuego = false;
            InicializarRevancha();
            PartidaTerminada?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Inserta la partida en la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public void InsertarEnBaseDeDatos()
        {
            Evento evento = new Evento(Etiquetas[EtiquetaPGN.Event], Etiquetas[EtiquetaPGN.Site], Etiquetas[EtiquetaPGN.EventDate], Etiquetas[EtiquetaPGN.Section]);
            evento.InsertarEnBaseDeDatos();
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand insert = new SqlCommand($"INSERT INTO Partida OUTPUT INSERTED.Id VALUES ('{Etiquetas[EtiquetaPGN.Date]}', '{Etiquetas[EtiquetaPGN.Result]}'," +
                    $"{Etiquetas[EtiquetaPGN.Time].ToSQL()}, {Etiquetas[EtiquetaPGN.UTCTime].ToSQL()}, {Etiquetas[EtiquetaPGN.UTCTime].ToSQL()}, {Etiquetas[EtiquetaPGN.Annotator].ToSQL()}," +
                    $"{Etiquetas[EtiquetaPGN.Mode].ToSQL()}, {Etiquetas[EtiquetaPGN.Termination].ToSQL()}, '{Etiquetas[EtiquetaPGN.Event]}', '{Etiquetas[EtiquetaPGN.Round]}'," +
                    $"{Etiquetas[EtiquetaPGN.Board].ToSQL()}, {Etiquetas[EtiquetaPGN.Stage].ToSQL()}, {Etiquetas[EtiquetaPGN.TimeControl].ToSQL()})", conexion))
                {
                    Id = (int)insert.ExecuteScalar();
                }
            }
            Jugador jugadorBlanco = new Jugador(Etiquetas[EtiquetaPGN.White], Etiquetas[EtiquetaPGN.WhiteNA], Etiquetas[EtiquetaPGN.WhiteType], int.TryParse(Etiquetas[EtiquetaPGN.WhiteElo], out int
                eloBlanco) ? (int?)eloBlanco : null, Etiquetas[EtiquetaPGN.WhiteTitle]);
            jugadorBlanco.InsertarEnBaseDeDatos();
            Jugador jugadorNegro = new Jugador(Etiquetas[EtiquetaPGN.Black], Etiquetas[EtiquetaPGN.BlackNA], Etiquetas[EtiquetaPGN.BlackType], int.TryParse(Etiquetas[EtiquetaPGN.BlackElo], out int
                eloNegro) ? (int?)eloNegro : null, Etiquetas[EtiquetaPGN.BlackTitle]);
            jugadorNegro.InsertarEnBaseDeDatos();
            Juega juegaBlanco = new Juega((int)Id, Etiquetas[EtiquetaPGN.White], Etiquetas[EtiquetaPGN.WhiteTitle], eloBlanco, "Blanco");
            juegaBlanco.InsertarEnBaseDeDatos();
            Juega juegaNegro = new Juega((int)Id, Etiquetas[EtiquetaPGN.Black], Etiquetas[EtiquetaPGN.BlackTitle], eloNegro, "Negro");
            juegaNegro.InsertarEnBaseDeDatos();
            PosicionInicial.InsertarEnBaseDeDatos();
            Tiene tiene = new Tiene(PosicionInicial.Descripcion, (int)Id, -1);
            if (Jugadas.Count > 0)
            {
                tiene.InicializarJugada(Jugadas[0].JugadaBlanca.ToStringEnIngles(PosicionInicial), (int?)Jugadas[0].NAGJugadaBlanca, Jugadas[0].ComentarioJugadaBlancaSinLlaves);
            }
            tiene.InsertarEnBaseDeDatos();
            NumeroUltimaPosicionEnBaseDeDatos = -1;
            ActualizarPosicionesEnBaseDeDatos();
        }

        /// <summary>
        /// Actualiza la base de datos para indicar las posiciones que tiene actualmente la partida.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public void ActualizarPosicionesEnBaseDeDatos()
        {
            if (Id != null)
            {
                if (NumeroUltimaPosicionEnBaseDeDatos != CantidadPosicionesSinContarInicial() - 1)
                {
                    using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                    {
                        conexion.Open();
                        if (NumeroUltimaPosicionEnBaseDeDatos >= CantidadPosicionesSinContarInicial())
                        {
                            using (SqlCommand delete = new SqlCommand($"DELETE FROM Tiene WHERE Id_Partida = {Id} AND Numero >= {CantidadPosicionesSinContarInicial()}", conexion))
                            {
                                delete.ExecuteNonQuery();
                            }
                            NumeroUltimaPosicionEnBaseDeDatos = CantidadPosicionesSinContarInicial() - 1;
                            using (SqlCommand update = new SqlCommand($"Update Tiene SET Jugada = null, NAG = null, Comentario = null WHERE Id_Partida = {Id} AND Numero = {NumeroUltimaPosicionEnBaseDeDatos}",
                                conexion))
                            {
                                update.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            for (int i = NumeroUltimaPosicionEnBaseDeDatos; i < CantidadPosicionesSinContarInicial(); i++)
                            {
                                Tiene tiene;
                                if (i >= 0)
                                {
                                    Jugadas[i / 2][i % 2].InsertarEnBaseDeDatos();
                                    tiene = new Tiene(Jugadas[i / 2][i % 2].Descripcion, (int)Id, i);
                                }
                                else
                                {
                                    PosicionInicial.InsertarEnBaseDeDatos();
                                    tiene = new Tiene(PosicionInicial.Descripcion, (int)Id, i);
                                }
                                if (CantidadPosicionesSinContarInicial() > i + 1)
                                {
                                    if (i % 2 == 0)
                                    {
                                        tiene.InicializarJugada(Jugadas[i / 2].JugadaNegra.ToStringEnIngles(Jugadas[i / 2][0]), (int?)Jugadas[i / 2].NAGJugadaNegra,
                                            Jugadas[i / 2].ComentarioJugadaNegraSinLlaves);
                                    }
                                    else
                                    {
                                        tiene.InicializarJugada(Jugadas[(i + 1) / 2].JugadaBlanca.ToStringEnIngles(Jugadas[i / 2][1]), (int?)Jugadas[(i + 1) / 2].NAGJugadaBlanca,
                                            Jugadas[(i + 1) / 2].ComentarioJugadaBlancaSinLlaves);
                                    }
                                }
                                tiene.InsertarEnBaseDeDatos();
                                NumeroUltimaPosicionEnBaseDeDatos = tiene.Numero;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene la cantidad de posicines sin contar la posición incial.
        /// </summary>
        /// <returns>Cantidad de posiciones sin contar la posición inical.</returns>
        public int CantidadPosicionesSinContarInicial() => Jugadas.Count > 0 ? (Jugadas.Count * 2 - (Jugadas[Jugadas.Count - 1].JugadaNegra == null ? 1 : 0)) : 0;

        /// <summary>
        /// Quita la jugada que llevó a <see cref="Posicion"/> de <see cref="TextoJugadas"/>.
        /// </summary>
        /// <returns>Posición donde empezaba la jugada.</returns>
        private int QuitarTextoJugadaActual()
        {
            int longitudTextoJugadasAntesDePosicionActual = 0;
            for (int i = 0; i < IndicePosicion / 2; i++)
            {
                longitudTextoJugadasAntesDePosicionActual += Jugadas[i].ToString().Length;
            }
            TextoJugadas = TextoJugadas.Remove(longitudTextoJugadasAntesDePosicionActual, Jugadas[IndicePosicion / 2].ToString().Length); //Quita la jugada con el comentario antiguo.
            return longitudTextoJugadasAntesDePosicionActual;
        }

        /// <summary>
        /// Determina si se cumple una condición en las primeras <paramref name="n"/> jugadas.
        /// </summary>
        /// <param name="condicion">Condición que se tiene que cumplir.</param>
        /// <param name="n">Cantidad de jugadas.</param>
        /// <returns></returns>
        public bool Cumple(Func<Posicion, bool> condicion, int n = 10000)
        {
            for (int i = 0; i < Math.Min(Jugadas.Count, n); i++)
            {
                if (Jugadas[i].Cumple(condicion))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Elimina la partida de la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public void BorrarDeBaseDeDatos()
        {
            if (Id != null)
            {
                using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                {
                    conexion.Open();
                    using (SqlCommand delete = new SqlCommand($"DELETE FROM Partida WHERE Id = {Id}", conexion))
                    {
                        delete.ExecuteNonQuery();
                    }
                }
                Id = null;
            }
        }

        /// <summary>
        /// Obtiene las partidas de la base de datos
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public static List<Partida> ObtenerPartidasDesdeBaseDeDatos()
        {
            List<Partida> partidas = new List<Partida>();
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand("SELECT Id FROM Partida", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            partidas.Add(new Partida(reader.GetInt32(0)));
                        }
                    }
                }
            }
            return partidas;
        }

        /// <summary>
        /// Establece un jugador en <see cref="Etiquetas"/>.
        /// </summary>
        /// <param name="jugador">Jugador que juega la partida.</param>
        /// <param name="equipo">Equipo con el que juega.</param>
        public void EstablecerJugador(Jugador jugador, Equipo equipo)
        {
            if (jugador != null)
            {
                if (equipo == Equipo.Blanco)
                {
                    Etiquetas[EtiquetaPGN.White] = jugador.Nombre;
                    Etiquetas[EtiquetaPGN.WhiteElo] = jugador.Elo.ToString();
                    Etiquetas[EtiquetaPGN.WhiteNA] = jugador.Na;
                    Etiquetas[EtiquetaPGN.WhiteTitle] = jugador.Titulo;
                    Etiquetas[EtiquetaPGN.WhiteType] = jugador.Tipo;
                }
                else
                {
                    Etiquetas[EtiquetaPGN.Black] = jugador.Nombre;
                    Etiquetas[EtiquetaPGN.BlackElo] = jugador.Elo.ToString();
                    Etiquetas[EtiquetaPGN.BlackNA] = jugador.Na;
                    Etiquetas[EtiquetaPGN.BlackTitle] = jugador.Titulo;
                    Etiquetas[EtiquetaPGN.BlackType] = jugador.Tipo;
                }
            }
        }

        /// <summary>
        /// Determina si un jugador ganó la partida.
        /// </summary>
        /// <param name="nombre">Nombre del jugador que se quiere saber si ganó la partida.</param>
        public bool Gano(string nombre) => (Etiquetas[EtiquetaPGN.Result] == "1-0" && Etiquetas[EtiquetaPGN.White] == nombre) || (Etiquetas[EtiquetaPGN.Result] == "0-1" &&
            Etiquetas[EtiquetaPGN.Black] == nombre);

        /// <summary>
        /// Determina si un jugador perdió la partida.
        /// </summary>
        /// <param name="nombre">Nombre del jugador que se quiere saber si perdió la partida.</param>
        public bool Perdio(string nombre) => (Etiquetas[EtiquetaPGN.Result] == "0-1" && Etiquetas[EtiquetaPGN.White] == nombre) || (Etiquetas[EtiquetaPGN.Result] == "1-0" &&
            Etiquetas[EtiquetaPGN.Black] == nombre);

        /// <summary>
        /// Determina si un jugador participó en la partida.
        /// </summary>
        /// <param name="nombre">Nombre del jugador que se quiere saber si participó.</param>
        public bool Jugo(string nombre) => Etiquetas[EtiquetaPGN.White] == nombre || Etiquetas[EtiquetaPGN.Black] == nombre;

        /// <summary>
        /// Devuelve los puntos que obtuvo un jugador en la partida.
        /// </summary>
        /// <param name="nombre">Nombre de jugador del que se quiere saber cuántos puntos obtuvo.</param>
        public double Puntos(string nombre) => Etiquetas[EtiquetaPGN.Result] == "1/2-1/2" ? .5 : (Gano(nombre) ? 1 : 0);

        /// <summary>
        /// Si está en la base de datos, detecta la apertura a la que pertenece esta partida.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        private void DetectarApertura()
        {
            if (Id != null)
            {
                using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                {
                    conexion.Open();
                    using (SqlCommand select = new SqlCommand($"SELECT c.Id_Apertura FROM (SELECT t.Id_Partida, t.Numero, MAX(t.Numero) OVER (PARTITION BY t.Id_Partida) AS NumeroMaximo, " +
                        $"p.Id_Apertura FROM Tiene AS t JOIN Posicion AS p ON t.Descripcion_Posicion = p.Descripcion WHERE p.Id_Apertura IS NOT NULL) AS c WHERE c.Numero = c.NumeroMaximo AND " +
                        $"c.Id_Partida = {Id}", conexion))
                    {
                        using (SqlDataReader reader = select.ExecuteReader())
                        {
                            reader.Read();
                            Apertura subVariante = new Apertura(reader.GetInt32(0));
                            Apertura variante = subVariante.Variante ?? subVariante;
                            Apertura apertura = variante.Variante ?? variante;
                            Etiquetas[EtiquetaPGN.ECO] = subVariante.ECO ?? variante.ECO ?? apertura.ECO;
                            Etiquetas[EtiquetaPGN.NIC] = subVariante.NIC ?? variante.NIC ?? apertura.NIC;
                            Etiquetas[EtiquetaPGN.Opening] = apertura.Nombre;
                            Etiquetas[EtiquetaPGN.SubVariation] = subVariante.Nombre;
                            Etiquetas[EtiquetaPGN.Variation] = variante.Nombre;
                        }
                    }
                }
            }
        }
        #endregion

        #region Controladores de eventos
        private void Etiquetas_PropertyChanged(object sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Etiquetas)));

        private void Reloj_TiempoAcabado(object sender, EventArgs e)
        {
            Etiquetas[EtiquetaPGN.Result] = UltimaPosicion.Turno == Equipo.Blanco ? "0-1" : "1-0";
            Terminar();
        }
        #endregion
    }
}
