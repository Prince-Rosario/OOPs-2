using Newtonsoft.Json.Bson;

namespace Rosario_repassTask
{
    //TODO: IMPLEMENT ALL THE FUNCTIONS //TODO:Write checks and catch exceptions.
    public class MainService : IMainService
    {
        //private static List<Discipline> disciplines = new List<Discipline>();
        private List<Professor> _professors;
        private List<Student> _students;

        public IEnumerable<Discipline> GetAllDisciplines()
        {
            return Storage.Disciplines;
        }

        public Discipline CreateDiscipline(int id, string name, int hours)
        {
            Discipline discipline = new Discipline(name, hours) { Id = id };

            // Add the new discipline to the list of disciplines in the Storage class
            Storage.Disciplines.Add(discipline);

            // Save the updated list of disciplines to the disciplines.json file
            Storage.SaveDisciplinesData(Storage.Disciplines.ToArray());

            return discipline;
        }

        public void LoadProfessors()
        {
            // Load the users from the users.json file
            Person[] users = Storage.LoadUsersData();

            // Filter the users to get only the professors and add them to the _professors list
            _professors = users.OfType<Professor>().ToList();
        }

        public void LoadStudents()
        {
            Person[] users = Storage.LoadUsersData();

            _students = users.OfType<Student>().ToList();
        }

        public void AssignTeacher(int teacherId, int disciplineId)
        {
            // Get the professor with the given id
            Professor professor = GetTeacherById(teacherId);
            if (professor == null)
            {
                Console.WriteLine("No such Professor Found");
                return;
            }

            // Get the discipline with the given id
            Discipline discipline = GetDisciplineById(disciplineId);
            if (discipline == null)
            {
                Console.WriteLine("No such Discipline Found");
                return;
            }

            // Assign the professor to the discipline
            discipline.AssignTeacher(professor);
        }

        /*  public void CreateMark(MarkValues mark, int studentId, int disciplineId)
          {
              // Find the student with the given id
              Student student = _students.Find(s => s.StudentId == studentId);
              if (student == null)
              {
                  Console.WriteLine("No such Student Found");
                  return;
              }
  
              // Find the discipline with the given id
              Discipline discipline = disciplines.Find(d => d.GetId() == disciplineId);
              if (discipline == null)
              {
                  Console.WriteLine("No such Discipline Found");
                  return;
              }
  
              //Create a new mark and add it to the student's marks
              Mark newMark = new Mark { markValue = mark, discipline = discipline, };
              student.AddOrEditMark(newMark);
          }
          */
        public void EditMark(MarkValues mark, int studentId, int disciplineId)
        {
            throw new NotImplementedException();
        }

        public List<Student> GetDisciplineStudents(int disciplineId)
        {
            List<Student> students = new List<Student>();

            // Check if Users is null
            if (Storage.Instance.Users == null)
            {
                Console.WriteLine("No users found.");
                return students;
            }

            Group groupWithDiscipline = GetAllGroups()
                .FirstOrDefault(g => g.Disciplines.Any(d => d.Id == disciplineId));

            if (groupWithDiscipline == null)
            {
                Console.WriteLine($"No group found with discipline ID {disciplineId}.");
                return students;
            }

            foreach (var user in Storage.Instance.Users)
            {
                Student student = user as Student;
                if (
                    student != null
                    && student.StudentGroup != null
                    && student.StudentGroup.GroupId == groupWithDiscipline.GroupId
                )
                {
                    students.Add(student);
                }
            }

            if (students.Count == 0)
            {
                Console.WriteLine($"No students found for discipline with ID {disciplineId}.");
            }

            return students;
        }

        public List<Professor> GetDisciplineTeachers(int disciplineId)
        {
            List<Professor> professors = new List<Professor>();
            foreach (var teacher in Storage.Instance.Users)
            {
                Professor p = teacher as Professor;
                if (p.ProfId != null && p.DiciplineId != null)
                    professors.Add(p);
            }
            return professors;
        }

        public List<Discipline> GetDisciplinesForGroup(Group group)
        {
            List<Discipline> disciplinesForGroup = new List<Discipline>();

            // Iterate over all disciplines
            foreach (var discipline in Storage.Disciplines)
            {
                // Iterate over all students in the discipline
                foreach (var student in discipline.GetStudents())
                {
                    // If the student's group matches the given group, add the discipline to the list
                    if (student.StudentGroup.GroupId == group.GroupId)
                    {
                        disciplinesForGroup.Add(discipline);
                        break;
                    }
                }
            }

            return disciplinesForGroup;
        }

