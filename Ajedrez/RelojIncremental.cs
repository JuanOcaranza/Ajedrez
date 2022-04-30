using System;

namespace Ajedrez
{
    /// <summary>
    /// Representa un reloj que agrega un tiempo luego de cada jugada
    /// </summary>
    public class RelojIncremental : Reloj
    {
        #region Propiedad
        /// <summary>
        /// Obtiene el tiempo que se agrega después de cada jugada
        /// </summary>
        public TimeSpan Incremento { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la calse <see cref="RelojIncremental"/> con el tiempo inicial, el turno y el incrmento.
        /// </summary>
        /// <param name="tiempo">Tiempo que tiene inicialmente cada jugador.</param>
        /// <param name="esTurnoBlanco">Si es el turno del blanco.</param>
        /// <param name="incremento">Tiempo que se agrega después de cada jugada.</param>
        /// <param name="tiempoAcabado">Controlador de eventos al que se llamará cuando <see cref="TiempoAcabado"/> se produzca.</param>
        /// <param name="iniciarInmediatamente">Si el reloj debe iniciarse inmediatamente luego de crearse la instancia.</param>
        public RelojIncremental(TimeSpan tiempo, bool esTurnoBlanco, TimeSpan incremento, EventHandler tiempoAcabado, bool iniciarInmediatamente = false) : base(tiempo, esTurnoBlanco, tiempoAcabado,
            iniciarInmediatamente) => Incremento = incremento;
        #endregion

        #region Métodos
        public override void CambiarTurno()
        {
            if (esTurnoBlanco)
            {
                TiempoBlanco = TiempoBlanco.Add(Incremento);
            }
            else
            {
                TiempoNegro = TiempoNegro.Add(Incremento);
            }
            base.CambiarTurno();
        }
        #endregion
    }
}
