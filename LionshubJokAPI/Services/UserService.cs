using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LionshubJokAPI.Entities;
using LionshubJokAPI.Services.Interfaces;
using MongoDB.Driver;
using LionshubJokAPI.Models;
using MongoDB.Bson;

namespace LionshubJokAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _user;
        private readonly IMongoDatabase _db;
        private readonly DbService _context = null;

        public UserService(IOptions<Settings> appSettings)
        {
            _context = new DbService(appSettings);
            _user = _context.Users;
            _db = _context.DataBase;
        }

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;
            var ss = _context.Users.AsQueryable().Any();
            var user = _context.Users.AsQueryable().FirstOrDefault(x => x.Email == email);
            // return null if user not found
            if (user == null)
                return null;
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _user.Find(user => true).ToList();
        }

        public User GetById(string id)
        {
            return _context.Users.Find(i => i.Id == id).FirstOrDefault();
        }

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new ApplicationException("Password is required");

            if (_context.Users.AsQueryable().Any(x => x.Email == user.Email))
                throw new ApplicationException("Email \"" + user.Email + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.InsertOne(user);
            //_context.SaveChanges();

            return user;
        }
        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.Find(u => u.Id == userParam.Id).FirstOrDefault();

            if (user == null)
                throw new ApplicationException("User not found");

            // update email if it has changed
            if (!string.IsNullOrWhiteSpace(userParam.Email) && userParam.Email != user.Email)
            {
                // throw error if the new username is already taken
                if (_context.Users.AsQueryable().Any(x => x.Email == userParam.Email))
                    throw new ApplicationException("Email " + userParam.Email + " is already taken");

                user.Email = userParam.Email;
            }
            if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            {
                user.Username = userParam.Username;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
                user.FirstName = userParam.FirstName;

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
                user.LastName = userParam.LastName;

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            var filter = Builders<User>.Filter.Eq("Id", userParam.Id);
            var update = Builders<User>.Update.Set("Email", user.Email)
                                               .Set("Username", user.Username)
                                               .Set("FirstName", user.FirstName)
                                               .Set("LastName", user.LastName)
                                               .Set("PasswordHash", user.PasswordHash)
                                               .Set("PasswordSalt", user.PasswordSalt);
            _context.Users.UpdateOne(filter, update);
            //_context.SaveChanges();
        }

        public void Delete(string id)
        {
            _context.Users.DeleteOne(p => p.Id == id);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
