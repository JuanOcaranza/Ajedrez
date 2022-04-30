using Ajedrez.Properties;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para EditarEvento.xaml
    /// </summary>
    public partial class EditarEvento : Editar<Evento>
    {
        #region Atributos
        /// <summary>
        /// Si se está creando un nuevo evento.
        /// </summary>
        private readonly bool nuevoEvento;

        /// <summary>
        /// Jugadores que podrían haber participado en el evento.
        /// </summary>
        private readonly List<Jugador> posiblesParticipantes;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EditarEvento"/> con el evento.
        /// </summary>
        /// <param name="evento">Evento que se va a editar.</param>
        public EditarEvento(Evento evento) : base(evento)
        {
            InitializeComponent();
            nuevoEvento = string.IsNullOrEmpty(evento.Nombre);
            try
            {
                posiblesParticipantes = Jugador.ObtenerJugadoresDesdeBaseDeDatos();
            }
            catch
            {
                MessageBox.Show("No se han podido obtener los jugadores de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close(); // No tiene sentido el evento sin los participantes.
            }
            grid.SetBinding(DataContextProperty, new Binding(nameof(Objeto)) { Source = this });
        }
        #endregion

        #region Controladores de eventos
        private void Aceptar_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (Objeto.Nombre != objetoAntiguo.Nombre)
            {
                try
                {
                    if (Evento.NombreUsadoEnBaseDeDatos(Objeto.Nombre))
                    {
                        MessageBox.Show("El nombre ya está siendo utilizado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("No se ha podido comprobar la disponibilidad de nombre en la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            try
            {
                if (nuevoEvento)
                {
                    Objeto.InsertarEnBaseDeDatos();
                }
                else
                {
                    using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                    {
                        conexion.Open();
                        StringBuilder cambios = new StringBuilder();
                        cambios.Append(Objeto.Nombre != objetoAntiguo.Nombre ? $"Nombre = '{Objeto.Nombre}', " : "");
                        cambios.Append(Objeto.Sitio != objetoAntiguo.Sitio ? $"Sitio = '{Objeto.Sitio}', " : "");
                        cambios.Append(Objeto.Fecha != objetoAntiguo.Fecha ? $"Fecha = {Objeto.Fecha.ToSQL()}, " : "");
                        cambios.Append(Objeto.Seccion != objetoAntiguo.Seccion ? $"Seccion = {Objeto.Seccion.ToSQL()}, " : "");
                        if (cambios.Length > 0)
                        {
                            cambios.Remove(cambios.Length - 2, 2); // Quita la última coma y el último espacio.
                            using (SqlCommand update = new SqlCommand($"UPDATE Evento SET {cambios} WHERE Nombre = '{Objeto.Nombre}'", conexion))
                            {
                                update.ExecuteNonQuery();
                            }
                        }
                    }
                    foreach (Participa participa in Objeto.Participaciones.Except(objetoAntiguo.Participaciones))
                    {
                        participa.InsertarEnBaseDeDatos();
                    }
                    foreach (Participa participa in objetoAntiguo.Participaciones.Except(Objeto.Participaciones))
                    {
                        participa.BorrarDeBaseDeDatos();
                    }
                }
            }
            catch
            {
                MessageBox.Show("No se han podidio almacenar los cambios en la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }

        private void SeleccionarJugador_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ElegirJugador elegirJugador = new ElegirJugador(posiblesParticipantes.Except(Objeto.Participaciones.Select(p => p.Jugador)) , this);
            elegirJugador.ShowDialog();
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = participaciones?.SelectedItems?.Count > 0;

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<Participa> participacionesAEliminar = new List<Participa>(participaciones.SelectedItems.OfType<Participa>());
            foreach (Participa participacion in participacionesAEliminar)
            {
                Objeto.Participaciones.Remove(participacion);
            }
        }

        private void Editar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = participaciones?.SelectedItems.Count == 1;

        private void Editar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarParticipacion editarParticipacion = new EditarParticipacion((Participa)participaciones.SelectedItem);
            editarParticipacion.ShowDialog();
            participaciones.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateSource();
        }
        #endregion
    }
}
