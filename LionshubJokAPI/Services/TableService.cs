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
    public class TableService : ITableService
    {
        private readonly IMongoCollection<Table> _tables;
        private readonly IMongoDatabase _db;
        private readonly DbService _context = null;
        public TableService(IOptions<Settings> settings)
        {
            _context = new DbService(settings);
            _tables = _context.Tables;
            _db = _context.DataBase;
            //var client = service.MongoClient;
            //var database = client.GetDatabase("MyJokDB");
            //_tables = database.GetCollection<Table>("Tables");
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
        public void Delete(Table tableIn)
        {
            _tables.DeleteOne(p => p.Id == tableIn.Id);
        }
        public void DeleteWithId(string tableID)
        {
            _tables.DeleteOne(p => p.Id == tableID);
        }

        public void DeleteAll()
        {
            List<Table> tables = Get();
            foreach (Table item in tables)
            {
                if (item.Id != "5e9ccee0928c691e703aaadc")
                    Delete(item);
            }
        }
        //public bool DeleteAll()
        //{

        //}

        //public bool Delete(string id)
        //{
        //    //var test = db.GetCollection<Entity>("test");
        //    //var filter = new BsonDocument();

        //    return _tables.;
        //}
    }
}
