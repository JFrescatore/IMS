using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CinderUtility { 

    public class Utility {
        public Utility()
        {
            if (!Directory.Exists("resources"))
                Directory.CreateDirectory("resources");
            if(!Directory.Exists("resources/profiles"))
                Directory.CreateDirectory("resources/profiles");
        }
    }

    public class DataRegistry
    {
        public DataRegistry() {
            database = new string[0, 2];
            if (!Directory.Exists("registry"))
                Directory.CreateDirectory("registry");
            if (!File.Exists("registry/registry.txt"))
                File.Create("registry/registry.txt").Close();
            else
                LoadUsers();
        }

        public void CreateUser(string user, string password, string salt)
        {
            Crytography.SHA256 sha = new Crytography.SHA256(password, salt);
            string hash = sha.ToString();
            RegisterUser(user.ToLower(), hash, true);
        }

        public bool LogInUser(string user, string input, string salt)
        {
            Crytography.SHA256 sha = new Crytography.SHA256(input, salt);
            string attempted_Input = sha.ToString();
            user = user.ToLower();
            for (int i = 0; i < database.Length / 2; i++)
            {
                if (database[i, 0] == user)
                    if (database[i, 1] == attempted_Input)
                        return true;
            }
            return false;
        }

        public bool RegisteredUser(string user) {
            user = user.ToLower();
            for (int i = 0; i < database.Length / 2; i++)
                if (user == database[i, 0])
                    return true;
            return false;
        }
        
        /* First element of database is the pointer to array, the second is the element of that array 
         * database[i] = [user, password]. 
         * database[i, 0] = user
         * database[i, 1] = password
        */
        public void RegisterUser(string user, string password, bool save = false)
        {
            user = user.ToLower();
            if (RegisteredUser(user))
                return;
            string[,] temp = new string[(database.Length / 2) + 1, 2];
            for (int i = 0; i < database.Length / 2; i++)
                for (int j = 0; j < 2; j++)
                    temp[i, j] = database[i, j];
            temp[(temp.Length / 2) - 1, 0] = user;
            temp[(temp.Length / 2) - 1, 1] = password;
            database = temp;
            if (save)
                SaveUsers();
        }

        public void DeleteUser(string user, string password) {
            user = user.ToLower();
            string[,] temp = new string[(database.Length / 2) + 1, 2];
            for(int i = 0, j = 0; i < database.Length / 2; i++)
            {
                if(database[i, 0] != user)
                {
                    temp[j, 0] = database[i, 0];
                    temp[j, 1] = database[i, 1];
                    j++;
                }
            }
            database = temp;
        }

        public void SaveUsers() {
            string line = "";
            for(int i = 0; i < database.Length / 2; i++)
                line += database[i, 0] + "." + database[i, 1] + "\n";
            try
            {
                StreamWriter sw = new StreamWriter("registry/registry.txt");
                sw.Write(line);
                sw.Close();
                LoadUsers();
            } catch { }
        }

        public void LoadUsers() {
            StreamReader sr = new StreamReader("registry/registry.txt");
            string line = "";
            while((line = sr.ReadLine()) != null)
            {
                string[] split = line.Split('.');
                if (split.Length != 2)
                    continue;
                RegisterUser(split[0], split[1]);
            }
            sr.Close();
        }
        private string[,] database;
    }

    namespace Crytography
    {
        public class SHA256
        {
            public SHA256() { }
            public SHA256(string input, string salt) {
                Create(input, salt);
            }

            public void Create(string input, string salt) {
                using (System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create())
                    hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input + salt + "*A28%2hJ"));
            }

            public override string ToString() {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                    builder.Append(hash[i].ToString("x2"));
                return builder.ToString(); 
            }
            private byte[] hash;
        }
    }

}

