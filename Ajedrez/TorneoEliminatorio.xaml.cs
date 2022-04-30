using Ajedrez.Controles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para Torneo.xaml
    /// </summary>
    public partial class TorneoEliminatorio : Torneo
    {
        #region Propiedades
        /// <summary>
        /// Obtiene o establece los jugadores del torneo.
        /// </summary>
        public ObservableCollection<Jugador> Jugadores { get; set; } = new ObservableCollection<Jugador>();

        public override List<Partida> Partidas => tabControl?.Items.OfType<TabItem>().Select(t => t.Content).OfType<ScrollViewer>().Select(s => s.Content).OfType<Ronda>().SelectMany(r =>
            r.Partidas).ToList();
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="TorneoEliminatorio"/> con la ventana que la posee.
        /// </summary>
        /// <param name="owner">Ventana a la que va a pertenecer esta ventana</param>
        public TorneoEliminatorio(MainWindow owner) : base(owner) => InitializeComponent();
        #endregion

        #region Métodos
        public override void PreperarSiguientePartida(Partida partida)
        {
            if (partida.Etiquetas[EtiquetaPGN.Result] == "1/2-1/2")
            {
                MessageBox.Show("Debe obtenerse un ganador, se jugará la revancha", "Empate", MessageBoxButton.OK, MessageBoxImage.Information);
                Jugar(partida.Revancha);
            }
            else
            {
                TerminarParticipacion(Jugadores.First(j => partida.Perdio(j.Nombre)));
                base.PreperarSiguientePartida(partida);
            }
        }

        protected override void PrepararSiguienteRonda()
        {
            if (Jugadores.Count > 1)
            {
                List<Partida> partidas = new List<Partida>();
                for (int i = 0; i < Jugadores.Count / 2; i++)
                {
                    partidas.Add(new Partida(Jugadores[i], Jugadores[Jugadores.Count / 2 + i], Evento, Tiempo, tabControl.Items.Count + 1));
                }
                tabControl.Items.Add(new TabItem()
                {
                    Content = new ScrollViewer()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        Content = new Ronda(partidas)
                    },
                    Header = $"Ronda {tabControl.Items.Count + 1}"
                });
            }
            else
            {
                MessageBox.Show($"Ha ganado {Jugadores[0].Titulo} {Jugadores[0].Nombre}", "Torneo terminado", MessageBoxButton.OK, MessageBoxImage.Information);
                TerminarParticipacion(Jugadores[0]);
                InsertarEnBaseDeDatos();
            }
        }

        /// <summary>
        /// Guarda en el evento la participacion de un jugador.
        /// </summary>
        /// <param name="jugador">Jugador que no jugará más partida.</param>
        private void TerminarParticipacion(Jugador jugador)
        {
            Evento.Participaciones.Add(new Participa(jugador, Evento, null, eloPromedio + diferencias[(int)(Partidas.Where(p => p.Jugo(jugador.Nombre)).Average(p => p.Puntos(jugador.Nombre)) * 100)],
                null, Jugadores.Count));
            Jugadores.Remove(jugador);
        }

        protected override bool NuevaRonda() => Math.Log(Jugadores.Count, 2) % 1 == 0; // Si la cantidad de jugadores es una potencia de dos

        public override void TerminarConfiguracion()
        {
            List<Jugador> participantesConElo = Jugadores.Where(j => j.Elo != null).ToList();
            eloPromedio = participantesConElo.Count > 0 ? (int)Math.Round(participantesConElo.Average(j => (int)j.Elo)) : 0;
            base.TerminarConfiguracion();
        }
        #endregion
    }
}
