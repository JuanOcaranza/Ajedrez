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
    /// Representa la explicación de una posición.
    /// </summary>
    public class Explicacion : IDataErrorInfo, ICloneable
    {
        #region Propiedades
        /// <summary>
        /// Obtiene o establece el identificador de la explicación en la base de datos.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Obtiene o establece la explicación de la posición.
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del autor de la explicación.
        /// </summary>
        public string Autor { get; set; }

        /// <summary>
        /// Obtiene o establece la descripcion de la posición que se está explicando.
        /// </summary>
        /// <remarks>Similar al FEN pero sin las cantidad de jugadas</remarks>
        public string DescripcionPosicion { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la explicación
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece las explicaciones que tienen que ver con esta explicación.
        /// </summary>
        public ObservableCollection<Explicacion> ExplicacionesRelacionadas { get; set; }

        public string Error => null;
        #endregion

        #region Indexer
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Descripcion):
                        return string.IsNullOrWhiteSpace(Descripcion) ? "Debe ingresar una explicación" : null;
                    case nameof(DescripcionPosicion):
                        try
                        {
                            return string.IsNullOrWhiteSpace(DescripcionPosicion) ? "Debe ingresar un descripción de la posición" : (Posicion.EsDescripcionValida(DescripcionPosicion) ? null :
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

        #region Constructores
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Explicacion"/> con el id, la descripcion, el autor, la descripción de la posición y las explicaciones relacionadas.
        /// </summary>
        /// <param name="id">Identificador de la explicación en la base de datos.</param>
        /// <param name="descripcion">Explicación de la posición.</param>
        /// <param name="autor">Nombre del autor de la explicación.</param>
        /// <param name="descripcionPosicion">Descripcion de la posición que se está explicando (similar al FEN pero sin las cantidades de jugadas).</param>
        /// <param name="nombre">Nombre de la explicación.</param>
        /// <param name="explicacionesRelacionadas">Explicaciones que tienen que ver con esta explicación.</param>
        public Explicacion(int? id, string descripcion, string autor, string descripcionPosicion, string nombre, ObservableCollection<Explicacion> explicacionesRelacionadas) :
            this(descripcion, autor, descripcionPosicion, nombre, explicacionesRelacionadas) => Id = id;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Explicacion"/> con el id, la descripcion, el autor y la descripción de la posición.
        /// </summary>
        /// <param name="id">Identificador de la explicación en la base de datos.</param>
        /// <param name="descripcion">Explicación de la posición.</param>
        /// <param name="autor">Nombre del autor de la explicación.</param>
        /// <param name="descripcionPosicion">Descripcion de la posición que se está explicando (similar al FEN pero sin las cantidades de jugadas).</param>
        /// <param name="nombre">Nombre de la explicación.</param>
        public Explicacion(int? id, string descripcion, string autor, string descripcionPosicion, string nombre) : this(id, descripcion, autor, descripcionPosicion, nombre, new
            ObservableCollection<Explicacion>()) { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Explicacion"/> con la información en la base de datos.
        /// </summary>
        /// <param name="id">Identificador de la explicacción en la base de datos.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public Explicacion(int id) : this()
        {
            Id = id;
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT * FROM Explicacion WHERE Id = {Id}", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        reader.Read();
                        Descripcion = reader.GetString(1);
                        Autor = reader.IsDBNull(2) ? null : reader.GetString(2);
                        DescripcionPosicion = new Posicion($"{reader.GetString(3)} 0 1").Descripcion;
                        Nombre = reader.IsDBNull(4) ? null : reader.GetString(4);
                    }
                }
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Explicacion"/> con el id, la descripcion, el autor, la descripción de la posición y las explicaciones relacionadas.
        /// </summary>
        /// <param name="id">Identificador de la explicación en la base de datos.</param>
        /// <param name="descripcion">Explicación de la posición.</param>
        /// <param name="autor">Nombre del autor de la explicación.</param>
        /// <param name="descripcionPosicion">Descripcion de la posición que se está explicando (similar al FEN pero sin las cantidades de jugadas).</param>
        /// <param name="nombre">Nombre de la explicación.</param>
        /// <param name="explicacionesRelacionadas">Explicaciones que tienen que ver con esta explicación.</param>
        public Explicacion(string descripcion, string autor, string descripcionPosicion, string nombre, ObservableCollection<Explicacion> explicacionesRelacionadas)
        {
            Descripcion = descripcion;
            Autor = autor;
            DescripcionPosicion = descripcionPosicion;
            Nombre = nombre;
            ExplicacionesRelacionadas = explicacionesRelacionadas;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Explicacion"/>.
        /// </summary>
        public Explicacion()
        {
            ExplicacionesRelacionadas = new ObservableCollection<Explicacion>();
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Establece <see cref="ExplicacionesRelacionadas"/> según la información en la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public void EstablecerExplicacionesRelacionadasDesdeLaBaseDeDatos()
        {
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT * FROM Tiene_Que_Ver WHERE {Id} IN (Id1, Id2)", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ExplicacionesRelacionadas.Add(new Explicacion(reader.GetInt32(0) == Id ? reader.GetInt32(1) : reader.GetInt32(0)));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inserta la explicación en la base de datos.
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
                using (SqlCommand insert = new SqlCommand($"INSERT INTO Explicacion OUTPUT INSERTED.Id VALUES ('{Descripcion}', {Autor.ToSQL()}, '{DescripcionPosicion}', {Nombre.ToSQL()})", conexion))
                {
                    Id = (int)insert.ExecuteScalar();
                }
                foreach (Explicacion explicacionRelacionada in ExplicacionesRelacionadas)
                {
                    using (SqlCommand insert = new SqlCommand($"INSERT INTO Tiene_Que_Ver VALUES ('{Id}', '{explicacionRelacionada.Id}')", conexion))
                    {
                        insert.ExecuteNonQuery();
                    }
                }
            }
        }

        public object Clone() => Id != null ? new Explicacion(Id, Descripcion, Autor, DescripcionPosicion, Nombre, new ObservableCollection<Explicacion>(ExplicacionesRelacionadas)) : null;

        public override string ToString() => $"{Nombre} ({DescripcionPosicion}) creada por {Autor}";

        /// <summary>
        /// Obtiene todas las explicaciones de la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public static List<Explicacion> ObtenerExplicacionesDesdeBaseDeDatos()
        {
            List<Explicacion> explicaciones = new List<Explicacion>();
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand("SELECT * FROM Explicacion", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Explicacion explicacion = new Explicacion(reader.GetInt32(0), reader.GetString(1), reader.IsDBNull(2) ? null : reader.GetString(2), reader.GetString(3), reader.IsDBNull(4) ?
                                null : reader.GetString(4));
                            explicaciones.Add(explicacion);
                        }
                    }
                }
            }
            return explicaciones;
        }
        #endregion
    }
}
