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
    /// Representa una apertura de ajedrez.
    /// </summary>
    public class Apertura : IDataErrorInfo, INotifyPropertyChanged, ICloneable
    {
        #region Atributos
        /// <summary>
        /// Identificador de la apertura en la base de datos.
        /// </summary>
        private int? id;

        /// <summary>
        /// Nombre de la apertura
        /// </summary>
        private string nombre;

        /// <summary>
        /// Designación de la apertura en Encyclopedia of Chess Opennings.
        /// </summary>
        private string eco;

        /// <summary>
        /// Designación de la apertura en New In Chess.
        /// </summary>
        private string nic;

        /// <summary>
        /// Apertura de la cual es variante.
        /// </summary>
        private Apertura variante;

        /// <summary>
        /// Partidas que se jugaron con esta apertura o una variante de esta apertura.
        /// </summary>
        private ListaPartidas partidas;

        /// <summary>
        /// Descripción de cada posición que pertenece a la apertura.
        /// </summary>
        private ObservableCollection<string> posiciones = new ObservableCollection<string>();
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o establece el identificador de la apertura en la base de datos.
        /// </summary>
        public int? Id
        {
            get => id;
            set
            {
                id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre de la apertura.
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
        /// Obtiene o establece la designación de la apertura en Encyclopedia of Chess Opennings.
        /// </summary>
        public string ECO
        {
            get => eco;
            set
            {
                eco = string.IsNullOrWhiteSpace(value) ? null : value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ECO)));
            }
        }

        /// <summary>
        /// Obtiene o establece la designación de la apertura en New In Chess.
        /// </summary>
        public string NIC
        {
            get => nic;
            set
            {
                nic = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NIC)));
            }
        }

        /// <summary>
        /// Obtiene o establece la apertura de la cual es variante.
        /// </summary>
        public Apertura Variante
        {
            get => variante;
            set
            {
                variante = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Variante)));
            }
        }

        /// <summary>
        /// Obtiene o establece la descripción de cada posición que pertenece a la apertura.
        /// </summary>
        public ObservableCollection<string> Posiciones
        {
            get => posiciones;
            set
            {
                posiciones = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Posiciones)));
            }
        }

        /// <summary>
        /// Obtiene o establece las partidas en las que se jugó esta apertura.
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
                    case nameof(ECO):
                        return ECO == null || ECO.Length == 3 && ECO[0] >= 'A' && ECO[0] < 'F' && char.IsDigit(ECO[1]) && char.IsDigit(ECO[2]) ? null :
                            "Debe ingresar la letra del volúmen (A, B, C, D o E) seguida de 2 dígitos";
                    default:
                        return null;
                }
            }
        }
        #endregion

        #region Constructores
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Apertura"/>.
        /// </summary>
        public Apertura() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Apertura"/> con el id, el nombre, los códigos eco y nic, la apertura de la que es variate y las posiciones de la apertura.
        /// </summary>
        /// <param name="id">Identificador de la apertura en la base de datos.</param>
        /// <param name="nombre">Nombre de la apertura.</param>
        /// <param name="eCO">Designación de la apertura en Encyclopedia of Chess Opennings.</param>
        /// <param name="nIC">Designación de la apertura en New In Chess.</param>
        /// <param name="variante">Apertura de la cual es variante.</param>
        /// <param name="posiciones">Descripción de cada posición que pertenece a la apertura.</param>
        public Apertura(int? id, string nombre, string eCO, string nIC, Apertura variante, ObservableCollection<string> posiciones) : this(nombre, eCO, nIC, variante, posiciones) => Id = id;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Apertura"/> con el nombre, los códigos eco y nic, la apertura de la que es variate y las posiciones de la apertura.
        /// </summary>
        /// <param name="nombre">Nombre de la apertura.</param>
        /// <param name="eCO">Designación de la apertura en Encyclopedia of Chess Opennings.</param>
        /// <param name="nIC">Designación de la apertura en New In Chess.</param>
        /// <param name="variante">pertura de la cual es variante.</param>
        /// /// <param name="posiciones">Descripción de cada posición que pertenece a la apertura.</param>
        public Apertura(string nombre, string eCO, string nIC, Apertura variante, ObservableCollection<string> posiciones)
        {
            Nombre = nombre;
            ECO = eCO;
            NIC = nIC;
            Variante = variante;
            Posiciones = posiciones;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Apertura"/> con el id, el nombre, los códigos eco y nic, y la apertura de la que es variate.
        /// </summary>
        /// <param name="id">Identificador de la apertura en la base de datos.</param>
        /// <param name="nombre">Nombre de la apertura.</param>
        /// <param name="eCO">Designación de la apertura en Encyclopedia of Chess Opennings.</param>
        /// <param name="nIC">Designación de la apertura en New In Chess.</param>
        /// <param name="variante">pertura de la cual es variante.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public Apertura(int? id, string nombre, string eCO, string nIC, Apertura variante) : this(id, nombre, eCO, nIC, variante, new ObservableCollection<string>()) => CompletarPosiciones();

        /// <summary>
        /// Inicializa una nueva instancia de la clse <see cref="Apertura"/> con la información de la base de datos.
        /// </summary>
        /// <param name="id">Identificador de la apertura en la base de datos.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public Apertura(int id)
        {
            Id = id;
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT * FROM Apertura WHERE Id = {Id}", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        reader.Read();
                        Nombre = reader.GetString(1);
                        ECO = reader.IsDBNull(2) ? null : reader.GetString(2);
                        NIC = reader.IsDBNull(3) ? null : reader.GetString(3);
                        Variante = reader.IsDBNull(4) ? null : new Apertura(reader.GetInt32(4));
                    }
                }
            }
            CompletarPosiciones();
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Inserta la apertura en la base de datos.
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
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand insert = new SqlCommand($"INSERT INTO Apertura OUTPUT INSERTED.Id VALUES ('{Nombre}', {ECO.ToSQL()}, {NIC.ToSQL()}, {Variante?.Id.ToString() ?? "null"})", conexion))
                {
                    Id = (int)insert.ExecuteScalar();
                }
            }
            foreach (string posicion in Posiciones)
            {
                new Posicion($"{posicion} 0 1").InsertarEnBaseDeDatos(this);
            }
        }

        public object Clone() => Id != null ? new Apertura(Id, Nombre, ECO, NIC, (Apertura)Variante?.Clone(), new ObservableCollection<string>(Posiciones)) : null;

        /// <summary>
        /// Obtiene los valores para <see cref="Partidas"/> según la base de datos.
        /// </summary>
        /// <returns>Lista de partidas que se jugaron con esta apertura o alguna de sus variantes.</returns>
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
                // Agrega las partidas de esta apertura.
                using (SqlCommand select = new SqlCommand($"SELECT c.Id_Partida, c.Id_Apertura FROM (SELECT t.Id_Partida, t.Numero, MAX(t.Numero) OVER (PARTITION BY t.Id_Partida) AS NumeroMaximo, " +
                    $"p.Id_Apertura FROM Tiene AS t JOIN Posicion AS p ON t.Descripcion_Posicion = p.Descripcion WHERE p.Id_Apertura IS NOT NULL) AS c WHERE c.Numero = c.NumeroMaximo AND " +
                    $"c.Id_Apertura = {Id}", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaPartidas.Add(new Partida(reader.GetInt32(0)));
                        }
                    }
                }
                // Agrega las partidas de las variantes de esta apertura y sus variantes.
                using (SqlCommand select = new SqlCommand($"SELECT Id FROM Apertura WHERE Id_Variante = {Id}", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Apertura apertura = new Apertura(reader.GetInt32(0));
                            listaPartidas.AddRange(apertura.ObtenerPartidasDesdeBaseDeDatos());
                        }
                    }
                }
            }
            return listaPartidas;
        }

        /// <summary>
        /// Obtiene los valores para <see cref="Posiciones"/> según la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>"
        private void CompletarPosiciones()
        {
            if (Id != null)
            {
                using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
                {
                    conexion.Open();
                    using (SqlCommand select = new SqlCommand($"SELECT Descripcion FROM Posicion WHERE Id_Apertura = {Id}", conexion))
                    {
                        using (SqlDataReader reader = select.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Posiciones.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Borra la apertura de la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public void BorraDeBaseDeDatos()
        {
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand update = new SqlCommand($"UPDATE Apertura SET Id_Variante = null WHERE Id_Variante = {Id}", conexion))
                {
                    update.ExecuteNonQuery();
                }
                using (SqlCommand delete = new SqlCommand($"DELETE FROM Apertura WHERE Id = {Id}", conexion))
                {
                    delete.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Obtiene todas las aperturas de la base de datos.
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public static List<Apertura> ObtenerAperturasDesdeBaseDeDatos()
        {
            List<Apertura> aperturas = new List<Apertura>();
            using (SqlConnection conexion = new SqlConnection(Settings.Default.AjedrezConnectionString))
            {
                conexion.Open();
                using (SqlCommand select = new SqlCommand($"SELECT * FROM Apertura", conexion))
                {
                    using (SqlDataReader reader = select.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            aperturas.Add(new Apertura(reader.GetInt32(0), reader.GetString(1), reader.IsDBNull(2) ? null : reader.GetString(2), reader.IsDBNull(3) ? null : reader.GetString(3),
                                reader.IsDBNull(4) ? null : new Apertura(reader.GetInt32(4))));
                        }
                    }
                }
            }
            return aperturas;
        }
        #endregion
    }
}
