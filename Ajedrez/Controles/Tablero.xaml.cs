using Ajedrez.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Ajedrez.Controles
{
    /// <summary>
    /// Lógica de interacción para Tablero.xaml
    /// </summary>
    public partial class Tablero : UserControl, INotifyPropertyChanged
    {
        #region Atributos
        /// <summary>
        /// Casilla de origen de la jugada.
        /// </summary>
        private Button casillaOrigen;

        /// <summary>
        /// Grosor del borde de la casilla seleccionada.
        /// </summary>
        private readonly Thickness thicknessBordeCasillaSeleccionada = new Thickness(4);

        /// <summary>
        /// Color que tenía el borde de una casilla antes de ser seleccionada.
        /// </summary>
        private Brush brushOriginal;

        /// <summary>
        /// Grosor que tenía el borde de una casilla antes de ser seleccionada.
        /// </summary>
        private Thickness thicknessOriginal;

        /// <summary>
        /// Arreglo con los botones que representan cada casilla.
        /// </summary>
        private readonly Button[] casillas;

        /// <summary>
        /// Jugada que se está intentando realizar.
        /// </summary>
        private Jugada intento;

        /// <summary>
        /// Si le toca jugar al usuario.
        /// </summary>
        private bool esTurnoUsuario = true;
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Tiene lugar cuando se realiza una jugada.
        /// </summary>
        public event EventHandler JugadaRealizada;
        #endregion

        #region Propiedades de dependencia
        /// <summary>
        /// Identifica la propiedad de dependencia <see cref="ColorCasillasBlancas"/>.
        /// </summary>
        public static readonly DependencyProperty ColorCasillasBlancasProperty = DependencyProperty.Register("ColorCasillasBlancas", typeof(Brush), typeof(Tablero), new
            PropertyMetadata(Brushes.LightGray));

        /// <summary>
        /// Identifica la propiedad de dependencia <see cref="ColorCasillasNegras"/>.
        /// </summary>
        public static readonly DependencyProperty ColorCasillasNegrasProperty = DependencyProperty.Register("ColorCasillasNegras", typeof(Brush), typeof(Tablero), new
            PropertyMetadata(Brushes.DarkGray));

        /// <summary>
        /// Identifica la propiedad de dependencia <see cref="Partida"/>.
        /// </summary>
        public static readonly DependencyProperty PartidaProperty = DependencyProperty.Register("Partida", typeof(Partida), typeof(Tablero), new FrameworkPropertyMetadata(new Partida(),
            Partida_PropertyChanged));

        /// <summary>
        /// Identifica la propiedad de dependecia <see cref="ColorBordeCasillaSeleccionada"/>.
        /// </summary>
        public static readonly DependencyProperty ColorBordeCasillaSeleccionadaProperty = DependencyProperty.Register("ColorBordeCasillaSeleccionada", typeof(Brush), typeof(Tablero),
            new PropertyMetadata(Brushes.Green));
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece el color que tendrán las casillas del mismo color que h1.
        /// </summary>
        public Brush ColorCasillasBlancas
        {
            get => (Brush)GetValue(ColorCasillasBlancasProperty);
            set => SetValue(ColorCasillasBlancasProperty, value);
        }

        /// <summary>
        /// Obtiene o establece el color que tendrán las casillas del mismo color que h2.
        /// </summary>
        public Brush ColorCasillasNegras
        {
            get => (Brush)GetValue(ColorCasillasNegrasProperty);
            set => SetValue(ColorCasillasNegrasProperty, value);
        }

        /// <summary>
        /// Obtiene o establece la partida que se está representando en el tablero.
        /// </summary>
        public Partida Partida
        {
            get => (Partida)GetValue(PartidaProperty);
            set => SetValue(PartidaProperty, value);
        }

        /// <summary>
        /// Obtiene o establece el color del borde de la casilla seleccionada.
        /// </summary>
        public Brush ColorBordeCasillaSeleccionada
        {
            get => (Brush)GetValue(ColorBordeCasillaSeleccionadaProperty);
            set => SetValue(ColorBordeCasillaSeleccionadaProperty, value);
        }

        /// <summary>
        /// Obtiene o establece los motores que juegan con las piezas de cada equipo.
        /// </summary>
        public Dictionary<Equipo, Motor> Motor { get; private set; } = new Dictionary<Equipo, Motor> { { Equipo.Blanco, null }, { Equipo.Negro, null } };
        #endregion

        #region Constructores
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Tablero"/>.
        /// </summary>
        public Tablero()
        {
            InitializeComponent();
            casillas = new Button[64]{ a1, b1, c1, d1, e1, f1, g1, h1, a2, b2, c2, d2, e2, f2, g2, h2, a3, b3, c3, d3, e3, f3, g3, h3, a4, b4, c4, d4, e4, f4, g4, h4, a5, b5, c5, d5, e5, f5, g5, h5,
                a6, b6, c6, d6, e6, f6, g6, h6, a7, b7, c7, d7, e7, f7, g7, h7, a8, b8, c8, d8, e8, f8, g8, h8};
            MostrarPosicion();
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Muestra en el control todas las piezas.
        /// </summary>
        public void MostrarPosicion()
        {
            VaciarTablero();
            for (int i = 0; i < 2; i++) // Para cada color
            {
                for (int j = 2; j < 8; j++) // Para cada tipo de pieza
                {
                    MostrarPieza((Pieza)j, (Pieza)i);
                }
            }
        }

        /// <summary>
        /// Quita todas las piezas del tablero.
        /// </summary>
        private void VaciarTablero()
        {
            foreach (Button casilla in casillas)
            {
                casilla.Content = null;
            }
        }

        /// <summary>
        /// Muestra un tipo de pieza en el control.
        /// </summary>
        /// <param name="pieza">Pieza a mostrar.</param>
        /// <param name="color">Color de la pieza a mostrar.</param>
        private void MostrarPieza(Pieza pieza, Pieza color)
        {
            ulong bitboardPieza = Partida.Posicion[pieza] & Partida.Posicion[color]; // Bitboard de la pieza a mostrar.
            ulong i = bitboardPieza & ~(bitboardPieza - 1); // Bitboard con solo el primer 1 (si hay alguno).
            while (i != 0) // Mientras haya algún bit 1
            {
                int posicionI = Bits.IndiceBitMenosSignificativo(i); // Posición (0..63) de la pieza a mostrar.
                casillas[posicionI].Content = pieza.ToUserControl(color);
                bitboardPieza ^= i; // Quita el bit 1 ya utilizado.
                i = bitboardPieza & ~(bitboardPieza - 1); // Asigna a i el siguiente bit 1 (si queda alguno).
            }

        }

        /// <summary>
        /// Muestra una jugada.
        /// </summary>
        /// <param name="jugada">Jugada a mostrar.</param>
        private void MostrarJugada(Jugada jugada)
        {
            if (jugada.Tipo == TipoJugada.Enroque)
            {
                if (Partida.Posicion.Turno == Equipo.Blanco)
                {
                    e1.Content = null;
                    if (jugada.Destino % 8 > 3) // Si es enroque corto
                    {
                        h1.Content = null;
                        g1.Content = Pieza.Rey.ToUserControl(Pieza.Blanca);
                        f1.Content = Pieza.Torre.ToUserControl(Pieza.Blanca);
                    }
                    else
                    {
                        a1.Content = null;
                        c1.Content = Pieza.Rey.ToUserControl(Pieza.Blanca); 
                        d1.Content = Pieza.Torre.ToUserControl(Pieza.Blanca);
                    }
                }
                else
                {
                    e8.Content = null;
                    if (jugada.Destino % 8 > 3) // Si es enroque corto
                    {
                        h8.Content = null;
                        g8.Content = Pieza.Rey.ToUserControl(Pieza.Negra);
                        f8.Content = Pieza.Torre.ToUserControl(Pieza.Negra);
                    }
                    else
                    {
                        a8.Content = null;
                        c8.Content = Pieza.Rey.ToUserControl(Pieza.Negra);
                        d8.Content = Pieza.Torre.ToUserControl(Pieza.Negra);
                    }
                }
                return; // No hay nada más que hacer si la jugada era un enroque.
            }
            casillas[jugada.Origen].Content = null;
            if ((jugada.Tipo & (TipoJugada)60) != 0) // Si es una coronación
            {
                if ((jugada.Tipo & TipoJugada.PromocionDama) != 0) // Si corona dama.
                {
                    casillas[jugada.Destino].Content = Pieza.Dama.ToUserControl(Partida.Posicion.Turno.ToPieza());
                }
                else if ((jugada.Tipo & TipoJugada.PromocionTorre) != 0) // Si corona torre.
                {
                    casillas[jugada.Destino].Content = Pieza.Torre.ToUserControl(Partida.Posicion.Turno.ToPieza());
                }
                else if ((jugada.Tipo & TipoJugada.PromocionAlfil) != 0) // Si corona alfil.
                {
                    casillas[jugada.Destino].Content = Pieza.Alfil.ToUserControl(Partida.Posicion.Turno.ToPieza());
                }
                else // Si corona caballo.
                {
                    casillas[jugada.Destino].Content = Pieza.Caballo.ToUserControl(Partida.Posicion.Turno.ToPieza());
                }
                return; // No hay nada más que hacer si la jugada era una coronación.
            }
            if ((jugada.Tipo & TipoJugada.CapturaAlPaso) != 0) // Si es una captura al paso
            {
                casillas[Partida.Posicion.Turno == Equipo.Blanco ? jugada.Destino - 8 : jugada.Destino + 8].Content = null; // Quita la pieza capturada al paso.
            }
            casillas[jugada.Destino].Content = jugada.Pieza.ToUserControl(Partida.Posicion.Turno.ToPieza()); // Pone la pieza en el destino.
        }

        /// <summary>
        /// Pone en juego la partida.
        /// </summary>
        public void Jugar()
        {
            if (Motor[Equipo.Blanco] != null)
            {
                Motor[Equipo.Blanco].Equipo = Equipo.Blanco;
                Motor[Equipo.Blanco].JugadaRealizada += Motor_JugadaRealizada;
                Motor[Equipo.Blanco].NombreIndicado += Motor_NombreIndicado;
                Motor[Equipo.Blanco].NuevaPartida(Partida);
                Partida.Etiquetas[EtiquetaPGN.WhiteType] = "program";
            }
            if (Motor[Equipo.Negro] != null)
            {
                Motor[Equipo.Negro].Equipo = Equipo.Negro;
                Motor[Equipo.Negro].JugadaRealizada += Motor_JugadaRealizada;
                Motor[Equipo.Negro].NombreIndicado += Motor_NombreIndicado;
                Motor[Equipo.Negro].NuevaPartida(Partida);
                Partida.Etiquetas[EtiquetaPGN.BlackType] = "program";
            }
            Partida.Jugar();
            PedirJugadaAlMotorSiLeTocaJugar();
        }

        /// <summary>
        /// Termina una jugada que incluye una coronación.
        /// </summary>
        /// <param name="coronacion">Tipo de coronación.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        private void FinalizarCoronación(TipoJugada coronacion)
        {
            Jugada jugada = Partida.Posicion.JugadasPosibles.Find(j => j.Equals(intento) && ((j.Tipo & coronacion) != 0)); // Obtiene la jugada correspondiente de la lista de jugadas posibles
            MostrarJugada(jugada);
            Partida.RealizarJugada(jugada);
            QuitarCasillaOrigen();
            PedirJugadaAlMotorSiLeTocaJugar();
            JugadaRealizada?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Realiza un jugada si es posible.
        /// </summary>
        /// <param name="jugadaAIntentar">Jugada que se quiere realizar.</param>
        /// <param name="coronacion">Letra que indica que pieza coronar.</param>
        /// <returns>True si debe debe detenerse la ejecución de <see cref="MoverPieza_Executed(object, RoutedEventArgs)"/></returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public void RealizarJugada(Jugada jugadaAIntentar, char coronacion = ' ')
        {
            Jugada jugada = Partida.Posicion.JugadasPosibles.Find(j => j.Equals(jugadaAIntentar)); // Obtiene la jugada correspondiente de la lista de jugadas posibles.
            if (jugada != null) // Si es una jugada valida.
            {
                if ((jugada.Tipo & (TipoJugada)60) != 0) // Si es una coronación
                {
                    intento = jugadaAIntentar;
                    if (coronacion != ' ')
                    {
                        FinalizarCoronación(CharToTipoPromocion(coronacion));
                        return;
                    }
                    if (!Settings.Default.CoronarSiempreDama)
                    {
                        // Crea los items del menú para elgeir que coronar.
                        MenuItem coronarDama = new MenuItem { Icon = Pieza.Dama.ToUserControl(Partida.Posicion.Turno.ToPieza()), Header = "Dama" };
                        MenuItem coronarTorre = new MenuItem { Icon = Pieza.Torre.ToUserControl(Partida.Posicion.Turno.ToPieza()), Header = "Torre" };
                        MenuItem coronarAlfil = new MenuItem { Icon = Pieza.Alfil.ToUserControl(Partida.Posicion.Turno.ToPieza()), Header = "Alfil" };
                        MenuItem coronarCaballo = new MenuItem { Icon = Pieza.Caballo.ToUserControl(Partida.Posicion.Turno.ToPieza()), Header = "Caballo" };
                        coronarDama.Click += CoronarDama_Click;
                        coronarTorre.Click += CoronarTorre_Click;
                        coronarAlfil.Click += CoronarAlfil_Click;
                        coronarCaballo.Click += CoronarCaballo_Click;
                        // Crea y muestra el menú para elegir que coronar.
                        ContextMenu menuCoronar = new ContextMenu { Items = { coronarDama, coronarTorre, coronarAlfil, coronarCaballo }, PlacementTarget = casillas[jugada.Destino], IsOpen = true };
                        return;
                    }
                    jugada = Partida.Posicion.JugadasPosibles.Find(j => j.Equals(jugadaAIntentar) && j.Tipo == TipoJugada.PromocionDama); // Elige la coronación de dama sin preguntar al usuario
                }
                MostrarJugada(jugada);
                Partida.RealizarJugada(jugada);
                PedirJugadaAlMotorSiLeTocaJugar();
                JugadaRealizada?.Invoke(this, new EventArgs());
            }
            QuitarCasillaOrigen();
        }

        /// <summary>
        /// Devuelve el color y grosor normal al borde de la casilla que se había seleccionada y pone en null <see cref="casillaOrigen"/>.
        /// </summary>
        private void QuitarCasillaOrigen()
        {
            if (casillaOrigen != null)
            {
                casillaOrigen.BorderBrush = brushOriginal;
                casillaOrigen.BorderThickness = thicknessOriginal;
                casillaOrigen = null;
            }
        }

        /// <summary>
        /// Si es el turno del motor, le pide una jugada y evita que mueva el usuario.
        /// </summary>
        private void PedirJugadaAlMotorSiLeTocaJugar()
        {
            if (Motor[Partida.Posicion.Turno] != null && Partida.EnJuego) // Si es null significa que el usuario controla esas piezas.
            {
                esTurnoUsuario = false;
                Motor[Partida.Posicion.Turno].RealizarJugada();
            }
            else
            {
                esTurnoUsuario = true;
            }
        }

        /// <summary>
        /// Obtiene el TipoJugada que representa la promoción indicada por un caracter.
        /// </summary>
        /// <param name="caracter">Caracter que indica el tipo de promoción.</param>
        public static TipoJugada CharToTipoPromocion(char caracter)
        {
            switch (caracter)
            {
                case 'q':
                    return TipoJugada.PromocionDama;
                case 'r':
                    return TipoJugada.PromocionTorre;
                case 'b':
                    return TipoJugada.PromocionAlfil;
                case 'n':
                    return TipoJugada.PromocionCaballo;
                default:
                    return TipoJugada.Imposible;
            }
        }
        #endregion

        #region Controladores de evento
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Partida.EsUltimaPosicion() && Partida.Etiquetas[EtiquetaPGN.Result] == "*" && esTurnoUsuario)
            {
                Button casillaDestino = sender as Button;
                if (casillaOrigen == null)
                {
                    if (casillaDestino.Content != null)
                    {
                        casillaOrigen = casillaDestino;
                        brushOriginal = casillaOrigen.BorderBrush;
                        thicknessOriginal = casillaOrigen.BorderThickness;
                        casillaOrigen.BorderBrush = ColorBordeCasillaSeleccionada;
                        casillaOrigen.BorderThickness = thicknessBordeCasillaSeleccionada;
                    }
                }
                else if (casillaOrigen != casillaDestino)
                {
                    RealizarJugada(new Jugada(casillaOrigen.Name, casillaDestino.Name));
                }
                else
                {
                    QuitarCasillaOrigen();
                }
            }
        }

        private void CoronarCaballo_Click(object sender, RoutedEventArgs e) => FinalizarCoronación(TipoJugada.PromocionCaballo);

        private void CoronarAlfil_Click(object sender, RoutedEventArgs e) => FinalizarCoronación(TipoJugada.PromocionAlfil);

        private void CoronarTorre_Click(object sender, RoutedEventArgs e) => FinalizarCoronación(TipoJugada.PromocionTorre);

        private void CoronarDama_Click(object sender, RoutedEventArgs e) => FinalizarCoronación(TipoJugada.PromocionDama);

        private static void Partida_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Tablero tablero)
            {
                tablero.PropertyChanged?.Invoke(tablero, new PropertyChangedEventArgs(nameof(Partida)));
                Partida partidaAnterior = e.OldValue as Partida;
                if (partidaAnterior.Revancha == null || partidaAnterior.Etiquetas[EtiquetaPGN.BlackType] != "program")
                {
                    tablero.Motor[Equipo.Blanco] = null;
                }
                if (partidaAnterior.Revancha == null || partidaAnterior.Etiquetas[EtiquetaPGN.WhiteType] != "program")
                {
                    tablero.Motor[Equipo.Negro] = null;
                }
            }
        }

        private void Motor_JugadaRealizada(object sender, (Jugada, char) e)
        {
            Dispatcher.Invoke(() => RealizarJugada(e.Item1, e.Item2));
        }

        private void Motor_NombreIndicado(object sender, (Equipo, string) e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.Item1 == Equipo.Blanco)
                {
                    Partida.Etiquetas[EtiquetaPGN.White] = e.Item2;
                }
                else
                {
                    Partida.Etiquetas[EtiquetaPGN.Black] = e.Item2;
                }
            });
        }

        private void Button_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(Button)) is Button origen && sender is Button destino)
            {
                RealizarJugada(new Jugada(origen.Name, destino.Name));
            }
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Settings.Default.MoverArrastrando && sender is Button origen && origen.Content != null)
            {
                DragDrop.DoDragDrop(origen, origen, DragDropEffects.Move);
            }
        }
        #endregion
    }
}
