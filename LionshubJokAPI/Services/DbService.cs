using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionshubJokAPI.Entities;
using LionshubJokAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LionshubJokAPI.Services
{
    public class DbService
    {
        private readonly IMongoDatabase _database = null;

        public DbService(IOptions<Settings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }
        public IMongoCollection<Table> Tables
        {
            get
            {
                return _database.GetCollection<Table>("Tables");
            }
        }
        public IMongoCollection<Gamer> Gamers
        {
            get
            {
                return _database.GetCollection<Gamer>("Gamers");
            }
        }
        public IMongoCollection<User> Users
        {
            get
            {
                return _database.GetCollection<User>("Users");
            }
        }

        public IMongoDatabase DataBase
        {
            get => _database;
        }
    }
}
