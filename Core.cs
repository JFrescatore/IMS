using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using CinderUtility.Crytography;
using CinderObjects;

namespace CinderObjects { 

    public class Profile
    {
        public Profile() {
            first_Name = "[NULL]";
            last_Name = "[NULL]";
            username = "[NULL]";
            salt = "";
            registered_Firearm = 0;
            registered_Ammo = 0;
            serials = new string[0, 2];
            upc = new string[0, 2];
            range_Reports = new string[0];
        }
        public Profile(string first, string last, string user) {
            first_Name = first;
            last_Name = last;
            username = user;
            salt = "";
            Random rand = new Random();
            for (int i = 0; i < 6; i++)
            {
                char character = (char)rand.Next(33, 125);
                if (character == (char)34 || character == (char)39)
                    character = '_';
                    salt += character;
            }
            path = "resources/profiles/" + username + ".json";

            registered_Firearm = 0;
            registered_Ammo = 0;
            serials = new string[0, 2];
            upc = new string[0, 2];
            range_Reports = new string[0];
        }
        public Profile(string identifier) {
            LoadProfile(identifier);
        }
        public Profile(Profile copy) {
            CopyProfile(copy);
        }

        public void LoadProfile(string identifier) {
            StreamReader sr;
            if (!identifier.Contains('/'))
            {
                if (!File.Exists("resources/profiles/" + identifier + ".json"))
                    return;
                sr = new StreamReader("resources/profiles/" + identifier + ".json");
            }
            else
            {
                if (!File.Exists(identifier))
                    return;
                sr = new StreamReader(identifier);
            }

            string fromJSON = "";
            string line = "";
            while ((line = sr.ReadLine()) != null)
                fromJSON += line;
            sr.Close();

            Profile loaded = JsonConvert.DeserializeObject<Profile>(fromJSON);
            first_Name = loaded.first_Name;
            last_Name = loaded.last_Name;
            username = loaded.username;
            salt = loaded.salt;
            serials = loaded.serials;
            registered_Firearm = loaded.registered_Firearm;
            upc = loaded.upc;
            registered_Ammo = loaded.registered_Ammo;
            path = loaded.path;
        }
        public void CopyProfile(Profile copy) {
            first_Name = copy.first_Name;
            last_Name = copy.last_Name;
            username = copy.username;
            salt = copy.salt;
            serials = copy.serials;
            registered_Firearm = copy.registered_Firearm;
            upc = copy.upc;
            registered_Ammo = copy.registered_Ammo;
            path = copy.path;
        }
        public void SaveProfile() {
            if (!File.Exists(path))
                File.Create(path).Close();

            string toJSON = JsonConvert.SerializeObject(this);
            StreamWriter sw = new StreamWriter(path);
            sw.Write(toJSON);
            sw.Close();
        }
        
        public void AddFirearm(Firearm firearm, bool autosave = true) {
            string firearm_Serial = firearm.serial;
            for (int i = 0; i < serials.Length / 2; i++)
                if (firearm_Serial == serials[i, 0])
                    return;

            string file_Path = "resources/firearms/" + firearm_Serial + ".json";

            string[,] temp = new string[serials.Length / 2 + 1, 2];
            for (int i = 0; i < serials.Length / 2; i++)
            {
                temp[i, 0] = serials[i, 0];
                temp[i, 1] = serials[i, 1];
            }
            temp[(temp.Length / 2) - 1, 0] = firearm_Serial;
            temp[(temp.Length / 2) - 1, 1] = file_Path;
            serials = temp;
            //firearm.AddOwner(profile_ID);
            registered_Firearm++;
            if (autosave)
                SaveProfile();
        }
        public void RemoveFirearm(Firearm firearm, bool autosave = true) {
            string firearm_Serial = firearm.serial;
            bool found = false;
            int skip_Index = -1;
            for (int i = 0; i < serials.Length / 2; i++)
                if (firearm_Serial == serials[i, 0])
                {
                    found = true;
                    skip_Index = i;
                    break;
                }

            if (!found)
                return;

            string[,] temp = new string[serials.Length / 2 - 1, 2];
            for (int i = 0, j = 0; i < serials.Length / 2; i++)
            {
                if (i == skip_Index)
                    i++;
                if (i >= serials.Length / 2)
                    break;
                temp[j, 0] = serials[i, 0];
                temp[j, 1] = serials[i, 1];
                j++;
            }
            serials = temp;
            //firearm.RemoveOwner(profile_ID);
            registered_Firearm--;
            if (autosave)
                SaveProfile();
        }

