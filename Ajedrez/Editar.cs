using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ajedrez
{
    /// <summary>
    /// Clase base para las ventanas que editan un objeto.
    /// </summary>
    /// <typeparam name="T">Tipo de objeto que se va a editar.</typeparam>
    public class Editar<T> : VentanaSinBotones where T : ICloneable
    {
        #region Atributos
        /// <summary>
        /// Cantidad de errores de validación.
        /// </summary>
        protected int cantidadErrores = 0;

        /// <summary>
        /// Objeto antes de las modificaciones.
        /// </summary>
        protected T objetoAntiguo;
        #endregion

        #region Propiedad
        /// <summary>
        /// Obtiene o establece el objeto que se está editando.
        /// </summary>
        public T Objeto { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="Editar{T}"/> con el objeto que se va a editar.
        /// </summary>
        /// <param name="objeto">Objeto que se va editar.</param>
        public Editar(T objeto)
        {
            objetoAntiguo = (T)objeto.Clone();
            Objeto = objeto;
        }
        #endregion

        #region Controladores de eventos
        protected void Aceptar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = cantidadErrores == 0;

        protected void TextBox_ValidationError(object sender, ValidationErrorEventArgs e) => cantidadErrores += e.Action == ValidationErrorEventAction.Added ? 1 : -1;

        protected void Cancelar_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        protected void Cancelar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Objeto = objetoAntiguo;
            Close();
        }
        #endregion
    }
}
