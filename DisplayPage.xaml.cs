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
using CinderObjects;
using System.Diagnostics;

namespace IMS
{
    /// <summary>
    /// Interaction logic for DisplayPage.xaml
    /// </summary>
    /// 

    public class QuickEntry : Grid { 
        public QuickEntry(int count, Firearm firearm = null, Ammunition ammunition = null)
        {
            for(int i = 0; i < cols.Length; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(cols[i]);
                ColumnDefinitions.Add(col);
            }
            
            string serial = (firearm != null) ? firearm.GetSerial() : "";
            string caliber = (firearm != null) ? firearm.GetCaliber() : "";
            string platform = (firearm != null) ? firearm.GetPlatform() : "";
            string model = (firearm != null) ? firearm.GetModel() : "";
            SetCount(count + 1);
            SetSerial(serial);
            SetCaliber(caliber);
            SetType(platform);
            SetModel(model);
            SetButtons();
        }

        void SetCount(int count)
        {
            Color color = new Color();
            color.R = 38;
            color.G = 63;
            color.B = 75;
            color.A = 255;

            TextBlock text = new TextBlock();
            text.Style = (Style)FindResource("EntryText");
            text.Text = "[" + count + "]";
            if (count >= 100)
                text.FontSize = 13;

            Children.Add(text);
            SetColumn(text, 0);
            if (count % 2 == 0)
                Background = new SolidColorBrush(color);
        }

        void SetSerial(string serial) {
            TextBlock text = new TextBlock();
            text.Style = (Style)FindResource("EntryText");
            text.Text = serial;
            Children.Add(text);
            SetColumn(text, 1);
        }

        void SetCaliber(string caliber)
        {
            TextBlock text = new TextBlock();
            text.Style = (Style)FindResource("EntryText");
            text.Text = caliber;
            Children.Add(text);
            SetColumn(text, 2);
        }

        void SetType(string type)
        {
            TextBlock text = new TextBlock();
            text.Style = (Style)FindResource("EntryText");
            text.Text = type;
            Children.Add(text);
            SetColumn(text, 3);
        }

        void SetModel(string model)
        {
            TextBlock text = new TextBlock();
            text.Style = (Style)FindResource("EntryText");
            text.Text = model;
            Children.Add(text);
            SetColumn(text, 4);
        }

        void SetButtons()
        {
            delete = new Button();
            delete.Style = (Style)FindResource("HeaderButton");
            delete.Height = 25; delete.Width = 25;
            delete.SetValue(ColumnProperty, 5);
            delete.HorizontalAlignment = HorizontalAlignment.Right;
            delete.Margin = new Thickness(0, 0, 5, 0);
            delete.Content = "D";
            Children.Add(delete);

            edit = new Button();
            edit.Style = (Style)FindResource("HeaderButton");
            edit.Height = 25; edit.Width = 25;
            edit.SetValue(ColumnProperty, 5);
            edit.HorizontalAlignment = HorizontalAlignment.Left;
            edit.Margin = new Thickness(5, 0, 0, 0);
            edit.Content = "E";
            Children.Add(edit);
        }

        public void SetDelete(RoutedEventHandler delete_Func)
        {
            delete.Click += delete_Func;
        }

        int[] cols = { 30, 175, 100, 150, 175, 75 };
        Button delete, edit;
    }

    public partial class DisplayPage : Page
    {
        Profile profile;
        string[,] Serials;
        Grid[] Entries;
        int page, max_Page;

        public DisplayPage(int type, Profile p)
        {
            InitializeComponent();
            Serials = new string[0,0];
            Entries = new Grid[0];
            profile = p;
            page = 1;
            Previous.IsEnabled = false;
            if (type == 0)
            {
                Serial.Content = "Serial Number";
                Type.Content = "Firearm Type";
                Model.Content = "Firearm Model";
                max_Page = (profile.GetRegFirearm() - 1) / 6 + 1;
                BuildFirearms();
            }
            else
            {
                Serial.Content = "Ammunition UPC";
                Type.Content = "Bullet Type";
                Model.Content = "Manufacuterer";
                max_Page = (profile.GetRegAmmo() - 1) / 6 + 1;
            }

            if (max_Page == 0 || max_Page == 1)
            {
                max_Page = 1;
                Next.IsEnabled = false;
            }

            PageCount.Text = "" + page + "/" + max_Page;
        }

        public void BuildFirearms()
        {
            Entries = new QuickEntry[0];
            Serials = profile.GetSerials();
            for(int i = 0; i < Serials.Length / 2; i++)
            {
                Firearm temp = new Firearm(Serials[i,0], true);
                QuickEntry entry = new QuickEntry(i,temp);
                entry.SetDelete(RemoveEntry);
                Grid[] grids = new Grid[Entries.Length + 1];
                for(int j = 0; j < Entries.Length; j++)
                {
                    grids[j] = Entries[j];
                }
                grids[grids.Length - 1] = entry;
                Entries = grids;
            }

            ReloadEntries();
        }

        public void BuildAmmunition()
        {

        }

        public void ReloadEntries()
        {
            Entry_Grid.Children.RemoveRange(2, 6);
            for(int i = 0 + (6 * (page - 1)), j = 1; j < 7; i++)
            {
                if (i >= Entries.Length)
                    break;
                Entries[i].SetValue(Grid.RowProperty, j);
                Entry_Grid.Children.Add(Entries[i]);
                j++;
            }
        }

        public void RemoveEntry(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Delete Entry");
            Button button = (Button)sender;
            Grid entry = (Grid)button.Parent;
            TextBlock serial_Text = (TextBlock)entry.Children[1];
            string serial = serial_Text.Text;
            Debug.WriteLine(serial);
            Firearm firearm = new Firearm(serial, true);
            profile.RemoveFirearm(firearm);
            BuildFirearms();
        }

        public void NextPage(object sender, RoutedEventArgs e)
        {
            page++;
            if (page == max_Page)
                Next.IsEnabled = false;
            if (page > 1)
                Previous.IsEnabled = true;
            PageCount.Text = "" + page + "/" + max_Page;
            ReloadEntries();
        }

        public void PreviousPage(object sender, RoutedEventArgs e)
        {
            page--;
            if (page <= 1)
                Previous.IsEnabled = false;
            if (page < max_Page)
                Next.IsEnabled = true;
            PageCount.Text = "" + page + "/" + max_Page;
            ReloadEntries();
        }
    }
}
