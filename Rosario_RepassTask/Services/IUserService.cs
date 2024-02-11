using System.Collections;

namespace Rosario_repassTask;

public interface IUserService
{
    IEnumerable<Person> GetAllUsers();

    void RegisterUser(Person person);
    bool LoginUser(string email, string pass);
    void Logout();
    void SetAsAdmin(int userId);
}
