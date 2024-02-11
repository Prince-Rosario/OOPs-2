namespace Rosario_repassTask;

public class Discipline
{
    public string Name { get; set; }
    public int Hours { get; set; }
    public int Id { get; set; }
    private List<Student> Students = new List<Student>();
    private Professor _professor;

    public Discipline(string name, int hours)
    {
        Name = name;
        Hours = hours;
    }

    public string GetName()
    {
        return Name;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetHours(int hours)
    {
        Hours = hours;
    }

    public int GetHours()
    {
        return Hours;
    }

    public List<Student> GetStudents()
    {
        return Students;
    }

    public int GetId()
    {
        return Id;
    }

    public void AssignTeacher(Professor professor)
    {
        _professor = professor;
    }

    public Professor GetTeacher()
    {
        return _professor;
    }
}
