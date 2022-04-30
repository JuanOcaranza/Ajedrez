using Ajedrez.Properties;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para EditarApertura.xaml
    /// </summary>
    public partial class EditarApertura : Editar<Apertura>
    {
        #region Atributo
        /// <summary>
        /// Aperturas del las que podría ser una variante.
        /// </summary>
        private readonly List<Apertura> posiblesVariantes = new List<Apertura>();
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EditarApertura"/> con la apertura.
        /// </summary>
        /// <param name="apertura">Apertura que se va a editar.</param>
        public EditarApertura(Apertura apertura) : base(apertura)
        {
            InitializeComponent();
            variante.Text = Objeto.Variante?.Nombre;
            grid.SetBinding(DataContextProperty, new Binding(nameof(Objeto)) { Source = this });
            try
            {
                using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                {
                    conexion.Open();
                    using (SqlCommand select = new SqlCommand($"SELECT * FROM Apertura{(Objeto.Id == null ? string.Empty : $" WHERE Id <> {Objeto.Id}")}", conexion))
                    {
                        using (SqlDataReader reader = select.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                posiblesVariantes.Add(new Apertura(reader.GetInt32(0), reader.GetString(1), reader.IsDBNull(2) ? null : reader.GetString(2), reader.IsDBNull(3) ? null : reader.GetString(3),
                                    reader.IsDBNull(4) ? null : new Apertura(reader.GetInt32(4))));
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("No se han podido obtener las aperturas de las que podría ser variante esta apertura", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion

        #region Controladores de eventos
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
                        cambios.Append(Objeto.Nombre != objetoAntiguo.Nombre ? $"Nombre = '{Objeto.Nombre}', " : "");
                        cambios.Append(Objeto.ECO != objetoAntiguo.ECO ? $"ECO = {Objeto.ECO.ToSQL()}, " : "");
                        cambios.Append(Objeto.NIC != objetoAntiguo.NIC ? $"NIC = {Objeto.NIC.ToSQL()}, " : "");
                        cambios.Append(Objeto.Variante?.Id != objetoAntiguo.Variante?.Id ? $"Id_Variante = {Objeto.Variante?.Id.ToString() ?? "null"}, " : "");
                        if (cambios.Length > 0)
                        {
                            cambios.Remove(cambios.Length - 2, 2); // Quita la última coma y el último espacio.
                            using (SqlCommand update = new SqlCommand($"UPDATE Apertura SET {cambios} WHERE Id = {Objeto.Id}", conexion))
                            {
                                update.ExecuteNonQuery();
                            }
                        }
                        foreach (string posicion in Objeto.Posiciones.Except(objetoAntiguo.Posiciones))
                        {
                            new Posicion($"{posicion} 0 1").InsertarEnBaseDeDatos(Objeto);
                        }
                        foreach (string posicion in objetoAntiguo.Posiciones.Except(Objeto.Posiciones))
                        {
                            using (SqlCommand uptate = new SqlCommand($"UPDATE Posicion SET Id_Apertura = null WHERE Descripcion = '{posicion}'", conexion))
                            {
                                uptate.ExecuteNonQuery();
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

        private void SeleccionarApertura_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ElegirApertura elegirApertura = new ElegirApertura(posiblesVariantes, this);
            try
            {
                elegirApertura.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EstablecerNulo_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Objeto?.Variante != null;

        private void EstablecerNUlo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Objeto.Variante = null;
            variante.Text = null;
        }

        private void AgregarPosicion_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AgregarPosicion agregarPosicion = new AgregarPosicion(this);
            try
            {
                agregarPosicion.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close(); // No tiene sentido una apertura sin posiciones.
            }
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = posiciones?.SelectedItems.Count > 0;

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<string> posicionesAEliminar = new List<string>(posiciones.SelectedItems.OfType<string>());
            foreach (string posicion in posicionesAEliminar)
            {
                Objeto.Posiciones.Remove(posicion);
            }
        }
        #endregion
    }
}
