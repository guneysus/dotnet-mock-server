using System.Collections.Generic;

namespace NMock
{
    public interface IUserRepository
    {
        User this[int index] { get; }

        List<User> GetAllUsers();
    }
}