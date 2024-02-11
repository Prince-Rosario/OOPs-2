namespace Rosario_repassTask
{
    public class Professor : Person
    {
        private string degree { get; set; }
        private int yearsOfExperience { get; set; }

        public int ProfId;

        public List<int> DiciplineId = new List<int>();

        public Professor(
            string firstname,
            string lastname,
            string secondname,
            DateTime dateofbirth,
            int teacherId,
            int discplineId
        )
            : base(firstname, lastname, secondname, dateofbirth)
        {
            ProfId = teacherId;
            DiciplineId.Add(discplineId);
        }

        public Professor(int _professorId, int _disciplineId)
            : base()
        {
            ProfId = _professorId;
            DiciplineId.Add(_disciplineId);
        }

        public Professor()
            : base() { }

        public string GetDegree()
        {
            return degree;
        }

        public void SetDegree(string _degree)
        {
            degree = _degree;
        }

        public void SetYears(int _yearsOfExperience)
        {
            yearsOfExperience = _yearsOfExperience;
        }

        public IEnumerable<Discipline> GetDisciplines()
        {
            return new List<Discipline>();
        }

        public void AddingDiscipline(int _disciplineId)
        {
            DiciplineId.Add(_disciplineId);
        }
    }
}
