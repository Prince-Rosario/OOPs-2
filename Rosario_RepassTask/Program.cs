using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Channels;
using System.Xml.Linq;

namespace Rosario_repassTask
{
    class Program
    {
        static IUserService userService = new UserService();

        private static IMainService mainService;

        private static void InitializeMainService()
        {
            mainService = new MainService();
            mainService.LoadProfessors();
            mainService.LoadStudents();
        }

        static void register()
        {
            Console.WriteLine("Enter Your First Name:");
            string firstName = Console.ReadLine();
            Console.WriteLine("Enter Your Last Name:");
            string lastName = Console.ReadLine();
            Console.WriteLine("Enter your last name:");
            string secondName = Console.ReadLine();

            Console.WriteLine("Enter Your Email:");
            string email = Console.ReadLine();
            Console.WriteLine("Enter Your Password:");
            string password = MaskPassword();
            Console.WriteLine("Enter Your Date of Birth:");
            DateTime dateOfBirth = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Enter Your Role (1 for Student, 2 for Teacher):");
            int role = int.Parse(Console.ReadLine());

            if (role == 1)
            {
                //Student Registration
                Console.WriteLine("Enter Your Student ID:");
                int studentId = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter Your Group ID:");
                int groupId = int.Parse(Console.ReadLine());

                if (!Group.IsValidGroup(groupId))
                {
                    Console.WriteLine("Invalid Group ID. Please try again.");
                    return;
                }

                Student newStudent = new Student(
                    firstName,
                    lastName,
                    secondName,
                    dateOfBirth,
                    studentId,
                    groupId
                );
                newStudent.SetCredentials(email, password);
                //newStudent.SetName();
                newStudent.UserRole = Role.Student;

                userService.RegisterUser(newStudent);
                Console.WriteLine("StudentUser Created Successfully!");
            }
            else if (role == 2)
            {
                //Teacher Registration
                Console.WriteLine("Enter Your Teacher ID:");
                int teacherId = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter Your Degree:");
                string degree = Console.ReadLine();
                Console.WriteLine("Enter Your Years Of Experience:");
                int yearsOfExperience = int.Parse(Console.ReadLine());

                Professor newProfessor = new Professor(
                    firstName,
                    lastName,
                    secondName,
                    dateOfBirth,
                    teacherId,
                    1
                );
                newProfessor.SetCredentials(email, password);
                newProfessor.UserRole = Role.Teacher;
                newProfessor.SetDegree(degree);
                newProfessor.SetYears(yearsOfExperience);

                userService.RegisterUser(newProfessor);
                Console.WriteLine("TeacherUser Created Successfully!");
            }
        }

        //TODO: implement login function

        static void login()
        {
            Console.WriteLine("Enter Your Email:");
            string email = Console.ReadLine();
            Console.WriteLine("Enter Your Password: (Masked)");
            string password = MaskPassword();

            bool loginResult = userService.LoginUser(email, password);

            if (loginResult)
            {
                Console.WriteLine("You have been logged in successfully!");
                LoggedInMenu();
            }
            else
            {
                Console.WriteLine("Invalid email or password. Please try again.");
            }
        }

        //TODO: implement logout

        static void logout()
        {
            userService.Logout();
            Console.WriteLine("You have been logged out!");
        }

        //TODO: implement exit function
        static void ExitMenu()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("Exit Screen:");
            Console.WriteLine();
            Console.WriteLine("Goodbye!");
            Environment.Exit(0);
            Storage.Instance.ActiveUser = null;
        }

        //clear

        /*static void clr()
        {
            Console.Clear();
        }*/

        //implement menu function

