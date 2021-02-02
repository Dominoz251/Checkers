using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Warcaby.Messages;

namespace Warcaby.Pages.Gameplay
{
    /// <summary>
    /// Logika interakcji dla klasy GameplayWindow.xaml
    /// </summary>
    public partial class GameplayWindow : Window
    {

        public delegate void WantMoveEventHandler(string message);
        public event WantMoveEventHandler WantMove;

        public delegate void MoveEventHandler(string message);
        public event MoveEventHandler Move;


        public delegate void DeletePawnEventHandler(string message);
        public event DeletePawnEventHandler DeletePawn;

        public Button[,] fields;
        public Ellipse[] whitePawns;
        public Ellipse[] redPawns;

        public GameplayWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            fields = new Button[8, 8] {
                {A1Button, B1Button, C1Button, D1Button, E1Button, F1Button, G1Button, H1Button},
                {A2Button, B2Button, C2Button, D2Button, E2Button, F2Button, G2Button, H2Button},
                {A3Button, B3Button, C3Button, D3Button, E3Button, F3Button, G3Button, H3Button},
                {A4Button, B4Button, C4Button, D4Button, E4Button, F4Button, G4Button, H4Button},
                {A5Button, B5Button, C5Button, D5Button, E5Button, F5Button, G5Button, H5Button},
                {A6Button, B6Button, C6Button, D6Button, E6Button, F6Button, G6Button, H6Button},
                {A7Button, B7Button, C7Button, D7Button, E7Button, F7Button, G7Button, H7Button},
                {A8Button, B8Button, C8Button, D8Button, E8Button, F8Button, G8Button, H8Button}
            };

            whitePawns = new Ellipse[12] { W1, W2, W3, W4, W5, W6, W7, W8, W9, W10, W11, W12 };
            redPawns = new Ellipse[12] { R1, R2, R3, R4, R5, R6, R7, R8, R9, R10, R11, R12 };
        }
        //ewent wywolywany w momencie wskazania popla na ktore ma sie ruszyc pionek
        protected virtual void OnWantMove(string message)
        {
            if (WantMove != null)
                WantMove(message);
        }

        protected virtual void OnMove(string message)
        {
            List<PawnToDelete> pawnsToDelete = DeletePawns();
            if (pawnsToDelete.Count > 0)
            {
                var pawnsToDeleteStr = JsonConvert.SerializeObject(pawnsToDelete);
                OnDeletePawn(pawnsToDeleteStr);
            }
            if (Move != null)
                Move(message);
        }

        protected virtual void OnDeletePawn(string message)
        {
            if (DeletePawn != null)
                DeletePawn(message);
        }

