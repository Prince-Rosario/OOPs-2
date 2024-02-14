using System;

namespace Rosario_repassTask
{
    public enum MarkValues
    {
        Unsatisfactory,
        Satisfactory,
        Good,
        Excellent,
        Absent
    }

    [Serializable]
    public class Mark
    {
        public MarkValues markValue { get; set; }
        public Student student { get; set; }
        public Discipline discipline { get; set; }

        public Mark(MarkValues mark, Discipline disc, Student stud)
        {
            markValue = mark;
            discipline = disc;
            student = stud;
        }

        public string GetMarkName()
        {
            return markValue.ToString();
        }

        public void SetMark(MarkValues mark)
        {
            markValue = mark;
        }
    }
}
