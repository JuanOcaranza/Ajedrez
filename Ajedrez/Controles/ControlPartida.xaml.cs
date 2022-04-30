using System.Windows.Controls;
using System.Windows.Data;

namespace Ajedrez.Controles
{
    /// <summary>
    /// Lógica de interacción para ControlPartida.xaml
    /// </summary>
    public partial class ControlPartida : UserControl
    {
        #region Propiedades
        /// <summary>
        /// Obtiene o establece la partida que se muestra en el control.
        /// </summary>
        public Partida Partida { get; set; }
        #endregion

        #region Construtor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ControlPartida"/> a partir de una partida.
        /// </summary>
        /// <param name="partida">Partida que se va a mostrar en el control.</param>
        public ControlPartida(Partida partida)
        {
            InitializeComponent();
            Partida = partida;
            grid.SetBinding(DataContextProperty, new Binding(nameof(Partida)) { Source = this });
        }
        #endregion
    }
}
