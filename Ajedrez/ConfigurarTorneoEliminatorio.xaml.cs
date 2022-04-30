using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para ConfigurarTorneo.xaml
    /// </summary>
    public partial class ConfigurarTorneoEliminatorio : Window
    {
        #region Atributos
        /// <summary>
        /// Cantidad de errores de validación.
        /// </summary>
        private int cantidadErrores = 0;

        /// <summary>
        /// Jugadores que pueden participar en el evento.
        /// </summary>
        private readonly List<Jugador> posiblesParticipantes;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece el torneo que se está configurando.
        /// </summary>
        public TorneoEliminatorio Torneo { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ConfigurarTorneoEliminatorio"/> con el torneo.
        /// </summary>
        /// <param name="torneo">Torneo que se va a configurar.</param>
        public ConfigurarTorneoEliminatorio(TorneoEliminatorio torneo)
        {
            InitializeComponent();
            Torneo = torneo;
            try
            {
                posiblesParticipantes = Jugador.ObtenerJugadoresDesdeBaseDeDatos();
            }
            catch
            {
                MessageBox.Show("No se han podido obtener los jugadores de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close(); // No se puede organizar el torneo sin los jugadores.
            }
            grid.SetBinding(DataContextProperty, new Binding(nameof(Torneo)) { Source = this });
        }
        #endregion

        #region Controladores de eventos
        private void Aceptar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = cantidadErrores == 0;

        private void Aceptar_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (Evento.NombreUsadoEnBaseDeDatos(Torneo.Evento.Nombre))
                {
                    MessageBox.Show("El nombre ya está siendo utilizado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (Torneo.Jugadores.Count > 1 && Math.Log(Torneo.Jugadores.Count, 2) % 1 == 0) // La cantidad  de jugadores debe ser una potencia de dos mayor a uno.
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("La cantidad de jugadores debe ser una potencia de dos mayor a uno", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("No se ha podido comprobar que el nombre estuviera dispoble", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_ValidationError(object sender, ValidationErrorEventArgs e) => cantidadErrores += e.Action == ValidationErrorEventAction.Added ? 1 : -1;

        private void Cancelar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Cancelar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SeleccionarJugador_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ElegirJugador elegirJugador = new ElegirJugador(posiblesParticipantes.Except(Torneo.Jugadores), this);
            try
            {
                elegirJugador.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close(); // Si no se puede elegir jugadores no tiene sentido configurar el torneo.
            }
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = jugadores?.SelectedItems?.Count > 0;

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<Jugador> jugadoresAEliminar = new List<Jugador>(jugadores.SelectedItems.OfType<Jugador>());
            foreach (Jugador jugador in jugadoresAEliminar)
            {
                Torneo.Jugadores.Remove(jugador);
            }
        }

        private void MoverArriba_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = jugadores?.SelectedItems.Count == 1 && jugadores.SelectedIndex > 0;

        private void MoverArriba_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int indice = jugadores.SelectedIndex;
            Torneo.Jugadores.Insert(indice - 1, (Jugador)jugadores.SelectedItem);
            Torneo.Jugadores.RemoveAt(indice + 1);
            jugadores.SelectedIndex = indice - 1;
        }

        private void MoverAbajo_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = jugadores?.SelectedItems.Count == 1 && jugadores.SelectedIndex < jugadores.Items.Count - 1;

        private void MoverAbajo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int indice = jugadores.SelectedIndex;
            Torneo.Jugadores.Insert(indice + 2, (Jugador)jugadores.SelectedItem);
            Torneo.Jugadores.RemoveAt(indice);
            jugadores.SelectedIndex = indice + 1;
        }
        #endregion
    }
}
