/// <summary>
/// 
/// Author:    Aaron Morgan and Xavier Davis
/// Partner:   Xavier Davis and Aaron Morgan
/// Date:      4/24/2020
/// Course:    CS 3500, University of Utah, School of Computing 
/// Copyright: CS 3500, Aaron Morgan and Xavier Davis
/// 
/// We, Aaron Morgan and Xavier Davis, certify that we wrote this code from scratch and did not copy it in part
/// or in whole from another source, with the exception of the Configuration code for the User Secrets, which was primarily 
/// provided by Prof. Jim de St. Germain for the University of Utah's Computing CS 3500 class, during the Spring 2020 term.
/// 
/// </summary>
using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Database
{
    /// <summary>
    /// This class handles the communication to the SQL Database that is reserved for
    /// the Agario game.
    /// </summary>
    public class AgarioDatabase
    {

        /// <summary>
        /// The information necessary for the program to connect to the Database
        /// </summary>
        private readonly string connection_string;

        /// <summary>
        /// Upon construction of this static class, build the connection string
        /// </summary>
        public AgarioDatabase()
        {
            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<AgarioDatabase>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("AgarioDBSecrets");

            connection_string = new SqlConnectionStringBuilder()
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
        }

        /// <summary>
        /// This method retrieves the data that is found in the HighScores table.
        /// </summary>
        /// <returns>A DataSet, containing the data from the HighScores table</returns>
        public DataSet Get_HighScores()
        {
            return Build_DataSet(connection_string, "HighScores", "LargestMass");
        }

        /// <summary>
        /// This method retrieves the data that is found in the TimeInFirst table.
        /// </summary>
        /// <returns>A DataSet, containing the data from the TimeInFirst table</returns>
        public DataSet Get_First_Times()
        {
            return Build_DataSet(connection_string, "TimeInFirst", "TotalTime");
        }

        /// <summary>
        /// This method retrieves the data that is found in the given Player table.
        /// The table data retrieved is dependent on which name is passed into the parameter.
        /// </summary>
        /// <param name="player_name">The name of the player</param>
        /// <returns>A DataSet, containing the data from the given Player table.</returns>
        public DataSet Get_Player_Data(string player_name)
        {
            return Build_DataSet(connection_string, $"{player_name}Data", "Mass");
        }

        /// <summary>
        /// This write the SQL command to insert the given player data into the
        /// HighScores table. If the player already exists in the table, it will check
        /// to see the largest values recorded in the player's table, then
        /// set the HighScores values (for that player) to those values.
        /// </summary>
        /// <param name="player_name">The player name</param>
        /// <param name="mass">The mass of the player's circle</param>
        /// <param name="rank">The rank the player achieved</param>
        /// <param name="time_alive">The time in h:m:s:ms format</param>
        public void Insert_HighScore_Data(string player_name, float mass, int rank, string time_alive)
        {
            string table_name = player_name + "Data";
            DataSet my_data_set = new DataSet();
            Console.WriteLine(time_alive);
            try
            {
                using (SqlConnection con = new SqlConnection(connection_string))
                {
                    con.Open();
                    string sql_command = @$"IF NOT EXISTS (SELECT*FROM HighScores WHERE PlayerName = '{player_name}')
                                                BEGIN
                                                INSERT INTO HighScores (PlayerName, LargestMass, LongestTimeAlive, HighestRank)
                                                VALUES ('{player_name}', {mass}, '{time_alive}', {rank})
                                                END
                                            ELSE
                                                BEGIN
                                                DECLARE @HighestRecordedMass float;
                                                DECLARE @HighestRecordedTimeAlive varchar(50);
                                                DECLARE @HighestRecordedRank int;

                                                SELECT @HighestRecordedMass = MAX(Mass) FROM {table_name}
                                                UPDATE HighScores SET LargestMass = @HighestRecordedMass WHERE PlayerName = '{player_name}'

                                                SELECT @HighestRecordedTimeAlive = MAX(TimePlayed) FROM {table_name}
                                                UPDATE HighScores SET LongestTimeAlive = @HighestRecordedTimeAlive WHERE PlayerName = '{player_name}'

                                                SELECT @HighestRecordedRank = MIN(Rank) FROM {table_name}
                                                UPDATE HighScores SET HighestRank = @HighestRecordedRank WHERE PlayerName = '{player_name}'

                                                END;";
                    SqlDataAdapter my_sql_data_adapter = new SqlDataAdapter(sql_command, con);

                    my_sql_data_adapter.Fill(my_data_set, $"{table_name}");
                }
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
            }
        }

        /// <summary>
        /// This write the SQL command to insert the given player data into the
        /// the player's own table. If the player doesn't exist in the database yet,
        /// a new table will be created.
        /// </summary>
        /// <param name="player_name">The player name</param>
        /// <param name="mass">The mass of the player's circle</param>
        /// <param name="rank">The rank the player achieved</param>
        /// <param name="time_alive">The time in h:m:s:ms format</param>
        public void Insert_Player_Data(string player_name, float mass, int rank, string time_alive)
        {
            string table_name = player_name + "Data";
            DataSet my_data_set = new DataSet();
            Console.WriteLine(time_alive);
            try
            {
                using (SqlConnection con = new SqlConnection(connection_string))
                {
                    con.Open();
                    string sql_command = @$"IF OBJECT_ID('{table_name}') IS NULL
                                                BEGIN
                                                CREATE TABLE {table_name} (
                                                PlayerName varchar(50) NOT NULL,
                                                Mass float NOT NULL,
                                                Rank int NOT NULL,
                                                TimePlayed varchar(50) NOT NULL,
	                                            GameSession int identity(1,1) NOT NULL
                                                )   
                                                INSERT INTO {table_name} (PlayerName, Mass, Rank, TimePlayed)
                                                VALUES
                                                ('{player_name}', {mass}, {rank}, '{time_alive}')
                                                END
                                            ELSE
                                                BEGIN
                                                INSERT INTO {table_name} (PlayerName, Mass, Rank, TimePlayed)
                                                VALUES
                                                ('{player_name}', {mass}, {rank}, '{time_alive}')
                                                END;
                                                ";
                    SqlDataAdapter my_sql_data_adapter = new SqlDataAdapter(sql_command, con);

                    my_sql_data_adapter.Fill(my_data_set, $"{table_name}");
                }
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
            }
        }

        /// <summary>
        /// This private helper method is used to construct the DataSet that
        /// contains the data that is retrieved from a given table, and will sort
        /// it in descending order based on the given column name.
        /// </summary>
        /// <param name="connection_string">The connection string that is used to communicate with the SQL database</param>
        /// <param name="table_name">The table to be retrieved from the database</param>
        /// <param name="column_name">The column name to be sorted by.</param>
        /// <returns></returns>
        private static DataSet Build_DataSet(string connection_string, string table_name, string column_name)
        {
            DataSet my_data_set = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connection_string))
                {
                    con.Open();

                    string sql_command = $"SELECT * FROM {table_name} ORDER BY {column_name} DESC";
                    SqlDataAdapter my_sql_data_adapter = new SqlDataAdapter(sql_command, con);

                    my_sql_data_adapter.Fill(my_data_set, $"{table_name}");
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