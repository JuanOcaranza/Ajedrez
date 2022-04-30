using Ajedrez.Controles;
using System;
using System.ComponentModel;

namespace Ajedrez
{
    /// <summary>
    /// Representa la información dada por un motor al analizar una posición.
    /// </summary>
    public class InformacionAnalisis : INotifyPropertyChanged
    {
        #region Atributos
        /// <summary>
        /// Profundidad con la que el motor analizó la posición.
        /// </summary>
        private string profundidad;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene la valoración de la posición desde la perspectiva del blanco.
        /// </summary>
        public string Valoracion { get; private set; }

        /// <summary>
        /// Obtiene la jugada que lleva a una posición con la valoración indicada.
        /// </summary>
        public string Jugada { get; private set; }

        /// <summary>
        /// Obtiene la mejor respuesta a <see cref="Jugada"/>
        /// </summary>
        public string Respuesta { get; private set; }

        /// <summary>
        /// Obtiene o establece la profundidad con la que el motor analizó la posición.
        /// </summary>
        public string Profundidad
        {
            get => profundidad;
            set
            {
                profundidad = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Profundidad)));
            }
        }
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructores
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InformacionAnalisis"/>.
        /// </summary>
        public InformacionAnalisis() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InformacionAnalisis"/> con la valoración, la profundidad, la jugada y la respuesta.
        /// </summary>
        /// <param name="medidaValoracion">"cp" si la valoracion se mide en centipeones, "mate" si se mide en jugadas para el jaque mate.</param>
        /// <param name="valor">Valoración de la posición desde la perspectiva del motor.</param>
        /// <param name="profundidad">Profundidad con la que se anlizó la posición.</param>
        /// <param name="jugada">Jugada que se está valorando.</param>
        /// <param name="respuesta">Mejor respuesta a la jugada indicada.</param>
        /// <param name="posicion">Posición que se analizó el motor.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public InformacionAnalisis(string medidaValoracion, string valor, string profundidad, string jugada, string respuesta, Posicion posicion)
        {
            EstablecerValoracion(medidaValoracion, valor, posicion.Turno == Equipo.Negro);
            Profundidad = profundidad;
            EstablecerJugadaYRespuesta(jugada, respuesta, posicion);
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Establece <see cref="Valoracion"/> desde la perspectiva del blanco.
        /// </summary>
        /// <param name="medidaValoracion">"cp" si la valoracion se mide en centipeones, "mate" si se mide en jugadas para el jaque mate.</param>
        /// <param name="valor">Valoración de la posición desde la perspectiva del motor</param>
        /// <param name="turnoNegro">Si le toca jugar a las negras</param>
        public void EstablecerValoracion(string medidaValoracion, string valor, bool turnoNegro)
        {
            string prefijo = string.Empty;
            decimal valorDecimal = decimal.Parse(valor);
            if (medidaValoracion == "cp")
            {
                valorDecimal /= 100;
            }
            else
            {
                prefijo = "#";
            }
            if (turnoNegro)
            {
                valorDecimal *= -1; // Convierte al valor correspondiente desde la perspectiva de las blancas.
            }
            Valoracion = prefijo + valorDecimal.ToString();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Valoracion)));
        }

        /// <summary>
        /// Establece <see cref="Jugada"/> y <see cref="Respuesta"/>.
        /// </summary>
        /// <param name="jugada">Cadena que representa la jugada con casilla de origen, destino y opcionalmente caracter de coronacion.</param>
        /// <param name="respuesta">Cadena que representa la mejor respuesta a jugada con casilla de origen, destino y opcionalmente caracter de coronacion.</param>
        /// <param name="posicion">Posicion en la que se realiza la jugada.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public void EstablecerJugadaYRespuesta(string jugada, string respuesta, Posicion posicion)
        {
            Jugada intentoDeJugadaValida = new Jugada(jugada.Substring(0, 2), jugada.Substring(2, 2));
            // Obtiene la jugada con igual destino, origen y tipo de coronación en caso de que haya.
            Jugada jugadaRecomendada = posicion.JugadasPosibles.Find(j => j.Equals(intentoDeJugadaValida) && (jugada.Length < 5 || Tablero.CharToTipoPromocion(jugada[4]) == j.Tipo));
            if (jugadaRecomendada != null)
            {
                Jugada = jugadaRecomendada.ToString(posicion);
                if (respuesta == "")
                {
                    Respuesta = respuesta;
                }
                else
                {
                    Posicion siguientePosicion = new Posicion(posicion, jugadaRecomendada);
                    Respuesta = siguientePosicion.JugadasPosibles.Find(j => j.Equals(new Jugada(respuesta.Substring(0, 2), respuesta.Substring(2, 2))) && (respuesta.Length < 5 ||
                    Tablero.CharToTipoPromocion(respuesta[4]) == j.Tipo))?.ToString(siguientePosicion) ?? "";
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Jugada)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Respuesta)));
            }
        }
        #endregion
    }
}