        public void AddAmmunition(Ammunition ammo) { }
        public void RemoveAmmunition(Ammunition ammo) { }

        public void AddRange(RangeReport range) { }
        public void RemoveRange(RangeReport range) { }

        public string first_Name { get; set; }
        public string last_Name { get; set; }
        public string username { get; set; }
        public string salt { get; set; }
        public string path { get; set; }
        public int registered_Firearm { get; set; }
        public string[,] serials { get; set; }
        public int registered_Ammo { get; set; }
        public string[,] upc { get; set; }
        public string[] range_Reports { get; set; }
    }

    public class Firearm
    {
        public Firearm() { }
        public Firearm(string serial, string caliber, string type, string model, string owner) {
            this.serial = serial;
            this.caliber = caliber;
            this.type = type;
            this.model = model;
            this.owner = owner;
            maker = "[NO ENTRY]";
            round_Count = 0;
            path = "resources/firearms/" + serial + "/" + serial + ".json";
            images = new string[0];
            clean_Dates = new string[0];
        }
        public Firearm(string identifier) {
            LoadFirearm(identifier);
        }
        public Firearm(Firearm copy) {
            CopyFirearm(copy);
        }

        public void LoadFirearm(string identifier) {
            StreamReader sr;
            if (!identifier.Contains('/'))
            {
                if (!File.Exists("resources/firearms/" + identifier + "/" + identifier + ".json"))
                    return;
                sr = new StreamReader("resources/firearms/" + identifier + "/" + identifier + ".json");
            }
            else
            {
                if (!File.Exists(identifier))
                    return;
                sr = new StreamReader(identifier);
            }

            string fromJSON = "";
            string line = "";
            while ((line = sr.ReadLine()) != null)
                fromJSON += line;
            sr.Close();

            Firearm loaded = JsonConvert.DeserializeObject<Firearm>(fromJSON);
            serial = loaded.serial;
            caliber = loaded.caliber;
            type = loaded.type;
            model = loaded.model;
            maker = loaded.maker;
            round_Count = loaded.round_Count;
            path = loaded.path;
            owner = loaded.owner;
            images = loaded.images;
            clean_Dates = loaded.clean_Dates;
        }
        public void CopyFirearm(Firearm copy) {
            serial = copy.serial;
            caliber = copy.caliber;
            type = copy.type;
            model = copy.model;
            maker = copy.maker;
            round_Count = copy.round_Count;
            path = copy.path;
            owner = copy.owner;
            images = copy.images;
            clean_Dates = copy.clean_Dates;
        }
        public void EditFirearm(Firearm n_Firearm) {
            if (serial != n_Firearm.serial)
                Directory.Delete("resources/firearms/" + serial, true);
            Profile profile = new Profile(owner);
            profile.RemoveFirearm(this);

            CopyFirearm(n_Firearm);
            SaveFirearm();
            profile.AddFirearm(this);
        }
        public void SaveFirearm() {
            if (!Directory.Exists("resources/firearms/" + serial))
                Directory.CreateDirectory("resources/firearms/" + serial);
            if (!Directory.Exists("resources/firearms/" + serial + "/photos"))
                Directory.CreateDirectory("resources/firearms/" + serial + "/photos");
            if (!File.Exists(path))
                File.Create(path).Close();

            string toJSON = JsonConvert.SerializeObject(this);
            StreamWriter sw = new StreamWriter(path);
            sw.Write(toJSON);
            sw.Close();
        }

        public void AddImage(string image_Path) { }
        public void RemoveImage(string image_Path) { }
        public void AddCleaningDate(string date) { }
        public void AddRoundCount(int count) { }
        
        public static bool operator ==(Firearm right, Firearm left)
        {
            if (right.serial == left.serial)
                return true;
            return false;
        }
        public static bool operator !=(Firearm right, Firearm left)
        {
            if (right.serial == left.serial)
                return false;
            return true;
        }


        public string serial { get; set; }
        public string caliber { get; set; }
        public string type { get; set; }
        public string model { get; set; }
        public string maker { get; set; }
        public int round_Count { get; set; }
        public string path { get; set; }
        public string owner { get; set; }
        public string[] images { get; set; }
        public string[] clean_Dates { get; set; }
    }

