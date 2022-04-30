using Ajedrez.Properties;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para VentanaApertura.xaml
    /// </summary>
    public partial class VentanaApertura : Window
    {
        #region Propiedades
        /// <summary>
        /// Apertura que se muestra en la ventana.
        /// </summary>
        public Apertura Apertura { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="VentanaApertura"/> con la apertura y la ventana que la posee.
        /// </summary>
        /// <param name="apertura">Apertura que se va a mostrar.</param>
        /// <param name="owner">Ventana que va a poseer a la nueva ventana</param>
        public VentanaApertura(Apertura apertura, ElegirApertura owner)
        {
            InitializeComponent();
            Owner = owner;
            Apertura = apertura;
            try
            {
                Apertura.Partidas = new ListaPartidas(Apertura.ObtenerPartidasDesdeBaseDeDatos());
            }
            catch
            {
                MessageBox.Show("No se han podido obtener las partidas de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            SetBinding(DataContextProperty, new Binding("Apertura") { Source = this });
        }
        #endregion


        #region Controladores de eventos
        private void Editar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Editar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarApertura editarApertura = new EditarApertura(Apertura);
            try
            {
                editarApertura.ShowDialog();
                Apertura.Partidas = new ListaPartidas(Apertura.ObtenerPartidasDesdeBaseDeDatos());
            }
            catch
            {
                MessageBox.Show("No se han podido obtener las partidas de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Settings.Default.EliminarDirectamente || MessageBox.Show("¿Está seguro de que quiere eliminar esta apertura?", "Confirmar eliminación", MessageBoxButton.OKCancel,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Apertura.BorraDeBaseDeDatos();
                }
                catch
                {
                    MessageBox.Show("Ha ocurrido un error al intentar elimnar la apertura de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Close();
            }
        }

        private void SeleccionarPartida_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ElegirPartida elegirPartida = new ElegirPartida(Apertura.Partidas, Owner.Owner);
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

        private void SeleccionarPartida_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Apertura?.Partidas.Count > 0;
        #endregion
    }
}
