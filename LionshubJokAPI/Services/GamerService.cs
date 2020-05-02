using LionshubJokAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionshubJokAPI.Services
{
    public class GamerService : IGamerService
    {
        private readonly IMongoCollection<Gamer> _gamer;
        private readonly DbService _context = null;
        public GamerService(IOptions<Settings> settings)
        {
            _context = new DbService(settings);
            _gamer = _context.Gamers;
        }

        public List<Gamer> Get()
        {
            return _gamer.Find(table => true).ToList();
        }

        public Gamer Get(string id)
        {
            return _gamer.Find<Gamer>(gamer => gamer.Id == id).FirstOrDefault();
        }

        public List<Gamer> GetGamersOnTable(string tableId)
        {
            return _gamer.Find(gamer => gamer.TableId == tableId).ToList();
        }

        public Gamer Create(Gamer gamer)
        {
            _gamer.InsertOne(gamer);
            return gamer;
        }
        public void Delete(Gamer gamer)
        {
            _gamer.DeleteOne(p => p.Id == gamer.Id);
        }
        public void DeleteOnTable(Table table)
        {
            List<Gamer> gamersOnTable = GetGamersOnTable(table.Id);
            foreach (Gamer item in gamersOnTable)
            {
                Delete(item);
            }
        }
        public void DeleteOnTableWithId(string tableId)
        {
            List<Gamer> gamersOnTable = GetGamersOnTable(tableId);
            foreach (Gamer item in gamersOnTable)
            {
                Delete(item);
            }
        }
        public void DeleteAll()
        {
            List<Gamer> allGamers = Get();
            foreach (Gamer item in allGamers)
            {
                if (item.TableId != "5e9ccee0928c691e703aaadc")
                    Delete(item);
            }
        }
    }
}
