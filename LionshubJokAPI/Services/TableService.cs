using LionshubJokAPI.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionshubJokAPI.Services
{
    public class TableService
    {
        private readonly IMongoCollection<Table> _tables;
        public TableService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MyJokDB"));
            var database = client.GetDatabase("MyJokDB");
            _tables = database.GetCollection<Table>("Tables");
        }

        public List<Table> Get()
        {
            return _tables.Find(table => true).ToList();
        }

        public Table Get(string id)
        {
            return _tables.Find<Table>(table => table.Id == id).FirstOrDefault();
        }
        public Table Create(Table table)
        {
            _tables.InsertOne(table);
            return table;
        }

     
    }
}
