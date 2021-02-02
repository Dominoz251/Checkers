using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Warcaby.Messages;
using Warcaby.Pages;
using Warcaby.Pages.Gameplay;

namespace Warcaby.ServerConnection
{
    class Client : Window
    {

        private static IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        private static IPAddress ipAddress = ipHostInfo.AddressList[0];
        private static IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
        private static readonly Socket ClientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        private const int BUFFER_SIZE = 2048;
        private const int PORT = 100;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];

        LoggingWindow loggingWindow = new LoggingWindow();
        private MenuWindow menuWindow;
        private SignUpWindow signUpWindow;
        private GameplayWindow gameplayWindow;
        private SearchingGameWindow searchingGameWindow;
        private WinWindow winWindow;
        private LostWindow lostWindow;
        private RankingWindow rankingWindow;

        public Button[,] fields;
        public Ellipse[] whitePawns;
        public Ellipse[] redPawns;

        public Client()
        {
            settingUpLoggingWindow();
            ConnectToServer();
        }

        private void settingUpLoggingWindow()
        {
            loggingWindow.SendMessage += MainWindow_SendMessage;
            loggingWindow.NewAccountWindow += MainWindow_NewAccountWindow;
            loggingWindow.Show();
        }

        private void settingUpMenuWindow()
        {
            this.menuWindow.PlayGame += MenuWindow_PlayGame;
            this.menuWindow.Logout += MenuWindow_Logout;
            this.menuWindow.ShowRanking += MenuWindow_ShowRanking;
            this.menuWindow.Show();
        }

        private void MenuWindow_ShowRanking()
        {
            SendRequest("show ranking");
        }

        private void MenuWindow_Logout()
        {
            SendRequest("logout");
            this.loggingWindow = new LoggingWindow();
            settingUpLoggingWindow();
            this.menuWindow.Close();
        }

        private void MainWindow_NewAccountWindow()
        {
            this.signUpWindow = new SignUpWindow();
            this.signUpWindow.NewAccount += SignUpWindow_NewAccount;
            this.signUpWindow.LoggingWindow += SignUpWindow_LoggingWindow;
            loggingWindow.Close();
            signUpWindow.Show();
        }

        private void SignUpWindow_LoggingWindow()
        {
            this.loggingWindow = new LoggingWindow();
            settingUpLoggingWindow();
            this.signUpWindow.Close();
        }

        private void SignUpWindow_NewAccount(string message)
        {
            SendRequest(message);
        }

        private void MainWindow_SendMessage(string message)
        {
            SendRequest(message);
        }

        private void MenuWindow_PlayGame()
        {
            SendRequest("want to play");
        }

