using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para ElegirJugador.xaml
    /// </summary>
    public partial class ElegirJugador : Window
    {
        #region Constructor
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ElegirJugador"/> con los jugadores y la ventana que la posee.
        /// </summary>
        /// <param name="jugadores">Jugadores que se van a mostrar.</param>
        /// <param name="owner">Ventana que va a poseer a la nueva ventana.</param>
        public ElegirJugador(IEnumerable<Jugador> jugadores, Window owner)
        {
            InitializeComponent();
            dataGrid.ItemsSource = jugadores;
            Owner = owner;
        }
        #endregion

        #region Controlador de eventos
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow fila && fila.DataContext is Jugador jugador)
            {
                if (Owner is ElegirRivales elegirRivales)
                {
                    if (elegirRivales.BlancoEsHumano)
                    {
                        elegirRivales.OpcionesBlanco.Add(jugador.Nombre);
                    }
                    if (elegirRivales.NegroEsHumano)
                    {
                        elegirRivales.OpcionesNegro.Add(jugador.Nombre);
                    }
                }
                else if (Owner is ConfigurarTorneoEliminatorio configurarTorneoEliminatorio)
                {
                    configurarTorneoEliminatorio.Torneo.Jugadores.Add(jugador);
                }
                else if (Owner is ConfigurarTorneoSuizo configurarTorneoSuizo)
                {
                    configurarTorneoSuizo.Torneo.Jugadores.Add(new Participante(jugador));
                }
                else
                {
                    try
                    {
                        if (Owner is MainWindow)
                        {
                            VentanaJugador ventanaJugador = new VentanaJugador(jugador, this);
                            Hide();
                            ventanaJugador.ShowDialog();
                        }
                        else if (Owner is EditarEvento editarEvento)
                        {
                            Participa participa = new Participa(jugador, editarEvento.Objeto);
                            EditarParticipacion editarParticipacion = new EditarParticipacion(participa);
                            Hide();
                            editarParticipacion.ShowDialog();
                            editarEvento.Objeto.Participaciones.Add(participa);
                        }
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
