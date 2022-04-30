using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para ElegirEvento.xaml
    /// </summary>
    public partial class ElegirEvento : Window
    {
        #region Constructor
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ElegirEvento"/> con los eventos y la ventana que la posee.
        /// </summary>
        /// <param name="eventos">Eventos que se van a mostrar.</param>
        /// <param name="owner">Ventana que va a poseer a la nueva ventana.</param>
        public ElegirEvento(IEnumerable<Evento> eventos, Window owner)
        {
            InitializeComponent();
            dataGrid.ItemsSource = eventos;
            Owner = owner;
        }
        #endregion

        #region Controlador de eventos
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow fila && fila.DataContext is Evento evento)
            {
                if (Owner is MainWindow)
                {
                    try
                    {
                        evento.EstablecerParticipacionesDesdeBaseDeDatos();

                    }
                    catch
                    {
                        MessageBox.Show("No se han podido establecer los participantes del evento", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    try
                    {
                        VentanaEvento ventanaEvento = new VentanaEvento(evento, this);
                        Hide();
                        ventanaEvento.ShowDialog();
                    }
                    catch
                    {
                        MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                Close();
            }
        }
        #endregion
    }
}
