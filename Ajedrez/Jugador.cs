using Ajedrez.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Ajedrez
{
    /// <summary>
    /// Representa un jugador de ajedrez.
    /// </summary>
    public class Jugador : IDataErrorInfo, INotifyPropertyChanged, ICloneable
    {
        #region Atributos
        /// <summary>
        /// Nombre del jugador.
        /// </summary>
        private string nombre;

        /// <summary>
        /// Dirección de red o correo electrónico del jugador.
        /// </summary>
        private string na;

        /// <summary>
        /// Tipo de jugador
        /// </summary>
        /// <remarks> "human" o "program".</remarks>
        private string tipo;

        /// <summary>
        /// Elo FIDE del jugador.
        /// </summary>
        private int? elo;

        /// <summary>
        /// Título FIDE de jugador.
        /// </summary>
        private string titulo;

        /// <summary>
        /// Partidas que jugó este jugador.
        /// </summary>
        private ListaPartidas partidas;
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece el nombre del jugador.
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
        /// Obtiene o establece la dirección de red o correo electrónico del jugador.
        /// </summary>
        public string Na
        {
            get => na;
            set
            {
                na = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Na)));
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de jugador.
        /// </summary>
        /// <remarks> "human" o "program".</remarks>
        public string Tipo
        {
            get => tipo;
            set
            {
                tipo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tipo)));
            }
        }

        /// <summary>
        /// Obtiene o establece el elo FIDE del jugador.
        /// </summary>
        public int? Elo
        {
            get => elo;
            set
            {
                elo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Elo)));
            }
        }

        /// <summary>
        /// Obtiene o establece el título FIDE de jugador.
        /// </summary>
        public string Titulo
        {
            get => titulo;
            set
            {
                titulo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Titulo)));
            }
        }

        /// <summary>
        /// Obtiene o establece las partidas que jugó este jugador.
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
                    case nameof(Elo):
                        return Elo < 0 ? "Debe ingresar un número positivo" : null;
                    default:
                        return null;
                }
            }
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Jugador"/> con el nombre, el nam el tipo el elo y el título.
        /// </summary>
        /// <param name="nombre">Nombre del jugador.</param>
        /// <param name="na">Dirección de red o correo electrónico del jugador.</param>
        /// <param name="tipo">Tipo de jugador (human o program).</param>
        /// <param name="elo">Elo FIDE de jugador.</param>
        /// <param name="titulo">Título FIDE del jugador.</param>
        public Jugador(string nombre, string na, string tipo, int? elo, string titulo)
        {
            Nombre = nombre;
            Na = na;
            Tipo = tipo;
            Elo = elo;
            Titulo = titulo;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Jugador"/>.
        /// </summary>
        public Jugador() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Jugador"/> con la información de la base de datos.
        /// </summary>
        /// <param name="nombre">Nombre del jugador en la base de datos.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public Jugador(string nombre)
        {
            Nombre = nombre;
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT Na, Tipo, Elo, Titulo FROM Jugador WHERE Nombre = '{Nombre}'", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Na = reader.IsDBNull(0) ? null : reader.GetString(0);
                            Tipo = reader.IsDBNull(1) ? null : reader.GetString(1);
                            Elo = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2);
                            Titulo = reader.IsDBNull(3) ? null : reader.GetString(3);
                        }
                    }
                }
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Inserta el jugador en la base de datos, si ya existe modifica Na y Tipo.
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
                using (SqlCommand select = new SqlCommand($"SELECT Nombre FROM Jugador WHERE Nombre = '{Nombre}'", conexion))
                {
                    if (select.ExecuteScalar() == null)
                    {
                        using (SqlCommand insert = new SqlCommand($"INSERT INTO Jugador VALUES ('{Nombre}', {Na.ToSQL()}, {Tipo.ToSQL()}, {Elo.ToSQL()}, {Titulo.ToSQL()})", conexion))
                        {
                            insert.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (SqlCommand update = new SqlCommand($"UPDATE Jugador SET Na = {Na.ToSQL()}, Tipo = {Tipo.ToSQL()} WHERE Nombre = '{Nombre}'", conexion))
                        {
                            update.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene los valores para <see cref="Partidas"/> según la base de datos.
        /// </summary>
        /// <returns>Lista de partidas que jugó este jugador.</returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public List<Partida> ObtenerPartidasDesdeBaseDeDatos()
        {
            List<Partida> listaPartidas = new List<Partida>();
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT Id_Partida FROM Juega WHERE Nombre_Jugador = '{Nombre}'", conexion))
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
            return listaPartidas;
        }

        public object Clone() => new Jugador(Nombre, Na, Tipo, Elo, Titulo);

        /// <summary>
        /// Obtiene todos los jugadores de la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public static List<Jugador> ObtenerJugadoresDesdeBaseDeDatos()
        {
            List<Jugador> jugadores = new List<Jugador>();
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT * FROM Jugador", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jugadores.Add(new Jugador(reader.GetString(0), reader.IsDBNull(1) ? null : reader.GetString(1), reader.IsDBNull(2) ? null : reader.GetString(2), reader.IsDBNull(3) ? null
                                : (int?)reader.GetInt32(3), reader.IsDBNull(4) ? null : reader.GetString(4)));
                        }
                    }
                }
            }
            return jugadores;
        }

        public override string ToString() => $"{Titulo} {Nombre} {Elo}";

        /// <summary>
        /// Invoca <see cref="PropertyChanged"/>.
        /// </summary>
        /// <param name="nombrePropiedad">Nombre de la propiedad modificada.</param>
        protected void PropiedadModificada([CallerMemberName] string nombrePropiedad = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));
        #endregion
    }
}
