using System;

namespace Ajedrez
{
    /// <summary>
    /// Representa un reloj utilizado durante una cantidad de jugadas.
    /// </summary>
    public class RelojParcial : Reloj
    {
        #region Atributos
        /// <summary>
        /// Obtiene la cantidad de jugadas restantes para el cambio de reloj.
        /// </summary>
        public int CantidadJugadas { get; private set; }
        #endregion

        #region Evento
        /// <summary>
        /// Tiene lugar cuando se terminaron las jugadas en las que debe utilizarce el reloj.
        /// </summary>
        public event EventHandler JugadasAcabadas;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RelojParcial"/> con el tiempo, el turno y la cantidad de jugadas.
        /// </summary>
        /// <param name="tiempo">Tiempo que tiene inicialmente cada jugador.</param>
        /// <param name="esTurnoBlanco">Si es el turno del blanco.</param>
        /// <param name="cantidadJugadas">Jugadas completas en las que se debe usar este reloj.</param>
        /// <param name="jugadasAcabadas">Controlador de eventos al que se llamará cuando <see cref="JugadasAcabadas"/> se produzca.</param>
        /// <param name="tiempoAcabado">Controlador de eventos al que se llamará cuando <see cref="TiempoAcabado"/> se produzca.</param>
        /// <param name="iniciarInmediatamente">Si el reloj debe iniciarse inmediatamente luego de crearse la instancia.</param>
        public RelojParcial(TimeSpan tiempo, bool esTurnoBlanco, int cantidadJugadas, EventHandler jugadasAcabadas, EventHandler tiempoAcabado, bool iniciarInmediatamente = false) : base(tiempo,
            esTurnoBlanco, tiempoAcabado, iniciarInmediatamente)
        {
            CantidadJugadas = cantidadJugadas;
            JugadasAcabadas += jugadasAcabadas;
        }
        #endregion

        #region Método
        public override void CambiarTurno()
        {
            if (!esTurnoBlanco)
            {
                CantidadJugadas--;
                if (CantidadJugadas < 1)
                {
                    temporizador.Stop();
                    JugadasAcabadas?.Invoke(this, new EventArgs());
                }
            }
            base.CambiarTurno();
        }
        #endregion
    }
}
