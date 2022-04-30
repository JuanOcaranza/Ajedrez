using Ajedrez.Controles;
using Ajedrez.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para ElegirRivales.xaml
    /// </summary>
    public partial class ElegirRivales : Window
    {
        #region Atributos
        /// <summary>
        /// Tablero en el que jugarán los rivales.
        /// </summary>
        private readonly Tablero Tablero;

        /// <summary>
        /// Directorio donde están los motores.
        /// </summary>
        private readonly DirectoryInfo directorioMotores = new DirectoryInfo(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Motores");

        /// <summary>
        /// Jugadores que pueden ser elegidos.
        /// </summary>
        private readonly List<Jugador> jugadores = new List<Jugador>();

        /// <summary>
        /// Tipo de cada jugador
        /// </summary>
        private readonly ModoDeJuego modo;
        #endregion

        #region Propiedad
        /// <summary>
        /// Obtiene o establece los jugadores que podrían usar las piezas blancas.
        /// </summary>
        public ObservableCollection<string> OpcionesBlanco { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Obtiene o establece los jugadores que podrían usar las piezas negras.
        /// </summary>
        public ObservableCollection<string> OpcionesNegro { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Obtiene un valor que indica si el jugador de piezas blancas es un humano.
        /// </summary>
        public bool BlancoEsHumano => modo == ModoDeJuego.HumanoContraHumano || modo == ModoDeJuego.HumanoContraPrograma;

        /// <summary>
        /// Obtiene un valor que indica si el jugador de piezas negras es un humano.
        /// </summary>
        public bool NegroEsHumano => modo == ModoDeJuego.HumanoContraHumano || modo == ModoDeJuego.ProgramaContraHumano;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clse <see cref="ElegirRivales"/> con el tablero.
        /// </summary>
        /// <param name="tablero">Tablero en el que jugaran los rivales.</param>
        /// <param name="modoDeJuego">Mode de juego con el que jugarán.</param>
        public ElegirRivales(Tablero tablero, ModoDeJuego modoDeJuego)
        {
            InitializeComponent();
            grid.DataContext = this;
            Tablero = tablero;
            modo = modoDeJuego;
            if (BlancoEsHumano)
            {
                OpcionesBlanco.Add("Anónimo");
            }
            else
            {
                foreach (FileInfo archivo in directorioMotores.GetFiles())
                {
                    OpcionesBlanco.Add(archivo.Name);
                }
            }
            if (NegroEsHumano)
            {
                OpcionesNegro.Add("Anónimo");
            }
            else
            {
                foreach (FileInfo archivo in directorioMotores.GetFiles())
                {
                    OpcionesNegro.Add(archivo.Name);
                }
            }
            if (modo != ModoDeJuego.ProgamaContraPrograma)
            {
                try
                {
                    using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                    {
                        conexion.Open();
                        using (SqlCommand select = new SqlCommand("SELECT * FROM Jugador WHERE Tipo = 'human' OR Tipo IS NULL", conexion))
                        {
                            using (SqlDataReader reader = select.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    jugadores.Add(new Jugador(reader.GetString(0), reader.IsDBNull(1) ? null : reader.GetString(1), reader.IsDBNull(2) ? null : reader.GetString(2), reader.IsDBNull(3) ?
                                        null : (int?)reader.GetInt32(3), reader.IsDBNull(4) ? null : reader.GetString(4)));
                                }
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("No se han podidio obtener los jugadores de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        #endregion

        #region Controladores de eventos
        private void Aceptar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = comboBoxBlanco.SelectedItem != null && comboBoxNegro.SelectedItem != null;

        private void Aceptar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (BlancoEsHumano)
                {
                    Tablero.Partida.EstablecerJugador(jugadores.Find(j => j.Nombre == comboBoxBlanco.SelectedItem.ToString()), Equipo.Blanco);
                }
                else
                {
                    Tablero.Motor[Equipo.Blanco] = new Motor(string.Format("{0}\\{1}", directorioMotores.FullName, comboBoxBlanco.SelectedItem.ToString()), Equipo.Blanco);
                }
                if (NegroEsHumano)
                {
                    Tablero.Partida.EstablecerJugador(jugadores.Find(j => j.Nombre == comboBoxNegro.SelectedItem.ToString()), Equipo.Negro);
                }
                else
                {
                    Tablero.Motor[Equipo.Negro] = new Motor(string.Format("{0}\\{1}", directorioMotores.FullName, comboBoxNegro.SelectedItem.ToString()), Equipo.Negro);
                }
                DialogResult = true;
            }
            catch
            {
                MessageBox.Show("Se producido un error al intentar establecer uno de los participantes", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
            }
            Close();
        }

        private void Aceptar_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isEnabled)
            {
                if (textBlockError != null)
                {
                    textBlockError.Visibility = isEnabled ? Visibility.Hidden : Visibility.Visible;
                }
            }
        }

        private void SeleccionarJugador_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = modo != ModoDeJuego.ProgamaContraPrograma;

        private void SeleccionarJugador_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ElegirJugador elegirJugador = new ElegirJugador(jugadores, this);
            try
            {
                elegirJugador.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close(); // Si no se pueden seleccionar los jugadores no se va a poder jugar la partida.
            }
        }

        private void Cancelar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Cancelar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        #endregion
    }
}
