using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para ElegirEtiquetas.xaml
    /// </summary>
    public partial class ElegirEtiquetas : Window
    {
        #region Propiedades
        /// <summary>
        /// Obtiene o estalece los elementos que se pueden mostrar en <see cref="dataGrid"/>
        /// </summary>
        public ElementoDataGridElegirEtiquetas[] Elementos { get; set; } = {
            new ElementoDataGridElegirEtiquetas("WhiteTitle", "Título FIDE del jugador de piezas blancas"),
            new ElementoDataGridElegirEtiquetas("BlackTitle", "Título FIDE del jugador de piezas negras"),
            new ElementoDataGridElegirEtiquetas("WhiteElo", "Elo FIDE del jugador de piezas blancas"),
            new ElementoDataGridElegirEtiquetas("BlackElo", "Elo FIDE del jugador de piezas negras"),
            new ElementoDataGridElegirEtiquetas("WhiteNA", "E-mail o dirección de red del jugador de piezas blancas"),
            new ElementoDataGridElegirEtiquetas("BlackNA", "E-mail o dirección de red del jugador de piezas negras"),
            new ElementoDataGridElegirEtiquetas("WhiteType", "Tipo(humano o programa) del jugador de piezas blancas"),
            new ElementoDataGridElegirEtiquetas("BlackType", "Tipo(humano o programa) del jugador de piezas negras"),
            new ElementoDataGridElegirEtiquetas("EventDate", "Fecha de inicio del evento"),
            new ElementoDataGridElegirEtiquetas("Section", "Sección de juego del torneo (Ej: Abierto o Reservado)"),
            new ElementoDataGridElegirEtiquetas("Stage", "Etapa del torneo"),
            new ElementoDataGridElegirEtiquetas("Board", "Número de tablero en el que se jugó la partida en un torneo por equipo o en una exhibición simultánea"),
            new ElementoDataGridElegirEtiquetas("Opening", "Nombre de la apertura"),
            new ElementoDataGridElegirEtiquetas("Variation", "Nombre de la variante de la apertura"),
            new ElementoDataGridElegirEtiquetas("SubVariation", "Nombre de la subvariante de la apertura"),
            new ElementoDataGridElegirEtiquetas("ECO", "Designación de la apertura en _Encyclopedia of Chess Openings_"),
            new ElementoDataGridElegirEtiquetas("NIC", "Designación de la apertura en la base de datos _New in Chess_"),
            new ElementoDataGridElegirEtiquetas("Time", "Hora local del comienzo de la partida"),
            new ElementoDataGridElegirEtiquetas("UTCTime", "Hora UTC del comienzo de la partida"),
            new ElementoDataGridElegirEtiquetas("UTCDate", "Fecha UTC del inicio del evento"),
            new ElementoDataGridElegirEtiquetas("TimeControl", "Control de tiempo usado en la partida"),
            new ElementoDataGridElegirEtiquetas("Termination", "Razón por la que terminó la partida"),
            new ElementoDataGridElegirEtiquetas("Annotator", "Nombre de la persona que anotó la partida"),
            new ElementoDataGridElegirEtiquetas("Mode", "Modo en el que se jugó la partida (Ej: Sobre el tablero, Correo en papel, Correo electrónico)"),
            new ElementoDataGridElegirEtiquetas("PlyCount", "Cantidad de medias jugadas"),
        };
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ElegirEtiquetas"/> con la ventana que la posee.
        /// </summary>
        /// <param name="owner">Ventana que va a poseer a la nueva ventana.</param>
        public ElegirEtiquetas(Window owner)
        {
            InitializeComponent();
            dataGrid.DataContext = this;
            Owner = owner;
        }
        #endregion

        #region Controladores de eventos
        private void Aceptar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Aceptar_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            List<EtiquetaPGN> etiquetasAIncluir = new List<EtiquetaPGN>();
            foreach (ElementoDataGridElegirEtiquetas item in Elementos)
            {
                if (item.Incluir)
                {
                    etiquetasAIncluir.Add((EtiquetaPGN)Enum.Parse(typeof(EtiquetaPGN), item.Etiqueta));
                }
            }
            if (Owner is MainWindow mainWindow)
            {
                mainWindow.EtiquetasOpcionales = etiquetasAIncluir;
                DialogResult = true;
                Close();
            }
        }

        private void Cancelar_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        #endregion
    }
}