        public void ConnectToServer()
        {
            int attempts = 0;

            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    ClientSocket.Connect(remoteEP);
                }
                catch (SocketException)
                {
                }
            }
            ClientSocket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallback), ClientSocket);
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
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);

            Console.Out.WriteLine(text);
            if (text.Contains("\"loginInReponse\":"))
            {
                Dispatcher.Invoke(new Action((() => LoginResponse(text))));
            }
            else if (text.Contains("\"newAccountResponse\":"))
            {
                Dispatcher.Invoke(new Action((() => NewAccountResponse(text))));
            }
            else if (text == "game found" || text == "wait")
            {
                Dispatcher.Invoke(new Action((() => SearchingGameResponse(text))));
            }
            else if (text.Contains("\"fromX\""))
            {
                Dispatcher.Invoke(new Action((() => ShowPossibleMoves(text))));
            }
            else if (text.Contains("\"obligatoryToX\""))
            {
                Dispatcher.Invoke(new Action((() => ShowObligatoryMoves(text))));
            }
            else if (text.Contains("\"color\""))
            {
                Dispatcher.Invoke(new Action((() => UpadeteChessboardState(text))));
            }
            else if (text.Contains("gameWin") || text.Contains("gameLost"))
            {
                Dispatcher.Invoke(new Action((() => gameResult(text))));
            }
            else if (text.Contains("\"username\""))
            {
                Dispatcher.Invoke(new Action((() => showRanking(text))));
            }

            Console.WriteLine("Received Text: " + text);
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);

        }

        private void LoginResponse(string message)
        {
            LoginInResponse loginInResponse = JsonConvert.DeserializeObject<LoginInResponse>(message);

            if (loginInResponse.loginInReponse == "succes loging in")
            {
                this.menuWindow = new MenuWindow();
                settingUpMenuWindow();
                loggingWindow.Close();

                menuWindow.loginNameLabel.Content = loggingWindow.name;
            }
            else if (loginInResponse.loginInReponse == "incorrect password")
            {
                MessageBox.Show("Niepoprawne hasło!");
                loggingWindow.PasswordBox.Password = "";
            }
            else if (loginInResponse.loginInReponse == "incorrect login")
            {
                MessageBox.Show("Nie ma takiego użytkownika!");
                loggingWindow.Username.Text = "";
                loggingWindow.PasswordBox.Password = "";
            }
            else if (loginInResponse.loginInReponse == "duplicate login")
            {
                MessageBox.Show("Użykownik z takim loginem jest już zalogowany!");
                loggingWindow.Username.Text = "";
                loggingWindow.PasswordBox.Password = "";
            }

        }


        private void NewAccountResponse(string message)
        {
            NewAccountResponsecs newAccount = JsonConvert.DeserializeObject<NewAccountResponsecs>(message);
            if (newAccount.newAccountResponse == "new account created")        //to zwraca serwer jak uda sie dodac konto
            {
                this.loggingWindow = new LoggingWindow();
                settingUpLoggingWindow();
                this.signUpWindow.Close();
                MessageBox.Show("Rejestracja przeszła pomyślnie! Możesz się zalogować.");

            }
            else if (newAccount.newAccountResponse == "creating new account failed")  //to zwraca serwer jak nie uda sie dodac konta
            {
                // usernameTextBox.Text = "";
                // PasswordBox1.Password = "";
                // PasswordBox2.Password = "";
                MessageBox.Show("Istnieje już taki użytkownik!");
            }
        }


        private void SearchingGameResponse(string message)
        {
            if (message == "game found")
            {
                this.gameplayWindow = new GameplayWindow();
                this.gameplayWindow.WantMove += GameplayWindow_WantMove;
                this.gameplayWindow.Move += GameplayWindow_Move;
                this.gameplayWindow.DeletePawn += GameplayWindow_DeletePawn;
                    fields = gameplayWindow.fields;
                whitePawns = gameplayWindow.whitePawns;
                redPawns = gameplayWindow.redPawns;
                menuWindow.Close();
                if (searchingGameWindow != null) searchingGameWindow.Close();
                this.gameplayWindow.Show();
            }
            else if (message == "wait")
            {
                this.searchingGameWindow = new SearchingGameWindow();
                this.menuWindow.Close();
                this.searchingGameWindow.Show();
            }
        }

        private void GameplayWindow_DeletePawn(string message)
        {
            SendRequest(message);
        }

        private void GameplayWindow_Move(string message)
        {
            SendRequest(message);
        }

        private void GameplayWindow_WantMove(string message)
        {
            SendRequest(message);
        }

        private void ShowPossibleMoves(string text)
        {
            List<Move> possibleMoves = JsonConvert.DeserializeObject<List<Move>>(text);
            Console.WriteLine("!!!!!!!!!!!!!!!!"+possibleMoves.ToString());
            //possibleMoves lista z polami ktore trzea podswietlic

            foreach(Button button in fields)
            {
                if (button.Background == Brushes.Blue || button.Background == Brushes.Yellow || button.Background == Brushes.GreenYellow
                    || button.Background == Brushes.LightYellow || button.Background == Brushes.LightGreen
                    || button.Background == Brushes.LightSkyBlue) button.Background = Brushes.Black;
            }

            fields[possibleMoves[0].fromX, possibleMoves[0].fromY].Background = Brushes.Yellow;

            foreach (Move move in possibleMoves)
            {
                Console.WriteLine("Teraz podswietle pole: " + fields[move.toX, move.toY].Name);
                fields[move.toX, move.toY].Background = Brushes.Blue;
            }
        }


        private void ShowObligatoryMoves(string text)
        {
            List<ObligatoryMoves> moves = JsonConvert.DeserializeObject<List<ObligatoryMoves>>(text);

            Console.WriteLine("!!!!!!!!!!!!!!" + text);

            foreach (Button button in fields)
            {
                if (button.Background == Brushes.Blue || button.Background == Brushes.Yellow || button.Background == Brushes.GreenYellow
                    || button.Background == Brushes.LightYellow || button.Background == Brushes.LightGreen
                    || button.Background == Brushes.LightSkyBlue) button.Background = Brushes.Black;
            }

            if (moves[moves.Count-1].obligatoryFromX == 100)
            {
                for (int i = 0; i < moves.Count-1; i++)
                {
                    if (i == 0)
                    {
                        fields[moves[i].obligatoryFromX, moves[i].obligatoryFromY].Background = Brushes.Yellow;
                    }
                    else
                    {
                        if (moves[i - 1].obligatoryToX == moves[i].obligatoryFromX && moves[i - 1].obligatoryToY == moves[i].obligatoryFromY)
                        {
                            fields[moves[i].obligatoryFromX, moves[i].obligatoryFromY].Background = Brushes.GreenYellow;
                            fields[moves[i].obligatoryToX, moves[i].obligatoryToY].Background = Brushes.Blue;
                        }
                        else
                        {
                            fields[moves[i].obligatoryFromX, moves[i].obligatoryFromY].Background = Brushes.Yellow;
                            fields[moves[i].obligatoryToX, moves[i].obligatoryToY].Background = Brushes.GreenYellow;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < moves.Count; i++)
                {
                    if (i == 0)
                    {
                        fields[moves[i].obligatoryFromX, moves[i].obligatoryFromY].Background = Brushes.LightYellow;
                    }
                    else
                    {
                        if (moves[i - 1].obligatoryToX == moves[i].obligatoryFromX &&
                            moves[i - 1].obligatoryToY == moves[i].obligatoryFromY)
                        {
                            fields[moves[i].obligatoryFromX, moves[i].obligatoryFromY].Background = Brushes.LightGreen;
                            fields[moves[i].obligatoryToX, moves[i].obligatoryToY].Background = Brushes.LightSkyBlue;
                        }
                        else
                        {
                            fields[moves[i].obligatoryFromX, moves[i].obligatoryFromY].Background = Brushes.LightYellow;
                            fields[moves[i].obligatoryToX, moves[i].obligatoryToY].Background = Brushes.LightGreen;
                        }

                    }
                }
            }
        }


        private void UpadeteChessboardState(string text)
        {
            List<Pawn> chessboardState = JsonConvert.DeserializeObject<List<Pawn>>(text);
            //chessboardState lista z akutalnym polozeniem pionkow
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@" + text);

            Pawn checkPawn = chessboardState.Find(p => (p.pozX == 100 && p.pozY == 100));

            if (checkPawn.color == "white")
            {
                this.gameplayWindow.yourTurnLabel.Content = "White player turn";
            }
            else if (checkPawn.color == "red")
            {
                this.gameplayWindow.yourTurnLabel.Content = "Red player turn";
            }

            chessboardState.Remove(checkPawn);

            int whiteCount = 0;
            int redCount = 0;

            foreach (Button button in fields)
            {
                if (button.Background == Brushes.Blue || button.Background == Brushes.Yellow || button.Background == Brushes.GreenYellow
                    || button.Background == Brushes.LightYellow || button.Background == Brushes.LightGreen
                    || button.Background == Brushes.LightSkyBlue) button.Background = Brushes.Black;
            }

            foreach (Pawn pawn in chessboardState)
            {
                if (pawn.color == "white")
                {
                    Console.WriteLine("Teraz ustawiam pionek: " + whitePawns[whiteCount].Name);
                    var margin = fields[pawn.pozX, pawn.pozY].Margin;
                    margin.Left = fields[pawn.pozX, pawn.pozY].Margin.Left + 5.0;
                    margin.Right = fields[pawn.pozX, pawn.pozY].Margin.Right + 5.0;
                    margin.Top = fields[pawn.pozX, pawn.pozY].Margin.Top + 5.0;
                    margin.Bottom = fields[pawn.pozX, pawn.pozY].Margin.Bottom + 5.0;
                    whitePawns[whiteCount].Margin = margin;
                    whiteCount++;
                }else if (pawn.color == "red")
                {
                    Console.WriteLine("Teraz ustawiam pionek: " + redPawns[redCount].Name);
                    var margin = fields[pawn.pozX, pawn.pozY].Margin;
                    margin.Left = fields[pawn.pozX, pawn.pozY].Margin.Left + 5.0;
                    margin.Right = fields[pawn.pozX, pawn.pozY].Margin.Right + 5.0;
                    margin.Top = fields[pawn.pozX, pawn.pozY].Margin.Top + 5.0;
                    margin.Bottom = fields[pawn.pozX, pawn.pozY].Margin.Bottom + 5.0;
                    redPawns[redCount].Margin = margin;
                    redCount++;
                }
            }

            for (int i = whiteCount; i < whitePawns.Length; i++)
            {
                whitePawns[i].Visibility= Visibility.Hidden;
            }
            for (int i = redCount; i < redPawns.Length; i++)
            {
                redPawns[i].Visibility= Visibility.Hidden;
            }
        }


        /// <summary>
        /// Close socket and exit program.
        /// </summary>
        private static void Exit()
        {
            SendString("exit"); // Tell the server we are exiting
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            Environment.Exit(0);
        }

        public void SendRequest(string request)
        {
            SendString(request);

            if (request.ToLower() == "exit")
            {
                Exit();
            }


        }

        /// <summary>
        /// Sends a string to the server with ASCII encoding.
        /// </summary>
        private static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        private void ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            Console.WriteLine(text);
        }

        private void gameResult(string text)
        {
            if (text == "gameWin")
            {
                this.winWindow= new WinWindow();
                this.winWindow.BackToMenu += WinWindow_BackToMenu;
                this.winWindow.Show();
            }
            else if (text == "gameLost")
            {
                this.lostWindow = new LostWindow();
                this.lostWindow.BackToMenu += LostWindow_BackToMenu;
                this.lostWindow.Show();
            }
            this.gameplayWindow.Close();
        }

        private void LostWindow_BackToMenu()
        {
            this.menuWindow = new MenuWindow();
            settingUpMenuWindow();
            this.lostWindow.Close();
        }

        private void WinWindow_BackToMenu()
        {
            this.menuWindow = new MenuWindow();
            settingUpMenuWindow();
            this.winWindow.Close();
        }

        private void showRanking(string text)
        {
            List<RankingPlayer> rankingPlayers = JsonConvert.DeserializeObject<List<RankingPlayer>>(text);
            this.rankingWindow = new RankingWindow();
            this.rankingWindow.BackToMenu += RankingWindow_BackToMenu;
            this.rankingWindow.Show();
            this.menuWindow.Close();
            foreach (var rank in rankingPlayers)
            {
                rankingWindow.DataGrid.Items.Add(rank);
            }

        }

        private void RankingWindow_BackToMenu()
        {
            this.menuWindow = new MenuWindow();
            this.rankingWindow.Close();
            settingUpMenuWindow();
        }

        private bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return Application.Current.Windows.OfType<T>().Any(w => w.GetType().Name.Equals(name));
        }
    }
}
