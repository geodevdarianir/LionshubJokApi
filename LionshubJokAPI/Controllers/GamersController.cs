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
    public class GamersController : ControllerBase
    {
        private readonly IGamerService _gamerService;
        public GamersController(IGamerService gamerService)
        {
            _gamerService = gamerService;
        }

        [HttpGet]
        public IActionResult GetText()
        {
            return Content("Hallo");
        }

        [HttpGet]
        public ActionResult<List<Gamer>> Get()
        {
            return _gamerService.Get();
        }
        [HttpGet]
        public ActionResult<List<Gamer>> GetGamersOnTables(string id)
        {
            return _gamerService.GetGamersOnTable(id);
        }

        [HttpPost("id:length(24)", Name = "GetGamer")]
        public ActionResult<Gamer> Get(string id)
        {
            var gamer = _gamerService.Get(id);
            if (gamer == null)
            {
                return NotFound();
            }
            return gamer;
        }

        [HttpPost]
        public ActionResult<Gamer> CreateGamerOnTable(Gamer gamer)
        {
            _gamerService.Create(gamer);
            return CreatedAtRoute("GetGamer", new { id = gamer.Id.ToString() }, gamer);
        }

        [HttpPost]
        public ActionResult<Gamer> DeleteAll()
        {
            _gamerService.DeleteAll();
            return Ok();
        }
    }
}