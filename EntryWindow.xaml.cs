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
using CinderObjects;
using CinderUtility;
using System.IO;

namespace IMS
{
    /// <summary>
    /// Interaction logic for EntryWindow.xaml
    /// </summary>
    public partial class EntryWindow : Window
    {
        public EntryWindow()
        {
            InitializeComponent();
            WarningReadout.Visibility = Visibility.Hidden;
        }

        public void Login(object sender, RoutedEventArgs e)
        {
            string user = Username.Text;
            string password = Password.Password;

            if (!File.Exists("resources/profiles/" + user + ".json"))
            {
                WarningReadout.Visibility = Visibility.Visible;
                return;
            }

            Profile profile = new Profile(user);
            DataRegistry registry = new DataRegistry();
            if (registry.LogInUser(user, password, profile.salt))
            {
                MainWindow mw = new MainWindow(profile);
                mw.Show();
                Close();
            }
            else
                WarningReadout.Visibility = Visibility.Visible;

        }
        public void Create(object sender, RoutedEventArgs e)
        {
            CreateWindow cw = new CreateWindow();
            cw.Show();
            Close();
        }
    }
}
