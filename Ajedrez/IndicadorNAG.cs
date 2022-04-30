namespace Ajedrez
{
    /// <summary>
    /// Especifica los indicadores NAG
    /// </summary>
    public enum IndicadorNAG
    {
        Nulo = 0,
        JugadaBuena = 1,
        JugadaMala = 2,
        JugadaMuyBuena = 3,
        JugadaMuyMala = 4,
        JugadaEspeculativa = 5,
        JugadaCuestionable = 6,
        JugadaForzada = 7,
        JugadaSingular = 8,
        PeorJugada = 9,
        PosicionIgualada = 10,
        IgualesChancesPosicionTranquila = 11,
        IgualesChancesPosicionActiva = 12,
        PosicionNoClara = 13,
        BlancoTienePequeñaVentaja = 14,
        NegroTienePequeñaVentaja = 15,
        BlancoTieneVentaja = 16,
        NegroTieneVentaja = 17,
        BlancoTieneVentajaDecisiva = 18,
        NegroTieneVentajaDecisiva = 19,
        BlancoTieneVentajaAplastante = 20,
        NegroTieneVentajaAplastante = 21,
        BlancoEnZugzwang = 22,
        NegroEnZugzwang = 23,
        BlancoTienePequeñaVentajaDeEspacio = 24,
        NegroTienePequeñaVentajaDeEspacio = 25,
        BlancoTieneVentajaDeEspacio = 26,
        NegroTieneVentajaDeEspacio = 27,
        BlancoTieneVentajaDeEspacioDecisiva = 28,
        NegroTieneVentajaDeEspacioDecisiva = 29,
        BlancoTienePequeñaVentajaDeDesarrollo = 30,
        NegroTienePequeñaVentajaDeDesarrollo = 31,
        BlancoTieneVentajaDeDesarrollo = 32,
        NegroTieneVentajaDeDesarrollo = 33,
        BlancoTieneVentajaDeDesarrolloDecisiva = 34,
        NegroTieneVentajaDeDesarrolloDecisiva = 35,
        BlancoTieneLaIniciativa = 36,
        NegroTieneLaIniciativa = 37,
        BlancoTieneIniciativaDuradera = 38,
        NegroTieneIniciativaDuradera = 39,
        BlancoTieneElAtaque = 40,
        NegroTieneElAtaque = 41,
        BlancoTieneInsuficienteCompensacionPorDesventajaMaterial = 42,
        NegroTieneInsuficienteCompensacionPorDesventajaMaterial = 43,
        BlancoTieneSuficienteCompensacionPorDesventajaMaterial = 44,
        NegroTieneSuficienteCompensacionPorDesventajaMaterial = 45,
        BlancoTieneCompensacionMasQueAdecuadPorDesventajaMaterial = 46,
        NegroTieneCompensacionMasQueAdecuadaPorDesventajaMaterial = 47,
        BlancoTieneUnPocoMasDeControlEnElCentro = 48,
        NegroTieneUnPocoMasDeControlEnElCentro = 49,
        BlancoTieneMayorControlEnElCentro = 50,
        NegroTieneMayorControlEnElCentro = 51,
        BlancoTieneControlDesicivoEnElCentro = 52,
        NegroTieneControlDesicivoEnElCentro = 53,
        BlancoTieneUnPocoMasDeControlEnElFlancoDeRey = 54,
        NegroTieneUnPocoMasDeControlEnElFlancoDeRey = 55,
        BlancoTieneMayorControlEnElFlancoDeRey = 56,
        NegroTieneMayorControlEnElFlancoDeRey = 57,
        BlancoTieneControlDesicivoEnElFlancoDeRey = 58,
        NegroTieneControlDesicivoEnElFlancoDeRey = 59,
        BlancoTieneUnPocoMasDeControlEnElFlancoDeDama = 60,
        NegroTieneUnPocoMasDeControlEnElFlancoDeDama = 61,
        BlancoTieneMayorControlEnElFlancoDeDama = 62,
        NegroTieneMayorControlEnElFlancoDeDama = 63,
        BlancoTieneControlDesicivoEnElFlancoDeDama = 64,
        NegroTieneControlDesicivoEnElFlancoDeDama = 65,
        BlancoTieneLaPrimeraFilaVulnerable = 66,
        NegroTieneLaPrimeraFilaVulnerable = 67,
        BlancoTieneLaPrimeraFilaBienDefendida = 68,
        NegroTieneLaPrimeraFilaBienDefendida = 69,
        BlancoTieneElReyMalDefendido = 70,
        NegroTieneElReyMalDefendido = 71,
        BlancoTieneElReyBienDefendido = 72,
        NegroTieneElReyBienDefendido = 73,
        BlancoTieneElReyMalUbicado = 74,
        NegroTieneElReyMalUbicado = 75,
        BlancoTieneElReyBienUbicado = 76,
        NegroTieneElReyBienUbicado = 77,
        BlancoTieneEstructuraDePeonesMuyDebil = 78,
        NegroTieneEstructuraDePeonesMuyDebil = 79,
        BlancoTieneEstructuraDePeonesDebil = 80,
        NegroTieneEstructuraDePeonesDebil = 81,
        BlancoTieneEstructuraDePeonesFuerte = 82,
        NegroTieneEstructuraDePeonesFuerte = 83,
        BlancoTieneEstructuraDePeonesMuyFuerte = 84,
        NegroTieneEstructuraDePeonesMuyFuerte = 85,
        BlancoTieneCaballoMalUbicado = 86,
        NegroTieneCaballoMalUbicado = 87,
        BlancoTieneCaballoBienUbicado = 88,
        NegroTieneCaballoBienUbicado = 89,
        BlancoTieneAlfilMalUbicado = 90,
        NegroTieneAlfilMalUbicado = 91,
        BlancoTieneAlfilBienUbicado = 92,
        NegroTieneAlfilBienUbicado = 93,
        BlancoTieneTorreMalUbicado = 94,
        NegroTieneTorreMalUbicado = 95,
        BlancoTieneTorreBienUbicado = 96,
        NegroTieneTorreBienUbicado = 97,
        BlancoTieneDamaMalUbicado = 98,
        NegroTieneDamaMalUbicado = 99,
        BlancoTieneDamaBienUbicado = 100,
        NegroTieneDamaBienUbicado = 101,
        BlancoTieneMalaCoordinacion = 102,
        NegroTieneMalaCoordinacion = 103,
        BlancoTieneBuenaCoordinacion = 104,
        NegroTieneBuenaCoordinacion = 105,
        BlancoHaJugadoLaAperturaMuyMal = 106,
        NegroHaJugadoLaAperturaMuyMal = 107,
        BlancoHaJugadoLaAperturaMal = 108,
        NegroHaJugadoLaAperturaMal = 109,
        BlancoHaJugadoLaAperturaBien = 110,
        NegroHaJugadoLaAperturaBien = 111,
        BlancoHaJugadoLaAperturaMuyBien = 112,
        NegroHaJugadoLaAperturaMuyBien = 113,
        BlancoHaJugadoElMedioJuegoMuyMal = 114,
        NegroHaJugadoElMedioJuegoMuyMal = 115,
        BlancoHaJugadoElMedioJuegoMal = 116,
        NegroHaJugadoElMedioJuegoMal = 117,
        BlancoHaJugadoElMedioJuegoBien = 118,
        NegroHaJugadoElMedioJuegoBien = 119,
        BlancoHaJugadoElMedioJuegoMuyBien = 120,
        NegroHaJugadoElMedioJuegoMuyBien = 121,
        BlancoHaJugadoElFinalMuyMal = 122,
        NegroHaJugadoElFinalMuyMal = 123,
        BlancoHaJugadoElFinalMal = 124,
        NegroHaJugadoElFinalMal = 125,
        BlancoHaJugadoElFinalBien = 126,
        NegroHaJugadoElFinalBien = 127,
        BlancoHaJugadoElFinalMuyBien = 128,
        NegroHaJugadoElFinalMuyBien = 129,
        BlancoTienUnPocoDeContrajuego = 130,
        NegroTienUnPocoDeContrajuego = 131,
        BlancoTienContrajuego = 132,
        NegroTienContrajuego = 133,
        BlancoTienContrajuegoDecisivo = 134,
        NegroTienContrajuegoDecisivo = 135,
        BlancoTienePresionDeTiempo = 136,
        NegroTienePresionDeTiempo = 137,
        BlancoTienePresionDeTiempoSevera = 138,
        NegroTienePresionDeTiempoSevera = 139,
    }

    /// <summary>
    /// Proporciona métodos de extensión para <see cref="IndicadorNAG"/>
    /// </summary>
    public static class MetodosIndicadorNAG
    {
        /// <summary>
        /// Obtiene el símbolo que representa el NAG.
        /// </summary>
        /// <param name="indicadorNAG">NAG del que se va a obtener el símbolo.</param>
        public static string ToStringSimbolo(this IndicadorNAG indicadorNAG)
        {
            switch (indicadorNAG)
            {
                case IndicadorNAG.JugadaBuena:
                    return "! ";
                case IndicadorNAG.JugadaMala:
                    return "? ";
                case IndicadorNAG.JugadaMuyBuena:
                    return "‼ ";
                case IndicadorNAG.JugadaMuyMala:
                    return "⁇ ";
                case IndicadorNAG.JugadaEspeculativa:
                    return "⁉ ";
                case IndicadorNAG.JugadaCuestionable:
                    return "⁈ ";
                case IndicadorNAG.JugadaForzada:
                    return "□ ";
                case IndicadorNAG.PosicionIgualada:
                    return "= ";
                case IndicadorNAG.PosicionNoClara:
                    return "∞ ";
                case IndicadorNAG.BlancoTienePequeñaVentaja:
                    return "⩲ ";
                case IndicadorNAG.NegroTienePequeñaVentaja:
                    return "⩱ ";
                case IndicadorNAG.BlancoTieneVentaja:
                    return "± ";
                case IndicadorNAG.NegroTieneVentaja:
                    return "∓ ";
                case IndicadorNAG.BlancoTieneVentajaDecisiva:
                    return "+ − ";
                case IndicadorNAG.NegroTieneVentajaDecisiva:
                    return "− + ";
                case IndicadorNAG.BlancoEnZugzwang: // Es el mismo símbolo en ambos casos.
                case IndicadorNAG.NegroEnZugzwang:
                    return "⨀ ";
                case IndicadorNAG.BlancoTieneVentajaDeDesarrollo: // Es el mismo símbolo en ambos casos.
                case IndicadorNAG.NegroTieneVentajaDeDesarrollo:
                    return "⟳ ";
                case IndicadorNAG.BlancoTieneLaIniciativa: // Es el mismo símbolo en ambos casos.
                case IndicadorNAG.NegroTieneLaIniciativa:
                    return "→ ";
                case IndicadorNAG.BlancoTieneElAtaque: // Es el mismo símbolo en ambos casos.
                case IndicadorNAG.NegroTieneElAtaque:
                    return "↑ ";
                case IndicadorNAG.BlancoTieneSuficienteCompensacionPorDesventajaMaterial: // Es el mismo símbolo en ambos casos.
                case IndicadorNAG.NegroTieneSuficienteCompensacionPorDesventajaMaterial:
                    return "=/∞ ";
                case IndicadorNAG.BlancoTienContrajuego: // Es el mismo símbolo en ambos casos.
                case IndicadorNAG.NegroTienContrajuego:
                    return "⇆ ";
                case IndicadorNAG.BlancoTienePresionDeTiempoSevera: // Es el mismo símbolo en ambos casos.
                case IndicadorNAG.NegroTienePresionDeTiempoSevera:
                    return "⨁ ";
                default:
                    return " "; // Los demas indicadores aun no tienen un símbolo definido.
            }
        }

        /// <summary>
        /// Obtiene el IndicadorNAG que representa el símbolo.
        /// </summary>
        /// <param name="simbolo">Cadena formada por el símbolo.</param>
        public static IndicadorNAG SimboloToNAG(string simbolo)
        {
            switch (simbolo)
            {
                case "!":
                    return IndicadorNAG.JugadaBuena;
                case "!!":
                    return IndicadorNAG.JugadaMuyBuena;
                case "!?":
                    return IndicadorNAG.JugadaEspeculativa;
                case "?":
                    return IndicadorNAG.JugadaMala;
                case "?!":
                    return IndicadorNAG.JugadaCuestionable;
                case "??":
                    return IndicadorNAG.JugadaMuyMala;
                default:
                    return IndicadorNAG.Nulo; // Los demas símbolos no pueden aparecer en un PGN.
            }
        }
    }
}
