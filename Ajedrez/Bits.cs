namespace Ajedrez
{
    /// <summary>
    /// Proporciona métodos estáticos que operan sobre los bits de un número.
    /// </summary>
    public static class Bits
    {
        #region Atributos
        /// <summary>
        /// Constante utilizada en el cálculo del bit más o menos significativo para obtener el valor corespondiente en <see cref="index64"/>.
        /// </summary>
        private const ulong debruijn64 = 0x03F7_9D71_B4CB_0A89;

        /// <summary>
        /// Valores posibles del índice del bit más o menos significativo ordenados de forma que se pueden obtener el valor correspondiente al realizar un operación con <see cref="debruijn64"/>.
        /// </summary>
        private static readonly int[] index64 = { 0, 47,  1, 56, 48, 27,  2, 60, 57, 49, 41, 37, 28, 16,  3, 61, 54, 58, 35, 52, 50, 42, 21, 44, 38, 32, 29, 23, 17, 11,  4, 62, 46, 55, 26, 59, 40, 36, 15,
            53, 34, 51, 20, 43, 31, 22, 10, 45, 25, 39, 14, 33, 19, 30,  9, 24, 13, 18,  8, 12,  7,  6,  5, 63 };
        #endregion

        #region Métodos
        /// <summary>
        /// Obitiene el índice del bit menos significativo cuyo valor es 1.
        /// </summary>
        /// <param name="u">Número en el que se determinará el bit menos significativo.</param>
        /// <returns>Índice (0..63) del primer bit 1, 64 si no hay.</returns>
        public static int IndiceBitMenosSignificativo(ulong u) => u == 0 ? 64 : index64[((u ^ (u - 1)) * debruijn64) >> 58];

        /// <summary>
        /// Calcula el número con los bits en el orden contrario.
        /// </summary>
        /// <param name="u">Número con los bits más significativos a la izquierda.param>
        /// <returns>Número con los bits más significativos a la derecha.</returns>
        public static ulong CambiarOrdenBits(ulong u) => (u & 0xFF) << 56 | (u & 0xFF00) << 40 | (u & 0xFF_0000) << 24 | (u & 0xFF00_0000) << 8 | (u & 0xFF_0000_0000) >> 8 |
            (u & 0xFF00_0000_0000) >> 24 | (u & 0xFF_0000_0000_0000) >> 40 | (u & 0xFF00_0000_0000_0000) >> 56;

        /// <summary>
        /// Obtiene el índice del bit más significativo cuyo valor es 1.
        /// </summary>
        /// <param name="u">Número en el que se determinará el bit más significativo.</param>
        /// <returns>Índice (0..63) del último bit 1, 64 si no hay.</returns>
        public static int IndiceBitMasSignificativo(ulong u)
        {
            if (u == 0)
            {
                return 64;
            }
            u |= u >> 1;
            u |= u >> 2;
            u |= u >> 4;
            u |= u >> 8;
            u |= u >> 16;
            u |= u >> 32;
            return index64[(u * debruijn64) >> 58];
            #endregion
        }
    }
}