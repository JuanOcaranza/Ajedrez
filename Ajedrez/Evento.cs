using Ajedrez.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace Ajedrez
{
    /// <summary>
    /// Representa un evento de ajedrez.
    /// </summary>
    public class Evento : IDataErrorInfo, INotifyPropertyChanged, ICloneable
    {
        #region Atributos
        /// <summary>
        /// Nombre del evento.
        /// </summary>
        private string nombre;

        /// <summary>
        /// Lugar donde ocurre el evento.
        /// </summary>
        private string sitio;

        /// <summary>
        /// Fecha de inicio del evento.
        /// </summary>
        /// <remarks>En formato aaaa.mm.dd</remarks>
        private string fecha;

        /// <summary>
        /// Sección del evento.
        /// </summary>
        private string seccion;

        /// <summary>
        /// Partidas jugadas en el evento.
        /// </summary>
        private ListaPartidas partidas;
        #endregion

        #region Evento
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece el nombre del evento.
        /// </summary>
        public string Nombre
        {
            get => nombre;
            set
            {
                nombre = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nombre)));
            }
        }

        /// <summary>
        /// Obtiene o establece el lugar donde ocurre el evento.
        /// </summary>
        public string Sitio
        {
            get => sitio;
            set
            {
                sitio = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sitio)));
            }
        }

        /// <summary>
        /// Obtiene o establece la fecha de inicio del evento.
        /// </summary>
        /// <remarks>En formato aaaa.mm.dd</remarks>
        public string Fecha
        {
            get => fecha;
            set
            {
                fecha = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Fecha)));
            }
        }

        /// <summary>
        /// Obtiene o establece la sección del evento.
        /// </summary>
        public string Seccion
        {
            get => seccion;
            set
            {
                seccion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Seccion)));
            }
        }

        /// <summary>
        /// Obtiene o establece las partidas jugadas en el evento.
        /// </summary>
        public ListaPartidas Partidas
        {
            get => partidas;
            set
            {
                partidas = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Partidas)));
            }
        }

        /// <summary>
        /// Obtiene o establece las participaciones en el evento.
        /// </summary>
        public ObservableCollection<Participa> Participaciones { get; set; } = new ObservableCollection<Participa>();

        public string Error => null;
        #endregion

        #region Indexer
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Nombre):
                        return string.IsNullOrWhiteSpace(Nombre) ? "Debe ingresar un nombre" : null;
                    case nameof(Sitio):
                        return string.IsNullOrWhiteSpace(Sitio) ? "Debe ingresar un sitio" : null;
                    default:
                        return null;
                }
            }
        }
        #endregion

        #region Constructores
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Evento"/> con el nombre, el sitio, la fecha y la sección.
        /// </summary>
        /// <param name="nombre">Nombre del evento.</param>
        /// <param name="sitio">Lugar donde ocurre el evento.</param>
        /// <param name="fecha">Fecha de inicio de evento (En formato AAAA.MM.DD).</param>
        /// <param name="seccion">Sección de evento.</param>
        public Evento(string nombre, string sitio, string fecha, string seccion)
        {
            Nombre = nombre;
            Sitio = sitio;
            Fecha = fecha;
            Seccion = seccion;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Evento"/>
        /// </summary>
        public Evento() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Evento"/> con el nombre, el sitio, la fecha, la sección y las participaciones.
        /// </summary>
        /// <param name="nombre">Nombre del evento.</param>
        /// <param name="sitio">Lugar donde ocurre el evento.</param>
        /// <param name="fecha">Fecha de inicio de evento (En formato AAAA.MM.DD).</param>
        /// <param name="seccion">Sección de evento.</param>
        /// <param name="participaciones">Participaciones en el evento.</param>
        public Evento(string nombre, string sitio, string fecha, string seccion, ObservableCollection<Participa> participaciones) : this(nombre, sitio, fecha, seccion) => Participaciones =
            participaciones;
        #endregion

        #region Métodos
        /// <summary>
        /// Inserta el evento en la base de datos, si ya existe lo actualiza.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public void InsertarEnBaseDeDatos()
        {
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT Nombre FROM Evento WHERE Nombre = '{Nombre}'", conexion))
                {
                    using (SqlCommand command = new SqlCommand(select.ExecuteScalar() == null ? $"INSERT INTO Evento VALUES ('{Nombre}', '{Sitio}', {Fecha.ToSQL()}, {Seccion.ToSQL()})" :
                        $"UPDATE Evento SET Sitio = '{Sitio}', Fecha = {Fecha.ToSQL()}, Seccion = {Seccion.ToSQL()} WHERE Nombre = '{Nombre}'", conexion))
                    {
                        command.ExecuteNonQuery();
                    }
                    foreach (Participa participa in Participaciones)
                    {
                        participa.InsertarEnBaseDeDatos();
                    }
                }
            }
        }

        public object Clone() => new Evento(Nombre, Sitio, Fecha, Seccion, new ObservableCollection<Participa>(Participaciones));

        /// <summary>
        /// Establece los valores para <see cref="Partidas"/> según la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public void EstablecerPartidasDesdeBaseDeDatos()
        {
            List<Partida> listaPartidas = new List<Partida>();
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT Id FROM Partida WHERE Nombre_Evento = '{Nombre}'", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaPartidas.Add(new Partida(reader.GetInt32(0)));
                        }
                    }
                }
            }
            Partidas = new ListaPartidas(listaPartidas);
        }

        /// <summary>
        /// Establecer los valores para <see cref="Participaciones"/> según la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public void EstablecerParticipacionesDesdeBaseDeDatos()
        {
            Participaciones = new ObservableCollection<Participa>();
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT Nombre_Jugador, Elo_Conseguido, Rendimiento, Puntos, Posicion FROM Participa WHERE Nombre_Evento = '{Nombre}'", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Participaciones.Add(new Participa(new Jugador(reader.GetString(0)), this, reader.IsDBNull(1) ? null : (int?)reader.GetInt32(1), reader.IsDBNull(2) ? null :
                                (int?)reader.GetInt32(2), reader.IsDBNull(3) ? null : (int?)reader.GetDecimal(3), reader.IsDBNull(4) ? null : (int?)reader.GetInt32(4)));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene todos los eventos de la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public static List<Evento> ObtenerEventosDesdeBaseDeDatos()
        {
            List<Evento> eventos = new List<Evento>();
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT * FROM Evento", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            eventos.Add(new Evento(reader.GetString(0), reader.GetString(1), reader.IsDBNull(2) ? null : reader.GetString(2), reader.IsDBNull(3) ? null : reader.GetString(3)));
                        }
                    }
                }
            }
            return eventos;
        }

        /// <summary>
        /// Determina si un nombre ya está siendo utilizado en la base de datos.
        /// </summary>
        /// <param name="nombre">Nombre que se quiere saber si está siendo utilizado.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public static bool NombreUsadoEnBaseDeDatos(string nombre)
        {
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT Nombre FROM Evento WHERE Nombre = '{nombre}'", conexion))
                {
                    return select.ExecuteScalar() != null;
                }
            }
        }
        #endregion
    }
}
