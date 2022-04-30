using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ajedrez.Controles
{
    /// <summary>
    /// Lógica de interacción para Ronda.xaml
    /// </summary>
    public partial class Ronda : UserControl
    {
        #region Propiedades
        /// <summary>
        /// Obtiene o establece las partidas de la ronda.
        /// </summary>
        public List<Partida> Partidas { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Ronda"/> a partir de las partidas de la ronda.
        /// </summary>
        /// <param name="partidas">Partidas que se juegan en esta ronda.</param>
        public Ronda(IEnumerable<Partida> partidas)
        {
            InitializeComponent();
            Partidas = new List<Partida>(partidas);
            Height = Partidas.Count * 50;
            stackPanel.SetBinding(DataContextProperty, new Binding(nameof(Partidas)) { Source = this });
            foreach (Partida partida in Partidas)
            {
                stackPanel.Children.Add(new ControlPartida(partida));
            }
        }
        #endregion
    }
}
