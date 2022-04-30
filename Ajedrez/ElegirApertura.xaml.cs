using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para ElegirApertura.xaml
    /// </summary>
    public partial class ElegirApertura : Window
    {
        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ElegirApertura"/> con las aperturas y la ventana que la posee.
        /// </summary>
        /// <param name="aperturas">Aperturas de entre las que se tiene que elegir.</param>
        /// <param name="owner">Ventana que a la que va a pertenecr la nueva ventana.</param>
        public ElegirApertura(IEnumerable<Apertura> aperturas, Window owner)
        {
            InitializeComponent();
            dataGrid.ItemsSource = aperturas;
            Owner = owner;
        }
        #endregion

        #region Controlador de eventos
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow fila && fila.DataContext is Apertura apertura)
            {
                if (Owner is MainWindow)
                {
                    try
                    {
                        VentanaApertura ventanaApertura = new VentanaApertura(apertura, this);
                        Hide();
                        ventanaApertura.ShowDialog();
                    }
                    catch
                    {
                        MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (Owner is EditarApertura editarApertura)
                {
                    editarApertura.Objeto.Variante = apertura;
                    editarApertura.variante.Text = apertura.Nombre;
                }
                Close();
            }
        }
        #endregion
    }
}
