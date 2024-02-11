using BAL.IServices;
using DAL.Repository.Models;
using DAL.Repository.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        public UserController(            
            IUserService userService,
            IConfiguration configuration,
            ILoggingService log
            )
        {            
            _userService = userService;
            _configuration = configuration;
            _log = log;
        }

        [HttpPost("login")]
        public IActionResult Login(SignInDTO request)
        {
            try
            {
                _log.Log($"Request Login is sent with {request}");

                User user = new User()
                {
                    UserName = request.UserName
                };

                User checkUser = _userService.CheckUser(user);

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
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("UpdateUser")]
        [Authorize]
        public IActionResult UpdateUser(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid User");
                }
                var response = _userService.UpdateUser(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("RemoveUser")]
        [Authorize]
        public IActionResult RemoveUser(int id)
        {
            try
            {
                var response = _userService.RemoveUser(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
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
