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

namespace Warcaby.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy LoggingWindow.xaml
    /// </summary>
    public partial class LoggingWindow : Window
    {
        public delegate void SendMessageEventHandler(string message);
        public event SendMessageEventHandler SendMessage;

        public delegate void NewAccountWindowEventHandler();
        public event NewAccountWindowEventHandler NewAccountWindow;

        public string name;

        public LoggingWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        protected virtual void OnSendMessage(string message)
        {
            if (SendMessage != null)
                SendMessage(message);
        }

        protected virtual void OnNewAccountWindow()
        {
            if (NewAccountWindow != null)
                NewAccountWindow();
        }


        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnNewAccountWindow();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //zapytanie do serwera o poprawność danych logowania
            LoginData loginData = new LoginData(Username.Text, PasswordBox.Password);
            var loginDataStr = JsonConvert.SerializeObject(loginData);
            OnSendMessage(loginDataStr);

            name = Username.Text;
        }

    }
}
