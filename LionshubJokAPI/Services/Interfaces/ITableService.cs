using LionshubJokAPI.Models;
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
        //bool DeleteAll();
        //bool Delete(string id);
    }
}
