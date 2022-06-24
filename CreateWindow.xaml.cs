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
    /// Interaction logic for NewAccountWindow.xaml
    /// </summary>
    public partial class CreateWindow : Window
    {
        public CreateWindow()
        {
            InitializeComponent();
        }

        public bool CheckInputs()
        {
            DataRegistry registry = new DataRegistry();
            if (FirstName.Text.Length <= 0 || FirstName.Text == " ")
            {
                WarningReadout.Visibility = Visibility.Visible;
                ErrorReadout.Text = "Must Enter Valid First Name";
                return false;
            }

            if (LastName.Text.Length <= 0 || LastName.Text == " ")
            {
                WarningReadout.Visibility = Visibility.Visible;
                ErrorReadout.Text = "Must Enter Valid Last Name";
                return false;
            }

            if (Username.Text.Length <= 0 || Username.Text == " ")
            {
                WarningReadout.Visibility = Visibility.Visible;
                ErrorReadout.Text = "Must Enter Valid Username";
                return false;
            }
            else if (registry.RegisteredUser(Username.Text))
            {
                WarningReadout.Visibility = Visibility.Visible;
                ErrorReadout.Text = "Username Unavailble";
                return false;
            }

            if (Password.Password.Length < 8)
            {
                WarningReadout.Visibility = Visibility.Visible;
                ErrorReadout.Text = "Password Must be 8 Characters Long";
                return false;
            }

            if (Password.Password != ConfirmPassword.Password)
            {
                WarningReadout.Visibility = Visibility.Visible;
                ErrorReadout.Text = "Passwords do not Match";
                return false;
            }

            WarningReadout.Visibility = Visibility.Hidden;
            return true;
        }

        public void CreateAccount(object sender, RoutedEventArgs e)
        {
            if (!CheckInputs())
                return;
            DataRegistry registry = new DataRegistry();
            Profile profile = new Profile(FirstName.Text, LastName.Text, Username.Text);
            profile.SaveProfile();
            registry.CreateUser(Username.Text, Password.Password, profile.salt);
            MainWindow mw = new MainWindow(profile);
            mw.Show();
            Close();
        }
        public void Cancel(object sender, RoutedEventArgs e)
        {
            EntryWindow ew = new EntryWindow();
            ew.Show();
            Close();
        }
    }
}
