using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para AgregarPosicion.xaml
    /// </summary>
    public partial class AgregarPosicion : Window, IDataErrorInfo
    {
        #region Propiedad
        /// <summary>
        /// Obtiene o establece la descripción de la posición que se va a agrear (similar al FEN pero sin las cantidades de jugadas).
        /// </summary>
        public string DescripcionPosicion { get; set; }

        public string Error => null;
        #endregion

        #region Indexer
        public string this[string columnName]
        {
            get
            {
                try
                {
                    return columnName == nameof(DescripcionPosicion) ? (string.IsNullOrEmpty(DescripcionPosicion) ? "Debe ingresar un descripción de la posición" :
                    (Posicion.EsDescripcionValida(DescripcionPosicion) ? null : "Valor inválido consulte el manual de usaurio para más información")) : null;
                }
                catch
                {
                    return "Valor inválido, no se puede comprobar, consulte el manual de usaurio";
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AgregarPosicion"/> con la ventana que la posee.
        /// </summary>
        /// <param name="owner">Ventana que va a poseer a la nueva ventana.</param>
        public AgregarPosicion(EditarApertura owner)
        {
            InitializeComponent();
            Owner = owner;
            grid.DataContext = this;
        }
        #endregion

        #region Controladores de eventos
        private void Aceptar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = this[nameof(DescripcionPosicion)] == null;

        private void Aceptar_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (Owner is EditarApertura editarApertura)
            {
                editarApertura.Objeto.Posiciones.Add(DescripcionPosicion);
                Close();
            }
        }

        private void Cancelar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Cancelar_Executed(object sender, ExecutedRoutedEventArgs e) => Close();
        #endregion
    }
}
