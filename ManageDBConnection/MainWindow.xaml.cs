using MySQLlibrary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ManageDBConnection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _server, _database, _port, _user, _password;

        public MainWindow()
        {
            InitializeComponent();

            ReadConnectionStringFromEnvironment();

            Server.Text = _server;
            DB.Text = _database;
            Port.Text = _port;
            User.Text = _user;
            Password.Text = _password;
        }

        private void btnWriteToEnvironment_Click(object sender, RoutedEventArgs e)
        {
            DataBase.WriteConnectionParametersToEvironment($"server={Server.Text};database={DB.Text};port={Port.Text};user={User.Text};password={Password.Text}");
            MessageBox.Show("Connection string maybe was saved");
        }

        private void ReadConnectionStringFromEnvironment() {
            DataBase.ReadConnectionParametersFromEvironment(out _server, out _database, out _port, out _user, out _password);            
        }
    }
}