using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Rosario_repassTask
{
    public enum Role
    {
        Student,
        Teacher,
        Admin
    }

    public class Person
    {
        public string FirstName;
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdministrator { get; set; }

        public int Id { get; set; }

        public Role UserRole { get; set; }

        //public int GroupId { get; set; }
        public Group StudentGroup { get; set; }

        // private bool IsTeacher { get; set; }

        public Person() { }

        public Person(string firstName, string lastName, string secondName, DateTime dateOfBirth)
        {
            FirstName = firstName;
            LastName = lastName;
            SecondName = secondName;
            DateOfBirth = dateOfBirth;
        }

        public string GetFullName()
        {
            /* Storage.Instance.ActiveUser.FirstName = FirstName;
             Storage.Instance.ActiveUser.LastName = LastName;
             Storage.Instance.ActiveUser.SecondName = SecondName;*/
            return $"{FirstName} {SecondName} {LastName}";
        }

        public void SetName(string _fullName)
        {
            string[] names = _fullName.Split(' ');
            if (names.Length == 1)
            {
                FirstName = names[0];
                SecondName = "";
                LastName = "";
            }
            else if (names.Length == 2)
            {
                FirstName = names[0];
                SecondName = "";
                LastName = names[1];
            }
            else if (names.Length >= 3)
            {
                FirstName = names[0];
                SecondName = names[1];
                LastName = names[names.Length - 1];
            }
        }

        public void SetBirthDate(DateTime birthDate)
        {
            DateOfBirth = birthDate;
        }

        public void SetCredentials(string email, string password)
        {
            Email = email;
            Password = HashPassword(password);
            Console.WriteLine(email, HashPassword(password));
        }

        //TODO: set as admin
        public void SetAsAdmin(bool isAdmin)
        {
            IsAdministrator = isAdmin;
        }

        /*   public void isteacher (bool isTeacher)
           {
               IsTeacher = isTeacher;
           } */



        public static bool IsValidEmail(string email)
        {
            string pattern =
                @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                + "@"
                + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
            return Regex.IsMatch(email, pattern);
        }

        public static Person GetPersonByEmail(string email, List<Person> users)
        {
            return users.Find(user => user != null && user.Email == email);
        }

        public string GetPassword()
        {
            return Password;
        }

        internal static string HashPassword(string password)
        {
            using (SHA256 hash = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = hash.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
