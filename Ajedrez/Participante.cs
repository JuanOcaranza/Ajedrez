using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ajedrez
{
    /// <summary>
    /// Representa un jugador que participa en un torneo suizo.
    /// </summary>
    public class Participante : Jugador
    {
        #region Atributos
        /// <summary>
        /// Puntos realizados por el jugador.
        /// </summary>
        private double puntos;

        /// <summary>
        /// Puntuación en el sistema de desempate acumulativo.
        /// </summary>
        private double acumulativo;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece los puntos realizados por el jugador.
        /// </summary>
        public double Puntos
        {
            get => puntos;
            set
            {
                puntos = value;
                PropiedadModificada();
                foreach ((Participante rival, double puntosObtenidos) rival in Rivales)
                {
                    rival.rival.PropiedadModificada(nameof(Neustadtl));
                    rival.rival.PropiedadModificada(nameof(Buchholz));
                }
            }
        }

        /// <summary>
        /// Obtiene o establce los colores con los que jugó cada ronda.
        /// </summary>
        public List<Equipo?> Colores { get; set; } = new List<Equipo?>();

        /// <summary>
        /// Obtiene la diferencia entre la cantidad de veces que jugó con blancas y la cantidad de veces que jugón con negras.
        /// </summary>
        public int DiferenciaColor => Colores.Count(e => e == Equipo.Blanco) - Colores.Count(e => e == Equipo.Negro);

        /// <summary>
        /// Obtiene el color con el que debería jugar la siguiente partida.
        /// </summary>
        public Equipo ColorDeseable => DiferenciaColor > 0 || (DiferenciaColor == 0 && Colores.Count > 1 && Colores[Colores.Count - 1] == Equipo.Blanco) ? Equipo.Negro : Equipo.Blanco;

        /// <summary>
        /// Obtiene la puntuación en el sistema de desempate Neustadtl (Sonneborn-Berger).
        /// </summary>
        public double Neustadtl => Rivales.Sum(p => p.rival.Puntos * p.puntosObtenidos);

        /// <summary>
        /// Obtiene la puntuación en el sistema de desempate Buchholz.
        /// </summary>
        public double Buchholz => Rivales.Sum(p => p.rival.Puntos);

        /// <summary>
        /// Obtiene o establece la puntuación en el sistema de desempate acumulativo.
        /// </summary>
        public double Acumulativo
        {
            get => acumulativo;
            set
            {
                acumulativo = value;
                PropiedadModificada();
            }
        }

        /// <summary>
        /// Obtiene o establece un valor que indica si el jugador ha recibido un bye por haber cantidad impar de jugadores.
        /// </summary>
        public bool Bye { get; set; }

        /// <summary>
        /// Obtiene o establece los jugadores con los que ha jugado y la puntuación obtenida contra cada uno.
        /// </summary>
        public ObservableCollection<(Participante rival, double puntosObtenidos)> Rivales { get; set; } = new ObservableCollection<(Participante rival, double puntosObtenidos)>();
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Participante"/> con el jugador.
        /// </summary>
        /// <param name="jugador">Jugador que va a participar en el torneo.</param>
        public Participante(Jugador jugador) : base(jugador.Nombre, jugador.Na, jugador.Tipo, jugador.Elo, jugador.Titulo) { }
        #endregion

        #region Método
        /// <summary>
        /// Determina si el jugador puede jugar con un color.
        /// </summary>
        /// <param name="equipo">Equipo con el que se quiere saber si puede jugar.</param>
        public bool Acepta(Equipo equipo) => !(DiferenciaMayorADos(equipo) || TresPartidasSeguidasMismoColor(equipo));

        /// <summary>
        /// Determina si se jugarían tres partidas seguidos con el mismo color.
        /// </summary>
        /// <param name="color">Color con el que se quiere jugar.</param>
        private bool TresPartidasSeguidasMismoColor(Equipo color) => Colores.Count > 1 && Colores[Colores.Count - 1] == color && Colores[Colores.Count - 2] == color;

        /// <summary>
        /// Determina si habría tres de diferencia entre la cantidad de partidas jugads con blancas y la cantidad jugada con negras .
        /// </summary>
        /// <param name="color">Colro con el que se quiere jugar.</param>
        private bool DiferenciaMayorADos(Equipo color) => color == Equipo.Blanco ? DiferenciaColor == 2 : DiferenciaColor == -2;

        /// <summary>
        /// Actualiza los valores del participante teniendo en cuenta la partida jugada.
        /// </summary>
        /// <param name="partida">Partida que jugó el participante.</param>
        /// <param name="rival">Jugador contra el que jugó la partida.</param>
        public void Actualizar(Partida partida, Participante rival)
        {
            if (partida == null)
            {
                Puntos++;
                Colores.Add(null);
                Bye = true;
                Acumulativo--;
            }
            else
            {
                double puntosGanados = partida.Gano(Nombre) ? 1 : (partida.Perdio(Nombre) ? 0 : .5);
                Rivales.Add((rival, puntosGanados));
                Puntos += puntosGanados;
                Colores.Add(partida.Etiquetas[EtiquetaPGN.White] == Nombre ? Equipo.Blanco : Equipo.Negro);
            }
            Acumulativo += Puntos;
            foreach (Participante oponente in Rivales.Select(p => p.rival))
            {
                oponente.PropiedadModificada(nameof(Neustadtl));
                oponente.PropiedadModificada(nameof(Buchholz));
            }
        }
        #endregion
    }
}
