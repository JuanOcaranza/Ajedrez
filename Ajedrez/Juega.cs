using Ajedrez.Properties;
using System;
using System.Data.SqlClient;
using System.IO;

namespace Ajedrez
{
    /// <summary>
    /// Representa una relación entre <see cref="Partida"/> y <see cref="Jugador"/>.
    /// </summary>
    public class Juega
    {
        #region Propiedades
        /// <summary>
        /// Obtiene o establece el Id de la partida que se juega.
        /// </summary>
        public int IdPartida { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del jugador que juega la partida.
        /// </summary>
        public string NombreJugador { get; set; }

        /// <summary>
        /// Obtiene o establece el título FIDE del jugador al momento de jugar la partida.
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Obtiene o establece el elo FIDE del jugador al momento de jugar la partida.
        /// </summary>
        public int? Elo { get; set; }

        /// <summary>
        /// Obtiene o establece el color con el que el jugador juega la partida.
        /// </summary>
        public string Color { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Juega"/> con el id de la partida y el nombre, título, elo y color del jugador.
        /// </summary>
        /// <param name="idPartida">Id de la partida que se juega.</param>
        /// <param name="nombreJugador">Nombre del jugador que juega la partida.</param>
        /// <param name="titulo">Título FIDE del jugador al momento de jugar la partida.</param>
        /// <param name="elo">Elo FIDE del jugador al mommento de jugar la partida.</param>
        /// <param name="color">Color con el que el jugador juega la partida (Blanco o Negro).</param>
        public Juega(int idPartida, string nombreJugador, string titulo, int? elo, string color)
        {
            IdPartida = idPartida;
            NombreJugador = nombreJugador;
            Titulo = titulo;
            Elo = elo;
            Color = color;
        }
        #endregion

        #region Método
        /// <summary>
        /// Inserta el juega en la base de datos.
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
                using (SqlCommand select = new SqlCommand($"SELECT Id_Partida FROM Juega WHERE Id_Partida = {IdPartida} AND Nombre_Jugador = '{NombreJugador}' AND Color = '{Color}'", conexion))
                {
                    if (select.ExecuteScalar() == null)
                    {
                        using (SqlCommand insert = new SqlCommand($"INSERT INTO Juega VALUES ({IdPartida}, '{NombreJugador}', {Titulo.ToSQL()}, {Elo.ToSQL()}, '{Color}')", conexion))
                        {
                            insert.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
