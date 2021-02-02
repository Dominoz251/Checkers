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

namespace Warcaby.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy RankingWindow.xaml
    /// </summary>
    public partial class RankingWindow : Window
    {

        public delegate void BacToMenuEventHandler();
        public event BacToMenuEventHandler BackToMenu;

        protected virtual void OnBackToMenu()
        {
            if (BackToMenu != null)
                BackToMenu();
        }
        public RankingWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void BackToMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            OnBackToMenu();
        }
    }
}
