using System;
using System.Text;

namespace Ajedrez
{
    /// <summary>
    /// Especifica los tipos de enroque.
    /// </summary>
    [Flags]
    public enum Enroque
    {
        CortoBlancas = 1,
        LargoBlancas = 2,
        CortoNegras = 4,
        LargoNegras = 8
    }

    /// <summary>
    /// Proporciona métodos de extensión para <see cref="Enroque"/>.
    /// </summary>
    public static class MetodosEnroque
    {
        /// <summary>
        /// Obtiene la representación en FEN de los enroques posibles.
        /// </summary>
        /// <param name="enroque">Enroque que se quiere representar.</param>
        public static string ToFEN(this Enroque enroque)
        {
            StringBuilder builder = new StringBuilder();
            if ((enroque & Enroque.CortoBlancas) != 0) // Si las blancas pueden enrocar corto.
            {
                builder.Append('K');
            }
            if ((enroque & Enroque.LargoBlancas) != 0) // Si las blancas pueden enrocar largo.
            {
                builder.Append('Q');
            }
            if ((enroque & Enroque.CortoNegras) != 0) // Si las negras pueden enrocar corto.
            {
                builder.Append('k');
            }
            if ((enroque & Enroque.LargoNegras) != 0) // Si las negras puedn enrocar largo.
            {
                builder.Append('q');
            }
            if (builder.Length == 0)
            {
                builder.Append('-');
            }
            return builder.ToString();
        }
    }
}
