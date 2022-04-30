using System.Windows;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para Progreso.xaml
    /// </summary>
    public partial class Progreso : VentanaSinBotones
    {
        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Progreso"/> con la ventan que la posee.
        /// </summary>
        /// <param name="owner">Ventana que va a poseer a la nueva ventana.</param>
        public Progreso(Window owner)
        {
            InitializeComponent();
            Owner = owner;
        }
        #endregion

        #region Método
        /// <summary>
        /// Actualiza el progreso.
        /// </summary>
        /// <param name="partidasLeidas">Cantidad de partidas leídas correctamente.</param>
        /// <param name="errores">Cantidad de errores ocurridos.</param>
        public void Actualizar(int partidasLeidas, int errores)
        {
            if (Title.Length > 20)
            {
                Title = "Leyendo partidas";
            }
            else
            {
                Title += ".";
            }
            textBlockProgreso.Text = $"{partidasLeidas} partidas leídas, {errores} errores, aproximadamente";
        }
        #endregion

        #region Controladores de eventos
        private void Cancelar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Cancelar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainWindow mainWindow = Owner as MainWindow;
            mainWindow.CancellationTokenSource.Cancel();
            Close();
        }
        #endregion
    }
}