    public class Ammunition
    {

    }

    public class RangeReport
    {

    }


    /*
    public class Profile
    {
        public Profile() 
        {
            profile_First = ""; profile_Last = "";
            profile_ID = ""; salt = "";
            serials = new string[0, 2];
            registered_Firearms = 0;
        }
        public Profile(string first, string last, string user)
        {
            profile_First = first; profile_Last = last;
            profile_ID = user;

            Random rand = new Random();
            salt = "";
            for (int i = 0; i < 12; i++)
                salt += (char)rand.Next(33,125);

            file_Path = "resources/profiles/" + profile_ID + "/" + profile_ID + ".json";
            serials = new string[0, 2];
            registered_Firearms = 0;
            upc = new string[0, 2];
            registered_Ammo = 0;
        }
        public Profile(string id)
        {
            LoadProfile(id);
        }
        public Profile(Profile copy)
        {
            profile_First = copy.profile_First;
            profile_Last = copy.profile_Last;
            profile_ID = copy.profile_ID;
            salt = copy.salt;
            file_Path = copy.file_Path;
        }
        public bool LoadProfile(string id) 
        {
            if (!File.Exists("resources/profiles/" + id + "/" + id + ".json"))
                return false;

            StreamReader sr = new StreamReader("resources/profiles/" + id + "/" + id + ".json");
            string fromJSON = "";
            string line = "";
            while ((line = sr.ReadLine()) != null)
                fromJSON += line;
            sr.Close();

            Profile loaded = JsonConvert.DeserializeObject<Profile>(fromJSON);
            profile_First = loaded.profile_First;
            profile_Last = loaded.profile_Last;
            profile_ID = loaded.profile_ID;
            salt = loaded.salt;
            serials = loaded.serials;
            registered_Firearms = loaded.registered_Firearms;
            upc = loaded.upc;
            registered_Ammo = loaded.registered_Ammo;
            file_Path = loaded.file_Path;
            return true;
        }
        public void SaveProfile() 
        {
            if (!Directory.Exists("resources/profiles/" + profile_ID))
                Directory.CreateDirectory("resources/profiles/" + profile_ID);
            if (!File.Exists("resources/profiles/" + profile_ID + "/" + profile_ID + ".json"))
                File.Create("resources/profiles/" + profile_ID + "/" + profile_ID + ".json").Close();

            string toJSON = JsonConvert.SerializeObject(this);
            StreamWriter sw = new StreamWriter(file_Path);
            sw.Write(toJSON);
            sw.Close();
        }

        public void AddFirearm(Firearm firearm)
        {
            string firearm_Serial = firearm.GetSerial();
            for(int i = 0; i < serials.Length / 2; i++)
                if (firearm_Serial == serials[i, 0])
                    return;

            string file_Path = "resources/firearms/" + firearm_Serial + ".json";
            string[] package = { firearm_Serial, file_Path };

            string[,] temp = new string[serials.Length / 2 + 1, 2];
            for (int i = 0; i < serials.Length / 2; i++){
                temp[i, 0] = serials[i, 0];
                temp[i, 1] = serials[i, 1];
            }
            temp[(temp.Length / 2) - 1, 0] = firearm_Serial;
            temp[(temp.Length / 2) - 1, 1] = file_Path;
            serials = temp;
            firearm.AddOwner(profile_ID);
            registered_Firearms++;
            SaveProfile();
        }

        public void RemoveFirearm(Firearm firearm)
        {
            string firearm_Serial = firearm.GetSerial();
            bool found = false;
            int skip_Index = -1;
            for (int i = 0; i < serials.Length / 2; i++)
                if (firearm_Serial == serials[i, 0])
                {
                    found = true;
                    skip_Index = i;
                    break;
                }

            if (!found)
                return;

            string[,] temp = new string[serials.Length / 2 - 1, 2];
            for(int i = 0, j = 0; i < serials.Length / 2; i++)
            {
                if (i == skip_Index)
                    i++;
                if (i >= serials.Length / 2)
                    break;
                temp[j, 0] = serials[i, 0];
                temp[j, 1] = serials[i, 1];
                j++;
            }
            serials = temp;
            firearm.RemoveOwner(profile_ID);
            registered_Firearms--;
            SaveProfile();
        }

        public void SetID(string id) { profile_ID = id; }
        public string GetID() { return profile_ID; }

        public void SetFirst(string first) { profile_First = first; }
        public string GetFirst() { return profile_First; }

        public void SetLast(string last) { profile_Last = last; }
        public string GetLast() { return profile_Last; }

        public string GetSalt() { return salt; }

        public string[,] GetSerials() { return serials; }
        public int GetRegFirearm() { return registered_Firearms; }

        public string[,] GetUPC() { return upc; }
        public int GetRegAmmo() { return registered_Ammo; }


        [JsonProperty] string[,] serials;
        [JsonProperty] int registered_Firearms;

        [JsonProperty] string[,] upc;
        [JsonProperty] int registered_Ammo;

        [JsonProperty] string profile_ID;
        [JsonProperty] string profile_First;
        [JsonProperty] string profile_Last;
        [JsonProperty] string salt;

        [JsonProperty] string file_Path;
    }

    public class Firearm
    {
        public Firearm() {
            serial = "";
            caliber = "";
            platform = "";
            model = "";
            make = "";
            round_Count = 0;
            cleaning_Dates = new string[0];
            range_Dates = new string[0];
            photo_Paths = new string[0];    
            owner = "";
            file_Path = "";
        }
        public Firearm(string serial, string caliber, string platform) {
            this.serial = serial;
            file_Path = "resources/firearms/" + serial + "/" + serial + ".json";
            this.caliber = caliber;
            this.platform = platform;
            model = "";
            make = "";
            round_Count = 0;
            cleaning_Dates = new string[0];
            range_Dates = new string[0];
            photo_Paths = new string[0];
            owner = "";
        }
        public Firearm(string serial, bool load) {
            LoadFirearm(serial);
        }
        public bool LoadFirearm(string serial) {
            if (!File.Exists("resources/firearms/" + serial + "/" + serial + ".json"))
                return false;

            StreamReader sr = new StreamReader("resources/firearms/" + serial + "/" + serial + ".json");
            string fromJSON = "";
            string line = "";
            while ((line = sr.ReadLine()) != null)
                fromJSON += line;
            sr.Close();

            Firearm firearm = JsonConvert.DeserializeObject<Firearm>(fromJSON);
            this.serial = firearm.serial;
            caliber = firearm.caliber;
            platform = firearm.platform;
            model = firearm.model;
            make = firearm.make;
            round_Count = firearm.round_Count;
            cleaning_Dates = firearm.cleaning_Dates;
            range_Dates = firearm.range_Dates;
            owner = firearm.owner;   
            file_Path = firearm.file_Path;
            return true;
        }

        public void SaveFirearm() {
            if (!Directory.Exists("resources/firearms/" + serial))
                Directory.CreateDirectory("resources/firearms/" + serial);
            if (!Directory.Exists("resources/firearms/" + serial + "/photos"))
                Directory.CreateDirectory("resources/firearms/" + serial + "/photos");
            if (!File.Exists("resources/firearms/" + serial + "/" + ".json"))
                File.Create("resources/firearms/" + serial + "/" + serial + ".json").Close();

            string toJSON = JsonConvert.SerializeObject(this);
            StreamWriter sw = new StreamWriter(file_Path);
            sw.Write(toJSON);
            sw.Close();
        }

        public void AddOwner(string new_Owner, bool overwrite = true) {
            if ((owner != "" || owner != null) && !overwrite)
                return;
            owner = new_Owner;
            SaveFirearm();
        }
        public void RemoveOwner(string remove_Owner) {
            owner = "";
            SaveFirearm();
        }
        public string GetOwner() { return owner; }

        public void SetSerial(string s) { serial = s; }
        public string GetSerial() { return serial; }

        public void SetCaliber(string c) { caliber = c; }
        public string GetCaliber() { return caliber; }

        public void SetPlatform(string p) { platform = p; }
        public string GetPlatform() { return platform; }

        public void SetModel(string m) { model = m; }
        public string GetModel() { return model; }

        public void SetMake(string m) { make = m; }
        public string GetMake() { return make; }

        public void AddRounds(int count) { round_Count += count; }
        public void SetRound(int count) { round_Count = count; }
        public int GetRoundCount() { return round_Count; }

        public void AddCleaningDate(string date)
        {
            string[] temp = new string[cleaning_Dates.Length + 1];
            for(int i = 0; i < cleaning_Dates.Length; i++)
                temp[i] = cleaning_Dates[i];
            temp[temp.Length - 1] = date;
            cleaning_Dates = temp;
        }
        public string[] GetCleaning() { return cleaning_Dates; }
        
        public void AddRangeDate(string date)
        {
            string[] temp = new string[range_Dates.Length + 1];
            for (int i = 0; i < range_Dates.Length; i++)
                temp[i] = range_Dates[i];
            temp[temp.Length - 1] = date;
            range_Dates = temp;
        }
        public string[] GetRange() { return range_Dates; }

        public void AddPhoto(string path)
        {
            string[] temp = new string[photo_Paths.Length + 1];
            for (int i = 0; i < photo_Paths.Length; i++)
                temp[i] = photo_Paths[i];
            temp[temp.Length - 1] = path;
            photo_Paths = temp;
        }
        public string[] GetPhoto() { return photo_Paths; }

        [JsonProperty] string serial;
        [JsonProperty] string caliber;
        [JsonProperty] string platform;

        [JsonProperty] string model;
        [JsonProperty] string make;
        [JsonProperty] int round_Count;
        [JsonProperty] string[] cleaning_Dates;
        [JsonProperty] string[] range_Dates;
        [JsonProperty] string[] photo_Paths;

        [JsonProperty] string owner;
        [JsonProperty] string file_Path;
    }
    public class Ammunition { }
    public class RangeReport
    {
        public RangeReport() {
            date = "";
            serials = new string[0];
            upc = new string[0];
            rounds = new int[0];
        }
        public RangeReport(string user, string date, string[] serials, string[] upc, int[] rounds)
        {
            shooter = user;
            this.date = date;
            this.serials = serials;
            this.upc = upc;
            this.rounds = rounds;
            file_Path = "resources/profiles/" + shooter + "/RangeReports/" + date + "_RangeReport" + ".json";
        }

        public RangeReport(string user, string date)
        {
            shooter = user;
            this.date = date;
            LoadReport();
        }

        public void UpdateFirearms()
        {
            for(int i = 0; i < rounds.Length; i++)
            {
                Firearm firearm = new Firearm(serials[i], true);
                firearm.AddRounds(rounds[i]);
                firearm.AddRangeDate(date);
                firearm.SaveFirearm();
            }
        }

        public bool LoadReport()
        {
            if (!File.Exists("resources/profiles/" + shooter + "/RangeReports/" + date + "_RangeReport" + ".json"))
                return false;

            StreamReader sr = new StreamReader("resources/profiles/" + shooter + "/RangeReports/" + date + "_RangeReport" + ".json");
            string fromJSON = "";
            string line = "";
            while ((line = sr.ReadLine()) != null)
                fromJSON += line;
            sr.Close();

            RangeReport report = JsonConvert.DeserializeObject<RangeReport>(fromJSON);
            file_Path = report.file_Path;
            serials = report.serials;
            upc = report.upc;
            rounds = report.rounds;
            return true;
        }

        public void SaveReport()
        {
            if (!Directory.Exists("resources/profiles/" + shooter))
                Directory.CreateDirectory("resources/profiles/" + shooter);
            if (!Directory.Exists("resources/profiles/" + shooter + "/RangeReports"))
                Directory.CreateDirectory("resources/profiles/" + shooter + "/RangeReports");
            if (!File.Exists("resources/profiles/" + shooter + "/RangeReports/" + date + "_RangeReport" + "/" + ".json"))
                File.Create("resources/profiles/" + shooter + "/RangeReports/" + date + "_RangeReport" + ".json").Close();
            

            string toJSON = JsonConvert.SerializeObject(this);
            StreamWriter sw = new StreamWriter(file_Path);
            sw.Write(toJSON);
            sw.Close();
        }

        string file_Path;
        [JsonProperty] string shooter;
        [JsonProperty] string date;
        [JsonProperty] string[] serials;
        [JsonProperty] string[] upc;
        [JsonProperty] int[] rounds;
    }*/
}

