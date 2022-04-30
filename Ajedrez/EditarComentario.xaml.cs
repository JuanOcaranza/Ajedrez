using System.Windows;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para EditarComentario.xaml
    /// </summary>
    public partial class EditarComentario : Window
    {
        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EditarComentario"/> con la ventana que la posee.
        /// </summary>
        /// <param name="owner">Ventana que va a poseer a la nueva ventana.</param>
        public EditarComentario(MainWindow owner)
        {
            InitializeComponent();
            Owner = owner;
            comentario.Text = owner.TableroAjedrez.Partida.ComentarioPosicionActual;
        }
        #endregion

        #region Controladores de evento
        private void Aceptar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Aceptar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Owner is MainWindow mainWindow)
            {
                mainWindow.TableroAjedrez.Partida.ComentarioPosicionActual = comentario.Text;
            }
            Close();
        }

        private void Cancelar_Executed(object sender, ExecutedRoutedEventArgs e) => Close();
        #endregion
    }
}
