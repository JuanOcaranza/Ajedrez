using System.Windows.Data;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para EditarParticipacion.xaml
    /// </summary>
    public partial class EditarParticipacion : Editar<Participa>
    {
        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EditarParticipacion"/> con la participación.
        /// </summary>
        /// <param name="participa">Participación que se va a editar.</param>
        public EditarParticipacion(Participa participa) : base(participa)
        {
            InitializeComponent();
            grid.SetBinding(DataContextProperty, new Binding(nameof(Objeto)) { Source = this });
        }
        #endregion
        
        #region Controlador de eventos
        private void Aceptar_Execute(object sender, ExecutedRoutedEventArgs e) => Close();
        #endregion
    }
}
