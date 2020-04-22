using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Database
{
    public class AgarioDatabase
    {

        /// <summary>
        /// The information necessary for the program to connect to the Database
        /// </summary>
        public readonly string connectionString;

        /// <summary>
        /// Upon construction of this static class, build the connection string
        /// </summary>
        public AgarioDatabase()
        {
            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<AgarioDatabase>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("AgarioDBSecrets");

            connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = SelectedSecrets["ServerURL"],
                InitialCatalog = SelectedSecrets["DBName"],
                UserID = SelectedSecrets["UserName"],
                Password = SelectedSecrets["DBPassword"],
                ConnectTimeout = 15
            }.ConnectionString;
        }


        /// <summary>
        ///  Test several connections and print the output to the console
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            AgarioDatabase testDatabase = new AgarioDatabase();
            Console.WriteLine(testDatabase.connectionString);

            DataSet test_set = testDatabase.Get_HighScores();
            foreach (DataRow my_data_row in test_set.Tables["HighScores"].Rows)
            {
                foreach(DataColumn my_data_column in test_set.Tables["HighScores"].Columns)
                {
                    Console.WriteLine(my_data_row[my_data_column]);
                }
            }
        }

        private void InsertPatron()
        {

            Console.WriteLine("What phone numbers exist?");

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand command = new SqlCommand("INSERT INTO Patrons (Name) values ('Aaron')", con))
                    using (SqlDataReader reader = command.ExecuteReader())

                        while (reader.Read())
                        {
                            {
                                Console.WriteLine($"{reader["Name"].ToString().Trim()} ({reader["CardNum"]}) - {reader["Phone"]}");
                            }
                        }
                }
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
            }
        }

        public DataSet Get_HighScores()
        {
            DataSet my_data_set = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string sql_command = "SELECT * FROM HighScores ORDER BY LargestMass DESC";
                    SqlDataAdapter my_sql_data_adapter = new SqlDataAdapter(sql_command, con);

                    my_sql_data_adapter.Fill(my_data_set, "HighScores");
                }
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
            }

            return my_data_set;
        }

        /// <summary>
        /// Getter for the TimeInFirst Data
        /// </summary>
        /// <returns></returns>
        public DataSet Get_First_Times()
        {
            DataSet my_data_set = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string sql_command = "SELECT * FROM TimeInFirst ORDER BY TotalTime DESC";
                    SqlDataAdapter my_sql_data_adapter = new SqlDataAdapter(sql_command, con);

                    my_sql_data_adapter.Fill(my_data_set, "TimeInFirst");
                }
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
            }
            return my_data_set;
        }
    }

}