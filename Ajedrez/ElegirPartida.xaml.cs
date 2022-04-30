using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para ElegirPartida.xaml
    /// </summary>
    public partial class ElegirPartida : Window
    {
        #region Constructor
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ElegirPartida"/> con las partidas.
        /// </summary>
        /// <param name="partidas">Partidas que se van a mostrar.</param>
        /// <param name="owner">Ventana que va a poseer a la nueva ventana.</param>
        public ElegirPartida(IEnumerable<Partida> partidas, Window owner)
        {
            InitializeComponent();
            dataGrid.ItemsSource = partidas;
            Owner = owner;
        }
        #endregion

        #region Controlador de eventos
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Owner is MainWindow mainWindow && sender is DataGridRow fila && fila.DataContext is Partida partida)
            {
                mainWindow.TableroAjedrez.Partida = partida;
                Close();
            }
        }
        #endregion
    }
}
