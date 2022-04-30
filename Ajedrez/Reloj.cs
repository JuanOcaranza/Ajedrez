using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace Ajedrez
{
    /// <summary>
    /// Representa un reloj de ajedrez
    /// </summary>
    public class Reloj : INotifyPropertyChanged
    {
        #region Atriubutos
        /// <summary>
        /// Intervalo de tiempo en el que se actualizan los relojes.
        /// </summary>
        protected static TimeSpan intervalo = TimeSpan.FromMilliseconds(10);

        /// <summary>
        /// Temporizador para actualizar los tiempos de cada jugador.
        /// </summary>
        protected DispatcherTimer temporizador = new DispatcherTimer(DispatcherPriority.Send) { Interval = intervalo, IsEnabled = false };

        /// <summary>
        /// Tiempo restante del blanco.
        /// </summary>
        private TimeSpan tiempoBlanco;

        /// <summary>
        /// Tiempo restante de negro.
        /// </summary>
        private TimeSpan tiempoNegro;

        /// <summary>
        /// Si es el turno del blanco o no.
        /// </summary>
        protected bool esTurnoBlanco;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece el yiempo restante del blanco.
        /// </summary>
        public TimeSpan TiempoBlanco
        {
            get => tiempoBlanco;
            protected set
            {
                tiempoBlanco = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextoTiempoBlanco)));
                if (tiempoBlanco.TotalMilliseconds < 1)
                {
                    TiempoAcabado?.Invoke(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el tiempo restante de negro.
        /// </summary>
        public TimeSpan TiempoNegro
        {
            get => tiempoNegro;
            protected set
            {
                tiempoNegro = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextoTiempoNegro)));
                if (tiempoNegro.TotalMilliseconds < 1)
                {
                    TiempoAcabado?.Invoke(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Obtiene el texto que representa el tiempo restante del blanco.
        /// </summary>
        public string TextoTiempoBlanco => TiempoBlanco.ToString(@"hh\:mm\:ss");

        /// <summary>
        /// Obtiene el texto que representa el tiempo restante del negro.
        /// </summary>
        public string TextoTiempoNegro => TiempoNegro.ToString(@"hh\:mm\:ss");
        #endregion

        #region Eventos
        /// <summary>
        /// Tiene lugar cuando se ha terminado el tiempo de uno de los jugadores.
        /// </summary>
        public event EventHandler TiempoAcabado;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="Reloj"/> con el tiempo inicial y el turno
        /// </summary>
        /// <param name="tiempo">Tiempo que tiene inicialmente cada jugador.</param>
        /// <param name="esTurnoBlanco">Si es el turno de las blanca.s</param>
        /// <param name="tiempoAcabado">Controlador de eventos al que se llamará cuando <see cref="TiempoAcabado"/> se produzca.</param>
        /// <param name="iniciarInmediatamente">Si el reloj debe iniciarse inmediatamente luego de crearse la instancia.</param>
        public Reloj(TimeSpan tiempo, bool esTurnoBlanco, EventHandler tiempoAcabado, bool iniciarInmediatamente = false)
        {
            TiempoBlanco = tiempo;
            TiempoNegro = tiempo;
            this.esTurnoBlanco = esTurnoBlanco;
            temporizador.Tick += Temporizador_Tick;
            TiempoAcabado += tiempoAcabado;
            if (iniciarInmediatamente)
            {
                temporizador.Start();
            }
        }
        #endregion

        #region Controlador de eventos
        protected virtual void Temporizador_Tick(object sender, EventArgs e)
        {
            if (esTurnoBlanco)
            {
                TiempoBlanco = TiempoBlanco.Subtract(intervalo);
            }
            else
            {
                TiempoNegro = TiempoNegro.Subtract(intervalo);
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Cambia a quien le toca jugar.
        /// </summary>
        public virtual void CambiarTurno() => esTurnoBlanco = !esTurnoBlanco;

        /// <summary>
        /// Inicia el reloj.
        /// </summary>
        public void Iniciar() => temporizador.Start();

        /// <summary>
        /// Detiene el reloj.
        /// </summary>
        public void Detener() => temporizador.Stop();
        #endregion
    }
}
