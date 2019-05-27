using System.Collections.Generic;

public interface IUserRepository
{
    User this[int index] { get; }

    List<User> GetAllUsers();
}