        public Professor GetTeacherById(int id)
        {
            // Find the professor with the given id

            Professor professor = _professors.Find(p => p.ProfId == id);

            if (professor == null)
            {
                Console.WriteLine("No such Professor Found");
            }

            return professor;
        }

        public Student GetStudentById(int id)
        {
            // Find the student with the given id
            foreach (Student student in _students)
            {
                if (student.StudentId == id)
                {
                    return student;
                }
            }
            return null;
        }

        public List<Professor> GetAllTeachers()
        {
            return _professors;
        }

        public Discipline GetDisciplineById(int id)
        {
            // Print all discipline IDs for debugging
            Console.WriteLine("All discipline IDs:");
            foreach (var discipline in Storage.Disciplines)
            {
                Console.WriteLine(discipline.Id);
            }

            // Find the discipline with the given id
            Discipline foundDiscipline = Storage.Disciplines.Find(d => d.Id == id);
            if (foundDiscipline == null)
            {
                Console.WriteLine("No such Discipline Found");
            }
            return foundDiscipline;
        }

        public List<Group> GetAllGroups()
        {
            return Storage.Groups;
        }

        public void CreateGroup(Group group)
        {
            // Add the new group to the list of groups in the Storage class
            Storage.Groups.Add(group);

            // Save the updated list of groups to the groups.json file
            Storage.SaveGroupsData(Storage.Groups.ToArray());
        }

        public void AssignDisciplineToGroup(int disciplineId, int groupId)
        {
            Group group = GetAllGroups().FirstOrDefault(g => g.GroupId == groupId);
            Discipline discipline = GetAllDisciplines().FirstOrDefault(d => d.Id == disciplineId);

            if (discipline == null)
            {
                Console.WriteLine($"No discipline found with ID {disciplineId}");
                return;
            }

            if (group == null)
            {
                Console.WriteLine($"No group found with ID {groupId}");
                return;
            }

            group.Disciplines.Add(discipline);

            // Update the students' groups
            foreach (var user in Storage.Instance.Users)
            {
                Student student = user as Student;
                if (
                    student != null
                    && student.StudentGroup != null
                    && student.StudentGroup.GroupId == groupId
                )
                {
                    student.StudentGroup.Disciplines.Add(discipline);
                }
            }

            Console.WriteLine(
                $"Discipline '{disciplineId}' assigned to group '{groupId}' successfully"
            );
            Storage.SaveGroupsData(GetAllGroups().ToArray());
        }

        public Group GetGroupById(int groupId)
        {
            // Find the group with the given id
            Group group = Storage.Groups.Find(g => g.GroupId == groupId);
            if (group == null)
            {
                Console.WriteLine("No such Group Found");
            }
            return group;
        }

        public List<Mark> GetGradesForStudent()
        {
            Person activeUser = Storage.Instance.ActiveUser;

            if (activeUser is Student student)
            {
                return student.Grades;
            }
            else
            {
                // If the active user is not a student, return an empty list
                return new List<Mark>();
            }
        }

        public void AddStudentToGroup(int studentId, int groupId)
        {
            // Find the student with the given id
            Student student = GetStudentById(studentId);
            if (student == null)
            {
                Console.WriteLine($"No student found with ID {studentId}.");
                return;
            }

            // Find the group with the given id
            Group group = GetGroupById(groupId);
            if (group == null)
            {
                Console.WriteLine($"No group found with ID {groupId}.");
                return;
            }

            // Assign the group to the student
            student.StudentGroup = group;

            // Update the student's group's disciplines
            student.StudentGroup.Disciplines = group.Disciplines;

            Console.WriteLine($"Student '{studentId}' added to group '{groupId}' successfully");
            Storage.SaveAllData();
        }

        public void SetMarkForStudent(Discipline discipline, Student student, MarkValues markValue)
        {
            // Create a new mark
            Mark mark = new Mark(markValue, discipline, student);

            // Add the mark to the student's grades
            student.Grades.Add(mark);

            // Update the student in the storage
            Storage.SaveUsersData(Storage.Instance.Users.ToArray());
        }
    }
}
