using Ajedrez.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Ajedrez
{
    /// <summary>
    /// Especifica los tipo de pieza.
    /// </summary>
    /// <remarks>Incluye colores y formas</remarks>
    public enum Pieza
    {
        Blanca = 0,
        Negra = 1,
        Peon = 2,
        Caballo = 3,
        Alfil = 4,
        Torre = 5,
        Dama = 6,
        Rey = 7
    }

    /// <summary>
    /// Especifica los tipos de caracteres que pueder representar a las piezas.
    /// </summary>
    public enum CaracterPieza
    {
        SimboloBlanco = 0,
        Español = 1,
        Ingles = 2,
        SimboloNegro = 3,
        InglesMinuscula = 4
    }

    /// <summary>
    /// Proporciona métodos de extensión para <see cref="Pieza"/> y un método estático para obtener la pieza representada por un caracter.
    /// </summary>
    public static class MetodosPieza
    {
        #region Atributos
        /// <summary>
        /// Indica la dirección a cada tipo de pieza.
        /// </summary>
        private static readonly Dictionary<Pieza, Dictionary<Pieza, Uri>> diccionarioControl = new Dictionary<Pieza, Dictionary<Pieza, Uri>>
        {
            {
                Pieza.Blanca, new Dictionary<Pieza, Uri>
                {
                    {Pieza.Peon, new Uri(@"Controles/Piezas/" + Settings.Default.ConjuntoPiezas + "/PeonBlanco.xaml", UriKind.Relative) },
                    {Pieza.Caballo, new Uri(@"Controles/Piezas/" + Settings.Default.ConjuntoPiezas + "/CaballoBlanco.xaml", UriKind.Relative) },
                    {Pieza.Alfil, new Uri(@"Controles/Piezas/" + Settings.Default.ConjuntoPiezas + "/AlfilBlanco.xaml", UriKind.Relative) },
                    {Pieza.Torre, new Uri(@"Controles/Piezas/" + Settings.Default.ConjuntoPiezas + "/TorreBlanca.xaml", UriKind.Relative) },
                    {Pieza.Dama, new Uri(@"Controles/Piezas/" + Settings.Default.ConjuntoPiezas + "/DamaBlanca.xaml", UriKind.Relative) },
                    {Pieza.Rey, new Uri(@"Controles/Piezas/" + Settings.Default.ConjuntoPiezas + "/ReyBlanco.xaml", UriKind.Relative) }
                }
            },
            {
                Pieza.Negra, new Dictionary<Pieza, Uri>
                {
                    {Pieza.Peon, new Uri(@"Controles/Piezas/" + Settings.Default.ConjuntoPiezas + "/PeonNegro.xaml", UriKind.Relative) },
                    {Pieza.Caballo, new Uri(@"Controles/Piezas/" + Settings.Default.ConjuntoPiezas + "/CaballoNegro.xaml", UriKind.Relative) },
                    {Pieza.Alfil, new Uri(@"Controles/Piezas/" + Settings.Default.ConjuntoPiezas + "/AlfilNegro.xaml", UriKind.Relative) },
                    {Pieza.Torre, new Uri(@"Controles/Piezas/" + Settings.Default.ConjuntoPiezas + "/TorreNegra.xaml", UriKind.Relative) },
                    {Pieza.Dama, new Uri(@"Controles/Piezas/" + Settings.Default.ConjuntoPiezas + "/DamaNegra.xaml", UriKind.Relative) },
                    {Pieza.Rey, new Uri(@"Controles/Piezas/" + Settings.Default.ConjuntoPiezas + "/ReyNegro.xaml", UriKind.Relative) }
                }
            }
        };

        private static readonly Dictionary<CaracterPieza, Dictionary<Pieza, char>> diccionarioCaracter = new Dictionary<CaracterPieza, Dictionary<Pieza, char>>
        {
            { CaracterPieza.SimboloBlanco, new Dictionary<Pieza, char> { {Pieza.Peon, '♙' }, {Pieza.Caballo, '♘' }, {Pieza.Alfil, '♗' }, {Pieza.Torre, '♖' }, {Pieza.Dama, '♕' },
                {Pieza.Rey, '♔' } } },
            { CaracterPieza.SimboloNegro, new Dictionary<Pieza, char> { {Pieza.Peon, '♟' }, {Pieza.Caballo, '♞' }, {Pieza.Alfil, '♝' }, {Pieza.Torre, '♜' }, {Pieza.Dama, '♛' },
                {Pieza.Rey, '♚' } } },
            { CaracterPieza.Español, new Dictionary<Pieza, char> { {Pieza.Peon, 'P' }, {Pieza.Caballo, 'C' }, {Pieza.Alfil, 'A' }, {Pieza.Torre, 'T' }, {Pieza.Dama, 'D' }, {Pieza.Rey, 'R' } } },
            { CaracterPieza.Ingles, new Dictionary<Pieza, char> { {Pieza.Peon, 'P' }, {Pieza.Caballo, 'N' }, {Pieza.Alfil, 'B' }, {Pieza.Torre, 'R' }, {Pieza.Dama, 'Q' }, {Pieza.Rey, 'K' } } },
            { CaracterPieza.InglesMinuscula, new Dictionary<Pieza, char> { {Pieza.Peon, 'p' }, {Pieza.Caballo, 'n' }, {Pieza.Alfil, 'b' }, {Pieza.Torre, 'r' }, {Pieza.Dama, 'q' },
                {Pieza.Rey, 'k' } } },
        };
        #endregion

        #region Métodos
        /// <summary>
        /// Obtiene el control que representa la pieza.
        /// </summary>
        /// <param name="pieza">Tipo de pieza.</param>
        /// <param name="color">Color de la pieza.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static UserControl ToUserControl(this Pieza pieza, Pieza color) => Application.LoadComponent(diccionarioControl[color][pieza]) as UserControl;

        /// <summary>
        /// Obtiene el caracter que representa la pieza.
        /// </summary>
        /// <param name="pieza">Tipo de pieza.</param>
        /// <param name="caracter">Tipo de caracteres.</param>
        public static char ToChar(this Pieza pieza, CaracterPieza caracter) => diccionarioCaracter[caracter][pieza];

        /// <summary>
        /// Obtiene el tipo de jugada que representa coronar la pieza
        /// </summary>
        /// <param name="pieza">Pieza que se corona</param>
        /// <exception cref="ArgumentException"></exception>
        public static TipoJugada ToTipoPromocion(this Pieza pieza)
        {
            switch (pieza)
            {
                case Pieza.Caballo:
                    return TipoJugada.PromocionCaballo;
                case Pieza.Alfil:
                    return TipoJugada.PromocionAlfil;
                case Pieza.Torre:
                    return TipoJugada.PromocionTorre;
                case Pieza.Dama:
                    return TipoJugada.PromocionDama;
                default:
                    throw new ArgumentException("No hay una promoción de este tipo de pieza", nameof(pieza));
            }
        }

        /// <summary>
        /// Obtiene la pieza que es representada por la letra.
        /// </summary>
        /// <param name="tipoCaracter">Tipo de caracteres.</param>
        /// <param name="letra">Letra que representa una pieza.</param>
        public static Pieza CharToPieza(CaracterPieza tipoCaracter, char letra) => diccionarioCaracter[tipoCaracter].First(d => d.Value == letra).Key;
        #endregion
    }
}
