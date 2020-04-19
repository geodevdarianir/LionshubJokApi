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
        void DeleteOnTable(Table table);
        public void DeleteOnTableWithId(string tableId);
        void DeleteAll();
    }
}
