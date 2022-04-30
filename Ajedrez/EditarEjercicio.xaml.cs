using Ajedrez.Properties;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para EditarDatosEjercicio.xaml
    /// </summary>
    public partial class EditarEjercicio : Editar<Ejercicio>
    {
        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EditarEjercicio"/> con el ejercicio.
        /// </summary>
        /// <param name="ejercicio">Ejericio que se va a editar.</param>
        public EditarEjercicio(Ejercicio ejercicio) : base(ejercicio)
        {
            InitializeComponent();
            grid.SetBinding(DataContextProperty, new Binding(nameof(Objeto)) { Source = this });
        }
        #endregion

        #region Controlador de eventos
        private void Aceptar_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (Objeto.Id != null)
                {
                    using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                    {
                        conexion.Open();
                        StringBuilder cambios = new StringBuilder();
                        cambios.Append(Objeto.Dificultad != objetoAntiguo.Dificultad ? $"Dificultad = {Objeto.Dificultad.ToSQL()}, " : "");
                        cambios.Append(Objeto.Solucion != objetoAntiguo.Solucion ? $"Solucion = '{Objeto.Solucion}', " : "");
                        if (Objeto.DescripcionPosicion != objetoAntiguo.DescripcionPosicion)
                        {
                            new Posicion($"{Objeto.DescripcionPosicion} 0 1 ").InsertarEnBaseDeDatos();
                            cambios.Append($"Descripcion_Posicion = '{Objeto.DescripcionPosicion}', ");

                        }
                        if (cambios.Length > 0)
                        {
                            cambios.Remove(cambios.Length - 2, 2); // Quita la última coma y el último espacio.
                            using (SqlCommand update = new SqlCommand($"UPDATE Ejercicio SET {cambios} WHERE Id = {Objeto.Id}", conexion))
                            {
                                update.ExecuteNonQuery();
                            }
                        }
                    }

                }
                else
                {
                    Objeto.InsertarEnBaseDeDatos();
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
