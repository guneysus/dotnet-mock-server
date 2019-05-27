using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

public class MockUserRepository : IUserRepository
{
    private List<User> _users = JsonConvert.DeserializeObject<List<User>>(@"
[{""id"":""1"",""createdAt"":""2019-05-06T19:32:07.034Z"",""name"":""Rosario Beier"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/prrstn/128.jpg""},{""id"":""2"",""createdAt"":""2019-05-07T10:28:32.699Z"",""name"":""Rocio Gibson DVM"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/SlaapMe/128.jpg""},{""id"":""3"",""createdAt"":""2019-05-06T17:00:24.825Z"",""name"":""Adeline Torphy"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/catadeleon/128.jpg""}]
");

    public User this[int index] => _users.FirstOrDefault(x => x.Id == index);

    public List<User> GetAllUsers() => _users;

}
