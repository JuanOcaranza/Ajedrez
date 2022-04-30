using Ajedrez.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Ajedrez
{
    /// <summary>
    /// Proporciona acceso a un motor de ajedrez.
    /// </summary>
    public class Motor
    {
        #region Atributos
        /// <summary>
        /// Acceso al proceso del motor.
        /// </summary>
        private readonly Process proceso;

        /// <summary>
        /// Opciones que se establecerán en el motor.
        /// </summary>
        private readonly List<(string nombre, string valor)> opciones = new List<(string nombre, string valor)>();
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece el nombre del motor.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene un valor que indica si el motor está disponible.
        /// </summary>
        public bool Listo { get; private set; }

        /// <summary>
        /// Obtiene la partida que se está analizando o jugando.
        /// </summary>
        public Partida Partida { get; private set; }

        /// <summary>
        /// Obtiene o establece el equipo con el que juega el módulo, null si solo analiza.
        /// </summary>
        public Equipo? Equipo { get; set; }

        /// <summary>
        /// Obtiene o establece la informacion del analisis.
        /// </summary>
        public ObservableCollection<InformacionAnalisis> Informacion { get; set; }
        #endregion

        #region Eventos
        /// <summary>
        /// Tiene lugar cuando el motor indica que jugada realizar.
        /// </summary>
        public event EventHandler<(Jugada, char)> JugadaRealizada;

        /// <summary>
        /// Tiene lugar cuando el motor que juega con las piezas de un equipo dice cual es su nombre.
        /// </summary>
        public event EventHandler<(Equipo, string)> NombreIndicado;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicialiaza una nueva instancia de la clase <see cref="Motor"/> con la ubicación y el equipo.
        /// </summary>
        /// <param name="ubicacion">Ubicación del ejecutable.</param>
        /// <param name="equipo">Equipo que es controlado por el motor, null si el motor solo analiza.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public Motor(string ubicacion, Equipo? equipo = null)
        {
            Equipo = equipo;
            if (Equipo == null)
            {
                Informacion = new ObservableCollection<InformacionAnalisis>();
                for (int i = 0; i < Settings.Default.CantidadDeLineasAnalisis; i++)
                {
                    Informacion.Add(new InformacionAnalisis());
                }
            }
            proceso = new Process { StartInfo = new ProcessStartInfo(ubicacion) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardInput = true, RedirectStandardOutput = true } };
            proceso.OutputDataReceived += Proceso_OutputDataReceived;
            proceso.Start();
            proceso.BeginOutputReadLine();
            EnviarLinea("uci");
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Envía una linea al motor.
        /// </summary>
        /// <param name="mensaje">Cadena que recibirá el motor.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        private void EnviarLinea(string mensaje)
        {
            if (mensaje == "isready")
            {
                Listo = false;
            }
            proceso.StandardInput.WriteLine(mensaje);
            if (mensaje == "ucinewgame")
            {
                EnviarLinea("isready");
            }
        }

        /// <summary>
        /// Informa al motor que analizará una nueva partida.
        /// </summary>
        /// <param name="partida">Partida que se debe analziar o jugar.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void NuevaPartida(Partida partida)
        {
            Partida = partida;
            DetenerAnalisis();
            EnviarLinea("ucinewgame");
        }

        /// <summary>
        /// Indica al motor que debe realizar una jugada.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void RealizarJugada()
        {
            EnviarLinea(string.Format("position {0} moves {1}", Partida.Etiquetas[EtiquetaPGN.SetUp] == "0" ? "startpos" : $"fen {Partida.Etiquetas[EtiquetaPGN.FEN]}", Partida.TextoJugadasUCI));
            string parametrosGo;
            if (string.IsNullOrEmpty(Partida.Etiquetas[EtiquetaPGN.TimeControl]))
            {
                parametrosGo = "depth " + Settings.Default.ProfundidadConTiempoIlimitado;
            }
            else
            {
                parametrosGo = string.Format("wtime {0} btime {1}", Partida.Reloj.TiempoBlanco.TotalMilliseconds, Partida.Reloj.TiempoNegro.TotalMilliseconds);
                if (Partida.Reloj is RelojParcial relojParcial)
                {
                    parametrosGo += string.Format(" movestogo {0}", relojParcial.CantidadJugadas);
                }
                else if (Partida.Reloj is RelojIncremental relojIncremental)
                {
                    parametrosGo += string.Format(" winc {0} binc {0}", relojIncremental.Incremento.TotalMilliseconds);
                }
            }
            EnviarLinea("go " + parametrosGo);
        }

        /// <summary>
        /// El motor analiza la última posición de la partida.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void Analizar()
        {
            DetenerAnalisis();
            EnviarLinea(string.Format("position {0} moves {1}", Partida.Etiquetas[EtiquetaPGN.SetUp] == "0" ? "startpos" : $"fen {Partida.Etiquetas[EtiquetaPGN.FEN]}", Partida.TextoJugadasUCI));
            EnviarLinea("go depth " + Settings.Default.ProfundidadAnalisis);
        }

        /// <summary>
        /// El motor seguirá analiza hasta que se le indique que pare.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void SeguirAnalizando()
        {
            EnviarLinea("go infinite");
        }

        /// <summary>
        /// El motor deja de analisar e indica cual le parece actualmente la mejor jugada.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void DetenerAnalisis() => EnviarLinea("stop");
        #endregion

        #region Controladores de eventos
        private void Proceso_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string[] palabras = e.Data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (palabras.Length > 0)
            {
                switch (palabras[0])
                {
                    case "info":
                        if (Equipo == null && palabras.Length > 19)
                        {
                            int indicePv = Array.IndexOf(palabras, "pv");
                            int indiceMultiPv = Array.IndexOf(palabras, "multipv");
                            int indiceDepth = Array.IndexOf(palabras, "depth");
                            int indiceScore = Array.IndexOf(palabras, "score");
                            int MultiPv = int.Parse(palabras[indiceMultiPv + 1]) - 1;
                            Informacion[MultiPv].Profundidad = palabras[indiceDepth + 1];
                            Informacion[MultiPv].EstablecerValoracion(palabras[indiceScore + 1], palabras[indiceScore + 2], Partida.UltimaPosicion.Turno == Ajedrez.Equipo.Negro);
                            Informacion[MultiPv].EstablecerJugadaYRespuesta(palabras[indicePv + 1], palabras.Length > (indicePv + 2) ? palabras[indicePv + 2] : "", Partida.UltimaPosicion);
                        }
                        break;
                    case "bestmove":
                        if (Equipo != null)
                        { 
                            JugadaRealizada?.Invoke(this, (new Jugada(palabras[1].Substring(0, 2), palabras[1].Substring(2, 2)), palabras[1].Length > 4 ? palabras[1][4] : ' '));
                        }
                        break;
                    case "option":
                        switch (palabras[2])
                        {
                            case "MultiPV":
                                opciones.Add(("MultiPV", Settings.Default.CantidadDeLineasAnalisis.ToString()));
                                break;
                            case "UCI_AnalyseMode":
                                opciones.Add(("UCI_AnalyseMode", Equipo == null ? "true" : "false"));
                                break;
                            case "Threads":
                                opciones.Add(("Threads", Settings.Default.HilosAnalisis));
                                break;
                            case "Hash":
                                opciones.Add(("Hash", Settings.Default.HashAnalisis));
                                break;
                        }
                        break;
                    case "readyok":
                        Listo = true;
                        break;
                    case "id":
                        if (palabras[1] == "name")
                        {
                            Nombre = palabras[2];
                            if (Equipo != null)
                            {
                                NombreIndicado?.Invoke(this, ((Equipo)Equipo, Nombre));
                            }
                        }
                        break;
                    case "uciok":
                        foreach ((string nombre, string valor) opcion in opciones)
                        {
                            EnviarLinea(string.Format("setoption name {0} value {1}", opcion.nombre, opcion.valor));
                        }
                        EnviarLinea("isready");
                        break;
                }
            }
        }
        #endregion
    }
}
