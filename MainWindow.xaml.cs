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
using CinderXAML;
using CinderUtility;
using System.IO;

namespace IMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Profile profile;

        Grid firearm_Grid;
        Firearm[] firearms;
        EntryElement[] firearm_Entries;
        int firearm_Page;
        int firearm_Max_Page;
        public MainWindow(Profile p)
        {
            InitializeComponent();
            profile = p;
            firearm_Grid = FirearmDisplay;
            /*
            for(int i = 0; i < 999; i++)
            {
                string serial = "TEST" + i;
                Firearm firearm = new Firearm(serial, "","","", profile.username);
                firearm.SaveFirearm();
                profile.AddFirearm(firearm);
            }
            */
            

            BuildFirearmDisplay();
        }

        public void BuildFirearmDisplay()
        {
            firearm_Page = 1;
            firearm_Max_Page = profile.registered_Firearm / 10 + 1;
            FirearmPageReadout.Text = firearm_Page + "/" + firearm_Max_Page;
            if (firearm_Page >= firearm_Max_Page)
                FirearmNext.IsEnabled = false;
            if (firearm_Page <= 1)
                FirearmPrevious.IsEnabled = false;

            firearm_Entries = new EntryElement[10];
            firearms = new Firearm[profile.registered_Firearm];

            for(int i = 0; i < profile.registered_Firearm; i++)
            {
                Firearm firearm = new Firearm(profile.serials[i, 0]);
                firearms[i] = firearm;
            }
            ReloadFirearmDisplay();
        }

        public void ReloadFirearmDisplay()
        {
            for(int i = (firearm_Page - 1) * 10, j = 0; j < 10; j++)
            {
                EntryElement entry;
                if (i >= firearms.Length)
                {
                    entry = new EntryElement(i, j);
                    entry.DisplayElement(FirearmDisplay);
                    i++;
                    continue;
                }

                entry = new EntryElement(firearms[i].serial, i, j);
                entry.DisplayElement(FirearmDisplay);
                i++;
            }
        }

        public void Previous(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if(button.Name == "FirearmPrevious")
            {
                firearm_Page--;
                if (firearm_Page <= 1)
                    FirearmPrevious.IsEnabled = false;
                FirearmNext.IsEnabled = true;
                FirearmPageReadout.Text = firearm_Page + "/" + firearm_Max_Page;
                ReloadFirearmDisplay();
            }
        }

        public void Next(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Name == "FirearmNext")
            {
                firearm_Page++;
                if (firearm_Page >= firearm_Max_Page)
                    FirearmNext.IsEnabled = false;
                FirearmPrevious.IsEnabled = true;
                FirearmPageReadout.Text = firearm_Page + "/" + firearm_Max_Page;
                ReloadFirearmDisplay();
            }
        }
    }
}
