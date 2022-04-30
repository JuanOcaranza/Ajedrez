using Ajedrez.Controles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Ajedrez
{
    /// <summary>
    /// Lógica de interacción para TorneoSuizo.xaml
    /// </summary>
    public partial class TorneoSuizo : Torneo, INotifyPropertyChanged
    {
        #region Atributos
        /// <summary>
        /// Cantidad de rondas,
        /// </summary>
        private int rondas;

        /// <summary>
        /// Pares posibles.
        /// </summary>
        private List<List<(int, int)>> pares;

        /// <summary>
        /// Participantes del torneo.
        /// </summary>
        private ObservableCollection<Participante> jugadores = new ObservableCollection<Participante>();
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Propiedades
        public override List<Partida> Partidas => tabControl?.Items.OfType<TabItem>().Select(t => t.Content).OfType<ScrollViewer>().Select(s => s.Content).OfType<Ronda>().SelectMany(r =>
            r.Partidas).ToList();

        /// <summary>
        /// Obtiene o establece los jugadores que participan del torneo.
        /// </summary>
        public ObservableCollection<Participante> Jugadores
        {
            get => jugadores;
            set
            {
                jugadores = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Jugadores)));
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="TorneoSuizo"/> con la ventana que la posee.
        /// </summary>
        /// <param name="owner">Ventana a la que va a pertenecer esta ventana.</param>
        public TorneoSuizo(MainWindow owner) : base(owner)
        {
            InitializeComponent();
            tabControl.DataContext = this;
        }
        #endregion

        #region Métodos
        public override void PreperarSiguientePartida(Partida partida)
        {
            Participante[] participantes = Jugadores.Where(j => partida.Jugo(j.Nombre)).ToArray();
            participantes[0].Actualizar(partida, participantes[1]);
            participantes[1].Actualizar(partida, participantes[0]);
            base.PreperarSiguientePartida(partida);
        }

        protected override void PrepararSiguienteRonda()
        {
            Jugadores = new ObservableCollection<Participante>(Jugadores.OrderByDescending(p => p.Puntos).ThenByDescending(p => p.Neustadtl).ThenByDescending(p => p.Buchholz).ThenByDescending(p =>
                p.Acumulativo).ThenBy(p => p.DiferenciaColor));
            if (tabControl.Items.Count <= rondas)
            {
                List<Partida> partidas = Emparejar(out Participante participanteLibre);
                tabControl.Items.Add(new TabItem()
                {
                    Content = new ScrollViewer()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        Content = new Ronda(partidas)
                    },
                    Header = $"Ronda {tabControl.Items.Count}"
                });
                if (participanteLibre != null)
                {
                    MessageBox.Show($"{participanteLibre.Titulo} {participanteLibre.Nombre} queda libre en esta ronda, recibe un punto", "Jugador libre", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    participanteLibre.Actualizar(null, null);
                }
            }
            else
            {
                MessageBox.Show($"Ha ganado {Jugadores[0].Titulo} {Jugadores[0].Nombre}", "Torneo terminado", MessageBoxButton.OK, MessageBoxImage.Information);
                foreach (Jugador jugador in Jugadores)
                {
                    TerminarParticipacion(jugador);
                }
                InsertarEnBaseDeDatos();
            }
        }

        public override void TerminarConfiguracion()
        {
            rondas = (int)Math.Ceiling(Math.Log(Jugadores.Count, 2));
            List<Participante> participantesConElo = Jugadores.Where(j => j.Elo != null).ToList();
            eloPromedio = participantesConElo.Count > 0 ? (int)Math.Round(participantesConElo.Average(j => (int)j.Elo)) : 0;
            pares = ArmarPares(NumerosMenores(Jugadores.Count + Jugadores.Count % 2)).ToList();
            base.TerminarConfiguracion();
        }

        /// <summary>
        /// Guarda en el evento la participacion de un jugador.
        /// </summary>
        /// <param name="jugador">Jugador que no jugará más partida.</param>
        private void TerminarParticipacion(Jugador jugador)
        {
            Evento.Participaciones.Add(new Participa(jugador, Evento, null, eloPromedio + diferencias[(int)(Partidas.Where(p => p.Jugo(jugador.Nombre)).Average(p => p.Puntos(jugador.Nombre)) * 100)],
                null, Jugadores.Count));
        }

        /// <summary>
        /// Empareja los participantes.
        /// </summary>
        /// <param name="restante">Participante que queda sin rival.</param>
        private List<Partida> Emparejar(out Participante restante)
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (List<(int, int)> lista in pares)
                {
                    List<Partida> partidas = new List<Partida>();
                    restante = null;
                    bool esValido = true;
                    for (int j = 0; j < lista.Count && esValido; j++)
                    {
                        if (lista[j].Item1 == Jugadores.Count || lista[j].Item2 == Jugadores.Count) // si en este emparejamiento está el jugador que queda sin rival.
                        {
                            int indiceParticipanteLibre = Math.Min(lista[j].Item1, lista[j].Item2);
                            if (Jugadores[indiceParticipanteLibre].Bye)
                            {
                                esValido = false;
                            }
                            else
                            {
                                restante = Jugadores[indiceParticipanteLibre];
                            }
                        }
                        else
                        {
                            Partida partida = CrearPartida(Jugadores[lista[j].Item1], Jugadores[lista[j].Item2], i == 1);
                            if (partida == null)
                            {
                                esValido = false;
                            }
                            else
                            {
                                partidas.Add(partida);
                            }
                        }
                    }
                    if (esValido)
                    {
                        return partidas;
                    }
                }
            }
            restante = null; 
            return new List<Partida>();
        }

        /// <summary>
        /// Genera todos los posibles pares de una lista de números.
        /// </summary>
        /// <param name="numeros">Números que se deben emparejar (debe haber un cantidad par).</param>
        private IEnumerable<List<(int, int)>> ArmarPares(List<int> numeros)
        {
            if (numeros.Count == 2)
            {
                yield return new List<(int, int)>() { (numeros[0], numeros[1]) };
            }
            for (int i = 1; i < numeros.Count; i++)
            {
                foreach (List<(int, int)> lista in ArmarPares(numeros.Except(new int[] { numeros[0], numeros[i] }).ToList()))
                {
                    yield return new List<(int, int)> { (numeros[0], numeros[i]) }.Concat(lista).ToList();
                }
            }
        }

        protected override bool NuevaRonda() => indicePartida % (Jugadores.Count / 2) == 0;

        /// <summary>
        /// Crea una lista con todos los enteros desde cero hasta n - 1.
        /// </summary>
        /// <param name="n">Primer entero que no debe estar en la lista.</param>
        private List<int> NumerosMenores(int n)
        {
            List<int> lista = new List<int>();
            for (int i = 0; i < n; i++)
            {
                lista.Add(i);
            }
            return lista;
        }

        /// <summary>
        /// Crea una partida entre dos jugadores, calculando el color que le corresponde a cada uno.
        /// </summary>
        /// <param name="participante1">Uno de los participantes que juegará la partida.</param>
        /// <param name="participante2">El otro participante que jugará la partida.</param>
        /// <param name="noImportaColor">Si se puede dar por válido el emparejamiento cuando un jugador no acepta el color que le tocaría.</param>
        /// <returns>La partida que va ser jugada, null si no es posible una partida entre los participantes.</returns>
        private Partida CrearPartida(Participante participante1, Participante participante2, bool noImportaColor)
        {
            if (!participante1.Rivales.ToList().Exists(p => p.rival.Nombre == participante2.Nombre) && (noImportaColor || (participante1.Acepta(Equipo.Blanco) && participante2.Acepta(Equipo.Negro)) ||
                (participante1.Acepta(Equipo.Negro) && participante2.Acepta(Equipo.Blanco))))
            {
                if (participante1.ColorDeseable != participante2.ColorDeseable)
                {
                    // Asigna a cada jugador el color con el que debería jugar la siguiente partida.
                    return NuevaPartida(participante1.ColorDeseable == Equipo.Blanco);
                }
                if (participante1.DiferenciaColor != participante2.DiferenciaColor)
                {
                    // Asigna el color intentando igualar las diferencias de color.
                    return NuevaPartida(participante1.DiferenciaColor < participante2.DiferenciaColor);
                }
                for (int i = participante1.Colores.Count - 1; i >= 0; i--)
                {
                    if (participante1.Colores[i] != participante2.Colores[i])
                    {
                        // Asigna los colores contrarios a la última vez que tuvieron colores distintos.
                        return NuevaPartida(participante1.Colores[i] == Equipo.Negro);
                    }
                }
                if (participante1.Neustadtl != participante2.Neustadtl)
                {
                    // Asigna al jugador con mayor puntuación Neustadtl el color con el que debería jugar.
                    return NuevaPartida(participante1.Neustadtl > participante2.Neustadtl ? participante1.ColorDeseable == Equipo.Blanco : participante2.ColorDeseable == Equipo.Negro);
                }
                if (participante1.Buchholz != participante2.Buchholz)
                {
                    // Asigna al jugador con mayor puntuación Buchholz el color con el que debería jugar.
                    return NuevaPartida(participante1.Buchholz > participante2.Buchholz ? participante1.ColorDeseable == Equipo.Blanco : participante2.ColorDeseable == Equipo.Negro);
                }
                if (participante1.Acumulativo != participante2.Acumulativo)
                {
                    // Asigna al jugador con mayor puntuación acumulativa el color con el que debería jugar.
                    return NuevaPartida(participante1.Acumulativo > participante2.Acumulativo ? participante1.ColorDeseable == Equipo.Blanco : participante2.ColorDeseable == Equipo.Negro);
                }
                // Asigna los colores aleatoriamente.
                return NuevaPartida(new Random().NextDouble() > .5);
            }
            return null;

            Partida NuevaPartida(bool participante1JuegaConBlancas) => new Partida(participante1JuegaConBlancas ? participante1 : participante2, participante1JuegaConBlancas ? participante2 :
                participante1, Evento, Tiempo, tabControl.Items.Count);
        }
        #endregion
    }
}
