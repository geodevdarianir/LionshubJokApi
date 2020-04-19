using LionshubJokAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionshubJokAPI.Services
{
    public interface IGamerService
    {
        List<Gamer> Get();

        Gamer Get(string id);

        List<Gamer> GetGamersOnTable(string tableId);

        Gamer Create(Gamer gamer);

        void Delete(Gamer gamer);
        void DeleteGamersOnTable(Table table);
        void DeleteAll();
    }
}
