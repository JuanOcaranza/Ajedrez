using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ajedrez
{
    /// <summary>
    /// Representa una lista de partidas de solo lectura con información adicional.
    /// </summary>
    public class ListaPartidas : IReadOnlyList<Partida>
    {
        #region Atributos
        /// <summary>
        /// Partidas de la lista.
        /// </summary>
        private readonly Partida[] partidas;

        /// <summary>
        /// Condición predeterminada para que una partida se considere ganada.
        /// </summary>
        private static readonly Func<Partida, bool> condicionVictoriaPredeterminada = p => p.Etiquetas[EtiquetaPGN.Result] == "1-0";

        /// <summary>
        /// Condición predeterminada para que una partida se considere pérdida.
        /// </summary>
        private static readonly Func<Partida, bool> condicionDerrotaPredeterminada = p => p.Etiquetas[EtiquetaPGN.Result] == "0-1";
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene el porcentaje de victorias del blanco o del jugador al que pertencen las partidas.
        /// </summary>
        public int Victoria { get; }

        /// <summary>
        /// Obtiene el porcentaje de victorias del negro o de jugador contrario al que pertencen las partias.
        /// </summary>
        public int Derrota { get; }

        /// <summary>
        /// Obtiene el porcentaja de tablas.
        /// </summary>
        public int Tablas { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con intercambio de damas en las primeras 25 jugadas.
        /// </summary>
        public int IntercambioDeDamas { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con pareja de alfiles para un equipo y no para el otro en las primeras 25 jugadas.
        /// </summary>
        public int ParejaDeAlfiles { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con columnas d y e abiertas en las primeras 25 jugadas.
        /// </summary>
        public int ColumnasCentralesAbiertas { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con peón de dama aislado para un equipo y no para el otro en las primeras 25 jugadas.
        /// </summary>
        public int PeonDamaAislado { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con reyes en flancos opuestos en las primeras 25 jugadas.
        /// </summary>
        public int ReyesEnFlancosOpuestos { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con un peón blanco en sexta o uno negro en tercera en las primeras 25 jugadas.
        /// </summary>
        public int PeonLlegaASexta { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con final de dama contra dama.
        /// </summary>
        public int FinalDamaContraDama { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con final de dama contra torre.
        /// </summary>
        public int FinalDamaContraTorre { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con final de dama contra alfil.
        /// </summary>
        public int FinalDamaContraAlfil { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con final de dama contra caballo.
        /// </summary>
        public int FinalDamaContraCaballo { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con final de torre contra torre.
        /// </summary>
        public int FinalTorreContraTorre { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con final de torre contra alfil.
        /// </summary>
        public int FinalTorreContraAlfil { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con final de torre contra caballo.
        /// </summary>
        public int FinalTorreContraCaballo { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con final de alfil contra alfil.
        /// </summary>
        public int FinalAlfilContraAlfil { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con final de alfil contra caballo.
        /// </summary>
        public int FinalAlfilContraCaballo { get; }

        /// <summary>
        /// Obtiene el porcentaja de partidas con final de caballo contra caballo.
        /// </summary>
        public int FinalCaballoContraCaballo { get; }

        public int Count => partidas.Length;
        #endregion

        #region Indexer
        public Partida this[int index] => partidas[index];
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ListaPartidas"/> con las partidas y las condiciones de victoria y derrota.
        /// </summary>
        /// <param name="listaPartidas">Partidas de la lista</param>
        /// <param name="condicionVictoria">Condición para que una partida se considere ganada, null para utilizar la perspectiva de las blancas</param>
        /// <param name="condicionDerrota">Condición para que una partida se considere pérdida, null para utilizar la perspectiva de las blancas</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OverflowException"></exception>
        public ListaPartidas(IEnumerable<Partida> listaPartidas, Func<Partida, bool> condicionVictoria = null, Func<Partida, bool> condicionDerrota = null)
        {
            partidas = listaPartidas.ToArray();
            if (Count > 0)
            {
                Victoria = this.Count(condicionVictoria ?? condicionVictoriaPredeterminada) * 100 / Count;
                Derrota = this.Count(condicionDerrota ?? condicionDerrotaPredeterminada) * 100 / Count;
                Tablas = this.Count(p => p.Etiquetas[EtiquetaPGN.Result] == "1/2-1/2") * 100 / Count;
                IntercambioDeDamas = this.Count(p => p.Cumple(o => o.IntercambioDeDamas(), 25)) * 100 / Count;
                ParejaDeAlfiles = this.Count(p => p.Cumple(o => o.ParejaDeAlfiles(), 25)) * 100 / Count;
                ColumnasCentralesAbiertas = this.Count(p => p.Cumple(o => o.ColumnasCentralesAbiertas(), 25)) * 100 / Count;
                PeonDamaAislado = this.Count(p => p.Cumple(o => o.PeonDamaAislado(), 25)) * 100 / Count;
                ReyesEnFlancosOpuestos = this.Count(p => p.Cumple(o => o.ReyesEnFlancosOpuestos(), 25)) * 100 / Count;
                PeonLlegaASexta = this.Count(p => p.Cumple(o => o.PeonLlegaASexta(), 25)) * 100 / Count;
                FinalDamaContraDama = this.Count(p => p.Cumple(o => o.FinalDePiezas(Pieza.Dama, Pieza.Dama))) * 100 / Count;
                FinalDamaContraTorre = this.Count(p => p.Cumple(o => o.FinalDePiezas(Pieza.Dama, Pieza.Torre))) * 100 / Count;
                FinalDamaContraAlfil = this.Count(p => p.Cumple(o => o.FinalDePiezas(Pieza.Dama, Pieza.Alfil))) * 100 / Count;
                FinalDamaContraCaballo = this.Count(p => p.Cumple(o => o.FinalDePiezas(Pieza.Dama, Pieza.Caballo))) * 100 / Count;
                FinalTorreContraTorre = this.Count(p => p.Cumple(o => o.FinalDePiezas(Pieza.Torre, Pieza.Torre))) * 100 / Count;
                FinalTorreContraAlfil = this.Count(p => p.Cumple(o => o.FinalDePiezas(Pieza.Torre, Pieza.Alfil))) * 100 / Count;
                FinalTorreContraCaballo = this.Count(p => p.Cumple(o => o.FinalDePiezas(Pieza.Torre, Pieza.Caballo))) * 100 / Count;
                FinalAlfilContraAlfil = this.Count(p => p.Cumple(o => o.FinalDePiezas(Pieza.Alfil, Pieza.Alfil))) * 100 / Count;
                FinalAlfilContraCaballo = this.Count(p => p.Cumple(o => o.FinalDePiezas(Pieza.Alfil, Pieza.Caballo))) * 100 / Count;
                FinalCaballoContraCaballo = this.Count(p => p.Cumple(o => o.FinalDePiezas(Pieza.Caballo, Pieza.Caballo))) * 100 / Count;
            }
            else
            {
                Victoria = 0;
                Derrota = 0;
                Tablas = 0;
                IntercambioDeDamas = 0;
                ParejaDeAlfiles = 0;
                ColumnasCentralesAbiertas = 0;
                PeonDamaAislado = 0;
                ReyesEnFlancosOpuestos = 0;
                PeonLlegaASexta = 0;
                FinalDamaContraDama = 0;
                FinalDamaContraTorre = 0;
                FinalDamaContraAlfil = 0;
                FinalDamaContraCaballo = 0;
                FinalTorreContraTorre = 0;
                FinalTorreContraAlfil = 0;
                FinalTorreContraCaballo = 0;
                FinalAlfilContraAlfil = 0;
                FinalAlfilContraCaballo = 0;
                FinalCaballoContraCaballo = 0;
            }
        }
        #endregion

        #region Métodos
        public IEnumerator<Partida> GetEnumerator()
        {
            return ((IEnumerable<Partida>)partidas).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return partidas.GetEnumerator();
        }
        #endregion
    }
}
