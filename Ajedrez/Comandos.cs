using System.Windows;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Contiene los comandos.
    /// </summary>
    public static class Comandos
    {
        /// <summary>
        /// Comando para mostrar la posición en la jugada siguiente.
        /// </summary>
        public static readonly RoutedUICommand JugadaSiguiente = new RoutedUICommand("Ju_gada siguiente", "JugadaSiguiente", typeof(MainWindow), new InputGestureCollection { new
            KeyGesture(Key.Right) });

        /// <summary>
        /// Comando para mostrar la posición en la jugada anterior.
        /// </summary>
        public static readonly RoutedUICommand JugadaAnterior = new RoutedUICommand("Jugada a_nterior", "JugadaAnterior", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.Left) });

        /// <summary>
        /// Comando para cerrar una ventana conservando los cambios realizados.
        /// </summary>
        public static readonly RoutedCommand Aceptar = new RoutedCommand("Aceptar", typeof(Window), new InputGestureCollection { new KeyGesture(Key.Enter) });

        /// <summary>
        /// Comando para cerrar una ventana descartando los cambios realizados.
        /// </summary>
        public static readonly RoutedCommand Cancelar = new RoutedCommand("Cancelar", typeof(Window), new InputGestureCollection { new KeyGesture(Key.Escape) });

        /// <summary>
        /// Comandos para editar los datos de la partida.
        /// </summary>
        public static readonly RoutedUICommand EditarDatosPartida = new RoutedUICommand("Modi_ficar datos...", "EditarDatosPartida", typeof(MainWindow), new InputGestureCollection { new
            KeyGesture(Key.D, ModifierKeys.Control) });

        /// <summary>
        /// Comandos para jugar la partida actual.
        /// </summary>
        public static readonly RoutedCommand Jugar = new RoutedCommand("Jugar", typeof(Window));

        /// <summary>
        /// Comandos para abandonar la partida acutal.
        /// </summary>
        public static readonly RoutedUICommand Abandonar = new RoutedUICommand("_Abandonar", "Abandonar", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.B,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando par establecer una nueva partida.
        /// </summary>
        public static readonly RoutedUICommand NuevaPartida = new RoutedUICommand("_Nueva", "NuevaPartida", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.N,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para ofrecer empate.
        /// </summary>
        public static readonly RoutedUICommand OfrecerTablas = new RoutedUICommand("_Tablas", "OfrecerTablas", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.M,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para ofrecer jugar una revancha.
        /// </summary>
        public static readonly RoutedUICommand OfrecerRevancha = new RoutedUICommand("Revanc_ha", "OfrecerRevancha", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.V,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para pedir al motor que realize una jugada inmediatamente.
        /// </summary>
        public static readonly RoutedUICommand JugarInmediatamente = new RoutedUICommand("Ju_gar inmediatamente", "JugarInmediatamente", typeof(MainWindow), new InputGestureCollection { new
            KeyGesture(Key.I, ModifierKeys.Control) });

        /// <summary>
        /// Comando para empezar o dejar de analizar la posición.
        /// </summary>
        public static readonly RoutedCommand Analizar = new RoutedCommand("Analizar", typeof(MainWindow));

        /// <summary>
        /// Comando para pedir al motor que continúe analizando la posición.
        /// </summary>
        public static readonly RoutedUICommand SeguirAnalizando = new RoutedUICommand("_Seguir analizando", "SeguirAnalizando", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.S,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para mostrar u ocultar la información sobre la posición provista por la base de datos.
        /// </summary>
        public static readonly RoutedCommand MostrarResultadosJugada = new RoutedCommand("MostrarResultadosJugada", typeof(MainWindow));

        /// <summary>
        /// Comando para insertar la partida actual en la base de datos.
        /// </summary>
        public static readonly RoutedUICommand InsertarPartida = new RoutedUICommand("Insertar en base de _datos", "InsertarPartida", typeof(MainWindow), new InputGestureCollection { new
            KeyGesture(Key.P, ModifierKeys.Control | ModifierKeys.Shift) });

        /// <summary>
        /// Comando para seleccionar una partida de la base de datos.
        /// </summary>
        public static readonly RoutedUICommand SeleccionarPartida = new RoutedUICommand("Se_leccionar partida...", "SeleccionarPartida", typeof(MainWindow), new InputGestureCollection { new
            KeyGesture(Key.P, ModifierKeys.Control) });

        /// <summary>
        /// Comando para eliminar la partida actual de la base de datos
        /// </summary>
        public static readonly RoutedUICommand EliminarPartida = new RoutedUICommand("B_orrar", "EliminarPartida", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.D,
            ModifierKeys.Control | ModifierKeys.Shift) });

        /// <summary>
        /// Comando para mostrar la posición inicial.
        /// </summary>
        public static readonly RoutedUICommand IrAlInicio = new RoutedUICommand("Ir a_l inicio", "IrAlInicio", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.Home), new
            KeyGesture(Key.Left, ModifierKeys.Control) });

        /// <summary>
        /// Comando para mostrar la posición final.
        /// </summary>
        public static readonly RoutedUICommand IrAlFinal = new RoutedUICommand("Ir al _final", "IrAlFinal", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.End), new
            KeyGesture(Key.Right, ModifierKeys.Control) });

        /// <summary>
        /// Comando para realizar un ejercicio de la base de datos.
        /// </summary>
        public static readonly RoutedUICommand RealizarEjercicio = new RoutedUICommand("Rea_lizar...", "RealizarEjercicio", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.E,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para crear un ejercicio e insertalo en la base de datos.
        /// </summary>
        public static readonly RoutedUICommand CrearEjercicio = new RoutedUICommand("Cre_ar...", "CrearEjercicio", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.E,
            ModifierKeys.Control | ModifierKeys.Shift) });

        /// <summary>
        /// Comando para editar el ejercicio seleccionado.
        /// </summary>
        public static readonly RoutedUICommand EditarEjercicio = new RoutedUICommand("Mo_dificar...", "EditarEjercicio", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.Y,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para eliminar el ejercicio seleccionado.
        /// </summary>
        public static readonly RoutedUICommand EliminarEjercicio = new RoutedUICommand("_Borrar", "EliminarEjercicio", typeof(MainWindow), new InputGestureCollection {new KeyGesture(Key.Y,
            ModifierKeys.Control | ModifierKeys.Shift) });

        /// <summary>
        /// Comando para seleccionar una Explicacion de la base de datos.
        /// </summary>
        public static readonly RoutedUICommand SeleccionarExplicacion = new RoutedUICommand("Cual_quiera...", "SeleccionarExplicacion", typeof(MainWindow), new InputGestureCollection { new
            KeyGesture(Key.X, ModifierKeys.Control) });

        /// <summary>
        /// Comando para crear una Explicación e insertala en la base de datos.
        /// </summary>
        public static readonly RoutedUICommand CrearExplicacion = new RoutedUICommand("Cre_ar...", "CrearExplicacion", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.X,
            ModifierKeys.Control | ModifierKeys.Shift) });

        /// <summary>
        /// Comando para editar la explicación seleccionada.
        /// </summary>
        public static readonly RoutedUICommand EditarExplicacion = new RoutedUICommand("Mo_dificar...", "EditarExplicacion", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.K,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para eliminar la explicación seleccionada.
        /// </summary>
        public static readonly RoutedUICommand EliminarExplicacion = new RoutedUICommand("_Borrar", "EliminarExplicacion", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.K,
            ModifierKeys.Control | ModifierKeys.Shift) });

        /// <summary>
        /// Comando para seleccionar una explicación relacionada con la actual.
        /// </summary>
        public static readonly RoutedUICommand SeleccionarExplicacionRelacionada = new RoutedUICommand("Relaci_onada...", "SeleccionarExplicacionRelacionada", typeof(MainWindow), new
            InputGestureCollection { new KeyGesture(Key.R, ModifierKeys.Control) });

        /// <summary>
        /// Comando para editar el comentario de la jugada actual.
        /// </summary>
        public static readonly RoutedUICommand EditarComentario = new RoutedUICommand("C_omentario...", "EditarComentario", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.C,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para editar el NAG de la jugada actual.
        /// </summary>
        public static readonly RoutedCommand EditarNAG = new RoutedCommand("EditarNAG", typeof(MainWindow));

        /// <summary>
        /// Comando para seleccionar un jugador de la base de datos.
        /// </summary>
        public static readonly RoutedUICommand SeleccionarJugador = new RoutedUICommand("Se_leccionar...", "SeleccionarJugador", typeof(Window), new InputGestureCollection { new KeyGesture(Key.J,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para crear un jugador e insertarlo en la base de datos.
        /// </summary>
        public static readonly RoutedUICommand CrearJugador = new RoutedUICommand("Cre_ar...", "CrearJugador", typeof(Window), new InputGestureCollection { new KeyGesture(Key.J,
            ModifierKeys.Control | ModifierKeys.Shift) });

        /// <summary>
        /// Comando para seleccionar una apertura de la base de datos.
        /// </summary>
        public static readonly RoutedUICommand SeleccionarApertura = new RoutedUICommand("Se_leccionar...", "SeleccionarApertura", typeof(Window), new InputGestureCollection { new KeyGesture(Key.A,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para crear una apertura e insertarla en la base de datos.
        /// </summary>
        public static readonly RoutedUICommand CrearApertura = new RoutedUICommand("Cre_ar...", "CrearApertura", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.A,
            ModifierKeys.Control | ModifierKeys.Shift) });

        /// <summary>
        /// Comando para poner el valor de un campo en null.
        /// </summary>
        public static readonly RoutedCommand EstablecerNulo = new RoutedCommand("EstablecerNulo", typeof(Window), new InputGestureCollection { new KeyGesture(Key.N, ModifierKeys.Control) });

        /// <summary>
        /// Comando para agregar un posición a la apertura.
        /// </summary>
        public static readonly RoutedCommand AgregarPosicion = new RoutedCommand("AgregarPosicion", typeof(EditarApertura), new InputGestureCollection { new KeyGesture(Key.P, ModifierKeys.Control) });

        /// <summary>
        /// Comando para editar el elemento seleccionado.
        /// </summary>
        public static readonly RoutedCommand Editar = new RoutedCommand("Editar", typeof(Window), new InputGestureCollection { new KeyGesture(Key.E, ModifierKeys.Control) });

        /// <summary>
        /// Comando para crear un evento e insertarlo en la base de datos.
        /// </summary>
        public static readonly RoutedUICommand CrearEvento = new RoutedUICommand("Cre_ar...", "CrearEvento", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.T, ModifierKeys.Control
            | ModifierKeys.Shift) });

        /// <summary>
        /// Comando para seleccionar un evento de la base de datos.
        /// </summary>
        public static readonly RoutedUICommand SeleccionarEvento = new RoutedUICommand("Se_leccionar...", "SeleccionarEvento", typeof(Window), new InputGestureCollection { new KeyGesture(Key.T,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para realizar un torneo eliminatorio.
        /// </summary>
        public static readonly RoutedUICommand RealizarTorneoEliminatorio = new RoutedUICommand("Realizar _torneo eliminatorio...", "RealizarTorneoElimnatorio", typeof(MainWindow), new
            InputGestureCollection { new KeyGesture(Key.W, ModifierKeys.Control) });

        /// <summary>
        /// Comando para mover arriba el elemento seleccionado.
        /// </summary>
        public static readonly RoutedCommand MoverArriba = new RoutedCommand("MoverArriba", typeof(ConfigurarTorneoEliminatorio), new InputGestureCollection { new KeyGesture(Key.W,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para mover abajo el elemento seleccionado.
        /// </summary>
        public static readonly RoutedCommand MoverAbajo = new RoutedCommand("MoverAbajo", typeof(ConfigurarTorneoEliminatorio), new InputGestureCollection { new KeyGesture(Key.S,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para realizar un torneo suizo.
        /// </summary>
        public static readonly RoutedUICommand RealizarTorneoSuizo = new RoutedUICommand("Realizar tor_neo suizo...", "RealizarTorneoSuizo", typeof(MainWindow), new InputGestureCollection { new
            KeyGesture(Key.W, ModifierKeys.Control | ModifierKeys.Shift) });

        /// <summary>
        /// Comando para abrir la ventana para configurar las opciones.
        /// </summary>
        public static readonly RoutedUICommand Configurar = new RoutedUICommand("C_onfigurar...", "Configurar", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.Q,
            ModifierKeys.Control) });

        /// <summary>
        /// Comando para restaurar la configuración predeterminada.
        /// </summary>
        public static readonly RoutedUICommand Restaurar = new RoutedUICommand("Res_taurar configuración predeterminada", "Restaurar", typeof(MainWindow), new InputGestureCollection { new
            KeyGesture(Key.Q, ModifierKeys.Control | ModifierKeys.Shift) });

        /// <summary>
        /// Comando para mostrar el FEN de la posición actual.
        /// </summary>
        public static readonly RoutedUICommand CopiarFEN = new RoutedUICommand("Copiar FEN de la posición ac_tual", "CopiarFEN", typeof(MainWindow), new InputGestureCollection { new
            KeyGesture(Key.F, ModifierKeys.Control) });

        /// <summary>
        /// Comando para minimiza la ventana principal.
        /// </summary>
        public static readonly RoutedCommand Minimizar = new RoutedCommand("Minimizar", typeof(MainWindow));

        /// <summary>
        /// Inserta en la base de datos todas las partidas de un achivo .pgn
        /// </summary>
        public static readonly RoutedUICommand InsertarTodasLasPartidasDeUnPGN = new RoutedUICommand("Insertar todas las partidas de un PGN...", "InsertarTodasLasPartidasDeUnPGN", typeof(MainWindow), new
            InputGestureCollection { new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift) });
    }
}
