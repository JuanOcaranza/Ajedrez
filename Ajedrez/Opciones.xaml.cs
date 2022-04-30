using static Ajedrez.Properties.Settings;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para Opciones.xaml
    /// </summary>
    public partial class Opciones : VentanaSinBotones
    {
        #region Atributos
        /// <summary>
        /// Cantidad de errores de validación.
        /// </summary>
        private int cantidadErrores = 0;

        /// <summary>
        /// Color de fondo de la base de datos.
        /// </summary>
        private readonly string backgroundBaseDeDatos = Default.BackgroundBaseDeDatos;

        /// <summary>
        /// Color de fondo de las jugadas.
        /// </summary>
        private readonly string backgroundJugadas = Default.BackgroundJugadas;

        /// <summary>
        /// Color de fondo de los jugadores.
        /// </summary>
        private readonly string backgroundJugadores = Default.BackgroundJugadores;

        /// <summary>
        /// Color de fondo del motor.
        /// </summary>
        private readonly string backgroundMotor = Default.BackgroundMotor;

        /// <summary>
        /// Color de fondo del reloj.
        /// </summary>
        private readonly string backgroundReloj = Default.BackgroundReloj;

        /// <summary>
        /// Cantidad de lines mostradas en el análisis.
        /// </summary>
        private readonly int cantidadDeLineasAnalisis = Default.CantidadDeLineasAnalisis;

        /// <summary>
        /// Tipo de caracteres usados en las jugadas para indicar las piezas.
        /// </summary>
        private readonly int carcateresPieza = Default.CaracteresPieza;

        /// <summary>
        /// Color de las casillas blancas.
        /// </summary>
        private readonly string colorCasillasBlancas = Default.ColorCasillasBlancas;

        /// <summary>
        /// Color de las casillas negras.
        /// </summary>
        private readonly string colorCasillasNegras = Default.ColorCasillasNegras;

        /// <summary>
        /// Si se debe coronar siempre dama.
        /// </summary>
        private readonly bool coronarSiempreDama = Default.CoronarSiempreDama;

        /// <summary>
        /// Si se deben eliminar valore de la base de datos sin necesidad de confirmanción.
        /// </summary>
        private readonly bool eliminarDirectamente = Default.EliminarDirectamente;

        /// <summary>
        /// Tamño del hash del motor de análisis.
        /// </summary>
        private readonly string hashAnalisis = Default.HashAnalisis;

        /// <summary>
        /// Cantidad de hilos para el motor de análisis.
        /// </summary>
        private readonly string hilosAnalisis = Default.HilosAnalisis;

        /// <summary>
        /// Nombre del motor de análisis.
        /// </summary>
        private readonly string motor = Default.Motor;

        /// <summary>
        /// Cantidad de jugadas desde la posición actual que analiza el motor.
        /// </summary>
        private readonly string profundidadAnalisis = Default.ProfundidadAnalisis;

        /// <summary>
        /// Cantidad de jugadsa desde la posición actual que analiza el motor cuando juega una partida sin tiempo.
        /// </summary>
        private readonly string profundidadConTiempoIlimitado = Default.ProfundidadConTiempoIlimitado;

        /// <summary>
        /// Si se puede volver atras mientras se está jugando una partida.
        /// </summary>
        private readonly bool sePuedeDeshacerEnJuego = Default.SePuedeDeshacerEnJuego;

        /// <summary>
        /// Si se puede exigir al motor rival que juegue inmediatamente.
        /// </summary>
        private readonly bool sePuedeExigirJugada = Default.SePuedeExigirJugada;

        /// <summary>
        /// Tamaño de las letras con las que se indican las jugadas.
        /// </summary>
        private readonly uint tamañoLetraJugadas = Default.TamañoLetraJugadas;

        /// <summary>
        /// Color de fondo de los data grid.
        /// </summary>
        private readonly string backgroundDataGrid = Default.BackgroundDataGrid;

        /// <summary>
        /// Color de fondo de las filas de los data grid.
        /// </summary>
        private readonly string backgroundDataGridRow = Default.BackgroundDataGridRow;

        /// <summary>
        /// Color de fondo de las filas alternativas de los data grid.
        /// </summary>
        private readonly string backgroundDataGridRowAlternativo = Default.BackgroundDataGridRowAlternativo;

        /// <summary>
        /// Color de fondo de las ventanas, grillas y controles que queden abajo.
        /// </summary>
        private readonly string background = Default.Background;

        /// <summary>
        /// Color de fondo de los botones.
        /// </summary>
        private readonly string backgroundBotones = Default.BackgroundBotones;

        /// <summary>
        /// Color de fondo de los menús.
        /// </summary>
        private readonly string backgroundMenu = Default.BackgroundMenu;

        /// <summary>
        /// Color de fondo de la barra de los menús.
        /// </summary>
        private readonly string backgroundBarraMenu = Default.BackgroundBarraMenu;

        /// <summary>
        /// Color de fondo de los combo box.
        /// </summary>
        private readonly string backgroundComboBox = Default.BackgroundComboBox;

        /// <summary>
        /// Color de los bloques de texto y las etiquetas.
        /// </summary>
        private readonly string colorTexto = Default.ColorTexto;

        /// <summary>
        /// Color del texto de la barra de menús.
        /// </summary>
        private readonly string colorTextoBarraMenu = Default.ColorTextoBarraMenu;

        /// <summary>
        /// Color del texto que indica que esa es la parte de la base de datos.
        /// </summary>
        private readonly string colorTextoBaseDeDatos = Default.ColorTextoBaseDeDatos;

        /// <summary>
        /// Color del texto de los botones.
        /// </summary>
        private readonly string colorTextoBotones = Default.ColorTextoBotones;

        /// <summary>
        /// Color del texto de los combo box.
        /// </summary>
        private readonly string colorTextoComboBox = Default.ColorTextoComboBox;

        /// <summary>
        /// Color del texto de los data grid.
        /// </summary>
        private readonly string colorTextoDataGrid = Default.ColorTextoDataGrid;

        /// <summary>
        /// Color del texto de las jugadas.
        /// </summary>
        private readonly string colorTextoJugadas = Default.ColorTextoJugadas;

        /// <summary>
        /// Color del texto con los nombres de los jugadores.
        /// </summary>
        private readonly string colorTextoJugadores = Default.ColorTextoJugadores;

        /// <summary>
        /// Color del texto de los menús.
        /// </summary>
        private readonly string colorTextoMenu = Default.ColorTextoMenu;

        /// <summary>
        /// Color del texto con el nombre del motor.
        /// </summary>
        private readonly string colorTextoMotor = Default.ColorTextoMotor;

        /// <summary>
        /// Color del texto que indica el timepo restante.
        /// </summary>
        private readonly string colorTextoReloj = Default.ColorTextoReloj;

        /// <summary>
        /// Color del texto de las cajas de texto.
        /// </summary>
        private readonly string colorTextoTextBox = Default.ColorTextoTextBox;

        /// <summary>
        /// Color del fondo de las cajas de texto.
        /// </summary>
        private readonly string backgroundTextBox = Default.BackgroundTextBox;

        /// <summary>
        /// Color del fonto de las list box.
        /// </summary>
        private readonly string backgroundListBox = Default.BackgroundListBox;

        /// <summary>
        /// Color del texto que indica lso errores.
        /// </summary>
        private readonly string colorTextoError = Default.ColorTextoError;

        /// <summary>
        /// Color del texto de los ítems en las listas.
        /// </summary>
        private readonly string colorTextoListBox = Default.ColorTextoListBox;

        /// <summary>
        /// Color del fondo de la cabezera de los data grid.
        /// </summary>
        private readonly string backgroundDataGridHeader = Default.BackgroundDataGridHeader;

        /// <summary>
        /// Color del texto de la cabezera de los data grid.
        /// </summary>
        private readonly string colorTextoDataGridHeader = Default.ColorTextoDataGridHeader;

        /// <summary>
        /// Color del borde de la casilla seleccionada.
        /// </summary>
        private readonly string colorBordeCasillaSeleccionada = Default.ColorBordeCasillaSeleccionada;

        /// <summary>
        /// Conjunto de piezas que se muestra en el tablero.
        /// </summary>
        private readonly string conjuntoPiezas = Default.ConjuntoPiezas;

        /// <summary>
        /// Si se debe avisa puede ser necesario reiniciar para aplicar todos los cambios.
        /// </summary>
        private readonly bool avisarReiniciar = Default.AvisarReiniciar;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Opciones"/>
        /// </summary>
        public Opciones()
        {
            InitializeComponent();
            try
            {
                foreach (DirectoryInfo directory in new DirectoryInfo(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Controles\\Piezas").EnumerateDirectories())
                {
                    comoboBoxConjuntoDePiezas.Items.Add(directory.Name); // Agrega cada carpeta con controles que representan piezas.
                }
            }
            catch
            {
                MessageBox.Show("No se han podido cargar todos los conjuntos de piezas", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            try
            {
                foreach (FileInfo file in new DirectoryInfo(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Motores").EnumerateFiles())
                {
                    comoboBoxMotor.Items.Add(file.Name);
                }
            }
            catch
            {
                MessageBox.Show("No se han podido cargar todos los motores de análisis", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            comboBoxMovimiento.SelectedIndex = Default.MoverArrastrando ? 1 : 0;
        }
        #endregion

        #region Controladores de eventos
        private void Aceptar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = cantidadErrores == 0;

        private void Aceptar_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Default.MoverArrastrando = comboBoxMovimiento.SelectedIndex == 1;
            if (Default.AvisarReiniciar)
            {
                MessageBox.Show("Es posible que tenga que reiniciar la aplicación para que se apliquen todos los cambios", "Reiniciar para aplicar cambios", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            Close();
        }

        private void TextBox_ValidationError(object sender, ValidationErrorEventArgs e) => cantidadErrores += e.Action == ValidationErrorEventAction.Added ? 1 : -1;

        private void Cancelar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Cancelar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Default.AvisarReiniciar = avisarReiniciar;
            Default.Background = background;
            Default.BackgroundBarraMenu = backgroundBarraMenu;
            Default.BackgroundBaseDeDatos = backgroundBaseDeDatos;
            Default.BackgroundBotones = backgroundBotones;
            Default.BackgroundComboBox = backgroundComboBox;
            Default.BackgroundDataGrid = backgroundDataGrid;
            Default.BackgroundDataGridHeader = backgroundDataGridHeader;
            Default.BackgroundDataGridRow = backgroundDataGridRow; 
            Default.BackgroundDataGridRowAlternativo = backgroundDataGridRowAlternativo;
            Default.BackgroundJugadas = backgroundJugadas;
            Default.BackgroundJugadores = backgroundJugadores;
            Default.BackgroundListBox = backgroundListBox;
            Default.BackgroundMotor = backgroundMotor;
            Default.BackgroundMenu = backgroundMenu;
            Default.BackgroundReloj = backgroundReloj;
            Default.BackgroundTextBox = backgroundTextBox;
            Default.CantidadDeLineasAnalisis = cantidadDeLineasAnalisis;
            Default.CaracteresPieza = carcateresPieza;
            Default.ColorBordeCasillaSeleccionada = colorBordeCasillaSeleccionada;
            Default.ColorCasillasBlancas = colorCasillasBlancas;
            Default.ColorCasillasNegras = colorCasillasNegras;
            Default.ColorTexto = colorTexto;
            Default.ColorTextoBarraMenu = colorTextoBarraMenu;
            Default.ColorTextoBaseDeDatos = colorTextoBaseDeDatos;
            Default.ColorTextoBotones = colorTextoBotones;
            Default.ColorTextoComboBox = colorTextoComboBox;
            Default.ColorTextoDataGrid = colorTextoDataGrid;
            Default.ColorTextoDataGridHeader = colorTextoDataGridHeader;
            Default.ColorTextoError = colorTextoError;
            Default.ColorTextoJugadas = colorTextoJugadas;
            Default.ColorTextoJugadores = colorTextoJugadores;
            Default.ColorTextoListBox = colorTextoListBox;
            Default.ColorTextoMenu = colorTextoMenu;
            Default.ColorTextoMotor = colorTextoMotor;
            Default.ColorTextoReloj = colorTextoReloj;
            Default.ColorTextoTextBox = colorTextoTextBox;
            Default.ConjuntoPiezas = conjuntoPiezas;
            Default.CoronarSiempreDama = coronarSiempreDama;
            Default.EliminarDirectamente = eliminarDirectamente;
            Default.HashAnalisis = hashAnalisis;
            Default.HilosAnalisis = hilosAnalisis;
            Default.Motor = motor;
            Default.ProfundidadAnalisis = profundidadAnalisis;
            Default.ProfundidadConTiempoIlimitado = profundidadConTiempoIlimitado;
            Default.SePuedeDeshacerEnJuego = sePuedeDeshacerEnJuego;
            Default.SePuedeExigirJugada = sePuedeExigirJugada;
            Default.TamañoLetraJugadas = tamañoLetraJugadas;
            Close();
        }
        #endregion
    }
}
