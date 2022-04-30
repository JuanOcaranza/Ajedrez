using Ajedrez.Properties;
using System;
using System.Data.SqlClient;
using System.IO;

namespace Ajedrez
{
    /// <summary>
    /// Representa la relación entre <see cref="Partida"/> y <see cref="Posicion"/>.
    /// </summary>
    public class Tiene
    {
        #region Propiedades
        /// <summary>
        /// Obtiene o establece la descripción de la posición.
        /// </summary>
        /// <remarks>similar al FEN pero sin las cantidades de jugadas.</remarks>
        public string DescripcionPosicion { get; set; }

        /// <summary>
        /// Obtiene o establece el id de la partida que tiene la posición
        /// </summary>
        public int IdPartida { get; set; }

        /// <summary>
        /// Obtiene o establece el número de la posición en la partida
        /// </summary>
        /// <remarks>-1 es la posición inicial</remarks>
        public int Numero { get; set; }

        /// <summary>
        /// Obtiene o establece la jugada que se realiza en la posición
        /// </summary>
        /// <remarks>Con notación algebraica en inglés, null si es la posición final</remarks>
        public string Jugada { get; set; }

        /// <summary>
        /// Obtiene o establece el NAG de la jugada
        /// </summary>
        public int? NAG { get; set; }

        /// <summary>
        /// Obtiene o establece el comentario sobre la jugada.
        /// </summary>
        public string Comentario { get; set; }
        #endregion

        #region Constructores
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Tiene"/> con la descripción de la posición, el id de la partida, el número de posición, la jugada, el NAG, y el comentario.
        /// </summary>
        /// <param name="descripcionPosicion">Descripción de la posición, similar al FEN pero sin las cantidades de jugadas.</param>
        /// <param name="idPartida">Id de la partida que tiene la posición.</param>
        /// <param name="numero">Número de la posición en la partida (-1 es la inicial).</param>
        /// <param name="jugada">Jugada que se realiza en la posición (Notación algebraica en inglés), null si es la útlima posición.</param>
        /// <param name="nAG">NAG de la jugada.</param>
        /// <param name="comentario">Comentario sobre la jugada.</param>
        public Tiene(string descripcionPosicion, int idPartida, int numero, string jugada, int? nAG, string comentario) : this(descripcionPosicion, idPartida, numero)
        {
            InicializarJugada(jugada, nAG, comentario);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Tiene"/> con la descripción de la posición, el id de la partida y el número de posición.
        /// </summary>
        /// <param name="descripcionPosicion">Descripción de la posición, similar al FEN pero sin las cantidades de jugadas.</param>
        /// <param name="idPartida">Id de la partida que tiene la posición.</param>
        /// <param name="numero">Número de la posición en la partida (-1 es la inicial).</param>
        public Tiene(string descripcionPosicion, int idPartida, int numero)
        {
            DescripcionPosicion = descripcionPosicion;
            IdPartida = idPartida;
            Numero = numero;
        }
        #endregion

        #region Método
        /// <summary>
        /// Inserta el tiene en la base de datos, si ya existe lo actualiza,
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
                using (SqlCommand select = new SqlCommand($"SELECT 1 FROM Tiene WHERE Id_Partida = {IdPartida} AND Numero = {Numero}", conexion))
                {
                    if (select.ExecuteScalar() == null)
                    {
                        using (SqlCommand insert = new SqlCommand($"INSERT INTO Tiene VALUES ('{DescripcionPosicion}', {IdPartida}, {Numero}, {Jugada.ToSQL()}, {NAG.ToSQL()}, {Comentario.ToSQL()})",
                            conexion))
                        {
                            insert.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (SqlCommand update = new SqlCommand($"UPDATE Tiene SET Descripcion_Posicion = '{DescripcionPosicion}', Jugada = {Jugada.ToSQL()}, Nag = {NAG.ToSQL()}, Comentario = " +
                            $"{Comentario.ToSQL()} WHERE Id_Partida = {IdPartida} AND Numero = {Numero}", conexion))
                        {
                            update.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inicializa la jugada con su NAG y Comentario.
        /// </summary>
        /// <param name="jugada">Jugada que se realiza en la posición (Notación algebraica en inglés).</param>
        /// <param name="nAG">NAG de la jugada.</param>
        /// <param name="comentario">Comentario sobre la jugada.</param>
        public void InicializarJugada(string jugada, int? nAG, string comentario)
        {
            Jugada = jugada;
            NAG = nAG;
            Comentario = comentario;
        }
        #endregion
    }
}
