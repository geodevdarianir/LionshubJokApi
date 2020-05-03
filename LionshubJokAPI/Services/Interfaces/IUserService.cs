using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionshubJokAPI.Entities;
using MongoDB.Bson;

namespace LionshubJokAPI.Services.Interfaces
{
    public interface IUserService
    {
        User Authenticate(string email, string password);
        IEnumerable<User> GetAll();
        User GetById(string id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(string id);
    }
}
