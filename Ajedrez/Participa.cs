using Ajedrez.Properties;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;

namespace Ajedrez
{
    /// <summary>
    /// Representa la realación entre un jugador y un evento.
    /// </summary>
    public class Participa : IDataErrorInfo, ICloneable
    {
        #region Propiedades
        /// <summary>
        /// Obtiene o establece el jugador que participa en el evento.
        /// </summary>
        public Jugador Jugador { get; set; }

        /// <summary>
        /// Obtiene o establece el evento en el que participa el jugador.
        /// </summary>
        public Evento Evento { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de elo ganado o perdido por el jugador en el evento.
        /// </summary>
        public int? EloConseguido { get; set; }

        /// <summary>
        /// Obtiene o establece el rendimiento del jugador en el evento.
        /// </summary>
        public int? Rendimiento { get; set; }

        /// <summary>
        /// Obtiene o establece los puntos realizados por el jugador en el evento.
        /// </summary>
        public decimal? Puntos { get; set; }

        /// <summary>
        /// Obtiene o establece la posición del jugador en el evento.
        /// </summary>
        public int? Posicion { get; set; }

        public string Error => null;
        #endregion

        #region Indexer
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Rendimiento):
                        return (Rendimiento ?? 1) >= 0 ? null : "El rendimiento no puede ser negativo";
                    case nameof(Puntos):
                        return (Puntos ?? 1) >= 0 ? null : "Los puntos no pueden ser negativos";
                    case nameof(Posicion):
                        return (Posicion ?? 1) > 0  ? null : "La posición no puede ser negativa ni cero";
                    default:
                        return null;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Participa"/> con el jugador, el evento, el elo conseguido, el rendimiento, los puntos y la posición.
        /// </summary>
        /// <param name="jugador">Jugador que participa en el evento.</param>
        /// <param name="evento">Evento en el que participa el jugador.</param>
        /// <param name="eloConseguido">Cantidad de elo ganado o perdido por el jugador en el evento.</param>
        /// <param name="rendimiento">Rendimiento del jugador en el evento.</param>
        /// <param name="puntos">Puntos realizados por el jugador en el evento.</param>
        /// <param name="posicion">Posición del jugador en el evento.</param>
        public Participa(Jugador jugador, Evento evento, int? eloConseguido, int? rendimiento, decimal? puntos, int? posicion)
        {
            Jugador = jugador;
            Evento = evento;
            EloConseguido = eloConseguido;
            Rendimiento = rendimiento;
            Puntos = puntos;
            Posicion = posicion;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Participa"/> con la información en la base de datos.
        /// </summary>
        /// <param name="jugador">Jugador que participa en el evento.</param>
        /// <param name="evento">Evento en el que participa el jugador.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public Participa(Jugador jugador, Evento evento)
        {
            Jugador = jugador;
            Evento = evento;
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT Elo_Conseguido, Rendimiento, Puntos, Posicion FROM Participa WHERE Nombre_Jugador = '{Jugador.Nombre}' AND Nombre_Evento = " +
                    $"'{Evento.Nombre}'", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            EloConseguido = reader.IsDBNull(0) ? null : (int?)reader.GetInt32(0);
                            Rendimiento = reader.IsDBNull(1) ? null : (int?)reader.GetInt32(1);
                            Puntos = reader.IsDBNull(2) ? null : (decimal?)reader.GetDecimal(2);
                            Posicion = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3);
                        }
                    }
                }
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Inserta la paticipación actual en la base de datos
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
                using (SqlCommand select = new SqlCommand($"SELECT 1 FROM Participa WHERE Nombre_Jugador = '{Jugador.Nombre}' AND Nombre_Evento = '{Evento.Nombre}'", conexion))
                {
                    using (SqlCommand command = new SqlCommand(select.ExecuteScalar() == null ? $"INSERT INTO Participa VALUES ('{Jugador.Nombre}', '{Evento.Nombre}', {EloConseguido.ToSQL()}, " +
                        $"{Rendimiento.ToSQL()}, {Puntos.ToSQL()}, {Posicion.ToSQL()})" : $"UPDATE Participa SET Elo_Conseguido = {EloConseguido.ToSQL()}, Rendimiento = {Rendimiento.ToSQL()}, " +
                        $"Puntos = {Puntos.ToSQL()}, Posicion = {Posicion.ToSQL()} WHERE Nombre_Jugador = '{Jugador.Nombre}' AND Nombre_Evento = '{Evento.Nombre}'", conexion))
                    {
                        command.ExecuteNonQuery(); // Inserta o actualiza según corresponda.
                    }
                }
            }
        }

        /// <summary>
        /// Borra la participación de la base de datos
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public void BorrarDeBaseDeDatos()
        {
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand delete = new SqlCommand($"DELETE FROM Participa WHERE Nombre_Jugador = '{Jugador.Nombre}' AND Nombre_Evento = '{Evento.Nombre}'", conexion))
                {
                    delete.ExecuteNonQuery();
                }
            }
        }

        public object Clone() => new Participa((Jugador)Jugador.Clone(), (Evento)Evento.Clone(), EloConseguido, Rendimiento, Puntos, Posicion);

        public override string ToString() => $"{Jugador.Titulo} {Jugador.Nombre}";
        #endregion
    }
}
