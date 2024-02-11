using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Rosario_repassTask;

public class Group
{
    public string Name { get; set; }
    public int GroupId { get; set; }

    public List<Discipline> Disciplines { get; set; }

    public Group()
    {
        Disciplines = new List<Discipline>();
    }

    public Group(int groupId, string name)
        : base()
    {
        GroupId = groupId;
        Name = name;
    }

    public string GetName()
    {
        return Name;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public static bool IsValidGroup(int groupId)
    {
        Group[] groups = Storage.LoadGroupsData();

        return groups.Any(group => group.GroupId == groupId);
    }
}
