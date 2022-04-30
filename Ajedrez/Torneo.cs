using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Clase base para los torneos.
    /// </summary>
    public abstract class Torneo : Window
    {
        #region Atributos
        /// <summary>
        /// Índice de la partida actual en <see cref="Partidas"/>,
        /// </summary>
        protected int indicePartida = 0;

        /// <summary>
        /// Diferencias con el promedio de elo para calcular el rendimiento teniendo en cuenta el porcentaje de puntuación máxima obtenido,
        /// </summary>
        protected readonly int[] diferencias = { -800, -677, -589, -538, -501, -470, -444, -422, -401, -383, -366, -351, -336, -322, -309, -296, -284, -273, -262, -251, -240, -230, -220, -211, -202,
            -193, -184, -175, -166, -158, -149, -141, -133, -125, -117, -110, -102, -95, -87, -80, -72, -65, -57, -50, -43, -36, -29, -21, -14, -7, 0, 7, 14, 21, 29, 36, 43, 50, 57, 65, 72, 80, 87, 95,
            102, 110, 117, 125, 133, 141, 149, 158, 166, 175, 184, 193, 202, 211, 220, 230, 240, 251, 262, 273, 284, 296, 309, 322, 336, 351, 366, 383, 401, 422, 444, 470, 501, 538, 589, 677, 800 };
        // Según tabla 8.1a en https://handbook.fide.com/chapter/B022017.

        /// <summary>
        /// Elo promedio de todos los participantes que tienen elo.
        /// </summary>
        protected int eloPromedio;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece el evento del torneo.
        /// </summary>
        public Evento Evento { get; set; } = new Evento();

        /// <summary>
        /// Obtiene las partidas del torneo.
        /// </summary>
        public abstract List<Partida> Partidas { get; }

        /// <summary>
        /// Obtiene o establece el tiempo con el que se juega el torneo.
        /// </summary>
        public string Tiempo { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor para ser llamado por las clases derivadas con la ventana que las posee.
        /// </summary>
        /// <param name="owner">Ventana a la que va a pertenecer la nueva ventana.</param>
        protected Torneo(MainWindow owner)
        {
            Owner = owner;
            Evento.Fecha = DateTime.Now.ToString("yyyy.MM.dd");
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Inicializa los valores que deben inicilizarse luego de llamar a la ventana de configuración.
        /// </summary>
        public virtual void TerminarConfiguracion()
        {
            Title = Evento.Nombre;
            PrepararSiguienteRonda();
        }

        /// <summary>
        /// Prepara la siguiente partida.
        /// </summary>
        /// <param name="partida">Partida que terminó.</param>
        public virtual void PreperarSiguientePartida(Partida partida)
        {
            indicePartida++;
            if (NuevaRonda())
            {
                PrepararSiguienteRonda();
            }
            try
            {
                ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Prepara la siguiente ronda o finaliza el torneo.
        /// </summary>
        protected abstract void PrepararSiguienteRonda();

        /// <summary>
        /// Determina si se debe empezar una nueva ronda.
        /// </summary>
        protected abstract bool NuevaRonda();

        /// <summary>
        /// Pone en juego una partida.
        /// </summary>
        /// <param name="partida">Partida que se va a jugar.</param>
        protected void Jugar(Partida partida)
        {
            partida.Etiquetas[EtiquetaPGN.Date] = DateTime.Now.ToString("yyyy.MM.dd");
            partida.Etiquetas[EtiquetaPGN.Time] = DateTime.Now.ToString("HH.mm.ss");
            partida.Etiquetas[EtiquetaPGN.UTCDate] = DateTime.UtcNow.ToString("yyyy.MM.dd");
            partida.Etiquetas[EtiquetaPGN.UTCTime] = DateTime.UtcNow.ToString("HH.mm.ss");
            MainWindow mainWindow = Owner as MainWindow;
            mainWindow.TableroAjedrez.Partida = partida;
            mainWindow.TableroAjedrez.Partida.Jugar();
            Hide();
            mainWindow.Activate();
        }

        /// <summary>
        /// Inserta el torneo en la bsae de datos.
        /// </summary>
        protected void InsertarEnBaseDeDatos()
        {
            try
            {
                Evento.InsertarEnBaseDeDatos();
                foreach (Partida partida in Partidas)
                {
                    partida.InsertarEnBaseDeDatos();
                }
                MessageBox.Show("El torneo y sus partidas se han guardado correctamente en la base de datos", "Torneo guardado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("No se ha podido guardar el torneo en la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion

        #region Controladores de eventos
        protected void Jugar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = indicePartida < Partidas?.Count;

        protected void Jugar_Executed(object sender, ExecutedRoutedEventArgs e) => Jugar(Partidas[indicePartida]);

        protected void Torneo_Closed(object sender, EventArgs e)
        {
            MainWindow mainWindow = Owner as MainWindow;
            mainWindow.Torneo = null;
            mainWindow.TableroAjedrez.Partida = new Partida();
        }
        #endregion
    }
}