namespace CinderXAML
{
    public class EntryElement
    {

        public EntryElement(int element_Index, int index)
        {
            entry_Index = index;
            count = element_Index;
            identifier = "-/-/-/-/-";
            caliber = "-/-/-/-/-";
            type = "-/-/-/-/-";
            model = "-/-/-/-/-";
            maker = "-/-/-/-/-";

            BuildGrid();
            BuildText(widths[1], 1, identifier);
            BuildText(widths[2], 2, caliber);
            BuildText(widths[3], 3, type);
            BuildText(widths[4], 4, model);
            BuildText(widths[5], 5, maker);
            BuildBorders();
        }

        public EntryElement(string i, int element_Index, int index) {
            identifier = i;
            count = element_Index;
            entry_Index = index;
            BuildGrid();
            GatherInfo(identifier);
        }

        public void BuildGrid()
        {
            display = new Grid();
            display.Style = (count % 2 == 0) ? (Style)Application.Current.Resources["AltEntry"] : (Style)Application.Current.Resources["Entry"];

            for (int i = 0; i < widths.Length; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(widths[i]);
                display.ColumnDefinitions.Add(col);
            }

            BuildText(50, 0, "[" + (count + 1) + "]");

            delete_Button = new Button();
            delete_Button.Style = (Style)Application.Current.Resources["EntryButton"];
            delete_Button.Width = 20; delete_Button.Height = 20;
            delete_Button.SetValue(Grid.ColumnProperty, 6);
            delete_Button.HorizontalAlignment = HorizontalAlignment.Right;
            delete_Button.Margin = new Thickness(0, 0, 30, 0);
            delete_Button.IsEnabled = false;
            display.Children.Add(delete_Button);

            edit_Button = new Button();
            edit_Button.Style = (Style)Application.Current.Resources["EntryButton"];
            edit_Button.Width = 20; edit_Button.Height = 20;
            edit_Button.SetValue(Grid.ColumnProperty, 6);
            edit_Button.HorizontalAlignment = HorizontalAlignment.Left;
            edit_Button.Margin = new Thickness(30, 0, 0, 0);
            edit_Button.IsEnabled = false;
            display.Children.Add(edit_Button);
        }

