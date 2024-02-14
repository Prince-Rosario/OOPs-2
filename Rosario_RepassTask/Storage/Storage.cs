using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Rosario_repassTask
{
    public class Storage
    {
        private static Storage? _Storage = null;
        private static readonly object Lock = new object();

        public static List<Group> Groups { get; set; }
        public List<Person> Users { get; set; }
        public Person? ActiveUser { get; set; }

        public static List<Discipline> Disciplines { get; set; }

        // Define the path to the Storage directory
        private static readonly string StorageDirectory = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..\\..\\..\\Storage"
        );

        // Define the path to the users.json file inside the Storage directory
        public static readonly string UsersFilePath = Path.Combine(StorageDirectory, "users.json");

        // Define Path to SuperUser
        private static readonly string SuperUserPath = Path.Combine(
            StorageDirectory,
            "ApplicationSettings.json"
        );

        // define the path to the groups.json file inside the Storage directory
        private static readonly string GroupsFilePath = Path.Combine(
            StorageDirectory,
            "groups.json"
        );

        private static readonly string DisciplinesFilePath = Path.Combine(
            StorageDirectory,
            "disciplines.json"
        );

        // Save users data to JSON file
        public static void SaveUsersData(Person[] users)
        {
            // Serialize users to JSON
            string json = JsonConvert.SerializeObject(
                users,
                Formatting.Indented,
                new JsonSerializerSettings { Converters = new[] { new StringEnumConverter() } }
            );
            // Write JSON data to users.json file
            try
            {
                File.WriteAllText(UsersFilePath, json);
            }
            catch (Exception eg)
            {
                Console.WriteLine($"Error saving Users.JSON file: {eg.Message}");
            }
        }

        // Save groups data to JSON file
        public static void SaveGroupsData(Group[] groups)
        {
            // Serialize groups to JSON
            string json = JsonConvert.SerializeObject(groups, Formatting.Indented);
            // Write JSON data to groups.json file
            try
            {
                File.WriteAllText(GroupsFilePath, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving Groups.JSON file: {e.Message}");
            }
        }

        public static void SaveDisciplinesData(Discipline[] disciplines)
        {
            // Serialize disciplines to JSON
            string json = JsonConvert.SerializeObject(disciplines, Formatting.Indented);
            // Write JSON data to disciplines.json file
            try
            {
                File.WriteAllText(DisciplinesFilePath, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving Disciplines.JSON file: {e.Message}");
            }
        }

        // Load users data from JSON file
        public static Person[] LoadUsersData()
        {
            if (File.Exists(UsersFilePath))
            {
                // Read JSON data from users.json file
                string json = File.ReadAllText(UsersFilePath);
                // Deserialize JSON data to List of Person
                List<Person> users =
                    JsonConvert.DeserializeObject<List<Person>>(
                        json,
                        new JsonSerializerSettings
                        {
                            Converters = new JsonConverter[]
                            {
                                new PersonConvertor(),
                                new StringEnumConverter()
                            }
                        }
                    ) ?? new List<Person>();

                return users.ToArray();
            }
            else
            {
                return new Person[0]; // Return an empty array if file doesn't exist
            }
        }

        // Load groups data from JSON file
        public static Group[] LoadGroupsData()
        {
            try
            {
                if (File.Exists(GroupsFilePath))
                {
                    // Read JSON data from groups.json file
                    string json = File.ReadAllText(GroupsFilePath);
                    // Deserialize JSON data to Group array
                    return JsonConvert.DeserializeObject<Group[]>(json) ?? new Group[0];
                }
                else
                {
                    Console.WriteLine($"Groups file does not exist at path: {GroupsFilePath}");
                    return new Group[0]; // Return an empty array if file doesn't exist
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading Groups.JSON file: {e.Message}");
                return new Group[0];
            }
        }

        public void LoadSuperUser()
        {
            if (File.Exists(SuperUserPath))
            {
                // Read JSON data from ApplicationSettings.json file
                string json = File.ReadAllText(SuperUserPath);
                // Deserialize JSON data to a JObject
                JObject superUserObject = JsonConvert.DeserializeObject<JObject>(json);

                // Extract the super user details from the JObject
                string email = superUserObject["AdminInfo"]["email"].ToString();
                string password = superUserObject["AdminInfo"]["password"].ToString();

                // Create a new instance of the super user
                Person superUser = new Person
                {
                    Email = email,
                    Password = Person.HashPassword(password),
                    UserRole = Role.Admin,
                    FirstName = "Admin",
                    LastName = "Adminov",
                    SecondName = "Adminovich",
                    DateOfBirth = new DateTime(1970, 1, 1)
                };

                // Set the super user as the active user
                ActiveUser = superUser;
            }
            else
            {
                Console.WriteLine("SuperUser file does not exist.");
            }
        }

        public static Discipline[] LoadDisciplinesData()
        {
            if (File.Exists(DisciplinesFilePath))
            {
                // Read JSON data from disciplines.json file
                string json = File.ReadAllText(DisciplinesFilePath);
                // Deserialize JSON data to Discipline array
                Discipline[] disciplines =
                    JsonConvert.DeserializeObject<Discipline[]>(json) ?? new Discipline[0];

                Console.WriteLine("Loaded disciplines:");
                foreach (var discipline in disciplines)
                {
                    Console.WriteLine(
                        $"ID: {discipline.Id}, Name: {discipline.Name}, Hours: {discipline.Hours}"
                    );
                }

                return disciplines;
            }
            else
            {
                Console.WriteLine("Disiplines file does not exist");
                return new Discipline[0]; // Return an empty array if file doesn't exist
            }
        }

        public static void LoadAllData()
        {
            Groups = new List<Group>(LoadGroupsData());
            Disciplines = new List<Discipline>(LoadDisciplinesData());
            if (_Storage != null)
            {
                _Storage.Users = new List<Person>(LoadUsersData());
                _Storage.LoadSuperUser();
            }
        }

        public static void SaveAllData()
        {
            SaveUsersData(Instance.Users.ToArray());
            SaveGroupsData(Groups.ToArray());
            SaveDisciplinesData(Disciplines.ToArray());
        }

        public void AddUser(Person user) //adding a person
        {
            Users.Add(user);
            string json = JsonConvert.SerializeObject(Users, Formatting.Indented);
            File.WriteAllText(UsersFilePath, json);
        }

        public void RemovePerson(Person user) //removing a person
        {
            if (user != null)
                Users.Remove(user);
        }

        public void EndActiveSession(Person mudinju) //clearing activeSession
        {
            if (mudinju == ActiveUser)
                ActiveUser = null;
        }

        private Storage()
        {
            this.Users = new List<Person>();
            this.ActiveUser = null;
        }

        public static Storage Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_Storage == null)
                    {
                        _Storage = new Storage();
                    }

                    return _Storage;
                }
            }
        }
    }
}
