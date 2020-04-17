using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionshubJokAPI.Models;
using LionshubJokAPI.Services;
using LionshubJoker.Joker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LionshubJokAPI.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    public class PlayController : ControllerBase
    {
        private readonly JokerService _jokerService;
        public PlayController(JokerService jokerService)
        {
            _jokerService = jokerService;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Content("Hallo");
        }
        [HttpPost]
        public IActionResult GeneratePlay(Models.Table table)
        {
            bool res = _jokerService.GeneratePlay(table.Id);
            if (res == true)
            {
                List<Models.RoundsAndGamers> rouns = new List<Models.RoundsAndGamers>();
                foreach (LionshubJoker.Joker.RoundsAndGamers item in _jokerService.rounds)
                {
                    rouns.Add(new Models.RoundsAndGamers
                    {
                        GamerID = item.CurrentGamer.Id,
                        handRound = item.Hand
                    });
                }
                Joker joker = new Joker
                {
                    play = JokerService.play,
                    rounds = rouns
                };
                return Ok(joker);
            }
            else
            {
                return NotFound(table);
            }
        }

        [HttpPost]
        public IActionResult StartHand(Models.RoundsAndGamers round)
        {

            PlayGame palay = _jokerService.StartPlay(round);
            return Ok(palay);
        }
    }
}