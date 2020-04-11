using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionshubJokAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LionshubJokAPI.Controllers
{
    [Route("LionshubApi/[controller]")]
    [ApiController]
    public class PlayController : ControllerBase
    {
        private  JokerService _jokerService;
        public PlayController()
        {
            //_jokerService = jokerService;
        }

        [HttpGet]
        public IActionResult StartPlay(string tableId)
        {
            JokerService service = new JokerService();
            //service.StartPlay();
            return Ok();
        }
    }
}