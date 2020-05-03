using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionshubJokAPI.Models;
using LionshubJokAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LionshubJokAPI.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        private readonly ITableService _tableService;
        public TablesController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [HttpGet]
        public ActionResult<List<Table>> Get()
        {
            return _tableService.Get();
        }


        [HttpPost("id:length(24)", Name = "GetTable")]
        public ActionResult<Table> Get(string id)
        {
            var table = _tableService.Get(id);
            if (table == null)
            {
                return NotFound();
            }
            return table;
        }

        [HttpPost]
        public ActionResult<Table> Create(Table table)
        {
            _tableService.Create(table);
            return CreatedAtRoute("GetTable", new { id = table.Id.ToString() }, table);
        }

        [HttpPost]
        public ActionResult<Table> DeleteAll()
        {
            _tableService.DeleteAll();
            return Ok();
        }
    }
}