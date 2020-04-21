using NetworkingNS;
using System;
using System.Net.Sockets;

namespace WebServerExample
{
    /// <summary>
    /// Initial Author: H. James de St. Germain
    /// Initial Date:   Spring 2020
    /// Main Author:    Aaron Morgan and Xavier Davis
    /// Update Date:    4/19/20
    /// 
    /// Code for a simple Web Server
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
        /// <param name="message">the message is necessary because... FIXME</param>
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
        ///   Create a web page!  The body of the returned message is the web page
        ///   "code" itself. Usually this would start with the HTML tag.  Take a look at:
        ///   https://www.sitepoint.com/a-basic-html5-template/
        /// </summary>
        /// <returns> A string the represents a web page.</returns>
        private static string Build_Main_Screen()
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
                                <h3>Visits to site: {counter}</h3>
                                <a href='localhost:11000'>Reload</a> 
                                <br/><br/>how are you...<br/><br/>
                                <a href='http://localhost:11000/highscores'>High Score Tab</a> 
                                <a href='http://localhost:11000/scoregraph'>High Score Graph</a> 
                                <form>
                                    <label for='Name Search'>Name Search</label>
                                    <input type='text' id='Name Search' name='scores'><br/><br/>
                                </form>
                                </center>
                                </body>
                                </html>
                                ";

            return message;
        }

        private static string Build_HighScore_Screen()
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
                                <h3>Visits to site: {counter}</h3>
                                <a href='localhost:11000'>Reload</a> 
                                <br/><br/>how are you...<br/><br/>
                                <a href='http://localhost:11000/highscores'>High Score Tab</a> 
                                <a href='http://localhost:11000/scoregraph'>High Score Graph</a> 
                                <form>
                                    <label for='Name Search'>Name Search</label>
                                    <input type='text' id='Name Search' name='scores'><br/><br/>
                                </form>
                                </center>
                                </body>
                                </html>
                                ";

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
                // by definition if there is a new line, then the request is done
                if (network_message_state.Message.Contains("GET /HTTP /1.1"))
                {
              
                    //Console.WriteLine(network_message_state.Message);
                    string main_screen = Build_Main_Screen();
                    Networking.Send(network_message_state.socket, BuildHTTPResponse(main_screen));

                    // the message response told the browser to disconnect, but
                    // if they didn't we will do it.
                    if (network_message_state.socket.Connected)
                    {
                        network_message_state.socket.Shutdown(SocketShutdown.Both);
                        network_message_state.socket.Close();
                    }
                }
                else if (network_message_state.Message.Contains("GET /highscores HTTP/1.1"))
                {
                    string high_scores = Build_HighScore_Screen();
                    Networking.Send(network_message_state.socket, BuildHTTPResponse(high_scores));

                    if (network_message_state.socket.Connected)
                    {
                        network_message_state.socket.Shutdown(SocketShutdown.Both);
                        network_message_state.socket.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Something went wrong... this is a bad error message. {exception}");
            }
        }
    }
}


