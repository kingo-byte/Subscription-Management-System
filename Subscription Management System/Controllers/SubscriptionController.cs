using BAL.IServices;
using DAL.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;

namespace Subscription_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        public SubscriptionController(ISubscriptionService subscriptionService) 
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetActiveSubscriptions(int userId) 
        {
            try 
            {
                var response = _subscriptionService.GetActiveSubscriptions(userId);

                return Ok(response);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetRemainingDays")]
        [Authorize]
        public IActionResult GetRemainingDays(int id) 
        {
            try
            {
                var response = _subscriptionService.GetRemainingDays(id);

                if (response == -1) 
                {
                    return BadRequest("Subscription was not found");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
