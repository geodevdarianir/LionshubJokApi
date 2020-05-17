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

        [HttpPost]
        public IActionResult GeneratePlay(Models.Table table)
        {
            bool res = _jokerService.GeneratePlay(table.Id);
            if (res == true)
            {
                Joker joker = JokerService.jokers.Where(p => p.TableID == table.Id).FirstOrDefault();
                if (joker == null)
                {
                    return NotFound(table);
                }
                else
                {
                    return Ok(joker);
                }
            }
            else
            {
                return NotFound(table);
            }
        }

        [HttpPost]
        public IActionResult StartHand(string tableID, Models.RoundsAndGamers round)
        {
            PlayGame play = _jokerService.StartPlay(tableID, round);
            return Ok(play);
        }

        [HttpPost]
        public IActionResult GetPlayState(string tableID)
        {
            Joker joker = _jokerService.GetPlayState(tableID);
            if (joker != null)
            {
                return Ok(joker);
            }
            else
            {
                return Content("404");
            }
        }

        [HttpPost]
        public IActionResult RemoveJoker(string tableID)
        {
            bool res = _jokerService.RemoveJoker(tableID);
            if (res)
            {
                return Ok(res);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult PutCardOnTable(int cardId, string tableID)
        {

            Joker joker = _jokerService.PutCardOnTable(cardId, tableID);
            if (joker is null)
            {
                return Content("404");
            }
            else
            {
                return Ok(joker);
            }
        }

        [HttpPost]
        public IActionResult BotPutCardOnTable(string tableID)
        {

            Joker joker = _jokerService.BotPutCardOnTable(tableID);
            if (joker is null)
            {
                return Content("404");
            }
            else
            {
                return Ok(joker);
            }
        }

        [Route("")]
        [HttpGet]
        public IActionResult GetCurrentGames()
        {
            return Ok(_jokerService.GetCurrentGames());
        }

        [HttpPost]
        public IActionResult AllowScores(string tableID, int gamerID)
        {
            return Ok(_jokerService.AllowScores(tableID, gamerID));
        }

        [HttpGet]
        public IActionResult TellScores(string tableID, int gamerID, int score)
        {
            return Ok(_jokerService.TellScore(tableID, gamerID, score));
        }

        [HttpGet]
        public IActionResult BotTellScores(string tableID, int gamerID)
        {
            return Ok(_jokerService.BotTellScore(tableID, gamerID));
        }

        [HttpPost]
        public IActionResult SetTrumpCard(string tableID, int color)
        {
            return Ok(_jokerService.SetTumpCard(tableID, color));
        }
        [HttpPost]
        public IActionResult SetJokerStrength(string tableID, int strengthOfCard, int cardID, int? giveANDtake)
        {
            return Ok(_jokerService.SetJokerStrength(tableID, strengthOfCard, cardID, giveANDtake));
        }

        [HttpPost]
        public IActionResult SwitchGamer(string tableID)
        {
            return Ok(_jokerService.SwitchCurrentGamer(tableID));
        }
    }
}