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
    [Route("api/[controller]")]
    [ApiController]
    public class GamerController : ControllerBase
    {
        private readonly GamerService _gamerService;
        public GamerController(GamerService gamerService)
        {
            _gamerService = gamerService;
        }

        [HttpGet]
        public ActionResult<List<Gamer>> Get()
        {
            return _gamerService.Get();
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
    }
}