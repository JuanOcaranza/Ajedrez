using Ajedrez.Properties;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para VentanaApertura.xaml
    /// </summary>
    public partial class VentanaJugador : Window
    {
        #region Atributos
        /// <summary>
        /// Condición para que una partida se considere ganada.
        /// </summary>
        private readonly Func<Partida, bool> condicionVictoria;

        /// <summary>
        /// Condición para que una partida se considere perdida.
        /// </summary>
        private readonly Func<Partida, bool> condicionDerrota;
        #endregion

        #region Propiedades
        /// <summary>
        /// Jugador que se muestra en la ventana.
        /// </summary>
        public Jugador Jugador { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="VentanaJugador"/> con el jugador y la ventana que la posee.
        /// </summary>
        /// <param name="jugador">Jugador que se va a mostrar.</param>
        /// <param name="owner">Ventana que va a poseer a la nueva ventana.</param>
        public VentanaJugador(Jugador jugador, ElegirJugador owner)
        {
            InitializeComponent();
            Owner = owner;
            Jugador = jugador;
            condicionVictoria = p => p.Gano(Jugador.Nombre);
            condicionDerrota = p => p.Perdio(Jugador.Nombre);
            try
            {
                Jugador.Partidas = new ListaPartidas(Jugador.ObtenerPartidasDesdeBaseDeDatos(), condicionVictoria, condicionDerrota);
            }
            catch
            {
                MessageBox.Show("No se han podido obtener las partidas de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            SetBinding(DataContextProperty, new Binding(nameof(Jugador)) { Source = this });
        }
        #endregion


        #region Controladores de eventos
        private void Editar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Editar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarJugador editarJugador = new EditarJugador(Jugador);
            editarJugador.ShowDialog();
            Jugador.Partidas = new ListaPartidas(Jugador.ObtenerPartidasDesdeBaseDeDatos(), condicionVictoria, condicionDerrota);
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Settings.Default.EliminarDirectamente || MessageBox.Show("Se borrarán todas las partidas jugadas por este jugador ¿continuar?", "Confirmar eliminación", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (Partida partida in Jugador.Partidas)
                    {
                        partida.BorrarDeBaseDeDatos();
                    }
                    using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                    {
                        conexion.Open();
                        using (SqlCommand delete = new SqlCommand($"DELETE FROM Jugador WHERE Nombre = '{Jugador.Nombre}'", conexion))
                        {
                            delete.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Ha ocurrido un error al intentar elimnar el jugador de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Close();
            }
        }

        private void SeleccionarPartida_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ElegirPartida elegirPartida = new ElegirPartida(Jugador.Partidas, Owner.Owner);
            Hide();
            try
            {
                elegirPartida.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }

        private void SeleccionarPartida_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Jugador?.Partidas.Count > 0;
        #endregion
    }
}
