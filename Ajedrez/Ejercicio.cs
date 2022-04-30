using Ajedrez.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace Ajedrez
{
    /// <summary>
    /// Representa un ejercicio de ajedrez.
    /// </summary>
    public class Ejercicio : IDataErrorInfo, INotifyPropertyChanged, ICloneable
    {
        #region Atributos
        /// <summary>
        /// Descripción de la posicion donde hay que realizar la solucion (Similar al FEN pero sin las cantidades de jugadas).
        /// </summary>
        private string descripcionPosicion;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece el identificador del ejercicio en la base de datos.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Obtiene o establece la dificultad del ejercicio en elo.
        /// </summary>
        public int? Dificultad { get; set; }

        /// <summary>
        /// Obtiene o establece la cadena que representa la jugada correcta en la posición.
        /// </summary>
        public string Solucion { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la posicion donde hay que realizar la solucion (Similar al FEN pero sin las cantidades de jugadas).
        /// </summary>
        public string DescripcionPosicion
        {
            get => descripcionPosicion;
            set
            {
                descripcionPosicion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Solucion))); // Hace que se vuelva a comprobrar la jugada en caso de que cambie la posición.
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
                    case nameof(Solucion):
                        if (DescripcionPosicion != null && this[DescripcionPosicion] == null) // Si no hay un error en la posición
                        {
                            Posicion posicion = new Posicion($"{DescripcionPosicion} 0 1");
                            return posicion.JugadasPosibles.Exists(j => j.ToString(posicion) == Solucion) ? null : "Debe ser una jugada válida en la posición";
                        }
                        return "Es necesaria una posición válida para validar la jugada";
                    case nameof(DescripcionPosicion):
                        try
                        {
                            return string.IsNullOrEmpty(DescripcionPosicion) ? "Debe ingresar una descripción de la posición" : (Posicion.EsDescripcionValida(DescripcionPosicion) ? null :
                            "Valor inválido consulte el manual de usuario para más información");
                        }
                        catch
                        {
                            return "Valor inválido, no se puede comprobar, consulte el manual de usuario";
                        }
                    default:
                        return null;
                }
            }
        }
        #endregion

        #region Evento
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructores
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Ejercicio"/>
        /// </summary>
        public Ejercicio() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Ejercicio"/> con la dificultad, la solución y la posición.
        /// </summary>
        /// <param name="dificultad">Dificultad del ejercicio en elo.</param>
        /// <param name="solucion">Representa la jugada correcta en la posición.</param>
        /// <param name="posicion">Descripción de la posicion donde hay que realizar la solucion (Similar al FEN pero sin las cantidades de jugadas).</param>
        public Ejercicio(int? dificultad, string solucion, string posicion)
        {
            Dificultad = dificultad;
            Solucion = solucion;
            DescripcionPosicion = posicion;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Ejercicio"/> con la información en la base de datos.
        /// </summary>
        /// <param name="id">Identificador del ejercicio en la base de datos.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception> 
        public Ejercicio(int id)
        {
            Id = id;
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT * FROM Ejercicio WHERE Id = {Id}", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        reader.Read();
                        Dificultad = reader.IsDBNull(1) ? null : (int?)reader.GetInt32(1);
                        Solucion = reader.GetString(2);
                        DescripcionPosicion = new Posicion($"{reader.GetString(3)} 0 1").Descripcion;
                    }
                }
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Ejercicio"/> con el Id, la dificultad, la solución y la posición.
        /// </summary>
        /// <param name="dificultad">Dificultad del ejercicio en elo.</param>
        /// <param name="solucion">Representa la jugada correcta en la posición.</param>
        /// <param name="posicion">Descripción de la posicion donde hay que realizar la solucion (Similar al FEN pero sin las cantidades de jugadas).</param>
        public Ejercicio(int? id, int? dificultad, string solucion, string posicion) : this(dificultad, solucion, posicion) => Id = id;
        #endregion

        #region Métodos
        /// <summary>
        /// Inserta el <see cref="Ejercicio"/> en la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public void InsertarEnBaseDeDatos()
        {
            new Posicion($"{DescripcionPosicion} 0 1").InsertarEnBaseDeDatos();
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand insert = new SqlCommand($"INSERT INTO Ejercicio OUTPUT INSERTED.Id VALUES ({Dificultad.ToSQL()}, '{Solucion}', '{DescripcionPosicion}')", conexion))
                {
                    Id = (int)insert.ExecuteScalar();
                }
            }
        }

        public object Clone() => Id != null ? new Ejercicio(Id, Dificultad, Solucion, DescripcionPosicion) : null;

        /// <summary>
        /// Obtiene todos los ejercicios de la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public static List<Ejercicio> ObtenerEjerciciosDesdeBaseDeDatos()
        {
            List<Ejercicio> ejercicios = new List<Ejercicio>();
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand("SELECT * FROM Ejercicio", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ejercicios.Add(new Ejercicio(reader.GetInt32(0), reader.IsDBNull(1) ? null : (int?)reader.GetInt32(1), reader.GetString(2), reader.GetString(3)));
                        }
                    }
                }
            }
            return ejercicios;
        }
        #endregion
    }
}
