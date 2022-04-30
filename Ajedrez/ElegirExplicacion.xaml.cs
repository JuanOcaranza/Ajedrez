using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para ElegirExplicacion.xaml
    /// </summary>
    public partial class ElegirExplicacion : Window
    {
        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ElegirExplicacion"/> con las explicaciones y la ventana que la posee.
        /// </summary>
        /// <param name="explicaicones">Explicaciones de entre las que se tiene que elegir.</param>
        /// <param name="owner">Ventana que va a poseer a la nueva ventana.</param>
        public ElegirExplicacion(IEnumerable<Explicacion> explicaciones, Window owner)
        {
            InitializeComponent();
            dataGrid.ItemsSource = explicaciones;
            Owner = owner;
        }
        #endregion

        #region Controlador de eventos
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow fila && fila.DataContext is Explicacion explicacion)
            {
                if (Owner is MainWindow mainWindow)
                {
                    try
                    {
                        mainWindow.TableroAjedrez.Partida = new Partida(new Posicion($"{explicacion.DescripcionPosicion} 0 1"));
                        explicacion.EstablecerExplicacionesRelacionadasDesdeLaBaseDeDatos();
                    }
                    catch (Exception ex) when (ex is InvalidCastException || ex is SqlException || ex is InvalidOperationException || ex is IOException)
                    {
                        MessageBox.Show("No se han podido establecer las explicaciones relacionadas", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch
                    {
                        MessageBox.Show("No se ha podido crear la partida a partir de la descripción de posición de la explicación seleccionada", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    mainWindow.Explicacion = explicacion;
                }
                else if(Owner is EditarExplicacion editarExplicacion)
                {
                    editarExplicacion.Objeto.ExplicacionesRelacionadas.Add(explicacion);
                }
                Close();
            }
        }
        #endregion
    }
}
