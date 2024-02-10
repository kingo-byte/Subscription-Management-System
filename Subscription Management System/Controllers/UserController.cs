using BAL.IServices;
using DAL.Repository.Models;
using DAL.Repository.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Subscription_Management_System.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService
            )
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("Register")]
        public IActionResult Register(SignUpDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid User");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User()
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                DateOfBirth = request.DateOfBirth,
                Nationality = request.Nationality
            };

            return Ok(_userService.AddUser(user));
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