        private List<PawnToDelete> DeletePawns()
        {
            List<PawnToDelete> pawnToDelete = new List<PawnToDelete>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (fields[i, j].Background == Brushes.GreenYellow)
                    {
                        pawnToDelete.Add(new PawnToDelete(i, j));
                    }
                }
            }

            return pawnToDelete;
        }

        private bool canMove()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (fields[i, j].Background == Brushes.LightGreen)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        private void A1Button_Click(object sender, RoutedEventArgs e)
        {
            if (A1Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 0, 0);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }

            else if (canMove() || fields[0, 0].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(0, 0);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }


        private void A2Button_Click(object sender, RoutedEventArgs e)
        {
            if (A2Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 1, 0);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[1,0].Background==Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(1, 0);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void A3Button_Click(object sender, RoutedEventArgs e)
        {
            if (A3Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 2, 0);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[2, 0].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(2, 0);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void A4Button_Click(object sender, RoutedEventArgs e)
        {
            if (A4Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 3, 0);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[3, 0].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(3, 0);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void A5Button_Click(object sender, RoutedEventArgs e)
        {
            if (A5Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 4, 0);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[4, 0].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(4, 0);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void A6Button_Click(object sender, RoutedEventArgs e)
        {
            if (A6Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 5, 0);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[5, 0].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(5, 0);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void A7Button_Click(object sender, RoutedEventArgs e)
        {
            if (A7Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 6, 0);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[6, 0].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(6, 0);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void A8Button_Click(object sender, RoutedEventArgs e)
        {
            if (A8Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 7, 0);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[7, 0].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(7, 0);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void B1Button_Click(object sender, RoutedEventArgs e)
        {
            if (B1Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 0, 1);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[0, 1].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(0, 1);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void B2Button_Click(object sender, RoutedEventArgs e)
        {
            if (B2Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 1, 1);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[1, 1].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(1, 1);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void B3Button_Click(object sender, RoutedEventArgs e)
        {
            if (B3Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 2, 1);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[2, 1].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(2, 1);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void B4Button_Click(object sender, RoutedEventArgs e)
        {
            if (B4Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 3, 1);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[3, 1].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(3, 1);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void B5Button_Click(object sender, RoutedEventArgs e)
        {
            if (B5Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 4, 1);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[4, 1].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(4, 1);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void B6Button_Click(object sender, RoutedEventArgs e)
        {
            if (B6Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 5, 1);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[5, 1].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(5, 1);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void B7Button_Click(object sender, RoutedEventArgs e)
        {
            if (B7Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 6, 1);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[6, 1].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(6, 1);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void B8Button_Click(object sender, RoutedEventArgs e)
        {
            if (B8Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 7, 1);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[7, 1].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(7, 1);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void C1Button_Click(object sender, RoutedEventArgs e)
        {
            if (C1Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 0, 2);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[0, 2].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(0, 2);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void C2Button_Click(object sender, RoutedEventArgs e)
        {
            if (C2Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 1, 2);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[1, 2].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(1, 2);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void C3Button_Click(object sender, RoutedEventArgs e)
        {
            if (C3Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 2, 2);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[2, 2].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(2, 2);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void C4Button_Click(object sender, RoutedEventArgs e)
        {
            if (C4Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 3, 2);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[3, 2].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(3, 2);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void C5Button_Click(object sender, RoutedEventArgs e)
        {
            if (C5Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 4, 2);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[4, 2].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(4, 2);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void C6Button_Click(object sender, RoutedEventArgs e)
        {
            if (C6Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 5, 2);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[5, 2].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(5, 2);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void C7Button_Click(object sender, RoutedEventArgs e)
        {
            if (C7Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 6, 2);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[6, 2].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(6, 2);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void C8Button_Click(object sender, RoutedEventArgs e)
        {
            if (C8Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 7, 2);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[7, 2].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(7, 2);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void D1Button_Click(object sender, RoutedEventArgs e)
        {
            if (D1Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 0, 3);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[0, 3].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(0, 3);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void D2Button_Click(object sender, RoutedEventArgs e)
        {
            if (D2Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 1, 3);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[1, 3].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(1, 3);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void D3Button_Click(object sender, RoutedEventArgs e)
        {
            if (D3Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 2, 3);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[2, 3].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(2, 3);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void D4Button_Click(object sender, RoutedEventArgs e)
        {
            if (D4Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 3, 3);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[3, 3].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(3, 3);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void D5Button_Click(object sender, RoutedEventArgs e)
        {
            if (D5Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 4, 3);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[4, 3].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(4, 3);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void D6Button_Click(object sender, RoutedEventArgs e)
        {
            if (D6Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 5, 3);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[5, 3].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(5, 3);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void D7Button_Click(object sender, RoutedEventArgs e)
        {
            if (D7Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 6, 3);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[6, 3].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(6, 3);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void D8Button_Click(object sender, RoutedEventArgs e)
        {
            if (D8Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 7, 3);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[7, 3].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(7, 3);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void E1Button_Click(object sender, RoutedEventArgs e)
        {
            if (E1Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 0, 4);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[0, 4].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(0, 4);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void E2Button_Click(object sender, RoutedEventArgs e)
        {
            if (E2Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 1, 4);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[1, 4].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(1, 4);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void E3Button_Click(object sender, RoutedEventArgs e)
        {
            if (E3Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 2, 4);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[2, 4].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(2, 4);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void E4Button_Click(object sender, RoutedEventArgs e)
        {
            if (E4Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 3, 4);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[3, 4].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(3, 4);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void E5Button_Click(object sender, RoutedEventArgs e)
        {
            if (E5Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 4, 4);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[4, 4].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(4, 4);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void E6Button_Click(object sender, RoutedEventArgs e)
        {
            if (E6Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 5, 4);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[5, 4].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(5, 4);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void E7Button_Click(object sender, RoutedEventArgs e)
        {
            if (E7Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 6, 4);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[6, 4].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(6, 4);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void E8Button_Click(object sender, RoutedEventArgs e)
        {
            if (E8Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 7, 4);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[7, 4].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(7, 4);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void F1Button_Click(object sender, RoutedEventArgs e)
        {
            if (F1Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 0, 5);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[0, 5].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(0, 5);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void F2Button_Click(object sender, RoutedEventArgs e)
        {
            if (F2Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 1, 5);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[1, 5].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(1, 5);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void F3Button_Click(object sender, RoutedEventArgs e)
        {
            if (F3Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 2, 5);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[2, 5].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(2, 5);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void F4Button_Click(object sender, RoutedEventArgs e)
        {
            if (F4Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 3, 5);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[3, 5].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(3, 5);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void F5Button_Click(object sender, RoutedEventArgs e)
        {
            if (F5Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 4, 5);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[4, 5].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(4, 5);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void F6Button_Click(object sender, RoutedEventArgs e)
        {
            if (F6Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 5, 5);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[5, 5].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(5, 5);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void F7Button_Click(object sender, RoutedEventArgs e)
        {
            if (F7Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 6, 5);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[6, 5].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(6, 5);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void F8Button_Click(object sender, RoutedEventArgs e)
        {
            if (F8Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 7, 5);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[7, 5].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(7, 5);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void G1Button_Click(object sender, RoutedEventArgs e)
        {
            if (G1Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 0, 6);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[0, 6].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(0, 6);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void G2Button_Click(object sender, RoutedEventArgs e)
        {
            if (G2Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 1, 6);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[1, 6].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(1, 6);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void G3Button_Click(object sender, RoutedEventArgs e)
        {
            if (G3Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 2, 6);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[2, 6].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(2, 6);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void G4Button_Click(object sender, RoutedEventArgs e)
        {
            if (G4Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 3, 6);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[3, 6].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(3, 6);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void G5Button_Click(object sender, RoutedEventArgs e)
        {
            if (G5Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 4, 6);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[4, 6].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(4, 6);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void G6Button_Click(object sender, RoutedEventArgs e)
        {
            if (G6Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 5, 6);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[5, 6].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(5, 6);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void G7Button_Click(object sender, RoutedEventArgs e)
        {
            if (G7Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 6, 6);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[6, 6].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(6, 6);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void G8Button_Click(object sender, RoutedEventArgs e)
        {
            if (G8Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 7, 6);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[7, 6].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(7, 6);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void H1Button_Click(object sender, RoutedEventArgs e)
        {
            if (H1Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 0, 7);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[0, 7].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(0, 7);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void H2Button_Click(object sender, RoutedEventArgs e)
        {
            if (H2Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 1, 7);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[1, 7].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(1, 7);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void H3Button_Click(object sender, RoutedEventArgs e)
        {
            if (H3Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 2, 7);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[2, 7].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(2, 7);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void H4Button_Click(object sender, RoutedEventArgs e)
        {
            if (H4Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 3, 7);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[3, 7].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(3, 7);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void H5Button_Click(object sender, RoutedEventArgs e)
        {
            if (H5Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 4, 7);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[4, 7].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(4, 7);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void H6Button_Click(object sender, RoutedEventArgs e)
        {
            if (H6Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 5, 7);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[5, 7].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(5, 7);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void H7Button_Click(object sender, RoutedEventArgs e)
        {
            if (H7Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 6, 7);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[6, 7].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(6, 7);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }

        private void H8Button_Click(object sender, RoutedEventArgs e)
        {
            if (H8Button.Background == Brushes.Blue)
            {
                Move move;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fields[i, j].Background == Brushes.Yellow)
                        {
                            move = new Move(i, j, 7, 7);
                            string moveStr = JsonConvert.SerializeObject(move);
                            OnMove(moveStr);
                        }
                    }
                }
            }
            else if (canMove() || fields[7, 7].Background == Brushes.LightYellow)
            {
                WantMove pawn = new WantMove(7, 7);
                var pawnStr = JsonConvert.SerializeObject(pawn);
                OnWantMove(pawnStr);
            }
        }
    }
}
