using LionshubJokAPI.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionshubJokAPI.Services
{
    public class GamerService
    {
        private readonly IMongoCollection<Gamer> _gamer;

        public GamerService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MyJokDB"));
            var database = client.GetDatabase("MyJokDB");
            _gamer = database.GetCollection<Gamer>("Gamers");
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
    }
}
