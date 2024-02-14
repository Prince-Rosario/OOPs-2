namespace Rosario_repassTask
{
    public interface IMainService
    {
        public IEnumerable<Discipline> GetAllDisciplines();
        public Discipline CreateDiscipline(int id, string name, int hours);
        public void AssignTeacher(int teacherId, int disciplineId);

        public void LoadProfessors();
        public void LoadStudents();

        //public void CreateMark(MarkValues mark, int studentId, int disciplineId);
        public void EditMark(MarkValues mark, int studentId, int disciplineId);
        public List<Student> GetDisciplineStudents(int disciplineId);
        public void SetMarkForStudent(Discipline discipline, Student student, MarkValues markValue);
        public List<Professor> GetDisciplineTeachers(int disciplineId);
        public List<Discipline> GetDisciplinesForGroup(Group group);
        public List<Professor> GetAllTeachers();
        public Professor GetTeacherById(int id);
        public Student GetStudentById(int id);
        public Discipline GetDisciplineById(int id);
        public List<Group> GetAllGroups();
        void CreateGroup(Group group);
        public void AssignDisciplineToGroup(int disciplineId, int groupId);
        public List<Mark> GetGradesForStudent();
        public void AddStudentToGroup(int studentId, int groupId);
    }
}
