using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LionshubJokAPI.Services.Interfaces;
using LionshubJokAPI.Models.Users;
using System.Linq;
using LionshubJokAPI.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using LionshubJokAPI.Models;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private readonly Settings _appSettings;
        public UsersController(IUserService userService, IOptions<Settings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Get()
        {
            return Content("Hallo");
        }

        [AllowAnonymous]
        //[HttpPost("authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Email, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            List<UserModel> model = new List<UserModel>();
            foreach (User item in users)
            {
                model.Add(new UserModel()
                {
                    Email = item.Email,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Id = item.Id,
                    Username = item.Username
                });
            }
            return Ok(model);
        }

        //[HttpGet("{id}")]
        [HttpGet]
        public IActionResult GetById(string id)
        {
            var user = _userService.GetById(id);
            UserModel model = new UserModel()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                Username = user.Username
            };
            return Ok(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody]RegisterModel model)
        {
            // map model to entity
            //var user = _mapper.Map<User>(model);
            User user = new User()
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username
            };
            try
            {
                // create user
                _userService.Create(user, model.Password);
                return Ok(new { message = "user registered successfully" });
            }
            catch (ApplicationException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}