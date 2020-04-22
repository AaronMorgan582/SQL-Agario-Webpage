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
                DataSource = SelectedSecrets["AgarioDataSource"],
                InitialCatalog = SelectedSecrets["AgarioInitialCatalog"],
                UserID = SelectedSecrets["AgarioUserID"],
                Password = SelectedSecrets["AgarioDBPassword"],
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
            /*            Console.WriteLine("\n---------- Read All Patrons ---------------");
                        AllPatrons();

                        Console.WriteLine("\n---------- Add Patrons ---------------");
                        AddPatrons();

                        Console.WriteLine("\n---------- Read All Phone Numbers ---------------");
                        AllPhones();

                        Console.WriteLine("\n---------- JOIN Patrons and Phone Numbers ---------------");
                        PatronsPhones();*/

            DataSet test_set = testDatabase.Get_HighScores_Test();
            foreach (DataRow my_data_row in test_set.Tables["HighScores"].Rows)
            {
                foreach(DataColumn my_data_column in test_set.Tables["HighScores"].Columns)
                {
                    Console.WriteLine(my_data_row[my_data_column]);
                }
            }
        }


        /// <summary>
        /// Open a connection to the SQL server and query for all patrons.
        /// 
        /// Important points in code below:
        /// (1) creation of the ConnectionString and opening of the connection
        /// (2) the use of the using statements
        /// (3) how to write a direct SQL query and send it to the server
        /// (4) how to retrieve the data
        /// </summary>
        void AllPatrons()
        {
            Console.WriteLine("Getting Connection ...");

            try
            {
                //create instance of database connection
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    //
                    // Open the SqlConnection.
                    //
                    con.Open();

                    //
                    // This code uses an SqlCommand based on the SqlConnection.
                    //
                    using (SqlCommand command = new SqlCommand("SELECT * FROM Patrons", con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0} {1}",
                                    reader.GetInt32(0), reader.GetString(1));
                            }
                        }
                    }
                }
                Console.WriteLine($"Successful SQL connection");
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection: {exception.Message}");
            }


        }

        /// <summary>
        /// Try to add a row to the database table
        /// Note:
        ///   (1) Fails because the user does not have permission to do so!
        /// </summary>
        void AddPatrons()
        {
            Console.WriteLine("Can we add a row?");

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand command = new SqlCommand("INSERT INTO Patrons VALUES (6,'Dav')", con))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("{0} {1}",
                                reader.GetInt32(0), reader.GetString(1));
                        }
                    }
                }
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
            }
        }

        /// <summary>
        /// Query all Phone numbers in table Phones
        /// 
        /// Notice:
        /// 
        /// (1) use of "dictionary" access
        /// (2) Select [named columns] syntax 
        /// </summary>
        public List<string> Get_HighScores()
        {
            List<string> high_scores = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand command = new SqlCommand("SELECT * FROM HighScores", con))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            high_scores.Add(reader["PlayerName"].ToString());

                            Console.WriteLine($"{reader["PlayerName"]} - {reader["LargestMass"]}");
                        }
                    }
                }
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
            }

            return high_scores; 
        }


        /// <summary>
        ///  JOIN the Phone number table with the Patrons table 
        ///  print all the phone numbers associated with each patron
        ///  
        ///  Notice: Explicit JOIN
        /// </summary>
        public void PatronsPhones()
        {

            Console.WriteLine("What phone numbers exist?");

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand command = new SqlCommand("SELECT * FROM Phones JOIN Patrons ON Phones.CardNum = PAtrons.CardNum", con))
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

        public DataSet Get_HighScores_Test()
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
    }

}