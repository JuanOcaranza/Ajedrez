using Ajedrez.Properties;
using System.Windows.Data;

namespace Ajedrez
{
    /// <summary>
    /// Extención de xaml para enlazar una configuración con elementos WPF
    /// </summary>
    public class OpcionExtension : Binding
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="OpcionExtension"/>.
        /// </summary>
        public OpcionExtension()
        {
            Initialize();
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="OpcionExtension"/> con el path del enlace.
        /// </summary>
        /// <param name="path">Nombre de la opción con la que se va a enlazar.</param>
        public OpcionExtension(string path) : base(path)
        {
            Initialize();
        }

        /// <summary>
        /// Establece el origen y el modo del enlace.
        /// </summary>
        private void Initialize()
        {
            Source = Settings.Default;
            Mode = BindingMode.TwoWay;
        }
    }
}
