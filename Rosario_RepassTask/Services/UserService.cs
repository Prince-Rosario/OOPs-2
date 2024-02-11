using System.Threading.Channels;
using Newtonsoft.Json;

namespace Rosario_repassTask
{
    public class UserService : IUserService
    {
        private List<Person> users;
        private Person? currentUser;

        public UserService()
        {
            var loadedUsers = Storage.LoadUsersData();
            users = loadedUsers != null ? new List<Person>(loadedUsers) : new List<Person>();
        }

        public IEnumerable<Person> GetAllUsers()
        {
            //Console.WriteLine(users);
            return Storage.Instance.Users;
        }

        public void RegisterUser(Person person)
        {
            // Create JsonSerializerSettings and add your custom converter
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new PersonConvertor());

            // Deserialize the existing users using the settings
            List<Person> users =
                JsonConvert.DeserializeObject<List<Person>>(
                    File.ReadAllText(Storage.UsersFilePath),
                    settings
                ) ?? new List<Person>();
            //Add the new user
            users.Add(person);
            string json = JsonConvert.SerializeObject(users, Formatting.Indented, settings);

            // Write the JSON back to the file
            try
            {
                File.WriteAllText(Storage.UsersFilePath, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving Users.JSON file: {e.Message}");
            }

            Storage.Instance.Users.Add(person);
        }

        public bool LoginUser(string email, string password)
        {
            //Storage.LoadUsersData();

            //load superUser
            Storage.Instance.LoadSuperUser();

            //check for match

            if (
                Storage.Instance.ActiveUser != null
                && Storage.Instance.ActiveUser.Email == email
                && Storage.Instance.ActiveUser.Password == Person.HashPassword(password)
            )
            {
                currentUser = Storage.Instance.ActiveUser;
                Console.WriteLine("Logged in as SuperUser");
                return true;
            }

            //if not check for other users

            //Hash the password
            string hashedPassword = Person.HashPassword(password);

            Person person = Person.GetPersonByEmail(email, users);

            if (person != null && person.Password == hashedPassword)
            {
                //currentUser = person;
                Storage.Instance.ActiveUser = person;
                Console.WriteLine("Logged in as Regular user");
                return true;
            }

            Console.WriteLine("False");
            return false;
        }

        public void Logout()
        {
            Storage.Instance.ActiveUser = null;
        }

        public void SetAsAdmin(int userId)
        {
            Console.WriteLine($"Setting user {userId} as admin...");
            Console.WriteLine("Current users:");
            foreach (var user in Storage.Instance.Users)
            {
                Console.WriteLine($"User ID: {user.Id}, Name: {user.FirstName} {user.LastName}");
            }

            foreach (var person in Storage.Instance.Users)
            {
                if (person.Id == userId)
                {
                    person.SetAsAdmin(true);
                    Console.WriteLine($"User {userId} set as admin.");
                    Storage.SaveAllData();
                    return;
                }
            }

            Console.WriteLine($"No user found with ID {userId}.");
        }
    }
}
