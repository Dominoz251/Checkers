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

namespace Warcaby.Pages.Gameplay
{
    /// <summary>
    /// Logika interakcji dla klasy WinWindow.xaml
    /// </summary>
    public partial class WinWindow : Window
    {
        public delegate void BacToMenuEventHandler();
        public event BacToMenuEventHandler BackToMenu;


        protected virtual void OnBackToMenu()
        {
            if (BackToMenu != null)
                BackToMenu();
        }
        public WinWindow()
        {
            InitializeComponent();
        }


        private void BackToMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            OnBackToMenu();
        }
    }
}
