using Ajedrez.Properties;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para EditarJugador.xaml
    /// </summary>
    public partial class EditarJugador : Editar<Jugador>
    {
        #region Atributo
        /// <summary>
        /// Si se está creando un nuevo jugador.
        /// </summary>
        private readonly bool nuevoJugador;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EditarJugador"/> con el jugador.
        /// </summary>
        /// <param name="jugador">Jugador que se va a editar.</param>
        public EditarJugador(Jugador jugador) : base(jugador)
        {
            InitializeComponent();
            nuevoJugador = string.IsNullOrEmpty(jugador.Nombre);
            tipo. ItemsSource = new string[] { "human", "program" }; // Valores posibles para el tipo del jugador.
            titulo.ItemsSource = new string[] { "GM", "IM", "WGM", "FM", "WIM", "CM", "WFM", "WCM", "-" }; // Valores posibles para el título del jugador.
            grid.SetBinding(DataContextProperty, new Binding(nameof(Objeto)) { Source = this });
        }
        #endregion

        #region Controlador de eventos
        private void Aceptar_Execute(object sender, ExecutedRoutedEventArgs e)
        {

            if (Objeto.Nombre != objetoAntiguo.Nombre)
            {
                try
                {
                    using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                    {
                        conexion.Open();
                        using (SqlCommand select = new SqlCommand($"SELECT Nombre FROM Jugador WHERE Nombre = '{Objeto.Nombre}'", conexion))
                        {
                            if (select.ExecuteScalar() != null)
                            {
                                MessageBox.Show("El nombre ya está siendo utilizado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("No se ha podido comprobar la disponibilidad de nombre en la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            try
            {
                if (nuevoJugador)
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
                        cambios.Append(Objeto.Na != objetoAntiguo.Na ? $"Na = {Objeto.Na.ToSQL()}, " : "");
                        cambios.Append(Objeto.Tipo != objetoAntiguo.Tipo ? $"Tipo = {Objeto.Tipo.ToSQL()}, " : "");
                        cambios.Append(Objeto.Elo != objetoAntiguo.Elo ? $"Elo = {Objeto.Elo.ToSQL()}, " : "");
                        cambios.Append(Objeto.Titulo != objetoAntiguo.Titulo ? $"Titulo = {Objeto.Titulo.ToSQL()}, " : "");
                        if (cambios.Length > 0)
                        {
                            cambios.Remove(cambios.Length - 2, 2); // Quita la última coma y el último espacio.
                            using (SqlCommand update = new SqlCommand($"UPDATE Jugador SET {cambios} WHERE Nombre = {Objeto.Nombre}", conexion))
                            {
                                update.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("No se han podidio almacenar los cambios en la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }
        #endregion
    }
}
