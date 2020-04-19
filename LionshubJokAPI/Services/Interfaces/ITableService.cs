using LionshubJokAPI.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionshubJokAPI.Services
{
    public interface ITableService
    {
        List<Table> Get();
        Table Get(string id);
        Table Create(Table table);
        void Delete(Table tableIn);
        void DeleteAll();
        void DeleteWithId(string tableID);
    }
}
