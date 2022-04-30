using Ajedrez.Properties;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para VentanaEvento.xaml
    /// </summary>
    public partial class VentanaEvento : Window
    {
        #region Propiedades
        /// <summary>
        /// Evento que se muestra en la ventana.
        /// </summary>
        public Evento Evento { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="VentanaEvento"/> con un evento y la ventan aque la posee.
        /// </summary>
        /// <param name="evento">Evento que se va a mostrar.</param>
        /// <param name="owner">Ventana que va a poseer a la nueva ventana.</param>
        public VentanaEvento(Evento evento, ElegirEvento owner)
        {
            InitializeComponent();
            Owner = owner;
            Evento = evento;
            try
            {
                Evento.EstablecerPartidasDesdeBaseDeDatos();
            }
            catch
            {
                MessageBox.Show("No se han podido obtener las partidas de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            SetBinding(DataContextProperty, new Binding(nameof(Evento)) { Source = this });
        }
        #endregion


        #region Controladores de eventos
        private void Editar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Editar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarEvento editarEvento = new EditarEvento(Evento);
            try
            {
                editarEvento.ShowDialog();
                Evento.EstablecerPartidasDesdeBaseDeDatos();
            }
            catch
            {
                MessageBox.Show("No se han podido obtener las partidas de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Settings.Default.EliminarDirectamente || MessageBox.Show("Se borrarán todas las partidas jugadas en este evento ¿continuar?", "Confirmar eliminación", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (Partida partida in Evento.Partidas)
                    {
                        partida.BorrarDeBaseDeDatos();
                    }
                    using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                    {
                        conexion.Open();
                        using (SqlCommand delete = new SqlCommand($"DELETE FROM Evento WHERE Nombre = '{Evento.Nombre}'", conexion))
                        {
                            delete.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Ha ocurrido un error al intentar elimnar el evento de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Close();
            }
        }

        private void SeleccionarPartida_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ElegirPartida elegirPartida = new ElegirPartida(Evento.Partidas, Owner.Owner);
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

        private void SeleccionarPartida_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Evento?.Partidas.Count > 0;
        #endregion
    }
}
