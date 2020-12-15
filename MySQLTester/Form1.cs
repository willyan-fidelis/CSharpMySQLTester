using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySQLTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)

        {
            try
            {
                using (MySqlConnection cnn = new MySqlConnection())
                {
                    cnn.ConnectionString = "server=localhost;database=mt_public_db;uid=root;pwd=myroot;port=3306";
                    cnn.Open();
                    MessageBox.Show("Conetado!");

                    var city = new MySQLCityCoveredRepository(cnn);

                    var elem = city.Get(2);
                    var list = city.Get();
                    
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro!" + ex.Message);
            }
        }
    }

    public class CityCovered
    {
        public long OID { get; set; }
        public string name { get; set; }
        public string nickname { get; set; }
    }
    public interface ICityCoveredRepository
    {
        CityCovered Get(int id);
        List<CityCovered> Get();
    }

    public class MySQLCityCoveredRepository : ICityCoveredRepository
    {
        private MySqlConnection _connection;
        public MySQLCityCoveredRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public CityCovered Get(int id)
        {
            //using (var conn = new MySqlConnection(_connection))
            using (var cmd = _connection.CreateCommand())
            {
                //_connection.Open();
                cmd.CommandText = "SELECT name,nickname FROM city_covered WHERE OID = @id";
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new CityCovered
                    {
                        OID = id,
                        name = reader.GetString(reader.GetOrdinal("name")),
                        nickname = reader.GetString(reader.GetOrdinal("nickname"))
                    };
                }
            }
        }
        public List<CityCovered> Get()
        {
            //using (var conn = new MySqlConnection(_connection))
            using (var cmd = _connection.CreateCommand())
            {
                //_connection.Open();
                cmd.CommandText = "SELECT OID,name,nickname FROM city_covered";
                //cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    List<CityCovered> cities = new List<CityCovered>();
                    while (reader.Read())
                    {
                        var elem = new CityCovered
                        {
                            OID = reader.GetInt64(reader.GetOrdinal("OID")),
                            name = reader.GetString(reader.GetOrdinal("name")),
                            nickname = reader.GetString(reader.GetOrdinal("nickname"))
                        };
                        cities.Add(elem);
                    }
                    return cities;
                }
            }
        }
    }
}
