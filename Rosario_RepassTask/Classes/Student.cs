namespace Rosario_repassTask;

public class Student : Person
{
    public int StudentId { get; set; }

    public Group StudentGroup { get; set; }

    public List<Mark> Grades { get; set; } = new List<Mark>();

    public Student(
        string firstName,
        string lastName,
        string secondName,
        DateTime dateOfBirth,
        int studentId,
        int groupId
    )
        : base(firstName, lastName, secondName, dateOfBirth)
    {
        StudentId = studentId;
        StudentGroup = new Group();
        StudentGroup.GroupId = groupId;
    }

    public Student()
        : base() { }

    public Group GetGroup()
    {
        return new Group();
    }

    public void SetGroup(int groupId)
    {
        StudentGroup.GroupId = groupId;
    }

    public IEnumerable<Mark> GetMarks()
    {
        return Grades;
    }

    public void AddOrEditMark(Mark newMark)
    {
        Mark existingMark = Grades.Find(mark => mark.discipline == newMark.discipline);
        if (existingMark != null)
        {
            existingMark.SetMark(newMark.markValue);
        }
        else
        {
            Grades.Add(newMark);
        }
    }
}
