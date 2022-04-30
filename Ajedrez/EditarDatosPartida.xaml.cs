using Ajedrez.Properties;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para EditarDatosPartida.xaml
    /// </summary>
    public partial class EditarDatosPartida : VentanaSinBotones
    {
        #region Atributos
        /// <summary>
        /// Cantidad de errores de validación.
        /// </summary>
        private int cantidadErrores = 0;

        /// <summary>
        /// Etiquetas que había cuando se cargó la ventana.
        /// </summary>
        private readonly Dictionary<EtiquetaPGN, string> etiquetasAntiguas;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece la partida en la que se modifican los datos.
        /// </summary>
        public Partida Partida { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EditarDatosPartida"/> con la partida.
        /// </summary>
        /// <param name="partida">Partida de la que se editarán los datos</param>
        public EditarDatosPartida(Partida partida)
        {
            InitializeComponent();
            Partida = partida;
            etiquetasAntiguas = new Dictionary<EtiquetaPGN, string>(Partida.Etiquetas);
            tabControl.SetBinding(DataContextProperty, new Binding("Partida") { Source = this });
            // Valores válidos para las etiquetas WhiteType y BlackType.
            string[] tipos = new string[] { "human", "program" };
            tipoBlanco.ItemsSource = tipos;
            tipoNegro.ItemsSource = tipos;
            // Valores válidos para las etiquetas WhiteTitle y BlackTitle.
            string[] titulos = new string[] { "GM", "IM", "WGM", "FM", "WIM", "CM", "WFM", "WCM", "-"};
            tituloBlanco.ItemsSource = titulos;
            tituloNegro.ItemsSource = titulos;
            // Valores válidos para la etiqueta Result
            resultado.ItemsSource = new string[] { "1-0", "0-1", "1/2-1/2", "*" };
            // Valores válidos para la etiqueta Termination.
            terminacion.ItemsSource = new string[] { "abandoned", "adjudication", "death", "emergency", "normal", "rules infraction", "time forfeit", "unterminated" };
        }
        #endregion

        #region Controladores de eventos
        private void Aceptar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = cantidadErrores == 0;

        private void Aceptar_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (Partida.Id != null)
                {
                    using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                    {
                        conexion.Open();
                        StringBuilder cambios = new StringBuilder();
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.Date] != etiquetasAntiguas[EtiquetaPGN.Date] ? $"Fecha = '{Partida.Etiquetas[EtiquetaPGN.Date]}', " : "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.Result] != etiquetasAntiguas[EtiquetaPGN.Result] ? $"Resultado = '{Partida.Etiquetas[EtiquetaPGN.Result]}', " : "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.Time] != etiquetasAntiguas[EtiquetaPGN.Time] ? $"Hora = {Partida.Etiquetas[EtiquetaPGN.Time].ToSQL()}, " : "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.UTCTime] != etiquetasAntiguas[EtiquetaPGN.UTCTime] ? $"Hora_UTC = {Partida.Etiquetas[EtiquetaPGN.UTCTime].ToSQL()}, " : "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.UTCDate] != etiquetasAntiguas[EtiquetaPGN.UTCDate] ? $"Fecha_UTC = {Partida.Etiquetas[EtiquetaPGN.UTCDate].ToSQL()}, " : "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.Annotator] != etiquetasAntiguas[EtiquetaPGN.Annotator] ? $"Anotador = {Partida.Etiquetas[EtiquetaPGN.Annotator].ToSQL()}, " : "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.Mode] != etiquetasAntiguas[EtiquetaPGN.Mode] ? $"Modo = {Partida.Etiquetas[EtiquetaPGN.Mode].ToSQL()}, " : "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.Termination] != etiquetasAntiguas[EtiquetaPGN.Termination] ? $"Terminacion = {Partida.Etiquetas[EtiquetaPGN.Termination].ToSQL()}, " :
                            "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.Round] != etiquetasAntiguas[EtiquetaPGN.Round] ? $"Ronda = '{ Partida.Etiquetas[EtiquetaPGN.Round]}', " : "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.Board] != etiquetasAntiguas[EtiquetaPGN.Board] ? $"Tablero = {Partida.Etiquetas[EtiquetaPGN.Board].ToSQL()}, " : "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.Stage] != etiquetasAntiguas[EtiquetaPGN.Stage] ? $"Etapa = {Partida.Etiquetas[EtiquetaPGN.Stage].ToSQL()}, " : "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.TimeControl] != etiquetasAntiguas[EtiquetaPGN.TimeControl] ? $"Tiempo = {Partida.Etiquetas[EtiquetaPGN.TimeControl].ToSQL()}, " : "");
                        if (cambios.Length > 0)
                        {
                            cambios.Remove(cambios.Length - 2, 2); // Quita la última coma y el último espacio.
                            using (SqlCommand update = new SqlCommand($"UPDATE Partida SET {cambios} WHERE Id = {Partida.Id}", conexion))
                            {
                                update.ExecuteNonQuery();
                            }
                            cambios.Clear();
                        }
                        if (Partida.Etiquetas[EtiquetaPGN.Event] != etiquetasAntiguas[EtiquetaPGN.Event])
                        {
                            Evento evento = new Evento(Partida.Etiquetas[EtiquetaPGN.Event], Partida.Etiquetas[EtiquetaPGN.Site], Partida.Etiquetas[EtiquetaPGN.EventDate],
                                Partida.Etiquetas[EtiquetaPGN.Section]);
                            evento.InsertarEnBaseDeDatos();
                            using (SqlCommand update = new SqlCommand($"UPDATE Partida SET Nombre_Evento = '{Partida.Etiquetas[EtiquetaPGN.Event]}' WHERE Id = {Partida.Id}", conexion))
                            {
                                update.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            cambios.Append(Partida.Etiquetas[EtiquetaPGN.Site] != etiquetasAntiguas[EtiquetaPGN.Site] ? $"Sito = '{Partida.Etiquetas[EtiquetaPGN.Site]}', " : "");
                            cambios.Append(Partida.Etiquetas[EtiquetaPGN.EventDate] != etiquetasAntiguas[EtiquetaPGN.EventDate] ? $"Fecha = {Partida.Etiquetas[EtiquetaPGN.EventDate].ToSQL()}, " : "");
                            cambios.Append(Partida.Etiquetas[EtiquetaPGN.Section] != etiquetasAntiguas[EtiquetaPGN.Section] ? $"Seccion = {Partida.Etiquetas[EtiquetaPGN.Section].ToSQL()}, " : "");
                            if (cambios.Length > 0)
                            {
                                cambios.Remove(cambios.Length - 2, 2); // Quita la última coma y el último espacio.
                                using (SqlCommand update = new SqlCommand($"UPDATE Evento SET {cambios} WHERE Nombre = '{Partida.Etiquetas[EtiquetaPGN.Event]}'", conexion))
                                {
                                    update.ExecuteNonQuery();
                                }
                                cambios.Clear();
                            }
                        }
                        int? elo = int.TryParse(Partida.Etiquetas[EtiquetaPGN.WhiteElo], out int eloBlanco) ? (int?)eloBlanco : null;
                        if (Partida.Etiquetas[EtiquetaPGN.White] != etiquetasAntiguas[EtiquetaPGN.White])
                        {
                            Jugador jugador = new Jugador(Partida.Etiquetas[EtiquetaPGN.White], Partida.Etiquetas[EtiquetaPGN.WhiteNA], Partida.Etiquetas[EtiquetaPGN.WhiteType], elo,
                                Partida.Etiquetas[EtiquetaPGN.WhiteTitle]);
                            jugador.InsertarEnBaseDeDatos();
                            using (SqlCommand update = new SqlCommand($"UPDATE Juega SET Nombre_Jugador = '{Partida.Etiquetas[EtiquetaPGN.White]}' WHERE Id_Partida = {Partida.Id} AND Nombre_Jugador = " +
                                $"'{etiquetasAntiguas[EtiquetaPGN.White]}'", conexion))
                            {
                                update.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            cambios.Append(Partida.Etiquetas[EtiquetaPGN.WhiteNA] != etiquetasAntiguas[EtiquetaPGN.WhiteNA] ? $"Na = {Partida.Etiquetas[EtiquetaPGN.WhiteNA].ToSQL()}, " : "");
                            cambios.Append(Partida.Etiquetas[EtiquetaPGN.WhiteType] != etiquetasAntiguas[EtiquetaPGN.WhiteType] ? $"Tipo = {Partida.Etiquetas[EtiquetaPGN.WhiteType].ToSQL()}, " : "");
                            if (cambios.Length > 0)
                            {
                                cambios.Remove(cambios.Length - 2, 2); // Quita la última coma y el último espacio.
                                using (SqlCommand update = new SqlCommand($"UPDATE Jugador SET {cambios} WHERE Nombre = '{Partida.Etiquetas[EtiquetaPGN.White]}'", conexion))
                                {
                                    update.ExecuteNonQuery();
                                }
                            }
                            cambios.Clear();
                        }
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.WhiteElo] != etiquetasAntiguas[EtiquetaPGN.WhiteElo] ? $"Elo = {elo.ToSQL()}, " : "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.WhiteTitle] != etiquetasAntiguas[EtiquetaPGN.WhiteTitle] ? $"Titulo = {Partida.Etiquetas[EtiquetaPGN.WhiteTitle].ToSQL()}, " : "");
                        if (cambios.Length > 0)
                        {
                            cambios.Remove(cambios.Length - 2, 2); // Quita la última coma y el último espacio.
                            using (SqlCommand update = new SqlCommand($"UPDATE Juega SET {cambios} WHERE Id_Partida = {Partida.Id} AND Nombre_Jugador = '{Partida.Etiquetas[EtiquetaPGN.White]}'", conexion))
                            {
                                update.ExecuteNonQuery();
                            }
                            cambios.Clear();
                        }
                        elo = int.TryParse(Partida.Etiquetas[EtiquetaPGN.BlackElo], out int eloNegro) ? (int?)eloBlanco : null;
                        if (Partida.Etiquetas[EtiquetaPGN.Black] != etiquetasAntiguas[EtiquetaPGN.Black])
                        {
                            Jugador jugador = new Jugador(Partida.Etiquetas[EtiquetaPGN.Black], Partida.Etiquetas[EtiquetaPGN.BlackNA], Partida.Etiquetas[EtiquetaPGN.BlackType], elo,
                                Partida.Etiquetas[EtiquetaPGN.BlackTitle]);
                            jugador.InsertarEnBaseDeDatos();
                            using (SqlCommand update = new SqlCommand($"UPDATE Juega SET Nombre_Jugador = '{Partida.Etiquetas[EtiquetaPGN.Black]}' WHERE Id_Partida = {Partida.Id} AND Nombre_Jugador = " +
                                $"'{etiquetasAntiguas[EtiquetaPGN.Black]}'", conexion))
                            {
                                update.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            cambios.Append(Partida.Etiquetas[EtiquetaPGN.BlackNA] != etiquetasAntiguas[EtiquetaPGN.BlackNA] ? $"Na = {Partida.Etiquetas[EtiquetaPGN.BlackNA].ToSQL()}, " : "");
                            cambios.Append(Partida.Etiquetas[EtiquetaPGN.BlackType] != etiquetasAntiguas[EtiquetaPGN.BlackType] ? $"Tipo = {Partida.Etiquetas[EtiquetaPGN.BlackType].ToSQL()}, " : "");
                            if (cambios.Length > 0)
                            {
                                cambios.Remove(cambios.Length - 2, 2); // Quita la última coma y el último espacio.
                                using (SqlCommand update = new SqlCommand($"UPDATE Jugador SET {cambios} WHERE Nombre = '{Partida.Etiquetas[EtiquetaPGN.Black]}'", conexion))
                                {
                                    update.ExecuteNonQuery();
                                }
                            }
                            cambios.Clear();
                        }
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.BlackElo] != etiquetasAntiguas[EtiquetaPGN.BlackElo] ? $"Elo = {elo.ToSQL()}, " : "");
                        cambios.Append(Partida.Etiquetas[EtiquetaPGN.BlackTitle] != etiquetasAntiguas[EtiquetaPGN.BlackTitle] ? $"Titulo = {Partida.Etiquetas[EtiquetaPGN.BlackTitle].ToSQL()}, " : "");
                        if (cambios.Length > 0)
                        {
                            cambios.Remove(cambios.Length - 2, 2); // Quita la última coma y el último espacio.
                            using (SqlCommand update = new SqlCommand($"UPDATE Juega SET {cambios} WHERE Id_Partida = {Partida.Id} AND Nombre_Jugador = '{Partida.Etiquetas[EtiquetaPGN.Black]}'", conexion))
                            {
                                update.ExecuteNonQuery();
                            }
                            cambios.Clear();
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

        private void TextBox_ValidationError(object sender, ValidationErrorEventArgs e) => cantidadErrores += e.Action == ValidationErrorEventAction.Added ? 1 : -1;

        private void Cancelar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Cancelar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (KeyValuePair<EtiquetaPGN, string> etiqueta in etiquetasAntiguas)
            {
                Partida.Etiquetas[etiqueta.Key] = etiqueta.Value;
            }
            Close();
        }
        #endregion
    }
}
