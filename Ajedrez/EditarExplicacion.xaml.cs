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
    /// Lógica de interacción para EditarExplicacion.xaml
    /// </summary>
    public partial class EditarExplicacion : Editar<Explicacion>
    {
        #region Atributo
        /// <summary>
        /// Explicaciones de la base de datos con las que podría estar realacionada.
        /// </summary>
        private readonly List<Explicacion> explicacionesQueSePodrianRelacionar = new List<Explicacion>();
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EditarExplicacion"/> con la explicación.
        /// </summary>
        /// <param name="explicacion">Explicación que se va a editar.</param>
        public EditarExplicacion(Explicacion explicacion) : base(explicacion)
        {
            InitializeComponent();
            grid.SetBinding(DataContextProperty, new Binding(nameof(Objeto)) { Source = this });
            try
            {
                using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                {
                    conexion.Open();
                    using (SqlCommand select = new SqlCommand($"SELECT * FROM Explicacion{(Objeto.Id == null ? string.Empty : $" WHERE Id <> {Objeto.Id}")}", conexion))
                    {
                        using (SqlDataReader reader = select.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                explicacionesQueSePodrianRelacionar.Add(new Explicacion(reader.GetInt32(0), reader.GetString(1), reader.IsDBNull(2) ? null : reader.GetString(2), reader.GetString(3),
                                    reader.IsDBNull(4) ? null : reader.GetString(4)));
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("No se han podidio obtener las explicaciones cons las que podría estar relacionada", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        cambios.Append(Objeto.Descripcion != objetoAntiguo.Descripcion ? $"Descripcion = '{Objeto.Descripcion}', " : "");
                        cambios.Append(Objeto.Autor != objetoAntiguo.Autor ? $"Autor = {Objeto.Autor.ToSQL()}, " : "");
                        cambios.Append(Objeto.Nombre != objetoAntiguo.Nombre ? $"Nombre = {Objeto.Nombre.ToSQL()}, " : "");
                        if (Objeto.DescripcionPosicion != objetoAntiguo.DescripcionPosicion)
                        {
                            new Posicion($"{Objeto.DescripcionPosicion} 0 1 ").InsertarEnBaseDeDatos();
                            cambios.Append($"Descripcion_Posicion = '{Objeto.DescripcionPosicion}', ");
                        }
                        if (cambios.Length > 0)
                        {
                            cambios.Remove(cambios.Length - 2, 2); // Quita la última coma y el último espacio.
                            using (SqlCommand update = new SqlCommand($"UPDATE Explicacion SET {cambios} WHERE Id = {Objeto.Id}", conexion))
                            {
                                update.ExecuteNonQuery();
                            }
                        }
                        foreach (Explicacion explicacionRelacionada in Objeto.ExplicacionesRelacionadas.Except(objetoAntiguo.ExplicacionesRelacionadas))
                        {
                            using (SqlCommand insert = new SqlCommand($"INSERT INTO Tiene_Que_Ver VALUES ('{Objeto.Id}', '{explicacionRelacionada.Id}')", conexion))
                            {
                                insert.ExecuteNonQuery();
                            }
                        }
                        foreach (Explicacion explicacionNoRelacionada in objetoAntiguo.ExplicacionesRelacionadas.Except(Objeto.ExplicacionesRelacionadas))
                        {
                            using (SqlCommand delete = new SqlCommand($"DELETE FROM Tiene_Que_Ver WHERE {Objeto.Id} IN (Id1, Id2) AND {objetoAntiguo.Id} IN (Id1, Id2)", conexion))
                            {
                                delete.ExecuteNonQuery();
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

        private void SeleccionarExplicacion_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ElegirExplicacion elegirExplicacion = new ElegirExplicacion(explicacionesQueSePodrianRelacionar.Except(Objeto.ExplicacionesRelacionadas), this);
            try
            {
                elegirExplicacion.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = explicacionesRelacionadas?.SelectedItems?.Count > 0;

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<Explicacion> explicacionesAEliminar = new List<Explicacion>(explicacionesRelacionadas.SelectedItems.OfType<Explicacion>());
            foreach (Explicacion explicacion in explicacionesAEliminar)
            {
                Objeto.ExplicacionesRelacionadas.Remove(explicacion);
            }
        }
        #endregion
    }
}
