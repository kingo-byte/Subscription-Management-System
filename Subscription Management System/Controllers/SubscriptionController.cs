using Azure.Core;
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
        private readonly ILoggingService _log;
        public SubscriptionController(ISubscriptionService subscriptionService, ILoggingService log)
        {
            _subscriptionService = subscriptionService;
            _log = log;
        }

        [HttpGet("GetActiveSubscriptions")]
        [Authorize]
        public IActionResult GetActiveSubscriptions(int userId)
        {
            try
            {
                _log.Log($"Request GetActiveSubscriptions is sent with ID {userId}");
                var response = _subscriptionService.GetActiveSubscriptions(userId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _log.Log($"Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetRemainingDays")]
        [Authorize]
        public IActionResult GetRemainingDays(int id)
        {
            try
            {
                _log.Log($"Request GetRemainingDays is sent with ID {id}");
                var response = _subscriptionService.GetRemainingDays(id);

                if (response == -1)
                {
                    return BadRequest("Subscription was not found");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _log.Log($"Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("AddSubcription")]
        [Authorize]
        public IActionResult AddSubscription(Subscription subscription)
        {
            try
            {
                _log.Log($"Request AddSubscription is sent with {subscription}");
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid subscription");
                }

                var response = _subscriptionService.AddSubscription(subscription);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _log.Log($"Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("RemoveSubscription")]
        [Authorize]
        public IActionResult RemoveSubscription(int id)
        {
            try
            {
                _log.Log($"Request RemoveSubscription is sent with ID {id}");
                bool response = _subscriptionService.RemoveSubscription(id);

                if (response)
                {
                    return Ok("subscription was successfuly removed");
                }

                return BadRequest("Invalid subscription");
            }
            catch (Exception ex)
            {
                _log.Log($"Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("DeactivateSubscription")]
        [Authorize]
        public IActionResult DeactivateSubscription(int id)
        {
            try
            {
                _log.Log($"Request DeactivateSubscription is sent with ID {id}");
                bool response = _subscriptionService.DeactivateSubscription(id);

                if (response)
                {
                    return Ok("subscription was successfuly deactivated");
                }

                return BadRequest("Invalid subscription");
            }
            catch (Exception ex)
            {
                _log.Log($"Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("ActivateSubscription")]
        [Authorize]
        public IActionResult ActivateSubscription(int id)
        {
            try
            {
                _log.Log($"Request ActivateSubscription is sent with ID {id}");
                bool response = _subscriptionService.ActivateSubscription(id);

                if (response)
                {
                    return Ok("subscription was successfuly activated");
                }

                return BadRequest("Invalid subscription");
            }
            catch (Exception ex)
            {
                _log.Log($"Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("UpdateSubscription")]
        [Authorize]
        public IActionResult UpdateSubscription(Subscription subscription)
        {
            try
            {
                _log.Log($"Request UpdateSubscription is sent with {subscription}");
                var response = _subscriptionService.UpdateSubscription(subscription);

                if (response != null)
                {
                    return Ok("subscription was successfuly updated");
                }
                return BadRequest("Invalid subscription");
            }
            catch (Exception ex)
            {
                _log.Log($"Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
