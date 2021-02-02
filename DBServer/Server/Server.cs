using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Security.RightsManagement;
using System.Text;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Data;
using DBServer.Server.GameLogic;
using DBServer.Server.Messages;
using Newtonsoft.Json;

namespace Server
{
    /// <summary>
    /// Logika serwera przyjmowanie polaczenia, komunikacja z klientem
    /// </summary>
    class Server
    {

        private static IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        private static IPAddress ipAddress = ipHostInfo.AddressList[0];
        private static IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
        private static readonly Socket serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private static readonly List<Player> players = new List<Player>();
        Queue<Player> searchingGameQueue = new Queue<Player>();
        private List<GameRoom> gameRooms = new List<GameRoom>();
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 100;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];


        public delegate void MessageSenderEventHandler(string word);
        public event MessageSenderEventHandler MessageSender;

        string connectionString;
        SqlConnection connection;

        public void AddMessage(string word)
        {
            OnMessageSender(word);
        }
        protected virtual void OnMessageSender(string word)
        {
            if (MessageSender != null)
                MessageSender(word);
        }

        public delegate void NewPlayerEventHandler(Player player);
        public event NewPlayerEventHandler NewPlayer;

        public void AddNewPlayer(Player player)
        {
            OnNewPlayer(player);
        }

        protected virtual void OnNewPlayer(Player player)
        {
            if (NewPlayer != null)
                NewPlayer(player);
        }

        public delegate void DeletePlayerEventHandler(Player player);
        public event DeletePlayerEventHandler DeletePlayer;

        protected virtual void OnDeletePlayer(Player player)
        {
            if (DeletePlayer != null)
                DeletePlayer(player);
        }

        public void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            AddMessage("Setting up server...");
            serverSocket.Bind(localEndPoint);
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server setup complete");
            AddMessage("Server setup complete");

