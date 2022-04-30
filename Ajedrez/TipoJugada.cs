using System;

namespace Ajedrez
{
    /// <summary>
    /// Especifica los tipos de jugada posibles.
    /// </summary>
    [Flags]
    public enum TipoJugada
    {
        Normal = 0,
        Captura = 1,
        Enroque = 2,
        PromocionCaballo = 4,
        PromocionAlfil = 8,
        PromocionTorre = 16,
        PromocionDama = 32,
        Jaque = 64,
        CapturaAlPaso = 128,
        AvanceDoble = 256,
        Imposible = 512,
        Prueba = 1024
    }
}
