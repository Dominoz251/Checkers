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
    /// Logika interakcji dla klasy SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        string username, password, password2;
        public delegate void NewAccountEventHandler(string message);
        public event NewAccountEventHandler NewAccount;
        
        public delegate void LoggingWindowEventHandler();
        public event LoggingWindowEventHandler LoggingWindow;

        public SignUpWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        protected virtual void OnNewAccount(string message)
        {
            if (NewAccount != null)
                NewAccount(message);
        }

        protected virtual void OnLoggingWindow()
        {
            if (LoggingWindow != null)
                LoggingWindow();
        }


        private void signUpButton_Click(object sender, RoutedEventArgs e)
        {
            username = usernameTextBox.Text;
            password = PasswordBox1.Password;
            password2 = PasswordBox2.Password;


            if (username == "" || password == "" || password2 == "") MessageBox.Show("Wypełnij wszystkie pola!");
            else if (password == password2)
            {
                NewLoginData loginData = new NewLoginData(usernameTextBox.Text, password);
                var loginDataStr = JsonConvert.SerializeObject(loginData);
                OnNewAccount(loginDataStr);
            }
            else MessageBox.Show("Wpisane hasła się od siebie różnią!");


        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnLoggingWindow();
        }
    }
}
