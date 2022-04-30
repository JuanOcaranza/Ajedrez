using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Ajedrez
{
    /// <summary>
    /// Clase base para las ventanas sin botones.
    /// </summary>
    public class VentanaSinBotones : Window
    {
        #region Métodos implementados externamente
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion

        #region Controladores de eventos
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            if (sender is MainWindow)
            {
                SetWindowLong(hwnd, -16, GetWindowLong(hwnd, -16) & ~0x800000); // Quita la barra de título.
            }
            else
            {
                SetWindowLong(hwnd, -16, GetWindowLong(hwnd, -16) & ~0x80000); // Quita las botones de la esquina superior derecha.
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4) // Si se precionó Alt+F4.
            {
                e.Handled = true; // Evita que se cierre la ventana.
            }
        }
        #endregion
    }
}
