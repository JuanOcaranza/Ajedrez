using System;

namespace Ajedrez
{
    /// <summary>
    /// Representa un reloj de arena
    /// </summary>
    public class RelojArena : Reloj
    {
        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="RelojArena"/> con el tiempo total y el turno
        /// </summary>
        /// <param name="tiempo">Tiempo total.</param>
        /// <param name="esTurnoBlanco">Si es el turno de las blancas.</param>
        /// <param name="tiempoAcabado">Controlador de eventos al que se llamará cuando <see cref="TiempoAcabado"/> se produzca.</param>
        /// <param name="iniciarInmediatamente">Si el reloj debe iniciarse inmediatamente luego de crearse la instancia.</param>
        public RelojArena(TimeSpan tiempo, bool esTurnoBlanco, EventHandler tiempoAcabado, bool iniciarInmediatamente = false) : base(TimeSpan.FromMilliseconds(tiempo.TotalMilliseconds / 2), esTurnoBlanco,
            tiempoAcabado, iniciarInmediatamente) { }
        #endregion

        #region Constrolador de eventos
        protected override void Temporizador_Tick(object sender, EventArgs e)
        {
            if (esTurnoBlanco)
            {
                TiempoBlanco = TiempoBlanco.Subtract(intervalo);
                TiempoNegro = TiempoNegro.Add(intervalo);
            }
            else
            {
                TiempoNegro = TiempoNegro.Subtract(intervalo);
                TiempoBlanco = TiempoBlanco.Add(intervalo);
            }
        }
        #endregion
    }
}
