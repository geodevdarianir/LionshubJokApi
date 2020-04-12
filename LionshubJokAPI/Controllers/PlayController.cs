using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionshubJokAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LionshubJokAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayController : ControllerBase
    {
        private readonly JokerService _jokerService;
        public PlayController(JokerService jokerService)
        {
            _jokerService = jokerService;
        }

        [HttpPost("tableId:length(24)")]
        public IActionResult StartPlay(string tableId)
        {
            _jokerService.GeneratePlay(tableId);

            //JokerService service = new JokerService();
            //service.StartPlay();
            return Ok();
        }
    }
}