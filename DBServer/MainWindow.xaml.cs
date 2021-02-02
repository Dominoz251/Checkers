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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DBServer.Server.GameLogic;


namespace DBServer
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        global::Server.Server server = new global::Server.Server();

        public MainWindow()
        {
            InitializeComponent();
            server.MessageSender += showMessage;
            server.NewPlayer += showNewPlayer;
            server.DeletePlayer += deletePlayer;
            server.SetupServer();
        }


        public void showMessage(string message)
        {
            // textBlock.Text += DateTime.Now.ToLongTimeString()+": "+message+"\n";
            Dispatcher.Invoke(new Action((() =>
                    textBlock.Text += DateTime.Now.ToLongTimeString() + ": " + message + "\n"
                )));
        }

        private void showNewPlayer(Player player)
        {
            Dispatcher.Invoke(new Action((() => playersListBox.Items.Add(player.login))));
        }

        private void deletePlayer(Player player)
        {
            Dispatcher.Invoke(new Action((() => playersListBox.Items.Remove(player.login))));
        }

    }
}