            connectionString = ConfigurationManager.ConnectionStrings["DBServer.Properties.Settings.Database1ConnectionString"].ConnectionString;
        }

        /// <summary>
        /// Close all connected client (we do not need to shutdown the server socket as its connections
        /// are already closed with the clients).
        /// </summary>
        public void CloseAllSockets()
        {
            foreach (Socket socket in clientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            serverSocket.Close();
        }

        private void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }

            clientSockets.Add(socket);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine(socket.GetType() + " connected, waiting for request...");
            // AddMessage(socket.GetType() + " connected, waiting for request...");

            serverSocket.BeginAccept(AcceptCallback, null);

            Console.Out.WriteLine("Socket: " + socket.GetHashCode());
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                AddMessage("Client forcefully disconnected");
                // Don't shutdown because the socket may be disposed and its disconnected anyway.

                Player player = players.Find(p => p.socket == current);

                if(player != null)
                {
                    OnDeletePlayer(player);
                    players.Remove(player);
                }

                current.Close();
                clientSockets.Remove(current);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Received Text: " + text);
            AddMessage("Received Text: " + text);

            //sprawdza czy wyslane zostaly dane logowania
            if ((text.Contains("\"login\":")) && (text.ToLower().Contains("\"password\":"))) // Client requested time
            {
                Console.WriteLine("text = " + text);
                newPlayerConnect(text, current);
            }
            //dodawanie nowego uzytkownika do bazy danych
            else if ((text.Contains("\"newLogin\":")) && (text.Contains("\"newPassword\":")))
            {
                createNewAccount(text, current);
            }
            //chec gry
            else if (text == "want to play")
            {
                wantToPlay(text, current);
            }
            else if (text.Contains("\"wantMoveFromX\""))
            {
                whereCanMove(text, current);
            }
            else if (text.Contains("\"toX\""))
            {
                playerMove(text, current);
            }
            else if (text.Contains("\"pawnToDeleteX\""))
            {
                deletePawns(text, current);
            }
            else if (text == "show ranking")
            {
                showRanking(current);
            }
            else if (text == "logout")
            {
                Player player = players.Find((p => p.socket == current));
                players.Remove(player);
                OnDeletePlayer(player);
            }
            else if (text.ToLower() == "exit") // Client wants to exit gracefully
            {
                // Always Shutdown before closing
                current.Shutdown(SocketShutdown.Both);
                current.Close();
                clientSockets.Remove(current);
                Player player= players.Find(p => p.socket == current);
                players.Remove(player);
                OnDeletePlayer(player);
                Console.WriteLine("Client disconnected");
                AddMessage("Client disconnected");
                return;
            }
            else
            {
                Console.WriteLine("Text is an invalid request");
                AddMessage("Text is an invalid request");
                byte[] data = Encoding.ASCII.GetBytes("Invalid request");
                current.Send(data);
                Console.WriteLine("Warning Sent");
                AddMessage("Warning Sent");
            }
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }

        private void newPlayerConnect(string text, Socket socket)
        {
            LoginData playerLoginData = JsonConvert.DeserializeObject<LoginData>(text);
            Console.WriteLine("Text is a get time request");
            Console.WriteLine("Recieved:" + text);

            //sprawdza czy w bazie danych sa takie dane logowania
            if (players.Find(p => p.login == playerLoginData.login) != null)
            {
                var respponse = new LoginInResponse("duplicate login");
                string responseConverted =
                    JsonConvert.SerializeObject(respponse);
                socket.Send(Encoding.ASCII.GetBytes(responseConverted));
            }
            else
            {

                string query = "SELECT COUNT(*) FROM Klients WHERE Username=@Login";
                using (connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@Login", playerLoginData.login);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (String.Format("{0}", reader[0]) == "0")
                            {
                                //Nie ma takigo użytkownika
                                Console.WriteLine("Nie ma takiego konta!");
                                var respponse = new LoginInResponse("incorrect login");
                                string responseConverted =
                                    JsonConvert.SerializeObject(respponse);
                                socket.Send(Encoding.ASCII.GetBytes(responseConverted));
                            }
                            else
                            {
                                //Istnieje już taki użytkownik
                                Console.Out.WriteLine("Jest już taki login!");
                                query = "SELECT Password FROM Klients WHERE Username=@Login";
                                using (connection = new SqlConnection(connectionString))
                                using (SqlCommand command2 = new SqlCommand(query, connection))
                                {
                                    connection.Open();
                                    command2.Parameters.AddWithValue("@Login", playerLoginData.login);
                                    using (SqlDataReader reader2 = command2.ExecuteReader())
                                    {
                                        while (reader2.Read())
                                        {
                                            if (String.Format("{0}", reader2[0]) == playerLoginData.password)
                                            {
                                                //Poprawne hasło
                                                Console.WriteLine("Wpisano poprawne hasło!");
                                                var respponse = new LoginInResponse("succes loging in");
                                                string responseConverted =
                                                    JsonConvert.SerializeObject(respponse);
                                                socket.Send(Encoding.ASCII.GetBytes(responseConverted));
                                                Player player = new Player(playerLoginData.login, socket);
                                                players.Add(player);
                                                AddNewPlayer(player);
                                            }
                                            else
                                            {
                                                //Niepoprawne hasło
                                                Console.WriteLine("Wpisano niepoprawne hasło!");
                                                var respponse = new LoginInResponse("incorrect password");
                                                string responseConverted =
                                                    JsonConvert.SerializeObject(respponse);
                                                socket.Send(Encoding.ASCII.GetBytes(responseConverted));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void createNewAccount(string text, Socket socket)
        {
            NewLoginData accountLoginData = JsonConvert.DeserializeObject<NewLoginData>(text);
            //tutaj dodawanie nowego uzytkownika
            // Login:  accountLoginData.newLogin
            // Password:  accountLoginData.newPassword

            //Sprawdzam czy jest taki użytkownik

            string query = "SELECT COUNT(*) FROM Klients WHERE Username=@Login";
            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                connection.Open();

                command.Parameters.AddWithValue("@Login", accountLoginData.newLogin);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (String.Format("{0}", reader[0]) == "0")
                        {//Nie ma takigo użytkownika - dodaje użytkownika
                            Console.WriteLine("Nie ma takiego konta!");

                            query = "INSERT INTO Klients VALUES (@Login, @Password, 0, 0)";

                            using (connection = new SqlConnection(connectionString))
                            using (SqlCommand command2 = new SqlCommand(query, connection))
                            {
                                command2.Parameters.AddWithValue("@Login", accountLoginData.newLogin);
                                command2.Parameters.AddWithValue("@Password", accountLoginData.newPassword);
                                connection.Open();
                                command2.ExecuteNonQuery();
                                connection.Close();
                            }
                            Console.WriteLine("Utworzono nowego użytkownika!");
                            var respponse = new NewAccountResponsecs("new account created");
                            string responseConverted =
                                JsonConvert.SerializeObject(respponse);
                            AddMessage(responseConverted);
                            socket.Send(Encoding.ASCII.GetBytes(responseConverted));
                        }
                        else
                        {//Istnieje już taki użytkownik
                            Console.Out.WriteLine("Istnieje już taki użytkownik!");
                            var respponse = new NewAccountResponsecs("creating new account failed");
                            string responseConverted =
                                JsonConvert.SerializeObject(respponse);
                            AddMessage(responseConverted);
                            socket.Send(Encoding.ASCII.GetBytes(responseConverted));
                        }
                    }
                }
            }
        }

        private void wantToPlay(string text, Socket socket)
        {
            Player player = players.Find((p => p.socket == socket));

            Pawn checkPawn;

            AddMessage(players.Find(p => p.socket == socket).login + ": " + text);
            if (searchingGameQueue.Count == 0)
            {
                socket.Send(Encoding.ASCII.GetBytes("wait"));
                searchingGameQueue.Enqueue(player);
            }
            else
            {
                Player pl = searchingGameQueue.Dequeue();
                socket.Send(Encoding.ASCII.GetBytes("game found"));
                pl.socket.Send(Encoding.ASCII.GetBytes("game found"));
                GameRoom gameRoom = new GameRoom(player, pl);
                pl.color = "white";
                player.color = "red";

                if (gameRoom.turn == gameRoom.player1)
                {
                    checkPawn = new Pawn(100, 100, "white");
                    gameRoom.pawns.Add(checkPawn);
                }
                else
                {
                    checkPawn = new Pawn(100, 100, "red");
                    gameRoom.pawns.Add(checkPawn);
                }
                string pawnsStr = JsonConvert.SerializeObject(gameRoom.pawns);
                gameRoom.pawns.Remove(checkPawn);

                gameRoom.player1.socket.Send(Encoding.ASCII.GetBytes(pawnsStr));
                gameRoom.player2.socket.Send(Encoding.ASCII.GetBytes(pawnsStr));
                gameRooms.Add(gameRoom);
                gameRoom.turn = player;
                whereCanMove("all",socket);
            }
        }


        private void whereCanMove(string text, Socket socket)
        {
            Player pl = players.Find(p => p.socket == socket);
            string currentColor = pl.color;
            GameRoom gameRoom = gameRooms.Find(g => (g.player1 == pl || g.player2 == pl));
            List<Move> possibleMoves = new List<Move>();
            List<Move> possibleMovesPom = new List<Move>();
            List<ObligatoryMoves> obligatoryMoves = new List<ObligatoryMoves>();
            List<Pawn> pawns=null;

            bool marker;

            if (text == "all")
                pawns = gameRoom.pawns;
            else
            {
                WantMove whereWantMove = JsonConvert.DeserializeObject<WantMove>(text);
                if (gameRoom.turn == pl && gameRoom.chessboardState[whereWantMove.wantMoveFromX, whereWantMove.wantMoveFromY] != null)
                {
                    pawns = new List<Pawn>();
                    Pawn pawn = gameRoom.pawns.Find(p =>
                        (p.pozX == whereWantMove.wantMoveFromX && p.pozY == whereWantMove.wantMoveFromY));
                    pawns.Add(pawn);
                }
            }

            if (pawns!=null)
            {
                foreach (var pawn in pawns.Where(p => p.color == currentColor))
                {
                    if (pawn.priority)
                    {
                        possibleMovesPom = priorityMove(gameRoom, pawn.pozX, pawn.pozY, possibleMovesPom, true, false, true, false);
                        marker = true;
                        
                        for(int i=1; i<=7; i++)
                        {
                            if(isFree(gameRoom, pawn.pozX + i, pawn.pozY - i) == "bussy")
                            {
                                if (gameRoom.chessboardState[pawn.pozX + i, pawn.pozY - i].color != pawn.color)
                                {
                                    if (isFree(gameRoom, pawn.pozX + i + 1, pawn.pozY - i - 1) == "free")
                                    {
                                        marker = false;

                                        foreach (var move in possibleMovesPom)
                                        {
                                            obligatoryMoves.Add(new ObligatoryMoves(move.fromX, move.fromY, move.toX, move.toY));
                                        }
                                        possibleMovesPom.Clear();
                                        obligatoryMoves.Add(new ObligatoryMoves(pawn.pozX + i - 1, pawn.pozY - i + 1, pawn.pozX + i, pawn.pozY - i));
                                        obligatoryMoves.Add(new ObligatoryMoves(pawn.pozX + i, pawn.pozY - i, pawn.pozX + i + 1, pawn.pozY - i - 1));
                                        obligatoryMoves = check(gameRoom, pawn.pozX + i + 1, pawn.pozY - i - 1, obligatoryMoves, pawn.color, true, false, true, false);
                                        obligatoryMoves = check(gameRoom, pawn.pozX + i + 1, pawn.pozY - i - 1, obligatoryMoves, pawn.color, true, false, false, true);
                                        obligatoryMoves = check(gameRoom, pawn.pozX + i + 1, pawn.pozY - i - 1, obligatoryMoves, pawn.color, false, true, true, false);

                                        break;
                                    }
                                    else break;
                                }
                                else break;
                            }
                        }

                        if(marker == true)
                        {
                            foreach (var moves in possibleMovesPom)
                            {
                                possibleMoves.Add(new Move(moves.fromX, moves.fromY, moves.toX, moves.toY));
                            }
                            possibleMovesPom.Clear();
                        }

                        /*if (isFree(gameRoom, pawn.pozX + 1, pawn.pozY - 1) == "bussy" && gameRoom.chessboardState[pawn.pozX + 1, pawn.pozY - 1].color != pawn.color)
                        {
                            if (isFree(gameRoom, pawn.pozX + 2, pawn.pozY - 2) == "free")
                            {
                                foreach (var move in possibleMovesPom)
                                {
                                    obligatoryMoves.Add(new ObligatoryMoves(move.fromX, move.fromY, move.toX, move.toY));
                                }
                                possibleMovesPom.Clear();
                                obligatoryMoves.Add(new ObligatoryMoves(pawn.pozX, pawn.pozY, pawn.pozX + 1, pawn.pozY - 1));
                                obligatoryMoves.Add(new ObligatoryMoves(pawn.pozX + 1, pawn.pozY - 1, pawn.pozX + 2, pawn.pozY - 2));
                                obligatoryMoves = check(gameRoom, pawn.pozX + 2, pawn.pozY - 2, obligatoryMoves, pawn.color, true, false, true, false);
                                obligatoryMoves = check(gameRoom, pawn.pozX + 2, pawn.pozY - 2, obligatoryMoves, pawn.color, true, false, false, true);
                                obligatoryMoves = check(gameRoom, pawn.pozX + 2, pawn.pozY - 2, obligatoryMoves, pawn.color, false, true, true, false);
                            }
                        }
                        else
                        {
                            foreach (var moves in possibleMovesPom)
                            {
                                possibleMoves.Add(new Move(moves.fromX, moves.fromY, moves.toX, moves.toY));
                            }
                            possibleMovesPom.Clear();
                        }*/



                        possibleMovesPom = priorityMove(gameRoom, pawn.pozX, pawn.pozY, possibleMovesPom, true, false, false, true);
                        marker = true;
                        for (int i = 1; i <= 7; i++)
                        {
                            if (isFree(gameRoom, pawn.pozX + i, pawn.pozY + i) == "bussy")
                            {
                                if (gameRoom.chessboardState[pawn.pozX + i, pawn.pozY + i].color != pawn.color)
                                {
                                    if (isFree(gameRoom, pawn.pozX + i + 1, pawn.pozY + i + 1) == "free")
                                    {
                                        marker = false;

                                        foreach (var move in possibleMovesPom)
                                        {
                                            obligatoryMoves.Add(new ObligatoryMoves(move.fromX, move.fromY, move.toX, move.toY));
                                        }
                                        possibleMovesPom.Clear();
                                        obligatoryMoves.Add(new ObligatoryMoves(pawn.pozX + i - 1, pawn.pozY + i - 1, pawn.pozX + i, pawn.pozY + i));
                                        obligatoryMoves.Add(new ObligatoryMoves(pawn.pozX + i, pawn.pozY + i, pawn.pozX + i + 1, pawn.pozY + i + 1));
                                        obligatoryMoves = check(gameRoom, pawn.pozX + i + 1, pawn.pozY + i + 1, obligatoryMoves, pawn.color, true, false, true, false);
                                        obligatoryMoves = check(gameRoom, pawn.pozX + i + 1, pawn.pozY + i + 1, obligatoryMoves, pawn.color, true, false, false, true);
                                        obligatoryMoves = check(gameRoom, pawn.pozX + i + 1, pawn.pozY + i + 1, obligatoryMoves, pawn.color, false, true, false, true);

                                        break;
                                    }
                                    else break;
                                }
                                else break;
                            }
                        }

                        if (marker == true)
                        {
                            foreach (var moves in possibleMovesPom)
                            {
                                possibleMoves.Add(new Move(moves.fromX, moves.fromY, moves.toX, moves.toY));
                            }
                            possibleMovesPom.Clear();
                        }


                        possibleMovesPom = priorityMove(gameRoom, pawn.pozX, pawn.pozY, possibleMovesPom, false, true, true, false);
                        marker = true;
                        for (int i = 1; i <= 7; i++)
                        {
                            if (isFree(gameRoom, pawn.pozX - i, pawn.pozY - i) == "bussy")
                            {
                                if (gameRoom.chessboardState[pawn.pozX - i, pawn.pozY - i].color != pawn.color)
                                {
                                    if (isFree(gameRoom, pawn.pozX - i - 1, pawn.pozY - i - 1) == "free")
                                    {
                                        marker = false;

                                        foreach (var move in possibleMovesPom)
                                        {
                                            obligatoryMoves.Add(new ObligatoryMoves(move.fromX, move.fromY, move.toX, move.toY));
                                        }
                                        possibleMovesPom.Clear();
                                        obligatoryMoves.Add(new ObligatoryMoves(pawn.pozX - i + 1, pawn.pozY - i + 1, pawn.pozX - i, pawn.pozY - i));
                                        obligatoryMoves.Add(new ObligatoryMoves(pawn.pozX - i, pawn.pozY - i, pawn.pozX - i - 1, pawn.pozY - i - 1));
                                        obligatoryMoves = check(gameRoom, pawn.pozX - i - 1, pawn.pozY - i - 1, obligatoryMoves, pawn.color, false, true, true, false);
                                        obligatoryMoves = check(gameRoom, pawn.pozX - i - 1, pawn.pozY - i - 1, obligatoryMoves, pawn.color, true, false, true, false);
                                        obligatoryMoves = check(gameRoom, pawn.pozX - i - 1, pawn.pozY - i - 1, obligatoryMoves, pawn.color, false, true, false, true);

                                        break;
                                    }
                                    else break;
                                }
                                else break;
                            }
                        }


                        if (marker == true)
                        {
                            foreach (var moves in possibleMovesPom)
                            {
                                possibleMoves.Add(new Move(moves.fromX, moves.fromY, moves.toX, moves.toY));
                            }
                            possibleMovesPom.Clear();
                        }



                        possibleMovesPom = priorityMove(gameRoom, pawn.pozX, pawn.pozY, possibleMovesPom, false, true, false, true);
                        marker = true;

                        for (int i = 1; i <= 7; i++)
                        {
                            if (isFree(gameRoom, pawn.pozX - i, pawn.pozY + i) == "bussy")
                            {
                                if (gameRoom.chessboardState[pawn.pozX - i, pawn.pozY + i].color != pawn.color)
                                {
                                    if (isFree(gameRoom, pawn.pozX - i - 1, pawn.pozY + i + 1) == "free")
                                    {
                                        marker = false;

                                        foreach (var move in possibleMovesPom)
                                        {
                                            obligatoryMoves.Add(new ObligatoryMoves(move.fromX, move.fromY, move.toX, move.toY));
                                        }
                                        possibleMovesPom.Clear();
                                        obligatoryMoves.Add(new ObligatoryMoves(pawn.pozX - i + 1, pawn.pozY + i - 1, pawn.pozX - i, pawn.pozY + i));
                                        obligatoryMoves.Add(new ObligatoryMoves(pawn.pozX - i, pawn.pozY + i, pawn.pozX - i - 1, pawn.pozY + i + 1));
                                        obligatoryMoves = check(gameRoom, pawn.pozX - i - 1, pawn.pozY + i + 1, obligatoryMoves, pawn.color, false, true, false, true);
                                        obligatoryMoves = check(gameRoom, pawn.pozX - i - 1, pawn.pozY + i + 1, obligatoryMoves, pawn.color, true, false, false, true);
                                        obligatoryMoves = check(gameRoom, pawn.pozX - i - 1, pawn.pozY + i + 1, obligatoryMoves, pawn.color, false, true, true, false);

                                        break;
                                    }
                                    else break;
                                }
                                else break;
                            }
                        }


                        if (marker == true)
                        {
                            foreach (var moves in possibleMovesPom)
                            {
                                possibleMoves.Add(new Move(moves.fromX, moves.fromY, moves.toX, moves.toY));
                            }
                            possibleMovesPom.Clear();
                        }
                    }
                    else if (pawn.color == "red")
                    {   //right up
                        if (isFree(gameRoom, pawn.pozX + 1, pawn.pozY - 1) == "free")
                        {
                            possibleMoves.Add(new Move(pawn.pozX, pawn.pozY, pawn.pozX + 1, pawn.pozY - 1));
                        }
                        else if (isFree(gameRoom, pawn.pozX + 1, pawn.pozY - 1) == "overRange")
                        {

                        }
                        else
                        {
                            obligatoryMoves = check(gameRoom, pawn.pozX, pawn.pozY, obligatoryMoves, pawn.color, true, false, true, false);
                        }
                        obligatoryMoves = check(gameRoom, pawn.pozX, pawn.pozY, obligatoryMoves, pawn.color, true, false, false, true);
                        //left up
                        if (isFree(gameRoom, pawn.pozX - 1, pawn.pozY - 1) == "free")
                        {
                            possibleMoves.Add(new Move(pawn.pozX, pawn.pozY, pawn.pozX - 1, pawn.pozY - 1));
                        }
                        else if (isFree(gameRoom, pawn.pozX - 1, pawn.pozY - 1) == "overRange")
                        {

                        }
                        else
                        {
                            obligatoryMoves = check(gameRoom, pawn.pozX, pawn.pozY, obligatoryMoves, pawn.color, false, true, true, false);
                            
                        }
                        obligatoryMoves = check(gameRoom, pawn.pozX, pawn.pozY, obligatoryMoves, pawn.color, false, true, false, true);
                    }
                    else if (pawn.color == "white")
                    {
                        //right down
                        if (isFree(gameRoom, pawn.pozX + 1, pawn.pozY + 1) == "free")
                        {
                            possibleMoves.Add(new Move(pawn.pozX, pawn.pozY, pawn.pozX + 1, pawn.pozY + 1));
                        }
                        else if (isFree(gameRoom, pawn.pozX + 1, pawn.pozY + 1) == "overRange")
                        {

                        }
                        else
                        {
                            obligatoryMoves = check(gameRoom, pawn.pozX, pawn.pozY, obligatoryMoves, pawn.color, true, false, false, true);
                            
                        }
                        obligatoryMoves = check(gameRoom, pawn.pozX, pawn.pozY, obligatoryMoves, pawn.color, true, false, true, false);

                        //left down
                        if (isFree(gameRoom, pawn.pozX - 1, pawn.pozY + 1) == "free")
                        {
                            possibleMoves.Add(new Move(pawn.pozX, pawn.pozY, pawn.pozX - 1, pawn.pozY + 1));
                        }
                        else if (isFree(gameRoom, pawn.pozX - 1, pawn.pozY + 1) == "overRange")
                        {

                        }
                        else
                        {
                            obligatoryMoves = check(gameRoom, pawn.pozX, pawn.pozY, obligatoryMoves, pawn.color, false, true, false, true);
                            
                        }
                        obligatoryMoves = check(gameRoom, pawn.pozX, pawn.pozY, obligatoryMoves, pawn.color, false, true, true, false);
                    }



                }

                if (pawns.Count > 1)
                {
                    if (possibleMoves.Count == 0 && obligatoryMoves.Count == 0)
                    {
                        socket.Send(Encoding.ASCII.GetBytes("gameLost"));
                        if (pl == gameRoom.player1)
                            gameRoom.player2.socket.Send(Encoding.ASCII.GetBytes("gameWon"));
                        else
                            gameRoom.player1.socket.Send(Encoding.ASCII.GetBytes("gameWon"));
                    }
                    else if (obligatoryMoves.Count != 0)
                    {
                        string obligatoryMove = JsonConvert.SerializeObject(obligatoryMoves, Formatting.Indented);
                        socket.Send(Encoding.ASCII.GetBytes(obligatoryMove));
                    }
                }
                else
                {
                    if (obligatoryMoves.Count == 0)
                    {
                        string possibleMoveStr = JsonConvert.SerializeObject(possibleMoves);
                        socket.Send(Encoding.ASCII.GetBytes(possibleMoveStr));
                    }
                    else
                    {
                        obligatoryMoves.Add(new ObligatoryMoves(100, 100, 100, 100));
                        string obligatoryMove = JsonConvert.SerializeObject(obligatoryMoves, Formatting.Indented);
                        socket.Send(Encoding.ASCII.GetBytes(obligatoryMove));
                    }
                }
            }
            
        }

        private string isFree(GameRoom gameRoom, int pozX, int pozY)
        {
            if (pozX < 0 || pozX > 7 || pozY < 0 || pozY > 7)
                return "overRange";
            else if (gameRoom.chessboardState[pozX, pozY] == null)
                return "free";
            else
                return "bussy";
        }

        private List<Move> priorityMove(GameRoom gameRoom, int pozX, int pozY, List<Move> possibleMoves, bool right, bool left, bool up, bool down)
        {
            if (right && up)
            {
                if (isFree(gameRoom, pozX + 1, pozY - 1) == "free")
                {
                    possibleMoves.Add(new Move(pozX, pozY, pozX+1, pozY-1));
                    priorityMove(gameRoom, pozX + 1, pozY - 1, possibleMoves, true, false, true, false);
                }
                // else
                // {
                //     if (isFree(gameRoom, pozX + 1, pozY - 1) == "bussy" &&
                //         gameRoom.chessboardState[pozX + 1, pozY - 1].color != color)
                //     {
                //         if (isFree(gameRoom, pozX + 2, pozY - 2) == "free")
                //         {
                //             foreach (var move in possibleMoves)
                //             {
                //                 obligatoryMoveses.Add(new ObligatoryMoves(move.fromX, move.fromY, move.toX, move.toY));
                //             }
                //             obligatoryMoveses.Add(new ObligatoryMoves(pozX, pozY, pozX + 1, pozY - 1));
                //             obligatoryMoveses.Add(new ObligatoryMoves(pozX + 1, pozY - 1, pozX + 2, pozY - 2));
                //             obligatoryMoveses = check(gameRoom, pozX + 2, pozY - 2, obligatoryMoveses, color, true, false, true, false);
                //             obligatoryMoveses = check(gameRoom, pozX + 2, pozY - 2, obligatoryMoveses, color, true, false, false, true);
                //             obligatoryMoveses = check(gameRoom, pozX + 2, pozY - 2, obligatoryMoveses, color, false, true, true, false);
                //         }
                //     }
                // }
            }
            else if (right && down)
            {
                if (isFree(gameRoom, pozX + 1, pozY + 1) == "free")
                {
                    possibleMoves.Add(new Move(pozX, pozY, pozX+1, pozY+1));
                    priorityMove(gameRoom, pozX + 1, pozY + 1, possibleMoves, true, false, false, true);
                }
            }
            else if (left && down)
            {
                if (isFree(gameRoom, pozX - 1, pozY + 1) == "free")
                {
                    possibleMoves.Add(new Move(pozX, pozY, pozX-1, pozY+1));
                    priorityMove(gameRoom, pozX - 1, pozY + 1, possibleMoves, false, true, false, true);
                }
            }
            else if (left && up)
            {
                if (isFree(gameRoom, pozX - 1, pozY - 1) == "free")
                {
                    possibleMoves.Add(new Move(pozX, pozY, pozX-1, pozY-1));
                    priorityMove(gameRoom, pozX - 1, pozY - 1, possibleMoves, false, true, true, false);
                }
            }

            return possibleMoves;
        }

        private List<ObligatoryMoves> check(GameRoom gameRoom, int pozX, int pozY, List<ObligatoryMoves> obligatoryMoves, string color, bool right, bool left, bool up, bool down)
        {
            //right up
            if (right && up)
            {
                if (isFree(gameRoom, pozX + 1, pozY - 1) == "bussy" && gameRoom.chessboardState[pozX + 1, pozY - 1].color != color)
                {
                    if (isFree(gameRoom, pozX + 2, pozY - 2) == "free")
                    {
                        obligatoryMoves.Add(new ObligatoryMoves(pozX, pozY, pozX + 1, pozY - 1));
                        obligatoryMoves.Add(new ObligatoryMoves(pozX + 1, pozY - 1, pozX + 2, pozY - 2));
                        obligatoryMoves = check(gameRoom, pozX + 2, pozY - 2, obligatoryMoves, color, true, false, true, false);
                        obligatoryMoves = check(gameRoom, pozX + 2, pozY - 2, obligatoryMoves, color, true, false, false, true);
                        obligatoryMoves = check(gameRoom, pozX + 2, pozY - 2, obligatoryMoves, color, false, true, true, false);
                    }
                }
                return obligatoryMoves;
            }
            //right down
            else if (right && down)
            {
                if (isFree(gameRoom, pozX + 1, pozY + 1) == "bussy" && gameRoom.chessboardState[pozX + 1, pozY + 1].color != color)
                {
                    if (isFree(gameRoom, pozX + 2, pozY + 2) == "free")
                    {
                        obligatoryMoves.Add(new ObligatoryMoves(pozX, pozY, pozX + 1, pozY + 1));
                        obligatoryMoves.Add(new ObligatoryMoves(pozX + 1, pozY + 1, pozX + 2, pozY + 2));
                        obligatoryMoves = check(gameRoom, pozX + 2, pozY + 2, obligatoryMoves, color, true, false, true, false);
                        obligatoryMoves = check(gameRoom, pozX + 2, pozY + 2, obligatoryMoves, color, true, false, false, true);
                        obligatoryMoves = check(gameRoom, pozX + 2, pozY + 2, obligatoryMoves, color, false, true, false, true);
                    }
                }
                return obligatoryMoves;
            }
            //left down
            else if (left && down)
            {
                if (isFree(gameRoom, pozX - 1, pozY + 1) == "bussy" && gameRoom.chessboardState[pozX - 1, pozY + 1].color != color)
                {
                    if (isFree(gameRoom, pozX - 2, pozY + 2) == "free")
                    {
                        obligatoryMoves.Add(new ObligatoryMoves(pozX, pozY, pozX - 1, pozY + 1));
                        obligatoryMoves.Add(new ObligatoryMoves(pozX - 1, pozY + 1, pozX - 2, pozY + 2));
                        obligatoryMoves = check(gameRoom, pozX - 2, pozY + 2, obligatoryMoves, color, false, true, true, false);
                        obligatoryMoves = check(gameRoom, pozX - 2, pozY + 2, obligatoryMoves, color, true, false, false, true);
                        obligatoryMoves = check(gameRoom, pozX - 2, pozY + 2, obligatoryMoves, color, false, true, false, true);
                    }
                }
                return obligatoryMoves;
            }
            //left up
            else if (left && up)
            {
                if (isFree(gameRoom, pozX - 1, pozY - 1) == "bussy" && gameRoom.chessboardState[pozX - 1, pozY - 1].color != color)
                {
                    if (isFree(gameRoom, pozX - 2, pozY - 2) == "free")
                    {
                        obligatoryMoves.Add(new ObligatoryMoves(pozX, pozY, pozX - 1, pozY - 1));
                        obligatoryMoves.Add(new ObligatoryMoves(pozX - 1, pozY - 1, pozX - 2, pozY - 2));
                        obligatoryMoves = check(gameRoom, pozX - 2, pozY - 2, obligatoryMoves, color, true, false, true, false);
                        obligatoryMoves = check(gameRoom, pozX - 2, pozY - 2, obligatoryMoves, color, false, true, true, false);
                        obligatoryMoves = check(gameRoom, pozX - 2, pozY - 2, obligatoryMoves, color, false, true, false, true);
                    }
                }
                return obligatoryMoves;
            }
            return obligatoryMoves;

        }


        private void playerMove(string text, Socket socket)
        {
            Player pl = players.Find(p => p.socket == socket);
            GameRoom gameRoom = gameRooms.Find(g => (g.player1 == pl || g.player2 == pl));
            Move move = JsonConvert.DeserializeObject<Move>(text);
            Pawn pawn;
            Pawn checkPawn;

            // foreach (var move in moves)
            {
                pawn = gameRoom.pawns.Find(p => (p.pozX == move.fromX && p.pozY == move.fromY));
                gameRoom.chessboardState[pawn.pozX, pawn.pozY] = null;
                pawn.pozX = move.toX;
                pawn.pozY = move.toY;
                if (pawn.color == "red" && pawn.pozY == 0)
                    pawn.priority = true;
                else if (pawn.color == "white" && pawn.pozY == 7)
                    pawn.priority = true;
                gameRoom.chessboardState[pawn.pozX, pawn.pozY] = pawn;

                if (gameRoom.turn == gameRoom.player1)
                {
                    checkPawn = new Pawn(100, 100, "white");
                    gameRoom.pawns.Add(checkPawn);
                }
                else
                {
                    checkPawn = new Pawn(100, 100, "red");
                    gameRoom.pawns.Add(checkPawn);
                }
                string pawnsStr = JsonConvert.SerializeObject(gameRoom.pawns);
                gameRoom.pawns.Remove(checkPawn);

                gameRoom.player1.socket.Send(Encoding.ASCII.GetBytes(pawnsStr));
                gameRoom.player2.socket.Send(Encoding.ASCII.GetBytes(pawnsStr));
                if (checkIfWin(gameRoom))
                {
                    gameRoom.player1.color = "";
                    gameRoom.player2.color = "";
                    gameRooms.Remove(gameRoom);
                    return;
                }
            }

            if (pl == gameRoom.player1)
            {
                gameRoom.turn = gameRoom.player2;
                whereCanMove("all", gameRoom.player2.socket);
            }
            else
            {
                gameRoom.turn = gameRoom.player1;
                whereCanMove("all",gameRoom.player1.socket);
            }
        }

        private bool checkIfWin(GameRoom gameRoom)
        {
            if (gameRoom.pawns.Where(p => p.color == "red").Count() == 0)
            {
                if (gameRoom.player1.color == "red")
                {
                    gameRoom.player1.socket.Send(Encoding.ASCII.GetBytes("gameLost"));
                    updateKlientScore(gameRoom.player1.login, false);
                    gameRoom.player2.socket.Send(Encoding.ASCII.GetBytes("gameWin"));
                    updateKlientScore(gameRoom.player2.login, true);
                }
                else
                {
                    gameRoom.player1.socket.Send(Encoding.ASCII.GetBytes("gameWin"));
                    updateKlientScore(gameRoom.player1.login, true);
                    gameRoom.player2.socket.Send(Encoding.ASCII.GetBytes("gameLost"));
                    updateKlientScore(gameRoom.player2.login, false);
                }

                return true;
            }
            else if (gameRoom.pawns.Where(p => p.color == "white").Count() == 0)
            {
                if (gameRoom.player1.color == "white")
                {
                    gameRoom.player1.socket.Send(Encoding.ASCII.GetBytes("gameLost"));
                    updateKlientScore(gameRoom.player1.login, false);
                    gameRoom.player2.socket.Send(Encoding.ASCII.GetBytes("gameWin"));
                    updateKlientScore(gameRoom.player2.login, true);
                }
                else
                {
                    gameRoom.player1.socket.Send(Encoding.ASCII.GetBytes("gameWin"));
                    updateKlientScore(gameRoom.player1.login, true);
                    gameRoom.player2.socket.Send(Encoding.ASCII.GetBytes("gameLost"));
                    updateKlientScore(gameRoom.player2.login, false);
                }
                return true;
            }
            return false;
        }

        private void updateKlientScore(String login, Boolean win)
        {
            Console.WriteLine("!!!!!!!!!!!!!!!!!!Aktualizacja wyniku");

            string query, pom;
            int oldValue = 0, newValue;

            if (win)
            {
                query = "SELECT [Won Games] FROM Klients WHERE Username=@Login";
                using (connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Login", login);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pom = String.Format("{0}", reader[0]);
                            oldValue = int.Parse(pom);
                            Console.WriteLine("oldValue = " + oldValue);
                        }
                    }
                    connection.Close();
                }

                newValue = ++oldValue;

                query = "UPDATE Klients SET [Won Games]=@NewValue WHERE Username=@Login";
                using (connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@NewValue", newValue);
                    connection.Open();
                    command.ExecuteScalar();
                    connection.Close();
                }
            }
            else
            {
                query = "SELECT [Lost Games] FROM Klients WHERE Username=@Login";
                using (connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Login", login);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pom = String.Format("{0}", reader[0]);
                            oldValue = int.Parse(pom);
                            Console.WriteLine("oldValue = " + oldValue);
                        }
                    }
                    connection.Close();
                }

                newValue = ++oldValue;

                query = "UPDATE Klients SET [Lost Games]=@NewValue WHERE Username=@Login";
                using (connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@NewValue", newValue);
                    connection.Open();
                    command.ExecuteScalar();
                    connection.Close();
                }
            }
        }

        private void deletePawns(string text, Socket socket)
        {
            List<PawnToDelete> pawnsToDelete = JsonConvert.DeserializeObject<List<PawnToDelete>>(text);
            GameRoom gameRoom = gameRooms.Find(g => (g.player1.socket==socket || g.player2.socket==socket));
            Pawn pawn;
            foreach (var pawnToDelete in pawnsToDelete)
            {
                pawn = gameRoom.pawns.Find(p => (p.pozX == pawnToDelete.pawnToDeleteX && p.pozY == pawnToDelete.pawnToDeleteY));
                gameRoom.chessboardState[pawnToDelete.pawnToDeleteX, pawnToDelete.pawnToDeleteY] = null;
                gameRoom.pawns.Remove(pawn);
            }
        }

        private void showRanking(Socket socket)
        {
            string query = "SELECT Username, [Won Games], [Lost Games] FROM Klients ORDER BY [Won Games] DESC";
            List<RankingPlayer> rankingPlayers = new List<RankingPlayer>();

            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rankingPlayers.Add(new RankingPlayer(reader["Username"].ToString(), reader["Won Games"].ToString(), reader["Lost Games"].ToString()));
                    }
                }
            }

            var rankingPlayersStr = JsonConvert.SerializeObject(rankingPlayers);
            socket.Send(Encoding.ASCII.GetBytes(rankingPlayersStr));
        }
    }
}
