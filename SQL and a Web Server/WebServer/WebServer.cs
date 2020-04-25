/// <summary>
/// 
/// Author:    Aaron Morgan and Xavier Davis
/// Partner:   Xavier Davis and Aaron Morgan
/// Date:      4/24/2020
/// Course:    CS 3500, University of Utah, School of Computing 
/// Copyright: CS 3500, Aaron Morgan and Xavier Davis
/// 
/// We, Aaron Morgan and Xavier Davis, certify that we wrote this code from scratch and did not copy it in part
/// or in whole from another source, with the exception of OnClientConnect, BuildHTTPResponseHeader, BuildHTTPResponse, RequestFromBrowserHandler, 
/// which was provided by Prof. Jim de St. Germain for the University of Utah's Computing CS 3500 class, during the Spring 2020 term.
/// 
/// </summary>

using Database;
using NetworkingNS;
using System;
using System.Data;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace WebServerExample
{

    /// <summary>
    /// This class contains the code for instrumenting an html webpage
    /// that displays data from the SQL database for the Agario game
    /// </summary>
    class WebServer
    {
        /// <summary>
        /// keep track of how many requests have come in.  Just used
        /// for display purposes.
        /// </summary>
        static private int counter = 0;

        /// <summary>
        /// Start the program and await for connections (e.g., from the browser).
        /// Press "Enter" to end, though in a real web server, you wouldn't end.... ;^)
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Web Server Active. Awaiting Clients! Press Enter to Quit.");
            Networking.Server_Create_Connection_Listener(OnClientConnect);

            Console.ReadLine();
        }

        /// <summary>
        /// Basic connect handler - i.e., a browser has connected!
        /// </summary>
        /// <param name="state"> Networking state object created by the Networking Code. Contains the socket.</param>
        private static void OnClientConnect(Preserved_Socket_State state)
        {
            state.on_data_received_handler = RequestFromBrowserHandler;
            Networking.await_more_data(state);
        }

        /// <summary>
        /// Create the HTTP response header, containing items such as
        /// the "HTTP/1.1 200 OK" line.
        ///        
        /// See: https://www.tutorialspoint.com/http/http_responses.htm
        /// </summary>
        /// <param name="message">The message is necessary because Content-Length will vary, corresponding to how long the body of the HTML message is</param>
        /// <returns></returns>
        private static string BuildHTTPResponseHeader(string message)
        {
            return $@"HTTP/1.1 200 OK
                    \r\nDate: {DateTime.Now}
                    \r\nContent-Length: {message.Length}
                    \r\nContent-Type: text/html
                    \r\nConnection: Closed
                    ";
        }

        /// <summary>
        ///   This creates the body of the HTML message, which in turn creates
        ///   the main web page.
        /// </summary>
        /// <returns> A string the represents a web page.</returns>
        private static string Build_Main_Page()
        {
            string message = $@"
                                <!DOCTYPE html>
                                <html>
                                <head>
                                    <title> Agario Database </title>
                                </head>
                                <body>
                                <center>
                                <h1>This is an Agario Database</h1>
                                <a href='http://localhost:11000/highscores'>High Score Tab</a>
                                <br>
                                <br>
                                <a href='http://localhost:11000/timeinfirst'>Time in First Place</a>
                                </center>
                                </body>
                                </html>";
            return message;
        }

        /// <summary>
        /// Helper method that builds the body for displaying
        /// all players' highscores retrieved from the SQL database
        /// </summary>
        /// <returns>String message body for the html page</returns>
        private static string Build_HighScore_Page()
        {
            string message = $@"
                                <!DOCTYPE html>
                                <html>
                                <head>
                                    <title> Agario High Scores </title>
                                </head>
                                <body>
                                <center>
                                <h1>Agario High Scores</h1>
                                <a href='http://localhost:11000/'>Home</a>
                                <br>
                                <br>
                                <table style= 'width: 100 %' cellpadding='10' border = '1'>
                                    <tbody>
                                        <tr>
                                            <th> Name </th>
                                            <th> Max Mass </th>
                                            <th> Time Alive </th>
                                            <th> Highest Rank </th>
                                        </tr>";

                                message += High_Score_Table_Info();
                                message += @"

                                    </tbody>
                                </table>

                                </center>
                                </body>
                                </html>";
            return message;
        }

        /// <summary>
        /// Helper method that builds the body for displaying
        /// a player's scores for each game played which is 
        /// retrieved from the SQL database
        /// </summary>
        /// <param name="name">Name of the Player</param>
        /// <returns>String message body for the html page</returns>
        private static string Build_Player_Page(string name)
        {
            string message = $@"
                                <!DOCTYPE html>
                                <html>
                                <head>
                                    <title> {name} Scores </title>
                                </head>
                                <body>
                                <center>
                                <h1>Agario High Scores</h1>
                                <a href='http://localhost:11000/'>Home</a>
                                <br>
                                <br>
                                <table style= 'width: 100 %' cellpadding='10' border = '1'>
                                    <tbody>
                                        <tr>
                                            <th> Name </th>
                                            <th> Mass </th>
                                            <th> Rank </th>
                                            <th> Time Played </th>
                                            <th> Game Session </th>
                                        </tr>";

                                message += Player_Table_Info(name);
                                message += @"

                                    </tbody>
                                </table>

                                </center>
                                </body>
                                </html>";
            return message;
        }

        /// <summary>
        /// Helper method that builds the body for the Confirmation
        /// Page that's displayed when data is inserted into a player's
        /// table with syntax: localhost/scores/name/mass/rank/start_time/end_time
        /// </summary>
        /// <param name="name">Name of the Player</param>
        /// <returns>String message body for the html page</returns>
        private static string Build_Confirmation_Page(string name)
        {
            string message = $@"
                                <!DOCTYPE html>
                                <html>
                                <head>
                                    <title> Entry Successful </title>
                                </head>
                                <body>
                                <center>
                                <h1>Success!</h1>
                                <p> Successfully inserted data into the database! Great goin' bucko! ;D</p>
                                <a href='http://localhost:11000/scores/{name}'>{name}'s Score Page</a>
                                <br>
                                <br>
                                <a href='http://localhost:11000/'>Home</a>
                                <br>
                                <br>
                                </center>
                                </body>
                                </html>";
            return message;
        }

        /// <summary>
        ///   Create a web page!  The body of the returned message is the web page
        ///   "code" itself. Usually this would start with the HTML tag.  Take a look at:
        ///   https://www.sitepoint.com/a-basic-html5-template/
        /// </summary>
        /// <returns> A string the represents a web page.</returns>
        private static string Build_First_Place_Length_Page()
        {
            string message = $@"
                                <!DOCTYPE html>
                                <html>
                                <head>
                                    <title> First Place Leaderboards </title>
                                </head>
                                <body style='background-color:#89cff0;'>
                                <center>
                                <h1>Longest Times in First Place</h1>
                                <a href='http://localhost:11000/'>Home</a>
                                <br>
                                <br>
                                <table style= 'width: 100 %; background-color: gold;' cellpadding='10' border = '1'>
                                    <tbody>
                                        <tr>
                                            <th style='background-color:white;'> Name </th>
                                            <th style='background-color:white;'> Total Time In First </th>
                                            <th style='background-color:white;'> Date/Time Player Entered First </th>
                                            <th style='background-color:white;'> Number of Players Eaten </th>
                                        </tr>";
                                message += Time_In_First_Table_Info();
                                message += @"
                                </tbody>
                                </table>
                                </center>
                                </body>
                                </html>";

            return message;
        }

        /// <summary>
        /// Create a response message string to send back to the connecting
        /// program (i.e., the web browser).  The string is of the form:
        /// 
        ///   HTTP Header
        ///   [new line]
        ///   HTTP Body
        ///   
        ///  The body is an HTML string.
        /// </summary>
        /// <returns></returns>
        private static string BuildHTTPResponse(string get_request)
        {
            string message = get_request;
            string header = BuildHTTPResponseHeader(message);

            return header + Environment.NewLine + message;
        }

        /// <summary>
        ///   When a request comes in (from a browser) this method will
        ///   be called by the Networking code.  When a full message has been
        ///   read (as defined by an empty line in the overall message) send
        ///   a response based on the request.
        /// </summary>
        /// <param name="network_message_state"> provided by the Networking code, contains socket and message</param>
        private static void RequestFromBrowserHandler(Preserved_Socket_State network_message_state)
        {
            Console.WriteLine($"{++counter,4}: {network_message_state.Message}");
            try
            {
                if (network_message_state.Message.Equals("GET / HTTP/1.1\r"))
                {
                    string main_page = Build_Main_Page();
                    Send_And_Close_Connection(network_message_state.socket, main_page);
                }
                else if (network_message_state.Message.Equals("GET /highscores HTTP/1.1\r"))
                {
                    string high_scores = Build_HighScore_Page();
                    Send_And_Close_Connection(network_message_state.socket, high_scores);
                }
                else if (network_message_state.Message.Equals("GET /timeinfirst HTTP/1.1\r"))
                {
                    string first_place_length = Build_First_Place_Length_Page();
                    Send_And_Close_Connection(network_message_state.socket, first_place_length);
                }
                else if (network_message_state.Message.Contains("scores"))
                {
                    string split_character = "/";
                    string[] split_message = Regex.Split(network_message_state.Message, split_character);

                    if (split_message.Length == 4)
                    {
                        // grabs the player name from the network message, minus " HTTP"
                        string name = split_message[2].Substring(0, split_message[2].Length - 5);
                        string sent_player_database = Build_Player_Page(name);
                        Send_And_Close_Connection(network_message_state.socket, sent_player_database);
                    }
                    else if (split_message.Length == 8)
                    {
                        float mass;
                        int rank;
                        long start_time;
                        long end_time;
                        string name = split_message[2];

                        float.TryParse(split_message[3], out mass);
                        int.TryParse(split_message[4], out rank);
                        long.TryParse(split_message[5], out start_time);
                        long.TryParse(split_message[6].Substring(0, split_message[6].Length - 5), out end_time);
                        long total_time = end_time - start_time;

                        TimeSpan converted_time = TimeSpan.FromMilliseconds(total_time);
                        string time_played = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", converted_time.Hours, converted_time.Minutes, converted_time.Seconds, converted_time.Milliseconds);

                        // Insert networking message into sql table
                        AgarioDatabase database = new AgarioDatabase();
                        database.Insert_Player_Data(name, mass, rank, time_played);
                        database.Insert_HighScore_Data(name, mass, rank, time_played);

                        string sent_player_database = Build_Confirmation_Page(name);
                        Send_And_Close_Connection(network_message_state.socket, sent_player_database);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Something went wrong... this is a bad error message. {exception}");
            }
        }

        private static void Send_And_Close_Connection(Socket socket, string database)
        {
            Networking.Send(socket, BuildHTTPResponse(database));
            // the message response told the browser to disconnect, but
            // if they didn't we will do it.
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        /// <summary>
        /// Helper method to create a dataset to store the table information
        /// from the High Scores SQL table and builds a portion of
        /// the http body message with the gathered information
        /// </summary>
        /// <returns>The http message portion containing a table of the information</returns>
        private static string High_Score_Table_Info()
        {
            AgarioDatabase database = new AgarioDatabase();
            DataSet high_score_set = database.Get_HighScores();

            return Build_Table_Data(high_score_set, "HighScores");
        }

        /// <summary>
        /// Helper method to create a dataset to store the table information
        /// from the specified player's SQL table and builds a portion of
        /// the http body message with the gathered information
        /// </summary>
        /// <returns>The http message portion containing a table of the information</returns>
        private static string Player_Table_Info(string player_name)
        {
            AgarioDatabase database = new AgarioDatabase();
            DataSet player_score_set = database.Get_Player_Data(player_name);

            return Build_Table_Data(player_score_set, $"{player_name}Data");
        }

        /// <summary>
        /// Helper method to create a dataset to store the table information
        /// from the TimeInFirst SQL table and builds a portion of the http
        /// body message with the gathered
        /// information
        /// </summary>
        /// <returns>The http message portion containing a table of the information</returns>
        private static string Time_In_First_Table_Info()
        {
            AgarioDatabase database = new AgarioDatabase();
            DataSet time_in_first = database.Get_First_Times();

            return Build_Table_Data(time_in_first, "TimeInFirst");
        }

        /// <summary>
        /// Helper method to construct the http body message in the 
        /// form of a table.
        /// </summary>
        /// <param name="dataset">DataSet holding the Table information</param>
        /// <param name="table_name">Name of the table found in the SQL database</param>
        /// <returns>Returns a portion of the http message containing the table cell data</returns>
        private static string Build_Table_Data(DataSet dataset, string table_name)
        {
            string table_info = "<tr>";
            foreach (DataRow my_data_row in dataset.Tables[table_name].Rows)
            {
                foreach (DataColumn my_data_column in dataset.Tables[table_name].Columns)
                {
                    table_info += "<td style='background-color:white;'>" + my_data_row[my_data_column] + "</td>";
                }
                table_info += "</tr>";
                table_info += "<tr>";
            }
            return table_info.Substring(0, table_info.Length - 4);
        }
    }
}