        static void LoginMenu()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("Login Screen:");
            Console.WriteLine();
            login();
        }

        //Display User Info
        //TODO
        static void DisplayUserInfo()
        {

            if (Storage.Instance.ActiveUser == null)
            {
                Console.WriteLine("No active user");
                return;
            }
            Console.WriteLine();
            Console.WriteLine("User Info:");
            Console.WriteLine();
            if (Storage.Instance.ActiveUser is Student student)
            {
                Console.WriteLine($"Name: {student.GetFullName()}");
                Console.WriteLine($"Role: {student.UserRole}");
                Console.WriteLine($"Student ID: {student.StudentId}");
            }
            else if (Storage.Instance.ActiveUser is Professor professor)
            {
                Console.WriteLine($"Name: {professor.GetFullName()}");
                Console.WriteLine($"Role: {professor.UserRole}");
                Console.WriteLine($"Teacher ID: {professor.ProfId}");
            }
            else
            {
                Console.WriteLine($"Name: {Storage.Instance.ActiveUser.GetFullName()}");
                Console.WriteLine($"Role: {Storage.Instance.ActiveUser.UserRole}");
            }
        }

        static void LoggedInMenu()
        {
            if (Storage.Instance.ActiveUser == null)
            {
                Console.WriteLine("No active user");
                return;
            }

            while (true)
            {
                DisplayUserInfo();
                if (Storage.Instance.ActiveUser == null)
                {
                    break;
                }

                switch (Storage.Instance.ActiveUser.UserRole)
                {
                    case Role.Student:
                        // Call the method to display the student menu
                        StudentMenu();
                        Console.WriteLine("Vada Studentu");
                        break;
                    case Role.Teacher:
                        // Call the method to display the teacher menu
                        TeacherMenu();
                        Console.WriteLine("Vada Teacheru");
                        break;
                    case Role.Admin:
                        // Call the method to display the admin menu
                        AdminMenu();
                        Console.WriteLine("Vada Adminu");
                        break;
                    default:
                        Console.WriteLine("Invalid role");
                        break;
                }

            }

        }

        static void RegistrationMenu()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("Registration Screen:");
            Console.WriteLine();
            register();
        }

        //User specific menus:

        //Student Menu

        static void StudentMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Student Menu:");
            Console.WriteLine();
            Console.WriteLine("1. Show disciplines for my group");
            Console.WriteLine("2. Check my grades");
            Console.WriteLine("3. Logout");
            Console.WriteLine("4. Exit");
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    ShowDisciplinesForGroup();
                    break;
                case "2":
                    CheckGrades();
                    break;
                case "3":
                    logout();
                    break;
                case "4":
                    ExitMenu();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        static void ShowDisciplinesForGroup()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("Disciplines for My Group:");
            Console.WriteLine();

            // Get the current user
            Person activeUser = Storage.Instance.ActiveUser;

            // Check if the current user is a student
            if (activeUser is Student student)
            {
                int groupId = student.StudentGroup.GroupId;
                // Get the student's group
                Group group = mainService.GetAllGroups().FirstOrDefault(g => g.GroupId == groupId);

                // Check if the group is valid
                if (group != null)
                {
                    Console.WriteLine($"Group ID: {group.GroupId}");

                    Console.WriteLine($"Group Name: {group.Name}");

                    List<Discipline> disciplines = group.Disciplines;

                    // Get the disciplines for the student's group
                    if (disciplines != null && disciplines.Count > 0)
                    {
                        Console.WriteLine("Disciplines:");

                        foreach (Discipline discipline in disciplines)
                        {
                            Console.WriteLine(
                                $"ID: {discipline.Id}, Name: {discipline.Name}, Hours: {discipline.Hours}"
                            );
                        }
                    }
                    else
                    {
                        Console.WriteLine("No disciplines found for this group.");
                    }
                }
                else
                {
                    Console.WriteLine($"Group not found with id {groupId}.");
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Active user is not a student.");
            }
        }

        static void CheckGrades()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("My Grades:");
            Console.WriteLine();

            // Get the student's grades
            List<Mark> grades = mainService.GetGradesForStudent();

            // Display the information for each grade
            foreach (Mark grade in grades)
            {
                Console.WriteLine(
                    $"Discipline: {grade.discipline.Name}, Mark: {grade.GetMarkName()}"
                );
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        //Teacher Menu

        static void TeacherMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Teacher Menu:");
            Console.WriteLine();
            Console.WriteLine("1. Show my disciplines");
            Console.WriteLine("2. Logout");
            Console.WriteLine("3. Exit");
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    ShowMyDisciplines();
                    break;
                case "2":
                    logout();
                    break;
                case "3":
                    ExitMenu();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        static void ShowMyDisciplines()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("My Disciplines:");
            Console.WriteLine();

            // Get the list of disciplines where the teacher is assigned
            Professor currentProfessor = Storage.Instance.ActiveUser as Professor;

            if (currentProfessor != null)
            {
                List<Discipline> myDisciplines = new List<Discipline>();
                foreach (var disciplineId in currentProfessor.DiciplineId)
                {
                    var discipline = mainService.GetDisciplineById(disciplineId);
                    if (discipline != null)
                    {
                        myDisciplines.Add(discipline);
                    }
                }

                foreach (Discipline discipline in myDisciplines)
                {
                    Console.WriteLine($"ID: {discipline.Id}, Name: {discipline.Name}");
                }

                Console.WriteLine();
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Set marks for discipline");
                Console.WriteLine("2. Edit discipline");
                string option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        SetMarksForDiscipline();
                        break;

                    case "2":
                        EditDiscipline();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("The current user is not a professor.");
            }
        }

        static void SetMarksForDiscipline()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("Set Marks for Discipline:");
            Console.WriteLine();

            Console.WriteLine("Enter the ID of the discipline:");
            int disciplineId = int.Parse(Console.ReadLine());

            // Get the discipline by ID
            Discipline discipline = mainService.GetDisciplineById(disciplineId);

            if (discipline != null)
            {
                Console.WriteLine();
                Console.WriteLine($"Discipline: {discipline.Name}");
                Console.WriteLine();

                // Get the list of students for the selected discipline
                List<Student> students = mainService.GetDisciplineStudents(disciplineId);

                if (students.Count == 0)
                {
                    Console.WriteLine("no students found for this discipline");
                    return;
                }

                // Display the information for each student
                foreach (Student student in students)
                {
                    Mark mark = student.GetMarks().FirstOrDefault(m => m.discipline == discipline);
                    string grade = mark != null ? mark.GetMarkName() : "No grade";
                    Console.WriteLine(
                        $"ID: {student.StudentId}, Name: {student.GetFullName()}, Grade: {grade}"
                    );
                }

                Console.WriteLine();
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Set mark");
                // Add more options if needed

                string option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        SetMarkForStudent(discipline);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid discipline ID. Please try again.");
            }
        }

        static void SetMarkForStudent(Discipline discipline)
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("Set Mark for Student:");
            Console.WriteLine();

            Console.WriteLine("Enter the ID of the student:");
            int studentId = int.Parse(Console.ReadLine());

            // Get the student by ID
            Student student = mainService.GetStudentById(studentId);

            if (student != null)
            {
                Console.WriteLine();
                Console.WriteLine($"Student: {student.GetFullName()}");
                Console.WriteLine();

                Console.WriteLine("Select a mark:");
                // Display the list of available marks
                foreach (MarkValues markValue in Enum.GetValues(typeof(MarkValues)))
                {
                    Console.WriteLine(markValue);
                }

                // Get the selected mark from user input
                string selectedMark = Console.ReadLine();

                if (Enum.TryParse(selectedMark, out MarkValues mark))
                {
                    Mark newMark = new Mark(mark, discipline, student);

                    student.AddOrEditMark(newMark);

                    Console.WriteLine("Mark Assigned Successfully!");

                    Storage.SaveAllData();
                }
                else
                {
                    Console.WriteLine("Invalid mark. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid student ID. Please try again.");
            }
        }

        static void EditDiscipline()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("Edit Discipline:");
            Console.WriteLine();

            Console.WriteLine("Enter the ID of the discipline:");
            int disciplineId = int.Parse(Console.ReadLine());

            // Get the discipline by ID
            Discipline discipline = mainService.GetDisciplineById(disciplineId);

            if (discipline != null)
            {
                Console.WriteLine();
                Console.WriteLine($"Discipline: {discipline.Name}");
                Console.WriteLine();

                Console.WriteLine("Enter the new name of the discipline:");
                string newName = Console.ReadLine();

                // Update the name of the discipline
                discipline.Name = newName;

                Console.WriteLine("Discipline updated successfully!");
                Storage.SaveAllData();
            }
            else
            {
                Console.WriteLine("Invalid discipline ID. Please try again.");
            }
        }

        static string MaskPassword()
        {
            string password = null;
            while (true)
            {
                var txt = Console.ReadKey(true);
                if (txt.Key == ConsoleKey.Enter)
                    break;
                password += txt.KeyChar;
            }

            return password;
        }

        static void Main(string[] args)
        {
            InitializeMainService();
            var storageInstance = Storage.Instance;
            Storage.LoadAllData();

            if (Storage.Groups != null && Storage.Groups.Count > 0)
            {
                Console.WriteLine("Groups Data has been Loaded");
            }
            else
            {
                Console.WriteLine("Groups Data has not been Loaded");
            }

            while (true)
            {
                Console.WriteLine("please select an option");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3.Exit");
                string option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        RegistrationMenu();
                        break;
                    case "2":
                        LoginMenu();
                        break;
                    case "3":
                        ExitMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }

        }

        // Admin Menu
        static void AdminMenu()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("Admin Menu:");
            Console.WriteLine();
            Console.WriteLine("1. Show all users");
            Console.WriteLine("2. Show all disciplines");
            Console.WriteLine("3. Show all groups");
            Console.WriteLine("4. Exit");
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    ShowAllUsers();
                    break;
                case "2":
                    ShowAllDisciplines();
                    break;
                case "3":
                    ShowAllGroups();
                    break;
                case "4":
                    ExitMenu();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        static void ShowAllUsers()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("All Users:");
            Console.WriteLine();
            foreach (var user in userService.GetAllUsers())
            {
                Console.WriteLine(
                    $"Name: {user.GetFullName()}, Birthdate: {user.DateOfBirth}, Role: {user.UserRole}, Is Administrator: {user.IsAdministrator}"
                );
            }
            Console.WriteLine();
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Set as administrator");
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    SetAsAdministrator();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        static void SetAsAdministrator()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("Set User as Administrator:");
            Console.WriteLine();
            Console.WriteLine("Enter the ID of the user:");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
                return;
            }

            userService.SetAsAdmin(userId);
        }

        static void ShowAllDisciplines()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("All Disciplines:");
            Console.WriteLine();
            foreach (var discipline in mainService.GetAllDisciplines())
            {
                Console.WriteLine(
                    $"ID: {discipline.GetId()}, Name: {discipline.GetName()}, Hours: {discipline.GetHours()}"
                );
            }
            Console.WriteLine();
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Create new discipline");
            Console.WriteLine("2. Assign a teacher to the discipline");
            Console.WriteLine("3. Assign a Discipline to a group");
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    CreateNewDiscipline();
                    break;
                case "2":
                    AssignTeacherToDiscipline();
                    break;
                case "3":
                    AssignDisciplineToGroup();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        static void CreateNewDiscipline()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("Create New Discipline:");
            Console.WriteLine();
            Console.WriteLine("Enter the name of the discipline:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter the number of hours for the discipline:");
            int hours = int.Parse(Console.ReadLine());

            int newDisciplineId =
                mainService.GetAllDisciplines().Max(discipline => discipline.Id) + 1;

            Discipline newDiscipline = mainService.CreateDiscipline(newDisciplineId, name, hours);

            if (newDiscipline != null)
            {
                Console.WriteLine("Discpline created successfully");
            }
            else
            {
                Console.WriteLine("failed to create");
            }
        }

        static void AssignTeacherToDiscipline()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("Assign Teacher to Discipline:");
            Console.WriteLine();
            Console.WriteLine("Enter the ID of the discipline:");
            int disciplineId = int.Parse(Console.ReadLine());

            // Get the discipline by ID
            Discipline discipline = mainService.GetDisciplineById(disciplineId);

            if (discipline != null)
            {
                Console.WriteLine();
                Console.WriteLine("Teachers:");
                Console.WriteLine();

                // Get the list of teachers
                List<Professor> teachers = mainService.GetAllTeachers();

                if (teachers != null)
                {
                    foreach (Professor professor in teachers)
                    {
                        Console.WriteLine(
                            $"ID: {professor.Id}, Name: {professor.FirstName} {professor.LastName}"
                        );
                    }
                }
                else
                {
                    Console.WriteLine("No teachers found");
                }

                // Display the information for each teacher


                Console.WriteLine();
                Console.WriteLine("Enter the ID of the teacher:");
                int teacherId = int.Parse(Console.ReadLine());

                // Get the teacher by ID
                Professor teacher = mainService.GetTeacherById(teacherId);

                if (teacher != null)
                {
                    mainService.AssignTeacher(teacherId, disciplineId);
                    Console.WriteLine("Teacher assigned to discipline successfully!");
                    Storage.SaveAllData();
                }
                else
                {
                    Console.WriteLine("Invalid teacher ID. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid discipline ID. Please try again.");
            }
        }

        static void AssignDisciplineToGroup()
        {
            Console.WriteLine("Assign Discipline to Group:");
            Console.WriteLine("Enter the ID of the discipline:");
            int disciplineId = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter the ID of the group:");
            int groupId = int.Parse(Console.ReadLine());

            mainService.AssignDisciplineToGroup(disciplineId, groupId);
            Console.WriteLine(
                $"Discipline '{disciplineId}' assigned to group '{groupId}' successfully!"
            );
        }

        static void ShowAllGroups()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("All Groups:");
            Console.WriteLine();
            foreach (var group in mainService.GetAllGroups())
            {
                Console.WriteLine($"ID: {group.GroupId}, Name: {group.Name}");
            }
            Console.WriteLine();
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Create group");
            Console.WriteLine("2. Assign Student to a group");
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    CreateGroup();
                    break;

                case "2":
                    assignStudentToGroup();
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        static void assignStudentToGroup()
        {
            Console.WriteLine("Enter the ID of the student:");
            int studentId = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the ID of the group:");
            int groupId = Convert.ToInt32(Console.ReadLine());

            mainService.AddStudentToGroup(studentId, groupId);
        }

        static void CreateGroup()
        {
            //clr();
            Console.WriteLine();
            Console.WriteLine("Create Group:");
            Console.WriteLine();
            Console.WriteLine("Enter the name of the group:");
            string name = Console.ReadLine();

            int newGroupId = Storage.Groups.Max(group => group.GroupId) + 1;

            Group newGroup = new Group(newGroupId, name);
            mainService.CreateGroup(newGroup);
            Console.WriteLine($"Group '{name}' created sucessfully!");
        }
    }
}
