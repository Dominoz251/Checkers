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
using Warcaby.Pages.Gameplay;

namespace Warcaby.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        public delegate void PlayGameEventHandler();
        public event PlayGameEventHandler PlayGame;

        public delegate void LogoutEventHandler();
        public event LogoutEventHandler Logout;

        public delegate void ShowRankingEventHandler();
        public event ShowRankingEventHandler ShowRanking;

        public MenuWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

        }

        protected virtual void OnPlayGame()
        {
            if (PlayGame != null)
                PlayGame();
        }
        
        protected virtual void OnLogout()
        {
            if (Logout != null)
                Logout();
        }

        protected virtual void OnShowRankign()
        {
            if (ShowRanking != null)
                ShowRanking();
        }

        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            OnLogout();
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            OnPlayGame();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnShowRankign();
        }
    }
}
