using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para ElegirEjercicio.xaml
    /// </summary>
    public partial class ElegirEjercicio : Window
    {
        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ElegirEjercicio"/> con los ejercicios y la ventana que la posee.
        /// </summary>
        /// <param name="ejercicios">Ejercicios de entre los que se tiene que elegir.</param>
        /// <param name="owner">Ventana que va a poseer a la nueva ventana.</param>
        public ElegirEjercicio(IEnumerable<Ejercicio> ejercicios, Window owner)
        {
            InitializeComponent();
            dataGrid.ItemsSource = ejercicios;
            Owner = owner;
        }
        #endregion

        #region Controlador de eventos
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Owner is MainWindow mainWindow && sender is DataGridRow fila && fila.DataContext is Ejercicio ejercicio)
            {
                try
                {
                    mainWindow.TableroAjedrez.Partida = new Partida(new Posicion($"{ejercicio.DescripcionPosicion} 0 1"));
                    mainWindow.Ejercicio = ejercicio;
                }
                catch
                {
                    MessageBox.Show("No se pudo inicializar la partida a partir de la descripción del ejercicio seleccionado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Close();
            }
        }
        #endregion
    }
}
