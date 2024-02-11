using Azure.Core;
using BAL.IServices;
using DAL.Repository.Models;
using DAL.Repository.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Subscription_Management_System.RetryMechanism;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Subscription_Management_System.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {        
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILoggingService _log;
        private int _maxAttemps;
        public UserController(            
            IUserService userService,
            IConfiguration configuration,
            ILoggingService log
            )
        {            
            _userService = userService;
            _configuration = configuration; 
            _log = log;
            _maxAttemps = int.Parse(_configuration.GetSection("AppSettings:MaxAttempts").Value);
        }

        [HttpPost("login")]
        public IActionResult Login(SignInDTO request)
        {
            try
            {
                if (HttpContext.Session.GetInt32("LoginAttemps") != null)
                {
                    HttpContext.Session.SetInt32("LoginAttemps", HttpContext.Session.GetInt32("LoginAttemps").Value - 1);
                }
                else 
                {
                    HttpContext.Session.SetInt32("LoginAttemps", _maxAttemps - 1);
                }

                _log.Log($"Request Login is sent with {request}");

                User user = new User()
                {
                    UserName = request.UserName
                };

                int? remainingAttempts = HttpContext.Session.GetInt32("LoginAttemps");

                User checkUser = RetryPolicy.ExecuteWithRetry(() => _userService.CheckUser(user), remainingAttempts.Value);

                if (checkUser == null)
                {
                    return BadRequest("Invalid Email or Password");
                }

                if (!VerifyPasswordHash(request.Password, checkUser.PasswordHash, checkUser.PasswordSalt))
                {
                    return BadRequest("Invalid Email or Password");
                }

                string token = CreateToken(checkUser);

                _log.Log($"token {token} is generated");

                return Ok(token);
            }
            catch (Exception ex)
            {
                _log.Log($"Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Register")]
        public IActionResult Register(SignUpDto request)
        {
            try
            {
                _log.Log($"Request Register is sent with {request}");
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
                _log.Log($"User {user} is created");
                return Ok(_userService.AddUser(user));
            }
            catch (Exception ex)
            {
                _log.Log($"Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("UpdateUser")]
        [Authorize]
        public IActionResult UpdateUser(User user)
        {
            try
            {
                _log.Log($"Request UpdateUser is sent with {user}");
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid User");
                }
                var response = _userService.UpdateUser(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _log.Log($"Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("RemoveUser")]
        [Authorize]
        public IActionResult RemoveUser(int id)
        {
            try
            {
                _log.Log($"Request RemoveUser is sent with ID {id}");
                var response = _userService.RemoveUser(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _log.Log($"Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
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

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}
