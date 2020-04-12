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
        public IActionResult GeneratePlay(Table table)
        {
            _jokerService.GeneratePlay(table.Id);

            //JokerService service = new JokerService();
            //service.StartPlay();
            return Ok();
        }

        
    }
}