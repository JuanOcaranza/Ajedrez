using Ajedrez.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : VentanaSinBotones
    {
        #region Atributos
        /// <summary>
        /// Cadena de filtro que determina que se deben mostrar archivos .pgn
        /// </summary>
        private const string FiltroPGN = "Archivos Portable Game Notation (*.pgn)|*.pgn";

        /// <summary>
        /// Ejercicio que se está realizando.
        /// </summary>
        private Ejercicio ejercicio;

        /// <summary>
        /// Explicación que se está mostrando.
        /// </summary>
        private Explicacion explicacion;

        /// <summary>
        /// Indica si el motor está funcionando correctamente y se lo puede utilizar.
        /// </summary>
        private bool motorHabilitado = true;

        /// <summary>
        /// Ventan que muestra el progreso de una lectura de archivo.
        /// </summary>
        private Progreso ventanaProgreso;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece las etiquetas PGN opcionales que se incluirán al gaurdar un archivo PGN.
        /// </summary>
        public List<EtiquetaPGN> EtiquetasOpcionales { get; set; } = new List<EtiquetaPGN>();

        /// <summary>
        /// Obtiene o establece el motor para analizar la posición.
        /// </summary>
        public Motor Motor { get; set; } = new Motor(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Motores\\" + Settings.Default.Motor);

        /// <summary>
        /// Obtiene o establece los resultados obtenidos por cada jugada que se ha relizada en la posición actual.
        /// </summary>
        public List<ResultadoJugada> Resultados { get; set; }

        /// <summary>
        /// Obtiene o establece el ejercicio que se está realizando.
        /// </summary>
        public Ejercicio Ejercicio
        {
            get => ejercicio;
            set
            {
                ejercicio = value;
                if (ejercicio == null)
                {
                    RestaurarToolTips();
                }
                else
                {
                    runNombreBlanco.ToolTip = "Dificultad del ejercicio";
                    runNombreNegro.ToolTip = null;
                    TableroAjedrez.Partida.Etiquetas[EtiquetaPGN.Black] = "Ejercicio";
                    TableroAjedrez.Partida.Etiquetas[EtiquetaPGN.White] = Ejercicio.Dificultad == null ? string.Empty : $"Dificultad: {Ejercicio.Dificultad}";
                }
            }
        }

        /// <summary>
        /// Obtiene o establece la explicación que se está mostrando.
        /// </summary>
        public Explicacion Explicacion
        {
            get => explicacion;
            set
            {
                explicacion = value;
                if (explicacion == null)
                {
                    RestaurarToolTips();
                    TableroAjedrez.IsHitTestVisible = true;
                }
                else
                {
                    TableroAjedrez.IsHitTestVisible = false;
                    runNombreBlanco.ToolTip = "Autor de la explicación";
                    runNombreNegro.ToolTip = "Nombre de la explicación";
                    Jugadas.ToolTip = "Explicación";
                    TableroAjedrez.Partida.Etiquetas[EtiquetaPGN.Black] = explicacion.Nombre == null ? string.Empty : explicacion.Nombre;
                    TableroAjedrez.Partida.Etiquetas[EtiquetaPGN.White] = explicacion.Autor == null ? string.Empty : explicacion.Autor;
                    TableroAjedrez.Partida.TextoJugadas = explicacion.Descripcion;
                    TableroAjedrez.Partida.Etiquetas[EtiquetaPGN.Result] = string.Empty;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el torneo que se está realizando.
        /// </summary>
        public Torneo Torneo { get; set; }

        /// <summary>
        /// Permite cancelar la lectura de un archivo.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; } =  new CancellationTokenSource();
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Motor.NuevaPartida(TableroAjedrez.Partida);
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Actualiza los resultados de la posición según la base de datos, no hace nada si los resultados no son visibles.
        /// </summary>
        private void ActualizarResultados()
        {
            if (mostrarResultadosJugada.IsChecked == true)
            {
                Resultados = new List<ResultadoJugada>();
                try
                {
                    using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                    {
                        conexion.Open();
                        using (SqlCommand select = new SqlCommand($"SELECT t.Jugada, COUNT(*), AVG(CASE WHEN Resultado = '1-0' THEN 100 WHEN Resultado <> '1-0' THEN 0 END), AVG(CASE WHEN Resultado = " +
                            $"'1/2-1/2' THEN 100 WHEN Resultado <> '1/2-1/2' THEN 0 END), AVG(CASE WHEN Resultado = '0-1' THEN 100 WHEN Resultado <> '0-1' THEN 0 END) FROM Tiene AS t JOIN Partida as p ON " +
                            $"t.Id_Partida = p.Id WHERE t.Jugada IS NOT NULL AND t.Descripcion_Posicion = '{TableroAjedrez.Partida.Posicion.Descripcion}' AND Resultado <> '*' GROUP BY t.Jugada ORDER BY 2 " +
                            $"DESC", conexion))
                        {
                            using (SqlDataReader reader = select.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Resultados.Add(new ResultadoJugada(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4)));
                                }
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Error al obtener las jugadas de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                resultadosJugada.ItemsSource = Resultados;
            }
        }

        /// <summary>
        /// Vuelva a poner el valor que tenían antes los tooltips.
        /// </summary>
        private void RestaurarToolTips()
        {
            runNombreBlanco.ToolTip = "Nombre del blanco";
            runNombreNegro.ToolTip = "Nombre del negro";
            Jugadas.ToolTip = "Jugadas realizadas en la partida";
        }

        /// <summary>
        /// Lee las partidas de un arhivo.
        /// </summary>
        /// <param name="nombreArchivo">Nombre de arhivo del que se van a leer las partidas.</param>
        /// <exception cref="OperationCanceledException"></exception>
        private List<Partida> LeerPGN(string nombreArchivo, CancellationToken cancellationToken, IProgress<(int, int)> progress)
        {
            List<Partida> partidas = new List<Partida>();
            StringBuilder builderPartida = new StringBuilder();
            bool enEtiqueta = false;
            bool enComentario = false;
            bool enJugadas = false;
            int cantidadErrores = 0;
            foreach (string linea in File.ReadAllLines(nombreArchivo).Append(string.Empty)) // Agrega un string vacio para garantizar que se agregue la última partida.
            {
                if (linea == string.Empty)
                {
                    // Una linea vacia indica el final de la sección de etiquetas o de la sección de jugadas si no está dentro de un comentario o de una etiqueta.
                    if (!enEtiqueta && !enComentario)
                    {
                        enJugadas = !enJugadas;
                        if (!enJugadas) // Si luego cambiar enJugadas no se está en la sección de jugadas, se está en la sección de etiquetas de una nueva partida.
                        {
                            if (builderPartida.Length > 0) // Si hay una partida.
                            {
                                try
                                {
                                    partidas.Add(new Partida(builderPartida.ToString())); // Agrega la partida a la lista de partidas.
                                }
                                catch
                                {
                                    cantidadErrores++;
                                }
                                builderPartida.Clear(); // Quita la partida ya agregada del StringBuilder.
                                if ((partidas.Count + cantidadErrores) % 150 == 0)
                                {
                                    progress.Report((partidas.Count, cantidadErrores));
                                    cancellationToken.ThrowIfCancellationRequested();
                                }
                            }
                        }
                    }
                    else
                    {
                        builderPartida.AppendLine();
                    }
                }
                else if (linea[0] != '%') // El % en la primera posición indica que la linea debe ignorarse.
                {
                    if (enJugadas)
                    {
                        if (linea[0] == ';') // El ; en la primera posición indica que la linea es un comentario.
                        {
                            if (builderPartida[builderPartida.Length - 3] == '}') // Comprueba si al final de la linea anterior se cerró un comentario.
                            {
                                // Si cerró el comentario borra el caracter que lo cierra y el espacio que lo separa del resto de jugadas.
                                builderPartida.Remove(builderPartida.Length - 3, 2);
                            }
                            else
                            {
                                // Si no se cerró un comentario en la última linea se abre uno nuevo para insertar el comentario de esta linea.
                                builderPartida.Append(" {");
                            }
                            builderPartida.AppendLine(linea.Substring(1, linea.Length - 1) + "} "); // Agrega el comentario entre llaves.
                        }
                        else
                        {
                            // La linea termina en comentario si no se cerró el comentario empezado en la linea anterior o si se empezó uno que no se terminó.
                            enComentario = (enComentario && !linea.Contains('}')) || (linea.LastIndexOf('{') > linea.LastIndexOf('}'));
                            builderPartida.AppendLine(linea);
                        }
                    }
                    else
                    {
                        // La linea termina en etiqueta si no se cerró la etiqueta empezada en la linea anterior o si se empezó una que no se terminó.
                        enEtiqueta = (enEtiqueta && !linea.Contains(']')) || (linea.LastIndexOf('[') > linea.LastIndexOf(']'));
                        builderPartida.AppendLine(linea);
                    }
                }
            }
            return partidas;
        }
        #endregion

        #region Controladores de eventos
        private void Window_Closed(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }

        private void JugadaAnterior_CanExecuted(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = TableroAjedrez?.Partida?.EsPrimeraPosicion() == false;

        private void JugadaSiguiente_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TableroAjedrez.Partida.IndicePosicion++;
            TableroAjedrez.MostrarPosicion();
            ActualizarResultados();
        }

        private void JugadaAnterior_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TableroAjedrez.Partida.IndicePosicion--;
            TableroAjedrez.MostrarPosicion();
            ActualizarResultados();
        }

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                TableroAjedrez.Partida.DeshacerJugada();
            }
            catch
            {
                MessageBox.Show("No se ha podido actualizar el resultado en la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            TableroAjedrez.MostrarPosicion();
            if (analizar.IsChecked == true)
            {
                try
                {
                    Motor.Analizar();
                }
                catch
                {
                    MessageBox.Show("El motor de análisis no responde correctamente, quedará deshabilitado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    motorHabilitado = false;
                }
            }
            ActualizarResultados();
        }

        private void JugadaSiguiente_CanExecuted(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = TableroAjedrez?.Partida.EsUltimaPosicion() == false;

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = TableroAjedrez?.Partida?.EnJuego == false;

        private async void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = FiltroPGN };
            if (openFileDialog.ShowDialog() == true)
            {
                ventanaProgreso = new Progreso(this);
                try
                {
                    ventanaProgreso.Show();
                    IsHitTestVisible = false;
                }
                catch
                {
                    MessageBox.Show("No se ha podido mostrar la ventana de progreso", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // No se puede seguir sin ventana de progreso.
                }
                Progress<(int, int)> progress = new Progress<(int, int)>(p => ventanaProgreso.Actualizar(p.Item1, p.Item2));
                try
                {
                    List<Partida> partidas = await Task.Run(() => LeerPGN(openFileDialog.FileName, CancellationTokenSource.Token, progress));
                    IsHitTestVisible = true;
                    ventanaProgreso.Close();
                    ElegirPartida elegirPartida = new ElegirPartida(partidas, this);
                    elegirPartida.ShowDialog();
                }
                catch (OperationCanceledException)
                {
                    IsHitTestVisible = true;
                    MessageBox.Show("Se ha cancelado la lectura del arhivos", "Lectura cancelado", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    ventanaProgreso.Close();
                    IsHitTestVisible = true;
                    MessageBox.Show("No se han podido leer las partidas", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TableroAjedrez_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableroAjedrez.Partida))
            {
                TableroAjedrez.MostrarPosicion();
                analizar.IsChecked = false;
                try
                {
                    Motor.NuevaPartida(TableroAjedrez.Partida);
                }
                catch
                {
                    MessageBox.Show("El motor de análisis no responde correctamente, quedará deshabilitado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    motorHabilitado = false;
                }
                Ejercicio = null;
                Explicacion = null;
                if (mostrarResultadosJugada.IsChecked == true)
                {
                    ActualizarResultados();
                }
            }
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ElegirEtiquetas elegirEtiquetas = new ElegirEtiquetas(this);
            if (elegirEtiquetas.ShowDialog() == true)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = FiltroPGN };
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        if (TableroAjedrez.Partida.Etiquetas[EtiquetaPGN.SetUp] == "1") // Agrega las etiquetas SetUp y FEN si son necesarias.
                        {
                            EtiquetasOpcionales.Add(EtiquetaPGN.SetUp);
                            EtiquetasOpcionales.Add(EtiquetaPGN.FEN);
                        }
                        File.WriteAllText(saveFileDialog.FileName, TableroAjedrez.Partida.ToPGN(EtiquetasOpcionales));
                    }
                    catch
                    {
                        MessageBox.Show("No se ha podido guardar la partida", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void EditarDatosPartida_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarDatosPartida editarDatosPartida = new EditarDatosPartida(TableroAjedrez.Partida);
            try
            {
                editarDatosPartida.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Undo_CanExecuted(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = TableroAjedrez?.Partida?.EsPrimeraPosicion() == false && TableroAjedrez.Partida.EsUltimaPosicion() &&
            (Settings.Default.SePuedeDeshacerEnJuego || !TableroAjedrez.Partida.EnJuego);

        private void Jugar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = TableroAjedrez?.Partida?.EnJuego == false && TableroAjedrez.Partida.Etiquetas[EtiquetaPGN.Result] ==
            "*" && Ejercicio == null && Explicacion == null;

        private void Jugar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ElegirRivales elegirRivales = new ElegirRivales(TableroAjedrez, (ModoDeJuego)Convert.ToInt32(e.Parameter));
            try
            {
                if (elegirRivales.ShowDialog() == true)
                {
                    TableroAjedrez.Jugar();
                }
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Abandonar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Ejercicio != null || (TableroAjedrez?.Partida?.EnJuego == true &&
            TableroAjedrez.Motor[TableroAjedrez.Partida.UltimaPosicion.Turno] == null);

        private void Abandonar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Ejercicio != null)
            {
                if (MessageBox.Show($"La jugada correcta era {Ejercicio.Solucion}" + Environment.NewLine + " ¿Quiere hacer otro ejercicio?", "Ejercicio abandonado", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Comandos.RealizarEjercicio.Execute(null, this);
                    }
                    catch
                    {
                        MessageBox.Show("No se ha podido seleccionar el ejercicio", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                TableroAjedrez.Partida.Abandonar(TableroAjedrez.Partida.UltimaPosicion.Turno);
            }
        }

        public void Partida_PartidaTerminada(object sender, EventArgs e) => Torneo?.PreperarSiguientePartida(TableroAjedrez.Partida);

        private void NuevaPartida_Executed(object sender, ExecutedRoutedEventArgs e) => TableroAjedrez.Partida = new Partida();

        private void OfrecerTablas_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = TableroAjedrez?.Partida?.EnJuego == true && TableroAjedrez.Motor[Equipo.Blanco] == null &&
            TableroAjedrez.Motor[Equipo.Negro] == null && TableroAjedrez.Partida.ProximaPropuestaDeTablas < 1;

        private void OfrecerTablas_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TableroAjedrez.Partida.ProximaPropuestaDeTablas = 5;
            if (MessageBox.Show("El rival ofrece tablas", "Oferta de Tablas", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                TableroAjedrez.Partida.AcordarTablas();
            }
        }

        private void OfrecerRevancha_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = TableroAjedrez?.Partida?.Revancha != null;

        private void OfrecerRevancha_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("El rival ofrece jugar una revancha", "Oferta de Revancha", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Motor temporal = TableroAjedrez.Motor[Equipo.Blanco];
                TableroAjedrez.Motor[Equipo.Blanco] = TableroAjedrez.Motor[Equipo.Negro];
                TableroAjedrez.Motor[Equipo.Negro] = temporal;
                TableroAjedrez.Partida = TableroAjedrez.Partida.Revancha;
                TableroAjedrez.Jugar();
            }
        }

        private void JugarInmediatamente_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Settings.Default.SePuedeExigirJugada &&
            TableroAjedrez.Motor[TableroAjedrez.Partida.Posicion.Turno]?.Listo == true;

        private void JugarInmediatamente_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                TableroAjedrez.Motor[TableroAjedrez.Partida.Posicion.Turno].DetenerAnalisis();
            }
            catch
            {
                MessageBox.Show("El motor de análisis no responde correctamente, quedará deshabilitado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                motorHabilitado = false;
            }
        }

        private void Analizar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Motor?.Partida?.EnJuego == false && Motor?.Listo == true && Ejercicio == null && motorHabilitado;

        private void Analizar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (analizar.IsChecked == true)
                {
                    Motor.Analizar();
                }
                else
                {
                    Motor.DetenerAnalisis();
                }
            }
            catch
            {
                MessageBox.Show("El motor de análisis no responde correctamente, quedará deshabilitado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                motorHabilitado = false;
            }
        }

        private void TableroAjedrez_JugadaRealizada(object sender, EventArgs e)
        {
            if (analizar.IsChecked == true)
            {
                Motor.Analizar();
            }
            ActualizarResultados();
            if (Ejercicio != null)
            {
                if ((TableroAjedrez.Partida.Jugadas[0].JugadaNegra ?? TableroAjedrez.Partida.Jugadas[0].JugadaBlanca).ToString(TableroAjedrez.Partida.PosicionInicial) == Ejercicio.Solucion)
                {
                    // Si se realizó la jugada correcta.
                    TableroAjedrez.Partida = new Partida();
                    if (MessageBox.Show("Ha resuelto este ejrcicio" + Environment.NewLine + " ¿Quiere hacer otro?", "Jugada correcta", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                        MessageBoxResult.Yes)
                    {
                        try
                        {
                            Comandos.RealizarEjercicio.Execute(null, this);
                        }
                        catch
                        {
                            MessageBox.Show("No se han podido seleccionar los ejercicios", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            TableroAjedrez.Partida = new Partida();
                        }
                    }
                }
                else
                {
                    if (MessageBox.Show("Ha fallado este ejrcicio" + Environment.NewLine + " ¿Quiere volver a intentarlo?", "Jugada incorrecta", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                        MessageBoxResult.Yes)
                    {
                        try
                        {
                            ApplicationCommands.Undo.Execute(null, this);
                        }
                        catch
                        {
                            MessageBox.Show("Error al intentar restablecer la posición", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            TableroAjedrez.Partida = new Partida();
                        }
                    }
                    else
                    {
                        TableroAjedrez.Partida = new Partida();
                    }
                }
            }
        }

        private void SeguirAnalizando_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = analizar.IsChecked == true;

        private void SeguirAnalizando_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Motor.SeguirAnalizando();
            }
            catch
            {
                MessageBox.Show("El motor de análisis no responde correctamente, quedará deshabilitado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                motorHabilitado = false;
            }
        }

        private void InsertarPartida_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = TableroAjedrez?.Partida?.EnJuego == false && Ejercicio == null && Explicacion == null;

        private void InsertarPartida_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
               TableroAjedrez.Partida.InsertarEnBaseDeDatos();
               MessageBox.Show("Se ha insertado la partida correctamente", "Partida insertada", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("No se ha podido insertar la partida en la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SeleccionarPartida_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ElegirPartida elegirPartida = new ElegirPartida(Partida.ObtenerPartidasDesdeBaseDeDatos(), this);
                elegirPartida.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se han podido obtener las partidas de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarPartida_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = TableroAjedrez?.Partida?.Id != null && TableroAjedrez.Partida.EnJuego == false;

        private void EliminarPartida_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Settings.Default.EliminarDirectamente || MessageBox.Show("¿Está seguro de que quiere eliminar esta partida?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                try
                {
                    TableroAjedrez.Partida.BorrarDeBaseDeDatos();
                    MessageBox.Show("Se ha eliminado la partida correctamente", "Partida eliminada", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show("Ha ocurrido un error al intentar elimnar la partida de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void MostrarResultadosJugada_Executed(object sender, ExecutedRoutedEventArgs e) => ActualizarResultados();

        private void IrAlInicio_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TableroAjedrez.Partida.IrAlInicio();
            TableroAjedrez.MostrarPosicion();
            ActualizarResultados();
        }

        private void IrAlFinal_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TableroAjedrez.Partida.IndicePosicion = TableroAjedrez.Partida.CantidadPosicionesSinContarInicial() - 1;
            TableroAjedrez.MostrarPosicion();
            ActualizarResultados();
        }

        private void RealizarEjercicio_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ElegirEjercicio elegirEjercicio = new ElegirEjercicio(Ejercicio.ObtenerEjerciciosDesdeBaseDeDatos(), this);
                elegirEjercicio.ShowDialog();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("No se han podido obtener los ejercios de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                TableroAjedrez.Partida = new Partida();
            }
        }

        private void CrearEjercicio_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarEjercicio editarDatosEjercicio = new EditarEjercicio(new Ejercicio());
            try
            {
                editarDatosEjercicio.ShowDialog();

            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditarEjercicio_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Ejercicio != null;

        private void EditarEjercicio_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarEjercicio editarDatosEjercicio = new EditarEjercicio(Ejercicio);
            try
            {
                editarDatosEjercicio.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarEjercicio_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Ejercicio?.Id != null;

        private void EliminarEjercicio_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Settings.Default.EliminarDirectamente || MessageBox.Show("¿Está seguro de que quiere eliminar este ejercicio?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                    {
                        conexion.Open();
                        using (SqlCommand delete = new SqlCommand($"DELETE FROM Ejercicio WHERE Id = {Ejercicio.Id}", conexion))
                        {
                            delete.ExecuteNonQuery();
                            Ejercicio = null;
                            TableroAjedrez.Partida = new Partida();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Ha ocurrido un error al intentar elimnar el ejercicio de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SeleccionarExplicacion_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ElegirExplicacion elegirExplicacion = new ElegirExplicacion(Explicacion.ObtenerExplicacionesDesdeBaseDeDatos(), this);
                elegirExplicacion.ShowDialog();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("No se han podido obtener las explicaciones de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                TableroAjedrez.Partida = new Partida();
            }
        }

        private void EditarExplicacion_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Explicacion != null;

        private void EditarExplicacion_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarExplicacion editarExplicacion = new EditarExplicacion(Explicacion);
            try
            {
                editarExplicacion.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CrearExplicacion_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarExplicacion editarExplicacion = new EditarExplicacion(new Explicacion());
            try
            {
                editarExplicacion.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarExplicacion_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Explicacion?.Id != null;

        private void EliminarExplicacion_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Settings.Default.EliminarDirectamente || MessageBox.Show("¿Está seguro de que quiere eliminar esta explicación?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                try
                { 
                    using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                    {
                        conexion.Open();
                        using (SqlCommand delete = new SqlCommand($"DELETE FROM Explicacion WHERE Id = {Explicacion.Id}", conexion))
                        {
                            delete.ExecuteNonQuery();
                            Explicacion = null;
                            TableroAjedrez.Partida = new Partida();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Ha ocurrido un error al intentar elimnar la partida de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SeleccionarExplicacionRealacionada_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ElegirExplicacion elegirExplicacion = new ElegirExplicacion(Explicacion.ExplicacionesRelacionadas, this);
            try
            {
                elegirExplicacion.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditarComentario_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = TableroAjedrez?.Partida?.Jugadas?.Count > 0 && !TableroAjedrez.Partida.EnJuego;

        private void EditarComentario_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarComentario editarComentario = new EditarComentario(this);
            try
            {
                editarComentario.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditarNAG_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int indicadorNAG = Convert.ToInt32(e.Parameter);
            if (indicadorNAG > 19 && TableroAjedrez.Partida.Posicion.Turno == Equipo.Blanco)
            {
                indicadorNAG++;
            }
            TableroAjedrez.Partida.NAGPosicionActual = indicadorNAG;
        }

        private void CrearJugador_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarJugador editarJugador = new EditarJugador(new Jugador());
            try
            {
                editarJugador.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CrearApertura_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarApertura editarApertura = new EditarApertura(new Apertura());
            try
            {
                editarApertura.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SeleccionarApertura_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ElegirApertura elegirApertura = new ElegirApertura(Apertura.ObtenerAperturasDesdeBaseDeDatos(), this);
                elegirApertura.ShowDialog();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("No se han podido obtener las aperturas de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SeleccionarJugador_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ElegirJugador elegirJugador = new ElegirJugador(Jugador.ObtenerJugadoresDesdeBaseDeDatos(), this);
                elegirJugador.ShowDialog();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("No se han podido obtener los ejercios de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CrearEvento_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditarEvento editarEvento = new EditarEvento(new Evento());
            try
            {
                editarEvento.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SeleccionarEvento_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ElegirEvento elegirEvento = new ElegirEvento(Evento.ObtenerEventosDesdeBaseDeDatos(), this);
                elegirEvento.ShowDialog();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("No se han podido obtener los eventos de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RealizarTorneoEliminatorio_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Torneo = new TorneoEliminatorio(this);
            try
            {
                ConfigurarTorneoEliminatorio configurarTorneo = new ConfigurarTorneoEliminatorio((TorneoEliminatorio)Torneo);
                if (configurarTorneo.ShowDialog() == true)
                {
                    Torneo.TerminarConfiguracion();
                    Torneo.ShowDialog();
                }
                else
                {
                    Torneo = null;
                }
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RealizarTorneoSuizo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Torneo = new TorneoSuizo(this);
                ConfigurarTorneoSuizo configurarTorneo = new ConfigurarTorneoSuizo((TorneoSuizo)Torneo);
                if (configurarTorneo.ShowDialog() == true)
                {
                    Torneo.TerminarConfiguracion();
                    Torneo.ShowDialog();
                }
                else
                {
                    Torneo = null;
                }
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Configurar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Opciones opciones = new Opciones();
            try
            {
                opciones.ShowDialog();
            }
            catch
            {
                MessageBox.Show("No se ha podido mostrar la ventana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Restaurar_Executed(object sender, ExecutedRoutedEventArgs e) => Settings.Default.Reset();

        private void CopiarFEN_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(TableroAjedrez.Partida.Posicion.ToString());
            }
            catch
            {
                MessageBox.Show("No se ha podido copiar el FEN", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e) => Close();

        private void Minimizar_Executed(object sender, ExecutedRoutedEventArgs e) => WindowState = WindowState.Minimized;

        private async void InsertarTodasLasPartidasDeUnPGN_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = FiltroPGN };
            if (openFileDialog.ShowDialog() == true)
            {
                ventanaProgreso = new Progreso(this);
                try
                {
                    ventanaProgreso.Show();
                    IsHitTestVisible = false;
                }
                catch
                {
                    MessageBox.Show("No se ha podido mostrar la ventana de progreso", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // No se puede seguir sin ventana de progreso.
                }
                Progress<(int, int)> progress = new Progress<(int, int)>(p => ventanaProgreso.Actualizar(p.Item1, p.Item2));
                try
                {
                    List<Partida> partidas = await Task.Run(() => LeerPGN(openFileDialog.FileName, CancellationTokenSource.Token, progress));
                    IsHitTestVisible = true;
                    try
                    {
                        foreach (Partida partida in partidas)
                        {
                            partida.InsertarEnBaseDeDatos();
                        }
                        MessageBox.Show("Se han insertado las partidas correctamente", "Partidas insertadas", MessageBoxButton.OK, MessageBoxImage.Information);
                        ventanaProgreso.Close();
                    }
                    catch
                    {
                        MessageBox.Show("No se han podido insertar algunas partidas en la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        ventanaProgreso.Close();
                    }
                }
                catch (OperationCanceledException)
                {
                    IsHitTestVisible = true;
                    MessageBox.Show("Se ha cancelado la lectura del arhivos", "Lectura cancelado", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    ventanaProgreso.Close();
                    IsHitTestVisible = true;
                    MessageBox.Show("No se han podido leer las partidas", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
    }
}