        public void BuildText(int width, int col, string content)
        {
            TextBlock text = new TextBlock();
            text.Style = (Style)Application.Current.Resources["EntryText"];
            text.Width = width;
            text.SetValue(Grid.ColumnProperty, col);
            text.Text = content;
            text.Margin = new Thickness(0,5,0,0);
            display.Children.Add(text);
        }

        public void BuildBorders()
        {
            for(int i = 0; i < widths.Length - 1; i++)
            {
                Border border = new Border();
                border.Style = (Style)Application.Current.Resources["EntryBorder"];
                border.Height = 20;
                border.SetValue(Grid.ColumnProperty, i);
                display.Children.Add(border);
            }
        }

        public void GatherInfo(string identifier, bool isFirearm = true)
        {
            if (isFirearm)
            {
                Firearm firearm = new Firearm(identifier);
                this.identifier = identifier;
                caliber = firearm.caliber;
                type = firearm.type;
                model = firearm.model;
                maker = firearm.maker;
            }

            BuildText(widths[1], 1, identifier);
            BuildText(widths[2], 2, caliber);
            BuildText(widths[3], 3, type);
            BuildText(widths[4], 4, model);
            BuildText(widths[5], 5, maker);
            BuildBorders();
        }

        public void DisplayElement(Grid main_Display)
        {
            main_Display.Children.Add(display);
            display.SetValue(Grid.RowProperty, entry_Index + 1);
        }

        public bool IsEntry(Grid element)
        {
            TextBlock text = (TextBlock)element.Children[1];
            if (text.Text == identifier)
                return true;
            return false;
        }

        public void SetDelete(RoutedEventHandler func)
        {
            delete_Button.IsEnabled = true;
            delete_Button.Click += func;
        }

        public void SetEdit(RoutedEventHandler func)
        {
            edit_Button.IsEnabled = true;
            edit_Button.Click += func;
        }

        public Grid display { get; set; }
        public Button delete_Button { get; set; }
        public Button edit_Button { get; set; }

        int[] widths = { 50, 175, 125, 175, 175, 175, 125 };
        public int entry_Index { get; set; }
        public int count { get; set; }
        public string identifier { get; set; }
        public string caliber { get; set; }
        public string type { get; set; }
        public string model { get; set; }
        public string maker { get; set; }
    }
}



